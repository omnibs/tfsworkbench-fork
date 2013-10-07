// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReportControllerTests.cs" company="None">
//   None
// </copyright>
// <summary>
//   The report controller tests ficture class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Tests
{
    using System;
    using System.ComponentModel;
    using System.Threading;
    using System.Windows.Controls;
    using System.Windows.Input;

    using NUnit.Framework;

    using Rhino.Mocks;

    using SharpArch.Testing.NUnit;

    using TfsWorkbench.Core.EventArgObjects;
    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.ReportViewer;
    using TfsWorkbench.Tests.Helpers;
    using TfsWorkbench.UIElements;

    /// <summary>
    /// The report controller tests ficture class.
    /// </summary>
    [TestFixture]
    public class ReportControllerTests : ReportViewerTestBase
    {
        private ManualResetEvent manualResetEvent;

        private IProjectDataService projectDataService;

        private ReportController controllerUnderTest;

        private IReportProxyWrapper reportProxyWrapper;

        private IDataProvider dataProvider;

        private IProjectData projectData;

        private bool hasRaisedApplicationException;

        private TextBlock uiElement;

        [SetUp]
        public void SetUp()
        {
            this.GenerateProjectData();
            this.GenerateDataProvider();
            this.GenerateProjectDataService();
            this.GenerateReportProxyWrapper();
            this.GenerateControllerUnderTest();
        }

        [TearDown]
        public void TearDown()
        {
            this.projectDataService = null;
            this.controllerUnderTest = null;
            this.reportProxyWrapper = null;
            this.dataProvider = null;
            this.projectData = null;
            if (this.uiElement != null)
            {
                this.uiElement.CommandBindings.Clear();
                this.uiElement = null;
            }
            ReportProxyWrapperHelper.ReportService2005 = null;
            ReportProxyWrapperHelper.ReportService2008 = null;
            ServiceManagerHelper.ClearDummyManager();
        }

        /// <summary>
        /// Test: Controller_show_report_should_show_applciation_exception_if_no_data_provider_present.
        /// </summary>
        [Test]
        public void Controller_show_report_should_show_applciation_exception_if_no_data_provider_present()
        {
            // Arrange
            this.SetApplicationExpecationRaisedExpectation();

            // Act
            this.dataProvider = null;
            this.controllerUnderTest.ShowReport(this.uiElement, this.GenerateCatalogItem());

            // Assert
            this.hasRaisedApplicationException.ShouldBeTrue();
        }

        /// <summary>
        /// Test: Controller_show_report_should_show_applciation_exception_if_no_project_data_present.
        /// </summary>
        [Test]
        public void Controller_show_report_should_show_applciation_exception_if_no_project_data_present()
        {
            // Arrange
            this.SetApplicationExpecationRaisedExpectation();

            // Act
            this.projectData = null;
            this.controllerUnderTest.ShowReport(this.uiElement, this.GenerateCatalogItem());

            // Assert
            this.hasRaisedApplicationException.ShouldBeTrue();
        }

        /// <summary>
        /// Test: Controller_should_select_2005_services_if_no_report_folder_found.
        /// </summary>
        [Test]
        public void Controller_should_select_2005_services_if_no_report_folder_found()
        {
            // Arrange
            const string ReportFolderToReturn = null;

            this.SetProviderReportFolder(ReportFolderToReturn);
            this.SetReportServices2005Expectation();

            // Act
            this.RaiseProjectDataChangedEventAndPause();

            // Assert
            this.AssertReportNodeGetWasCalled();
        }

        /// <summary>
        /// Test: Controller_should_select_2008_services_if_report_folder_found.
        /// </summary>
        [Test]
        public void Controller_should_select_2008_services_if_report_folder_found()
        {
            // Arrange
            const string ReportFolderToReturn = "/Report/Folder/Path";

            this.SetProviderReportFolder(ReportFolderToReturn);
            this.SetReportServices2008Expectation();

            // Act
            this.RaiseProjectDataChangedEventAndPause();

            // Assert
            this.AssertReportNodeGetWasCalled();
        }

        /// <summary>
        /// Asserts the report node get was called.
        /// </summary>
        private void AssertReportNodeGetWasCalled()
        {
            this.reportProxyWrapper.VerifyAllExpectations();
        }

        /// <summary>
        /// Generates the report proxy wrapper.
        /// </summary>
        private void SetReportServices2005Expectation()
        {
            ReportProxyWrapperHelper.ReportService2005 = this.reportProxyWrapper;
            this.SetReportNodeGetExpectation();
        }

        /// <summary>
        /// Sets the report services2008 node get expectation.
        /// </summary>
        private void SetReportServices2008Expectation()
        {
            ReportProxyWrapperHelper.ReportService2008 = this.reportProxyWrapper;
            this.SetReportNodeGetExpectation();
        }

        /// <summary>
        /// Sets the report node get expectation.
        /// </summary>
        private void SetReportNodeGetExpectation()
        {
            this.reportProxyWrapper
                .Expect(rs => rs.GetRootReportNode(null, null, null))
                .IgnoreArguments()
                .Return(null)
                .Repeat.Once();
        }

        /// <summary>
        /// Sets the report folder return value.
        /// </summary>
        /// <param name="folderToReturn">The folder to return.</param>
        private void SetProviderReportFolder(string folderToReturn)
        {
            this.projectDataService.CurrentDataProvider
                .Expect(dp => dp.GetReportFolder(null))
                .IgnoreArguments()
                .Return(folderToReturn);
        }

        /// <summary>
        /// Creates the controller with project changed event handler.
        /// </summary>
        private void SetControllerProjectDataChangedHandler()
        {
            this.controllerUnderTest.PropertyChanged += this.GetOnPropertyChangedHandler();
        }

        /// <summary>
        /// Sets the application expecation raised expectation.
        /// </summary>
        private void SetApplicationExpecationRaisedExpectation()
        {
            this.uiElement = new TextBlock();
            this.hasRaisedApplicationException = false;

            ExecutedRoutedEventHandler onApplicationException = (s, e) => this.hasRaisedApplicationException = true;

            this.uiElement.CommandBindings.Add(
                new CommandBinding(CommandLibrary.ApplicationExceptionCommand, onApplicationException));
        }

        /// <summary>
        /// Gets the on property changed handler.
        /// </summary>
        /// <returns>The property change event handler.</returns>
        private PropertyChangedEventHandler GetOnPropertyChangedHandler()
        {
            this.manualResetEvent = new ManualResetEvent(false);

            return (s, e) =>
                {
                    if (e.PropertyName.Equals("HasLoadedReportList"))
                    {
                        this.manualResetEvent.Set();
                    }
                };
        }

        /// <summary>
        /// Raises the project data changed event and pauses.
        /// </summary>
        private void RaiseProjectDataChangedEventAndPause()
        {
            this.projectDataService
                .Raise(
                    pds => pds.ProjectDataChanged += null, 
                    this.projectDataService,  
                    new ProjectDataChangedEventArgs(null, this.projectDataService.CurrentProjectData));

            if (this.manualResetEvent != null)
            {
                this.manualResetEvent.WaitOne(1000);
            }
        }

        /// <summary>
        /// Generates the controller under test.
        /// </summary>
        private void GenerateControllerUnderTest()
        {
            this.controllerUnderTest = new ReportController();

            this.SetControllerProjectDataChangedHandler();
        }

        /// <summary>
        /// Generates the project data service.
        /// </summary>
        private void GenerateProjectDataService()
        {
            this.projectDataService = MockRepository.GenerateMock<IProjectDataService>();

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

            ServiceManagerHelper.MockServiceManager(this.projectDataService);
        }

        /// <summary>
        /// Generates the project data.
        /// </summary>
        private void GenerateProjectData()
        {
            this.projectData = MockRepository.GenerateMock<IProjectData>();
        }

        /// <summary>
        /// Generates the data provider.
        /// </summary>
        private void GenerateDataProvider()
        {
            this.dataProvider = MockRepository.GenerateMock<IDataProvider>();

            this.dataProvider
                .Expect(dp => dp.GetReportServiceEndPoint(null))
                .IgnoreArguments()
                .Return(new Uri("http://dummy/service/endpoint.asmx"));
        }

        /// <summary>
        /// Generates the report proxy wrapper.
        /// </summary>
        private void GenerateReportProxyWrapper()
        {
            this.reportProxyWrapper = MockRepository.GenerateMock<IReportProxyWrapper>();
        }
    }
}
