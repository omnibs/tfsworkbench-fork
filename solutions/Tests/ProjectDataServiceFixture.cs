// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectDataServiceFixture.cs" company="None">
//   None.
// </copyright>
// <summary>
//   Defines the ProjectDataServiceFixture type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;

    using NUnit.Framework;

    using Rhino.Mocks;

    using SharpArch.Testing.NUnit;

    using TfsWorkbench.Core.DataObjects;
    using TfsWorkbench.Core.Helpers;
    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.Core.Properties;
    using TfsWorkbench.Core.Services;
    using TfsWorkbench.Tests.Helpers;

    [TestFixture]
    public class ProjectDataServiceFixture
    {
        private ProjectDataService serviceUnderTest;

        private IProjectData projectData;

        private IDataProvider dataProvider;

        private IWorkbenchItemRepository workbenchItemRepository;

        private ILinkManagerService linkManagerService;

        private IServiceManager serviceManager;

        [SetUp]
        public void SetUp()
        {
            this.GenerateLinkChangeService();
            this.GenerateServiceManager();
            this.GenerateServiceUnderTest();
            this.GenerateRepository();
            this.GenerateProjectData();
            this.GenerateDataProvider();
        }

        [TearDown]
        public void TearDown()
        {
            this.serviceUnderTest = null;
            this.projectData = null;
            this.dataProvider = null;
            this.workbenchItemRepository = null;
            this.linkManagerService = null;
            this.serviceManager = null;
        }

        [Test]
        public void Constructor_WithoutParameters_UsesDefaultDependencyServices()
        {
            // Act
            ServiceManager.Instance = this.serviceManager;
            this.serviceUnderTest = new ProjectDataService();
            ServiceManager.Instance = null;

            // Assert
            this.serviceManager.VerifyAllExpectations();
            this.serviceUnderTest.LinkManagerService.ShouldEqual(this.linkManagerService);
        }

        [Test]
        public void Constructor_WithParameter_UsesSpecfiedService()
        {
            // Arrange
            var linkManagerService = MockRepository.GenerateMock<ILinkManagerService>();

            // Act
            this.serviceUnderTest = new ProjectDataService(linkManagerService);

            // Assert
            this.serviceUnderTest.LinkManagerService.ShouldEqual(linkManagerService);
        }

        [Test]
        public void Constructor_WithNullParameter_ThrowsException()
        {
            // Arrange
            ILinkManagerService linkManagerService = null;

            // Act
            try
            {
                new ProjectDataService(linkManagerService);
                Assert.Fail("Exception Not Thrown");
            }
            catch (ArgumentNullException)
            {
            }

            // Assert
            Assert.Pass();
        }

        [Test]
        public void GetCurrentProjectData_WithValueSet_ReturnsSetValue()
        {
            // Arrange
            this.serviceUnderTest.CurrentProjectData = this.projectData;

            // Act
            var result = this.serviceUnderTest.CurrentProjectData;

            // Assert
            result.ShouldEqual(this.projectData);
        }

        [Test]
        public void SetCurrentProjectData_WithAlternativeValue_RaisesProjectDataChangedEvent()
        {
            // Arrange
            var hasRaised = false;
            this.serviceUnderTest.ProjectDataChanged += (s, e) => hasRaised = true;
            
            // Act
            this.SetCurrentProjectData();

            // Assert
            hasRaised.ShouldBeTrue();
        }

        [Test]
        public void SetCurrentProjectData_WithSameValue_DoesNotRaiseProjectDataChangedEvent()
        {
            // Arrange
            this.serviceUnderTest.CurrentProjectData = this.projectData;
            var hasRaised = false;
            this.serviceUnderTest.ProjectDataChanged += (s, e) => hasRaised = true;

            // Act
            this.SetCurrentProjectData();

            // Assert
            hasRaised.ShouldBeFalse();
        }

        [Test]
        public void GetCurrentDataProvider_WhenValueSet_ReturnsSetValue()
        {
            // Arrange
            this.serviceUnderTest.CurrentDataProvider = this.dataProvider;

            // Act
            var currentDataProvider = this.serviceUnderTest.CurrentDataProvider;

            // Assert
            currentDataProvider.ShouldEqual(this.dataProvider);
        }

        [Test]
        public void GetHighlightProviders_ReturnsNonNullCollection()
        {
            // Act
            var highlightProviders = this.serviceUnderTest.HighlightProviders;

            // Assert
            highlightProviders.ShouldNotBeNull();
        }

        [Test]
        public void ClearAll_WhenNoProjectDataSet_DoesNotThrowException()
        {
            // Arrange
            this.serviceUnderTest.CurrentProjectData = null;

            // Act
            this.serviceUnderTest.ClearAllCurrentProjectData();

            // Assert
            Assert.Pass("Exception not thrown");
        }

        [Test]
        public void ClearAll_WhenProjectDataSet_ClearsTheWorkbenchItemRepository()
        {
            // Arrange
            this.workbenchItemRepository
                .Expect(wir => wir.Clear())
                .Repeat.Once();

            this.SetCurrentProjectData();

            // Act
            this.serviceUnderTest.ClearAllCurrentProjectData();

            // Assert
            this.workbenchItemRepository.VerifyAllExpectations();
        }

        [Test]
        public void ClearAll_AfterClearingRepository_TriggersAllViewMapUpdateEvents()
        {
            // Arrange
            var hasClearedRepository = false;

            this.workbenchItemRepository
                .Expect(wir => wir.Clear())
                .WhenCalled(mi => hasClearedRepository = true)
                .Repeat.Any();

            var updatedViews = new List<ViewMap>();
            Func<ViewMap> createViewMap = () =>
                {
                    var viewMap = new ViewMap();
                    viewMap.LayoutUpdated += (s, e) =>
                        {
                            hasClearedRepository.ShouldBeTrue();
                            updatedViews.Add(viewMap);
                        };

                    return viewMap;
                };

            this.projectData
                .AddViewMap(createViewMap())
                .AddViewMap(createViewMap())
                .AddViewMap(createViewMap());

            this.SetCurrentProjectData();

            // Act
            this.serviceUnderTest.ClearAllCurrentProjectData();

            // Assert
            foreach (var viewMap in this.projectData.ViewMaps)
            {
                updatedViews.Contains(viewMap).ShouldBeTrue();
            }
        }

        [Test]
        public void ClearAll_WhenProjectDataExists_ClearsProjectNodes()
        {
            // Arrange
            this.projectData.ProjectNodes.Add("Test Node 1", MockRepository.GenerateMock<IProjectNode>());
            this.projectData.ProjectNodes.Add("Test Node 2", MockRepository.GenerateMock<IProjectNode>());
            this.projectData.ProjectNodes.Add("Test Node 3", MockRepository.GenerateMock<IProjectNode>());

            this.SetCurrentProjectData();

            // Act
            this.serviceUnderTest.ClearAllCurrentProjectData();

            // Assert
            this.projectData.ProjectNodes.Any().ShouldBeFalse();
        }

        [Test]
        public void ClearAll_WhenProjectDataExists_ClearsLinkTypes()
        {
            // Arrange
            this.projectData.LinkTypes.Add("Link Type 1");
            this.projectData.LinkTypes.Add("Link Type 2");
            this.projectData.LinkTypes.Add("Link Type 3");

            this.SetCurrentProjectData();

            // Act
            this.serviceUnderTest.ClearAllCurrentProjectData();

            // Assert
            this.projectData.LinkTypes.Any().ShouldBeFalse();
        }

        [Test]
        public void ClearAll_WhenProjectDataExists_ClearsProjectFilter()
        {
            // Arrange
            this.projectData.Filter = "A filter string";

            this.SetCurrentProjectData();

            // Act
            this.serviceUnderTest.ClearAllCurrentProjectData();

            // Assert
            this.projectData.Filter.ShouldEqual(string.Empty);
        }

        [Test]
        public void ClearAll_WhenProjectDataExists_ClearsAllLinks()
        {
            // Arrange
            this.SetCurrentProjectData();

            this.linkManagerService
                .Expect(lms => lms.ClearAllLinks())
                .Repeat.Once();

            // Act
            this.serviceUnderTest.ClearAllCurrentProjectData();

            // Assert
            this.linkManagerService.VerifyAllExpectations();
        }

        [Test]
        public void LoadProjectLayoutData_WhenFileDoesNotExist_ThrowsException()
        {
            // Arrange
            var nonExistentPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            // Act
            try
            {
                this.serviceUnderTest.LoadProjectLayoutData(nonExistentPath);
                Assert.Fail("Exception not  thrown");
            }
            catch (FileNotFoundException)
            {
            }

            // Assert
            Assert.Pass();
        }

        [Test]
        public void LoadProjectLayoutData_WhenFileExists_LoadsProjectLayoutObject()
        {
            // Arrange
            const string ProjectName = "Test Project";

            var filePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            using (var sw = new StreamWriter(filePath))
            {
                new XmlSerializer(typeof(ProjectData))
                    .Serialize(sw, new ProjectData { ProjectName = ProjectName });
            }

            // Act
            var loadedProject = this.serviceUnderTest.LoadProjectLayoutData(filePath);
            File.Delete(filePath);

            // Assert
            loadedProject.ProjectName.ShouldEqual(ProjectName);
        }

        [Test]
        public void SaveProjectLayoutData_WhenProjectDataIsNull_DoesNothing()
        {
            // Arrange
            ProjectData nullProjectData = null;
            var filePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            // Act
            this.serviceUnderTest.SaveProjectLayoutData(nullProjectData, filePath);

            // Assert
            File.Exists(filePath).ShouldBeFalse();
        }

        [Test]
        public void SaveProjectLayoutData_WhenProjectDataIsNotTypeOfProjectData_DoesNothing()
        {
            // Arrange
            var alternativeTypeProjectData = MockRepository.GenerateMock<IProjectData>();
            var filePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            // Act
            this.serviceUnderTest.SaveProjectLayoutData(alternativeTypeProjectData, filePath);

            // Assert
            File.Exists(filePath).ShouldBeFalse();
        }

        [Test]
        public void SaveProjectLayoutData_WhenProjectDataIsValidAndDirectoryDoesNotExist_CreatesDirectory()
        {
            // Arrange
            var directoryToCreate = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            var filePath = Path.Combine(directoryToCreate, Path.GetRandomFileName());

            // Act
            this.serviceUnderTest.SaveProjectLayoutData(new ProjectData(), filePath);

            // Assert
            Directory.Exists(directoryToCreate).ShouldBeTrue();
            Directory.Delete(directoryToCreate, true);
        }

        [Test]
        public void SaveProjectLayoutData_WhenProjectDataIsValid_SavesSerialisedInstanceToSpecifiedFile()
        {
            // Arrange
            const string ProjectName = "Project Name";
            var filePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            // Act
            this.serviceUnderTest.SaveProjectLayoutData(new ProjectData { ProjectName = ProjectName }, filePath);

            // Assert
            File.Exists(filePath).ShouldBeTrue();
            using (var sr = new StreamReader(filePath))
            {
                var projectFromDisk = (IProjectData)new XmlSerializer(typeof(ProjectData)).Deserialize(sr);

                projectFromDisk.ProjectName.ShouldEqual(ProjectName);
            }
        }

        [Test]
        public void DefaultFilePath_WhenUriIsNull_ThrowsException()
        {
            // Arrange
            Uri nullUri = null;

            // Act
            try
            {
                this.serviceUnderTest.DefaultFilePath(nullUri, string.Empty);
                Assert.Fail("Exception Not Thrown");
            }
            catch (ArgumentNullException)
            {
            }

            // Assert
            Assert.Pass();
        }

        [Test]
        public void DefaultFilePath_WithValidParameters_ReturnsExpectedPath()
        {
            // Arrange
            var collectionUri = new Uri("http://host:8080/App/Collection");
            const string ProjectName = "ProjectName";

            var expectedPath = string.Format(
                    @"{0}\{1}\{2}.{3}.xml", 
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    Settings.Default.ProjectFileSubDirectory,
                    "host.App.Collection",
                    ProjectName);

            // Act
            var defaultPath = this.serviceUnderTest.DefaultFilePath(collectionUri, ProjectName);

            // Assert
            defaultPath.ShouldEqual(expectedPath);

            Debug.WriteLine(defaultPath);
        }

        [Test]
        public void GeneratePathFilters_WithIterationAndArea_ReturnsExpectedFilter()
        {
            // Arrange
            const string IterationPath = "Project/Iteration";
            const string AreaPath = "Project/Iteration";
            var expectedFilter = string.Format(
                "[{0}] UNDER '{1}' AND [{2}] UNDER '{3}'",
                Settings.Default.IterationPathFieldName,
                IterationPath,
                Settings.Default.AreaPathFieldName,
                AreaPath);

            // Act
            var filterResult = this.serviceUnderTest.GeneratePathFilter(IterationPath, AreaPath);

            // Assert
            filterResult.ShouldEqual(expectedFilter);
        }

        [Test]
        public void GeneratePathFilters_WithIterationAreaAndAdditionalFilter_ReturnsExpectedFilter()
        {
            // Arrange
            const string IterationPath = "Project/Iteration";
            const string AreaPath = "Project/Iteration";
            const string AdditionalFilter = "[System.Title] = 'test'";

            var expectedFilter = string.Format(
                "[{0}] UNDER '{1}' AND [{2}] UNDER '{3}' AND {4}",
                Settings.Default.IterationPathFieldName,
                IterationPath,
                Settings.Default.AreaPathFieldName,
                AreaPath,
                AdditionalFilter);

            // Act
            var filterResult = this.serviceUnderTest.GeneratePathFilter(IterationPath, AreaPath, AdditionalFilter);

            // Assert
            filterResult.ShouldEqual(expectedFilter);
        }

        [Test]
        public void CreateNewItem_WithValidItemName_ReturnsNewItemInstance()
        {
            // Arrange
            const string WorkItemType = "Type Name 1";
            this.SetUpItemCreationExpections(WorkItemType);

            // Act
            var result = this.serviceUnderTest.CreateNewItem(WorkItemType);

            // Assert
            result.ShouldNotBeNull();

            this.dataProvider.VerifyAllExpectations();
        }

        [Test]
        public void CreateNewChild_WithValidParameters_ReturnsNewChildInstance()
        {
            // Arrange
            var parent = MockRepository.GenerateStub<IWorkbenchItem>();
            const string LinkTypeName = "Link Type 1";
            const string ChildItemType = "Child Name 1";

            var childCreationParameters = MockRepository.GenerateStub<IChildCreationParameters>();
            childCreationParameters.Expect(ccp => ccp.ChildTypeName).Return(ChildItemType).Repeat.Any();
            childCreationParameters.Expect(ccp => ccp.Parent).Return(parent).Repeat.Any();
            childCreationParameters.Expect(ccp => ccp.LinkTypeName).Return(LinkTypeName).Repeat.Any();

            this.SetUpChildCreationExpections(childCreationParameters);

            // Act
            ServiceManager.Instance = this.serviceManager;
            var result = this.serviceUnderTest.CreateNewChild(childCreationParameters);
            ServiceManager.Instance = null;

            // Assert
            result.ShouldNotBeNull();

            this.dataProvider.VerifyAllExpectations();
            this.linkManagerService.VerifyAllExpectations();
        }

        [Test]
        public void CreateDuplicate_WithValidParamters_CreatesDupliateInstance()
        {
            // Arrange
            var itemToDuplicate = MockRepository.GenerateMock<IWorkbenchItem>();
            const string ItemType = "Item Type 1";
            itemToDuplicate
                .Expect(itd => itd[Settings.Default.TypeFieldName])
                .Return(ItemType)
                .Repeat.Any();

            this.SetUpDuplicationExpectations(itemToDuplicate);

            // Act
            var duplicate = this.serviceUnderTest.CreateDuplicate(itemToDuplicate);

            // Assert
            duplicate.GetTypeName().ShouldEqual(ItemType);
        }

        /// <summary>
        /// Sets up duplication expectations.
        /// </summary>
        /// <param name="itemToDuplicate">The item to duplicate.</param>
        private void SetUpDuplicationExpectations(IWorkbenchItem itemToDuplicate)
        {
            var typeName = itemToDuplicate.GetTypeName();
            var itemTypeData = new ItemTypeData(typeName);

            this.projectData.ItemTypes.Add(itemTypeData);

            this.SetUpItemCreationExpections(typeName);
        }

        /// <summary>
        /// Sets up child creation expections.
        /// </summary>
        /// <param name="childCreationParameters">The child creation parameters.</param>
        private void SetUpChildCreationExpections(IChildCreationParameters childCreationParameters)
        {
            Predicate<ILinkItem> predicate =
                li => li.Parent == childCreationParameters.Parent && li.LinkName == childCreationParameters.LinkTypeName;

            this.linkManagerService
                .Expect(lms => lms.AddLink(null))
                .IgnoreArguments()
                .Constraints(
                    Rhino.Mocks.Constraints.Is.Matching(predicate))
                .Repeat.Once();

            this.SetUpItemCreationExpections(childCreationParameters.ChildTypeName);
        }

        /// <summary>
        /// Sets up item creation expections.
        /// </summary>
        /// <param name="workItemType">Type of the work item.</param>
        private void SetUpItemCreationExpections(string workItemType)
        {
            this.SetCurrentProjectData();
            this.SetCurrentDataProvider();

            var valueProvider = MockRepository.GenerateStub<IValueProvider>();

            valueProvider
                .Expect(vp => vp.GetValue(Settings.Default.TypeFieldName))
                .Return(workItemType)
                .Repeat.Any();

            this.dataProvider
                .Expect(dp => dp.CreateValueProvider(this.projectData, workItemType))
                .Return(valueProvider)
                .Repeat.Once();
        }

        /// <summary>
        /// Generates the service manager.
        /// </summary>
        private void GenerateServiceManager()
        {
            this.serviceManager = MockRepository.GenerateMock<IServiceManager>();

            this.serviceManager
                .Expect(sm => sm.GetService<ILinkManagerService>())
                .Return(this.linkManagerService)
                .Repeat.Any();
        }

        /// <summary>
        /// Generates the link change service.
        /// </summary>
        private void GenerateLinkChangeService()
        {
            this.linkManagerService = MockRepository.GenerateMock<ILinkManagerService>();
        }

        /// <summary>
        /// Generates the service under test.
        /// </summary>
        private void GenerateServiceUnderTest()
        {
            this.serviceUnderTest = new ProjectDataService(this.linkManagerService);
        }

        /// <summary>
        /// Generates the data provider.
        /// </summary>
        private void GenerateDataProvider()
        {
            this.dataProvider = MockRepository.GenerateMock<IDataProvider>();
        }

        /// <summary>
        /// Generates the repository.
        /// </summary>
        private void GenerateRepository()
        {
            this.workbenchItemRepository = MockRepository.GenerateMock<IWorkbenchItemRepository>();
        }

        /// <summary>
        /// Generates the project data.
        /// </summary>
        private void GenerateProjectData()
        {
            this.projectData = MockRepository.GenerateStub<IProjectData>();

            this.projectData
                .Expect(pd => pd.WorkbenchItems)
                .Return(this.workbenchItemRepository)
                .Repeat.Any();

            var viewMaps = new ObservableCollection<ViewMap>();

            this.projectData
                .Expect(pd => pd.ViewMaps)
                .Return(viewMaps)
                .Repeat.Any();

            var projectNodes = new Dictionary<string, IProjectNode>();

            this.projectData
                .Expect(pd => pd.ProjectNodes)
                .Return(projectNodes)
                .Repeat.Any();

            this.projectData.ProjectNodes
                .Add(Settings.Default.IterationPathFieldName, MockRepository.GenerateStub<IProjectNode>());

            this.projectData.ProjectNodes
                .Add(Settings.Default.AreaPathFieldName, MockRepository.GenerateStub<IProjectNode>());

            var linkTypes = new Collection<string>();

            this.projectData
                .Expect(pd => pd.LinkTypes)
                .Return(linkTypes)
                .Repeat.Any();

            var itemTypes = new ItemTypeDataCollection();
            this.projectData
                .Expect(pd => pd.ItemTypes)
                .Return(itemTypes)
                .Repeat.Any();
        }

        /// <summary>
        /// Sets the current project data.
        /// </summary>
        private void SetCurrentProjectData()
        {
            this.serviceUnderTest.CurrentProjectData = this.projectData;
        }

        /// <summary>
        /// Sets the current data provider.
        /// </summary>
        private void SetCurrentDataProvider()
        {
            this.serviceUnderTest.CurrentDataProvider = this.dataProvider;
        }
    }
}