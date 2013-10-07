namespace TfsWorkbench.Tests.WpfUiProjectSelector
{
    using System;
    using System.Threading;

    using NUnit.Framework;

    using Rhino.Mocks;

    using SharpArch.Testing.NUnit;

    using TfsWorkbench.Core.EventArgObjects;
    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.Tests.Helpers;
    using TfsWorkbench.WpfUI.ProjectSelector;
    using TfsWorkbench.WpfUI.Properties;

    /// <summary>
    /// The project selector service test fixture class.
    /// </summary>
    [TestFixture]
    public class ProjectSelectorServiceFixture
    {
        private IDataProvider dataProvider;

        private IProjectSelectorService serviceUnderTest;

        private IProjectData projectData;

        private IProjectDataService projectDataService;

        [SetUp]
        public void SetUp()
        {
            this.dataProvider = MockRepository.GenerateMock<IDataProvider>();
            this.projectData = ProjectDataGenerationHelper.GenerateProjectData();

            this.GenerateProjectDataService();
        }

        [TearDown]
        public void TearDown()
        {
            this.projectDataService = null;
            this.dataProvider = null;
            this.serviceUnderTest = null;
            this.projectData = null;

            ServiceManagerHelper.ClearDummyManager();
        }

        [Test]
        public void CreateInstance_CalledWithNoParameter_CreatesInstanceWithDefaultValues()
        {
            // Act
            ServiceManagerHelper.MockServiceManager(this.projectDataService);
            this.serviceUnderTest = ProjectSelectorService.CreateInstance();
            ServiceManagerHelper.ClearDummyManager();

            // Assert
            this.AssertServiceCreatedWithExpectedDependencies();
        }

        [Test]
        public void CreateInstance_WithParameter_CreatesInstanceWithSpecfiedValues()
        {
            // Act
            this.InitialiseService();

            // Assert
            this.AssertServiceCreatedWithExpectedDependencies();
        }

        [Test]
        public void CreateInstance_WithNoDataProvider_ThrowsException()
        {
            // Arrange
            this.dataProvider = null;

            // Act
            try
            {
                this.InitialiseService();
                Assert.Fail("Exception Not Thrown");
            }
            catch (ArgumentException)
            {
            }

            // Assert
            Assert.Pass();
        }

        [Test]
        public void BeginEnsureNodesLoaded_WhenCalledWithNullParameters_ThrowsException()
        {
            // Arrange
            IProjectData nullProjectData = null;
            Action nullCallBack = null;
            this.InitialiseService();

            // Act
            try
            {
                this.serviceUnderTest.BeginEnsureNodesLoaded(nullProjectData, nullCallBack);
                Assert.Fail("Exception Not Throw");
            }
            catch (ArgumentNullException)
            {
            }

            try
            {
                this.serviceUnderTest.BeginEnsureNodesLoaded(this.projectData, nullCallBack);
                Assert.Fail("Exception Not Throw");
            }
            catch (ArgumentNullException)
            {
            }

            // Assert
            Assert.Pass();
        }

        [Test]
        public void BeginEnsureNodesLoaded_WhenNodesAlreadyLoaded_CallsBackWithoutQueryingProvider()
        {
            // Arrange
            var hasCalledBack = false;

            this.InitialiseService();

            this.dataProvider
                .Expect(dp => dp.LoadProjectNodes(this.projectData))
                .Repeat.Never();

            // Act
            this.serviceUnderTest.BeginEnsureNodesLoaded(this.projectData, () => hasCalledBack = true);

            // Assert
            hasCalledBack.ShouldBeTrue();
            this.dataProvider.VerifyAllExpectations();
        }

        [Test]
        public void BeginEnsureNodesLoaded_WhenNodesNotLoaded_QueriesProviderThenCallsBack()
        {
            // Arrange
            this.projectData = ProjectDataGenerationHelper.GenerateProjectDataWithoutNodes();
            var hasCalledBack = false;

            this.InitialiseService();

            this.dataProvider
                .Expect(dp => dp.LoadProjectNodes(this.projectData))
                .Repeat.Once();

            var resetEvent = new AutoResetEvent(false);
            Action callBack = () =>
            {
                hasCalledBack = true;
                resetEvent.Set();
            };

            // Act
            this.serviceUnderTest.BeginEnsureNodesLoaded(this.projectData, callBack);
            resetEvent.WaitOne(1000);

            // Assert
            hasCalledBack.ShouldBeTrue();
            this.dataProvider.VerifyAllExpectations();
        }

        [Test]
        public void BeginEnsureNodesLoaded_WhenLoadingNodes_DoesNotBlockExecution()
        {
            // Arrange
            var resetEvent = new AutoResetEvent(false);
            this.projectData = ProjectDataGenerationHelper.GenerateProjectDataWithoutNodes();
            var loadProjectNodeComplete = false;

            this.InitialiseService();

            this.dataProvider
                .Expect(dp => dp.LoadProjectNodes(this.projectData))
                .WhenCalled(mi => 
                {
                    resetEvent.WaitOne(1000);
                    loadProjectNodeComplete = true;
                })
                .Repeat.Any();

            // Act
            this.serviceUnderTest.BeginEnsureNodesLoaded(this.projectData, () => { });
            var hasBlocked = loadProjectNodeComplete;
            resetEvent.Set();

            // Assert
            hasBlocked.ShouldBeFalse();
        }

        [Test]
        public void BeginEnsureNodesLoaded_WhenProviderThrowsException_RaisesAsyncExceptionEvent()
        {
            // Arrange
            var resetEvent = new AutoResetEvent(false);
            Exception thrownException = null;
            var exceptionToThrow = new Exception();

            this.projectData = ProjectDataGenerationHelper.GenerateProjectDataWithoutNodes();

            this.InitialiseService();

            this.dataProvider
                .Expect(dp => dp.LoadProjectNodes(this.projectData))
                .Throw(exceptionToThrow);

            this.serviceUnderTest.AsyncException += (s, e) =>
                {
                    thrownException = e.Context;
                    resetEvent.Set();
                };

            // Act
            this.serviceUnderTest.BeginEnsureNodesLoaded(this.projectData, () => { });
            resetEvent.WaitOne(1000);

            // Assert
            exceptionToThrow.ShouldEqual(thrownException);
        }

        [Test]
        public void ShowProjectSelector_WhenExecuted_ShowsProjectSelectorAndReturnsProjectData()
        {
            // Arrange
            this.InitialiseService();

            this.dataProvider
                .Expect(pd => pd.ShowProjectSelector())
                .Return(this.projectData)
                .Repeat.Once();

            // Act
            var result = this.serviceUnderTest.ShowProjectSelector();

            // Assert
            this.dataProvider.VerifyAllExpectations();
            result.ShouldEqual(this.projectData);
        }

        [Test]
        public void BeginVolumeCheck_WhenCalledWithNullParameters_ThrowsException()
        {
            // Arrange
            IProjectData nullProjectData = null;
            Action<int> nullCallBack = null;

            this.InitialiseService();

            // Act
            try
            {
                this.serviceUnderTest.BeginVolumeCheck(nullProjectData, nullCallBack);
                Assert.Fail("Exception Not Thrown");
            }
            catch (ArgumentNullException)
            {
            }

            try
            {
                this.serviceUnderTest.BeginVolumeCheck(this.projectData, nullCallBack);
                Assert.Fail("Exception Not Thrown");
            }
            catch (ArgumentNullException)
            {
            }

            // Assert
            Assert.Pass();
        }

        [Test]
        public void BeginVolumeCheck_WithValidParameters_CallsGetItemCountAndCallsBackWithVolume()
        {
            // Arrange
            const int ExpectedCount = 100;

            this.dataProvider
                .Expect(dp => dp.GetItemCount(null, null))
                .IgnoreArguments()
                .Return(ExpectedCount)
                .Repeat.Once();

            var returnedCount = 0;
            var resetEvent = new AutoResetEvent(false);
            Action<int> callBack = i =>
                {
                    returnedCount = i;
                    resetEvent.Set();
                };

            this.InitialiseService();

            // Act
            this.serviceUnderTest.BeginVolumeCheck(this.projectData, callBack);
            resetEvent.WaitOne(1000);

            // Assert
            this.dataProvider.VerifyAllExpectations();
            returnedCount.ShouldEqual(ExpectedCount);
        }

        [Test]
        public void BeginVolumeCheck_WhenProviderThrowsException_RaisesAsyncExceptionEvent()
        {
            // Arrange
            var expectedException = new Exception();

            this.dataProvider
                .Expect(dp => dp.GetItemCount(null, null))
                .IgnoreArguments()
                .Throw(expectedException);

            this.InitialiseService();

            Exception returnedException = null;
            var resetEvent = new AutoResetEvent(false);
            this.serviceUnderTest.AsyncException += (s, e) =>
                {
                    returnedException = e.Context;
                    resetEvent.Set();
                };

            // Act
            this.serviceUnderTest.BeginVolumeCheck(this.projectData, i => { });
            resetEvent.WaitOne(1000);

            // Assert
            returnedException.ShouldNotBeNull();
            expectedException.ShouldEqual(returnedException);
        }

        [Test]
        public void BeginVolumeCheck_WhenGettingItemCount_DoesNotBlockExecution()
        {
            // Arrange
            this.dataProvider
                .Expect(dp => dp.GetItemCount(null, null))
                .IgnoreArguments()
                .Return(0)
                .Repeat.Any();

            var hasCalledBack = false;
            var resetEvent = new AutoResetEvent(false);
            Action<int> callBack = i =>
            {
                hasCalledBack = true;
                resetEvent.Set();
            };

            this.InitialiseService();

            // Act
            this.serviceUnderTest.BeginVolumeCheck(this.projectData, callBack);
            var hasBlocked = hasCalledBack;
            resetEvent.WaitOne(1000);

            // Assert
            hasBlocked.ShouldBeFalse();
        }

        [Test]
        public void BeginLoad_WhenCallWithNullParameters_ThrowsException()
        {
            // Arrange
            IProjectData nullProjectData = null;
            Action nullCallBack = null;

            this.InitialiseService();

            // Act
            try
            {
                this.serviceUnderTest.BeginLoad(nullProjectData, nullCallBack);
                Assert.Fail("Exception Not Thrown");
            }
            catch (ArgumentNullException)
            {
            }

            try
            {
                this.serviceUnderTest.BeginLoad(this.projectData, nullCallBack);
                Assert.Fail("Exception Not Thrown");
            }
            catch (ArgumentNullException)
            {
            }

            // Assert
            Assert.Pass();
        }

        [Test]
        public void BeginLoad_WhenProjectMatchesGlobal_BeginsProjectRefresh()
        {
            // Arrange
            this.InitialiseService();

            this.dataProvider
                .Expect(dp => dp.BeginRefreshAllProjectData(null))
                .IgnoreArguments()
                .Repeat.Once();

            // Act
            this.serviceUnderTest.BeginLoad(this.projectData, () => { });

            // Assert
            this.dataProvider.VerifyAllExpectations();
        }

        [Test]
        public void BeginLoad_WhenProjectDataDoesNotMatchGlobal_CallsBeginLoadOnProvider()
        {
            // Arrange
            this.dataProvider
                .Expect(dp => dp.BeginLoadProjectData(null, null))
                .IgnoreArguments()
                .Repeat.Once();

            this.InitialiseService();
            var localProjectData = this.projectData;
            this.projectData = null;

            // Act
            this.serviceUnderTest.BeginLoad(localProjectData, () => { });

            // Assert
            this.dataProvider.VerifyAllExpectations();
        }

        [Test]
        public void BeginLoad_WhenProviderRaisesLoadedEvent_CallsBack()
        {
            // Arrange
            this.InitialiseService();
            var hasCalledBack = false;
            Action callBack = () => hasCalledBack = true;
            this.serviceUnderTest.BeginLoad(this.projectData, callBack);

            // Act
            this.dataProvider
                .Raise(dp => dp.ElementDataLoaded += null, null, new ProjectDataEventArgs(this.projectData));

            // Assert
            hasCalledBack.ShouldBeTrue();
        }

        [Test]
        public void BeginLoad_WhenProviderRaisesLoadErrorEvent_RaisesAsyncExceptionEvent()
        {
            // Arrange
            var expectedException = new Exception();
            Exception returnedException = null;
            this.InitialiseService();
            this.serviceUnderTest.AsyncException += (s, e) => returnedException = e.Context;
            this.serviceUnderTest.BeginLoad(this.projectData, () => { });

            // Act
            this.dataProvider
                .Raise(dp => dp.ElementDataLoadError += null, null, new ExceptionEventArgs(expectedException));

            // Assert
            returnedException.ShouldNotBeNull();
            expectedException.ShouldEqual(returnedException);
        }

        [Test]
        public void BeginLoad_WhenSucessfullyCompletes_SetsPreviousProjectSettingsInProvider()
        {
            // Arrange
            this.InitialiseService();
            this.serviceUnderTest.BeginLoad(this.projectData, () => { });

            this.dataProvider
                .Expect(dp => dp.LastProjectCollectionUrl = this.projectData.ProjectCollectionUrl)
                .Repeat.Once();

            this.dataProvider
                .Expect(dp => dp.LastProjectName = this.projectData.ProjectName)
                .Repeat.Once();

            // Act
            this.dataProvider
                .Raise(dp => dp.ElementDataLoaded += null, null, new ProjectDataEventArgs(this.projectData));

            // Assert
            this.dataProvider.VerifyAllExpectations();
        }

        [Test]
        public void GetLastProjectData_WhenNoPreviousDataExists_ReturnsNull()
        {
            // Arrange
            this.InitialiseService();

            this.dataProvider
                .Expect(dp => dp.LastProjectName)
                .Return(null)
                .Repeat.Once();

            // Act
            var result = this.serviceUnderTest.GetLastProjectData();

            // Assert
            this.dataProvider.VerifyAllExpectations();
            result.ShouldBeNull();
        }

        [Test]
        public void GetLastProjectData_WhenPreviousDataExists_ReturnsPreviousData()
        {
            // Arrange
            const string DummyProjectCollectionEndPoint = "http://host:8080/tfs/colleciton";
            const string DummyProjectName = "Test Project";

            this.InitialiseService();

            this.dataProvider
                .Expect(dp => dp.LastProjectName)
                .Return(DummyProjectName)
                .Repeat.Once();

            this.dataProvider
                .Expect(dp => dp.LastProjectCollectionUrl)
                .Return(DummyProjectCollectionEndPoint)
                .Repeat.Once();

            this.projectDataService
                .Expect(pds => pds.LoadProjectLayoutData(null))
                .IgnoreArguments()
                .Return(this.projectData)
                .Repeat.Once();

            // Act
            var result = this.serviceUnderTest.GetLastProjectData();

            // Assert
            this.dataProvider.VerifyAllExpectations();
            this.projectDataService.VerifyAllExpectations();
            result.ShouldEqual(this.projectData);
        }

        [Test]
        public void GetLastProjectData_WhenProviderThrowsException_RaisesAsyncExceptionEvent()
        {
            // Arrange
            this.InitialiseService();

            var exceptionToThrow = new Exception();
            this.dataProvider
                .Expect(dp => dp.LastProjectName)
                .Throw(exceptionToThrow);

            Exception actualException = null;
            this.serviceUnderTest.AsyncException += (s, e) => actualException = e.Context;

            // Act
            this.serviceUnderTest.GetLastProjectData();

            // Assert
            actualException.ShouldNotBeNull();
            actualException.Message.ShouldEqual(Resources.String052);
            actualException.InnerException.ShouldEqual(exceptionToThrow);
        }

        [Test]
        public void CreateLoader_WhenNoAlternativeFactorySet_ReturnsLoaderWithVolumeCheckInstance()
        {
            // Arrange
            this.InitialiseService();

            // Act
            var loader = this.serviceUnderTest.CreateLoader(this.projectData);

            // Assert
            loader.ShouldBeOfType(typeof(LoaderWithVolumeCheck));
        }

        [Test]
        public void CreateLoader_PassesCriteriaToLoader()
        {
            // Arrange
            this.InitialiseService();

            IProjectData actualCriteria = null;
            ((ProjectSelectorService)this.serviceUnderTest).VolumeCheckLoaderFactory = c =>
                {
                    actualCriteria = c;
                    return MockRepository.GenerateMock<ILoaderWithVolumeCheck>();
                };

            var criteriaToSend = this.projectData;

            // Act
            this.serviceUnderTest.CreateLoader(criteriaToSend);

            // Assert
            actualCriteria.ShouldNotBeNull();
            actualCriteria.ShouldEqual(criteriaToSend);
        }

        [Test]
        public void HasProjectNodes_WhenProjectHasNodes_ReturnsTrue()
        {
            // Arrange
            var projectDataWithNodes = ProjectDataGenerationHelper.GenerateProjectData();
            this.InitialiseService();

            // Act
            var result = this.serviceUnderTest.HasProjectNodes(projectDataWithNodes);

            // Assert
            result.ShouldBeTrue();
        }

        [Test]
        public void HasProjectNodes_WhenProjectHasNoNodes_ReturnsFalse()
        {
            // Arrange
            var projectDataWithoutNodes = ProjectDataGenerationHelper.GenerateProjectDataWithoutNodes();
            this.InitialiseService();

            // Act
            var result = this.serviceUnderTest.HasProjectNodes(projectDataWithoutNodes);

            // Assert
            result.ShouldBeFalse();
        }

        /// <summary>
        /// Initialises the service.
        /// </summary>
        private void InitialiseService()
        {
            this.serviceUnderTest = ProjectSelectorService.CreateInstance();
        }

        /// <summary>
        /// Asserts the service created with expected dependencies.
        /// </summary>
        private void AssertServiceCreatedWithExpectedDependencies()
        {
            var projectSelectorService = this.serviceUnderTest as ProjectSelectorService;
            projectSelectorService.ShouldNotBeNull();

            if (projectSelectorService == null)
            {
                return;
            }

            projectSelectorService.ProjectDataService.ShouldEqual(this.projectDataService);
            projectSelectorService.DataProvider.ShouldEqual(this.dataProvider);
        }

        /// <summary>
        /// Generates the project data service.
        /// </summary>
        private void GenerateProjectDataService()
        {
            this.projectDataService = MockRepository.GenerateMock<IProjectDataService>();

            ServiceManagerHelper.MockServiceManager(this.projectDataService);

            this.projectDataService
                .Expect(pds => pds.CurrentDataProvider)
                .WhenCalled(mi => mi.ReturnValue = this.dataProvider)
                .Return(null)
                .Repeat.Any();

            this.projectDataService
                .Expect(pds => pds.CurrentProjectData)
                .WhenCalled(mi => mi.ReturnValue = this.projectData)
                .Return(null)
                .Repeat.Any();
        }
    }
}
