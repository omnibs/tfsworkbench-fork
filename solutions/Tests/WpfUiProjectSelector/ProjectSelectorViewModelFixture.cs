namespace TfsWorkbench.Tests.WpfUiProjectSelector
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Threading;

    using NUnit.Framework;

    using Rhino.Mocks;

    using SharpArch.Testing.NUnit;

    using TfsWorkbench.Core.EventArgObjects;
    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.Core.Properties;
    using TfsWorkbench.Tests.Helpers;
    using TfsWorkbench.UIElements;
    using TfsWorkbench.UIElements.PopupControls;
    using TfsWorkbench.WpfUI.ProjectSelector;

    [TestFixture]
    public class ProjectSelectorViewModelFixture
    {
        /// <summary>
        /// The property changed notification list.
        /// </summary>
        private readonly List<string> propertyChangedNotifcations = new List<string>();

        /// <summary>
        /// The project selector service instance.
        /// </summary>
        private IProjectSelectorService projectSelectorService;

        /// <summary>
        /// The project data service instance.
        /// </summary>
        private IProjectDataService projectDataService;

        /// <summary>
        /// The view model under test.
        /// </summary>
        private ProjectSelectorViewModel viewModelUnderTest;

        /// <summary>
        /// The previous project data.
        /// </summary>
        private IProjectData lastProjectData;

        /// <summary>
        /// The command context. Passed into commands as the element to target.
        /// </summary>
        private UIElement commandContext;

        /// <summary>
        /// Sets up the test environment.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            this.commandContext = new TextBlock();
            this.lastProjectData = ProjectDataGenerationHelper.GenerateProjectDataWithoutNodes();
            this.projectSelectorService = this.GenerateProjectSelectorService();
            this.projectDataService = GenerateProjectDataService();
            this.propertyChangedNotifcations.Clear();
        }

        /// <summary>
        /// Tears down to test environment.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            this.commandContext = null;
            this.projectSelectorService = null;
            this.viewModelUnderTest = null;
            this.propertyChangedNotifcations.Clear();
        }

        /// <summary>
        /// Constructor, with out parameters, uses default system objects.
        /// </summary>
        [Test]
        public void Constructor_WithOutParameters_UsesDefaultObjects()
        {
            // Arrange
            var projectDataService = MockRepository.GenerateMock<IProjectDataService>();
            var projectData = ProjectDataGenerationHelper.GenerateProjectData();

            projectDataService
                .Expect(pds => pds.CurrentProjectData)
                .Return(projectData)
                .Repeat.Once();
            
            // Act
            ServiceManagerHelper.MockServiceManager(projectDataService);
            ServiceManagerHelper.RegisterServiceInstance(this.projectSelectorService);
            this.viewModelUnderTest = new ProjectSelectorViewModel();
            ServiceManagerHelper.ClearDummyManager();

            // Assert
            projectDataService.VerifyAllExpectations();
            this.viewModelUnderTest.ProjectData.ShouldEqual(projectData);
            this.viewModelUnderTest.ProjectSelectorService.ShouldEqual(this.projectSelectorService);
        }

        [Test]
        public void Constructor_WithNullServiceParameters_ThrowsException()
        {
            // Arrange
            var projectData = ProjectDataGenerationHelper.GenerateProjectData();
            IProjectSelectorService nullSelectorService = null;
            IProjectDataService nullDataService = null;

            var dataService = MockRepository.GenerateMock<IProjectDataService>();
            var selectorService = MockRepository.GenerateMock<IProjectSelectorService>();

            // Act
            try
            {
                new ProjectSelectorViewModel(projectData, nullSelectorService, nullDataService);
                Assert.Fail("Exception not thrown");
            }
            catch (ArgumentNullException)
            {
            }

            try
            {
                new ProjectSelectorViewModel(projectData, selectorService, nullDataService);
                Assert.Fail("Exception not thrown");
            }
            catch (ArgumentNullException)
            {
            }

            try
            {
                new ProjectSelectorViewModel(projectData, nullSelectorService, dataService);
                Assert.Fail("Exception not thrown");
            }
            catch (ArgumentNullException)
            {
            }

            // Assert
            Assert.Pass();
        }

        /// <summary>
        /// Constructor, with null project data, does not throw exception.
        /// </summary>
        [Test]
        public void Constructor_WithNullProjectData_DoesNotThrowException()
        {
            // Arrange
            IProjectData projectData = null;

            // Act
            this.InitialiseViewModelWithSpecifiedProjectData(projectData);

            // Assert
            Assert.Pass();
        }

        /// <summary>
        /// Constructor, with non null project data parameter, sets selection properties.
        /// </summary>
        [Test]
        public void Constructor_WithNonNullProjectData_SetsSelectionProperties()
        {
            // Arrange
            var projectData = ProjectDataGenerationHelper.GenerateProjectData();

            // Act
            this.InitialiseViewModelWithSpecifiedProjectData(projectData);

            // Assert
            this.AssertProjectSelectionDataHasBeenSet(projectData);
        }

        /// <summary>
        /// Constructor, with project data with null project node, clears path roots.
        /// </summary>
        [Test]
        public void Constructor_WithProjectDataWithNullProjectNode_ClearsPathRoots()
        {
            // Arrange
            var projectData = ProjectDataGenerationHelper.GenerateProjectDataWithoutNodes();

            // Act
            this.InitialiseViewModelWithSpecifiedProjectData(projectData);

            // Assert
            this.AssertProjectRootNodesAreNull();
        }

        /// <summary>
        /// Constructor, with project data with null project node, clears path roots.
        /// </summary>
        [Test]
        public void Constructor_WithProjectDataIncludingProjectNode_SetsPathRoots()
        {
            // Arrange
            var projectData = ProjectDataGenerationHelper.GenerateProjectData();

            // Act
            this.InitialiseViewModelWithSpecifiedProjectData(projectData);

            // Assert
            this.AssertProjectRootNodesHaveBeenSet(projectData);
        }

        /// <summary>
        /// Constructor, with null project data, does not initialises properties.
        /// </summary>
        [Test]
        public void Constructor_WithNullProjectDataAndNoPreviousSelectionData_DoesNotInitialisesProperties()
        {
            // Arrange
            IProjectData projectData = null;
            this.lastProjectData = null;

            // Act
            this.InitialiseViewModelWithSpecifiedProjectData(projectData);

            // Assert
            this.AssertProjectDataPropertiesAreCleared();
        }

        /// <summary>
        /// Constructor, with null project data and previous selection data, initialises properties.
        /// </summary>
        [Test]
        public void Constructor_WithNullProjectDataAndPreviousSelectionData_InitialisesProperties()
        {
            // Arrange
            IProjectData projectData = null;

            // Act
            this.InitialiseViewModelWithSpecifiedProjectData(projectData);

            // Assert
            this.AssertProjectSelectionDataHasBeenSet(this.lastProjectData);
        }

        /// <summary>
        /// Constructor, generates show project selector command.
        /// </summary>
        [Test]
        public void Constructor_GeneratesCommandObjects()
        {
            // Arrange
            IProjectData projectData = null;

            // Act
            this.InitialiseViewModelWithSpecifiedProjectData(projectData);

            // Assert
            this.AssertCommandsAreGenerated();
        }

        /// <summary>
        /// Show project selector command, when executed, shows the project selector dialog.
        /// </summary>
        [Test]
        public void ShowProjectSelectorCommand_WhenExecuted_ShowsTheProjectSelectorDialog()
        {
            // Arrange
            this.projectSelectorService
                .Expect(pss => pss.ShowProjectSelector())
                .Return(null)
                .Repeat.Once();

            this.InitialiseViewModelWithNullProjectData();

            // Act
            this.viewModelUnderTest.ShowProjectSelectorCommand.Execute(null);

            // Assert
            this.projectSelectorService.VerifyAllExpectations();
        }

        /// <summary>
        /// Show project selector command, when project selector returns instance, sets project data.
        /// </summary>
        [Test]
        public void ShowProjectSelectorCommand_WhenProjectSelectorReturnsInstance_SetsProjectData()
        {
            // Arrange
            var projectData = ProjectDataGenerationHelper.GenerateProjectData();

            this.projectSelectorService
                .Expect(pss => pss.ShowProjectSelector())
                .Return(projectData)
                .Repeat.Once();

            this.InitialiseViewModelWithNullProjectData();

            // Act
            this.viewModelUnderTest.ShowProjectSelectorCommand.Execute(null);

            // Assert
            this.projectSelectorService.VerifyAllExpectations();
            this.AssertProjectSelectionDataHasBeenSet(projectData);
        }

        /// <summary>
        /// Show project selector command, when project selector returns null, does not alter selected project details.
        /// </summary>
        [Test]
        public void ShowProjectSelectorCommand_WhenProjectSelectorReturnsNull_DoesNotAlterSelectedProjectDetails()
        {
            // Arrange
            this.projectSelectorService
                .Expect(pss => pss.ShowProjectSelector())
                .Return(null)
                .Repeat.Once();

            this.InitialiseViewModelWithProjectData();

            var projectData = this.viewModelUnderTest.ProjectData;

            // Act
            this.viewModelUnderTest.ShowProjectSelectorCommand.Execute(null);

            // Assert
            this.projectSelectorService.VerifyAllExpectations();
            this.AssertProjectSelectionDataHasBeenSet(projectData);
        }

        /// <summary>
        /// Show project selector command, can always execute.
        /// </summary>
        [Test]
        public void ShowProjectSelectorCommand_CanAlwaysExecute()
        {
            // Arrange
            this.InitialiseViewModelWithNullProjectData();

            // Act
            var canExecute = this.viewModelUnderTest.ShowProjectSelectorCommand.CanExecute(null);

            // Assert
            canExecute.ShouldBeTrue();
        }

        /// <summary>
        /// Load project data command, when projec data is null, cannot execute.
        /// </summary>
        [Test]
        public void LoadProjectDataCommand_WhenProjecDataIsNull_CannotExecute()
        {
            // Arrange
            this.InitialiseViewModelWithNullProjectData();

            // Act
            var canExecute = this.viewModelUnderTest.LoadProjectDataCommand.CanExecute(null);

            // Assert
            canExecute.ShouldBeFalse();
        }

        [Test]
        public void LoadProjectDataCommand_WhenIterationPathNotSet_CannotExecute()
        {
            // Arrange
            this.InitialiseViewModelWithProjectData();
            this.viewModelUnderTest.AreaPath = "Test";

            // Act
            this.viewModelUnderTest.IterationPath = null;
            var canExecuteWithNullPath = this.viewModelUnderTest.LoadProjectDataCommand.CanExecute(null);
            this.viewModelUnderTest.IterationPath = string.Empty;
            var canExecuteWithEmptyPath = this.viewModelUnderTest.LoadProjectDataCommand.CanExecute(null);

            // Assert
            canExecuteWithNullPath.ShouldBeFalse();
            canExecuteWithEmptyPath.ShouldBeFalse();
        }

        [Test]
        public void LoadProjectDataCommand_WhenAreaPathNotSet_CannotExecute()
        {
            // Arrange
            this.InitialiseViewModelWithProjectData();
            this.viewModelUnderTest.IterationPath = "Test";

            // Act
            this.viewModelUnderTest.AreaPath = null;
            var canExecuteWithNullPath = this.viewModelUnderTest.LoadProjectDataCommand.CanExecute(null);
            this.viewModelUnderTest.AreaPath = string.Empty;
            var canExecuteWithEmptyPath = this.viewModelUnderTest.LoadProjectDataCommand.CanExecute(null);

            // Assert
            canExecuteWithNullPath.ShouldBeFalse();
            canExecuteWithEmptyPath.ShouldBeFalse();
        }

        [Test]
        public void LoadProjectDataCommand_WhenProjectDataExistsAndPathsSet_CanExecute()
        {
            // Arrange
            this.InitialiseViewModelWithProjectData();
            this.viewModelUnderTest.IterationPath = "Test";
            this.viewModelUnderTest.AreaPath = "Test";

            // Act
            var canExecute = this.viewModelUnderTest.LoadProjectDataCommand.CanExecute(null);

            // Assert
            canExecute.ShouldBeTrue();
        }

        [Test]
        public void LoadProjectDataCommand_WhenAborted_RaisesApplicationMessageAndRemovesBusyStatus()
        {
            // Arrange
            this.InitialiseViewModelWithProjectData();
            this.viewModelUnderTest.IterationPath = "Test";

            var loader = this.GenerateLoaderAndAttachToService();

            this.viewModelUnderTest.LoadProjectDataCommand.Execute(this.commandContext);

            // Act
            Action invocation = () => loader.Raise(l => l.Aborted += null, loader, EventArgs.Empty);
            Predicate<ExecutedRoutedEventArgs> predicate = e => !this.viewModelUnderTest.IsBusy;

            // Assert
            this.AssertCommandWasExecuted(CommandLibrary.ApplicationMessageCommand, invocation, predicate);
        }

        [Test]
        public void LoadProjectDataCommand_WhenVolumeWarning_ShowsDecisionDialog()
        {
            // Arrange
            this.InitialiseViewModelWithProjectData();
            this.viewModelUnderTest.IterationPath = "Test";

            var loader = this.GenerateLoaderAndAttachToService();

            this.viewModelUnderTest.LoadProjectDataCommand.Execute(this.commandContext);

            // Act
            Action invocation = () => loader.Raise(l => l.VolumeWarning += null, loader, new ContextEventArgs<DecisionControl>(new DecisionControl()));
            Predicate<ExecutedRoutedEventArgs> predicate = e => e.Parameter is DecisionControl;

            // Assert
            this.AssertCommandWasExecuted(CommandLibrary.ShowDialogCommand, invocation, predicate);
        }

        [Test]
        public void LoadProjectDataCommand_OnConfirmLoadDataWhenCurrentProjectDataDoesNotMatchGlobal_ExecutesCloseProjectEvent()
        {
            // Arrange
            this.InitialiseViewModelWithProjectData();
            this.viewModelUnderTest.IterationPath = "Test";

            var loader = this.GenerateLoaderAndAttachToService();

            this.viewModelUnderTest.LoadProjectDataCommand.Execute(this.commandContext);

            this.SetCurrentProjectData(null);

            // Act
            Action invocation = () => loader.Raise(l => l.ConfirmLoadData += null, loader, new CancelEventArgs());
            Predicate<ExecutedRoutedEventArgs> predicate = e => true;

            // Assert
            this.AssertCommandWasExecuted(CommandLibrary.CloseProjectCommand, invocation, predicate);
        }

        [Test]
        public void LoadProjectDataCommand_OnConfirmLoadDataWhenCurrentProjectDataMatchesGlobal_DoesNotExecuteCloseProjectEvent()
        {
            // Arrange
            this.InitialiseViewModelWithProjectData();
            this.viewModelUnderTest.IterationPath = "Test";

            var loader = this.GenerateLoaderAndAttachToService();

            this.viewModelUnderTest.LoadProjectDataCommand.Execute(this.commandContext);

            this.SetCurrentProjectData(this.viewModelUnderTest.ProjectData);

            // Act
            Action invocation = () => loader.Raise(l => l.ConfirmLoadData += null, loader, new CancelEventArgs());

            // Assert
            this.AssertCommandWasNotExecuted(CommandLibrary.CloseProjectCommand, invocation);
        }

        [Test]
        public void LoadProjectDataCommand_WhenCloseCurrentProjectIsCancelled_SetsCancelArgsToTrue()
        {
            // Arrange
            this.InitialiseViewModelWithProjectData();
            this.viewModelUnderTest.IterationPath = "Test";

            var loader = this.GenerateLoaderAndAttachToService();

            this.viewModelUnderTest.LoadProjectDataCommand.Execute(this.commandContext);
            var cancelEventArgs = new CancelEventArgs();
            this.SetCurrentProjectData(MockRepository.GenerateStub<IProjectData>());

            // Act
            loader.Raise(l => l.ConfirmLoadData += null, loader, cancelEventArgs);

            // Assert
            cancelEventArgs.Cancel.ShouldBeTrue();
        }

        [Test]
        public void LoadProjectDataCommand_WhenLoadComplete_ExecutesCloseDialog()
        {
            // Arrange
            this.InitialiseViewModelWithProjectData();
            this.viewModelUnderTest.IterationPath = "Test";

            var loader = this.GenerateLoaderAndAttachToService();

            this.viewModelUnderTest.LoadProjectDataCommand.Execute(this.commandContext);

            // Act
            Action invocation = () => loader.Raise(l => l.Complete += null, loader, new ProjectDataEventArgs(null));
            Predicate<ExecutedRoutedEventArgs> predicate = e => e.Parameter == this.commandContext;

            // Assert
            this.AssertCommandWasExecuted(CommandLibrary.CloseDialogCommand, invocation, predicate);
        }

        [Test]
        public void ServiceAsyncExceptionEvents_WhenRaised_ExecutesApplicationExceptionCommand()
        {
            // Arrange
            var exception = new Exception();
            this.InitialiseViewModelWithNullProjectData();
            this.GenerateLoaderAndAttachToService();
            this.viewModelUnderTest.LoadProjectDataCommand.Execute(this.commandContext);

            // Act
            Action invocation = () => this.projectSelectorService.Raise(pss => pss.AsyncException += null, null, new ExceptionEventArgs(exception));
            Predicate<ExecutedRoutedEventArgs> predicate = e => e.Parameter == exception;

            // Assert
            this.AssertCommandWasExecuted(CommandLibrary.ApplicationExceptionCommand, invocation, predicate);
        }

        /// <summary>
        /// Service async exception events, when raised, sets is busy to false.
        /// </summary>
        [Test]
        public void ServiceAsyncExceptionEvents_WhenRaised_SetsIsBusyToFalse()
        {
            // Arrange
            this.InitialiseViewModelWithProjectData();
            this.viewModelUnderTest.EnsureProjectNodesLoadedCommand.Execute(null);

            // Act
            var busyStatusBeforeException = this.viewModelUnderTest.IsBusy;
            this.projectSelectorService.Raise(pss => pss.AsyncException += null, null, new ExceptionEventArgs(new Exception()));
            var busyStatusAfterException = this.viewModelUnderTest.IsBusy;

            // Assert
            busyStatusBeforeException.ShouldBeTrue();
            busyStatusAfterException.ShouldBeFalse();
        }

        [Test]
        public void ServiceAsyncExceptionEvents_WhenRaised_DisposesLoaderInstance()
        {
            // Arrange
            var projectLoader = MockRepository.GenerateMock<ILoaderWithVolumeCheck>();

            projectLoader.Expect(pl => pl.Dispose()).Repeat.Once();

            this.projectSelectorService
                .Expect(pss => pss.CreateLoader(null))
                .IgnoreArguments()
                .Return(projectLoader)
                .Repeat.Once();

            this.InitialiseViewModelWithProjectData();
            this.viewModelUnderTest.LoadProjectDataCommand.Execute(null);

            // Act
            this.projectSelectorService.Raise(pss => pss.AsyncException += null, null, new ExceptionEventArgs(new Exception()));

            // Assert
            this.projectSelectorService.VerifyAllExpectations();
            projectLoader.VerifyAllExpectations();
        }

        [Test]
        public void ServiceAsyncExceptionEvents_WhenRaised_SetsErrorMessage()
        {
            // Arrange
            const string ExpectedErrorMessage = "Error Message";
            this.InitialiseViewModelWithProjectData();

            // Act
            this.projectSelectorService.Raise(pss => pss.AsyncException += null, null, new ExceptionEventArgs(new Exception(ExpectedErrorMessage)));

            // Assert
            this.viewModelUnderTest.ErrorMessage.ShouldEqual(ExpectedErrorMessage);
        }

        /// <summary>
        /// Set iteration path, when has listener, raises event with expected property name.
        /// </summary>
        [Test]
        public void SetIterationPath_WhenHasListener_RaisesEventWithExpectedPropertyName()
        {
            // Arrange
            const string ExpectedPropertyName = "IterationPath";
            this.InitialiseViewModelWithProjectData();
            this.SetPropertyChangedListener();

            // Act
            this.viewModelUnderTest.IterationPath = "Test";

            // Assert
            this.AssertLastPropertyChangedWas(ExpectedPropertyName);
        }

        /// <summary>
        /// Set area path, when property changed has listener, raises project changed event with expected property name.
        /// </summary>
        [Test]
        public void SetAreaPath_WhenPropertyChangedHasListener_RaisesProjectChangedEventWithExpectedPropertyName()
        {
            // Arrange
            const string ExpectedPropertyName = "AreaPath";
            this.InitialiseViewModelWithProjectData();
            this.SetPropertyChangedListener();

            // Act
            this.viewModelUnderTest.AreaPath = "Test";

            // Assert
            this.AssertLastPropertyChangedWas(ExpectedPropertyName);
        }

        /// <summary>
        /// Cancel command, can always execute.
        /// </summary>
        [Test]
        public void CancelCommand_CanAlwaysExecute()
        {
            // Arrange
            this.InitialiseViewModelWithNullProjectData();

            // Act
            var canExecute = this.viewModelUnderTest.CancelCommand.CanExecute(null);

            // Assert
            canExecute.ShouldBeTrue();
        }

        /// <summary>
        /// Cancel command, when executed, calls close dialog.
        /// </summary>
        [Test]
        public void CancelCommand_WhenExecuted_CallsCloseDialog()
        {
            // Arrange
            this.InitialiseViewModelWithNullProjectData();

            // Act
            Action invocation = () => this.viewModelUnderTest.CancelCommand.Execute(this.commandContext);

            // Assert
            this.AssertCommandWasExecuted(CommandLibrary.CloseDialogCommand, invocation, e => true);
        }

        [Test]
        public void CancelCommand_WhenExecuted_DisposesLoader()
        {
            // Arrange
            var projectLoader = MockRepository.GenerateMock<ILoaderWithVolumeCheck>();

            projectLoader.Expect(pl => pl.Dispose()).Repeat.Once();

            this.projectSelectorService
                .Expect(pss => pss.CreateLoader(null))
                .IgnoreArguments()
                .Return(projectLoader)
                .Repeat.Once();

            this.InitialiseViewModelWithProjectData();
            this.viewModelUnderTest.LoadProjectDataCommand.Execute(null);

            // Act
            this.viewModelUnderTest.CancelCommand.Execute(this.commandContext);

            // Assert
            this.projectSelectorService.VerifyAllExpectations();
            projectLoader.VerifyAllExpectations();
        }

        /// <summary>
        /// Ensure project nodes loaded command, when executed, calls Begin Ensure Nodes Loaded.
        /// </summary>
        [Test]
        public void EnsureProjectNodesLoadedCommand_WhenExecuted_CallsBeginEnsureNodesLoaded()
        {
            // Arrange
            this.InitialiseViewModelWithProjectData();

            this.projectSelectorService
                .Expect(pss => pss.BeginEnsureNodesLoaded(null, null))
                .IgnoreArguments()
                .Repeat.Once();

            // Act
            this.viewModelUnderTest.EnsureProjectNodesLoadedCommand.Execute(null);

            // Assert
            this.projectSelectorService.VerifyAllExpectations();
        }

        /// <summary>
        /// Ensure project nodes loaded command, when executed, sets busy status as expected.
        /// </summary>
        [Test]
        public void EnsureProjectNodesLoadedCommand_WhenExecuted_SetsBusyStatusAsExpected()
        {
            // Arrange
            this.InitialiseViewModelWithProjectData();

            Action callBack = null;

            this.projectSelectorService
                .Expect(pss => pss.BeginEnsureNodesLoaded(null, null))
                .IgnoreArguments()
                .WhenCalled(mi => callBack = (Action)mi.Arguments.ElementAt(1))
                .Repeat.Any();

            // Act
            var busyStatusBefore = this.viewModelUnderTest.IsBusy;
            this.viewModelUnderTest.EnsureProjectNodesLoadedCommand.Execute(null);
            var busyStatusDuring = this.viewModelUnderTest.IsBusy;
            callBack();
            var busyStatusAfter = this.viewModelUnderTest.IsBusy;

            // Assert
            busyStatusBefore.ShouldBeFalse();
            busyStatusDuring.ShouldBeTrue();
            busyStatusAfter.ShouldBeFalse();
        }

        /// <summary>
        /// Ensure project nodes loaded command, when executed, raises node property changed.
        /// </summary>
        [Test]
        public void EnsureProjectNodesLoadedCommand_WhenExecuted_RaisesNodePropertyChanged()
        {
            // Arrange
            var expectedNotifications = new[] { "AreaRootNode", "IterationRootNode" };
            var notifications = new List<string>();

            this.InitialiseViewModelWithProjectData();

            this.projectSelectorService
                .Expect(pss => pss.BeginEnsureNodesLoaded(null, null))
                .IgnoreArguments()
                .WhenCalled(mi => ((Action)mi.Arguments.ElementAt(1))())
                .Repeat.Any();

            this.viewModelUnderTest.PropertyChanged += (s, e) => notifications.Add(e.PropertyName);

            // Act
            this.viewModelUnderTest.EnsureProjectNodesLoadedCommand.Execute(null);

            // Assert
            expectedNotifications.All(notifications.Contains).ShouldBeTrue();
        }

        [Test]
        public void EnsureProjectNodesLoadedCommand_WhenExecutedAndContextIsExpandable_CallsExpand()
        {
            // Arrange
            this.InitialiseViewModelWithProjectData();

            this.projectSelectorService
                .Expect(pss => pss.BeginEnsureNodesLoaded(null, null))
                .IgnoreArguments()
                .WhenCalled(mi => ((Action)mi.Arguments.ElementAt(1))())
                .Repeat.Any();

            var expandable = MockRepository.GenerateMock<IExpandable>();
            expandable.Expect(e => e.Expand()).Repeat.Once();

            // Act
            this.viewModelUnderTest.EnsureProjectNodesLoadedCommand.Execute(expandable);

            // Assert
            expandable.VerifyAllExpectations();
        }

        [Test]
        public void EnsureProjectNodesLoadedCommand_WhenExceutedAndProjectNodesArePresent_SkipsNodeLoadProcess()
        {
            // Arrange
            this.InitialiseViewModelWithProjectData();

            this.projectSelectorService
                .Expect(pss => pss.HasProjectNodes(null))
                .IgnoreArguments()
                .Return(true)
                .Repeat.Once();

            this.projectSelectorService
                .Expect(pss => pss.BeginEnsureNodesLoaded(null, null))
                .IgnoreArguments()
                .Repeat.Never();

            // Act
            this.viewModelUnderTest.EnsureProjectNodesLoadedCommand.Execute(null);

            // Assert
            this.projectSelectorService.VerifyAllExpectations();
        }

        [Test]
        public void EnsureProjectNodesLoadedCommand_WhenProjectDataIsNotPresent_CannotExecute()
        {
            // Arrange
            this.lastProjectData = null;
            this.InitialiseViewModelWithNullProjectData();

            // Act
            var canExecute = this.viewModelUnderTest.EnsureProjectNodesLoadedCommand.CanExecute(null);

            // Assert
            canExecute.ShouldBeFalse();
        }

        /// <summary>
        /// Generates the project data service.
        /// </summary>
        /// <returns>An instance of the project data service.</returns>
        private static IProjectDataService GenerateProjectDataService()
        {
            return MockRepository.GenerateMock<IProjectDataService>();
        }

        /// <summary>
        /// Sets the global project data service current project data to the specified value.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        private void SetCurrentProjectData(IProjectData projectData)
        {
            this.projectDataService
                .Expect(pds => pds.CurrentProjectData)
                .Return(projectData)
                .Repeat.Any();
        }

        /// <summary>
        /// Asserts the command raised.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="invocation">The invocation.</param>
        /// <param name="predicate">The predicate.</param>
        private void AssertCommandWasExecuted(ICommand command, Action invocation, Predicate<ExecutedRoutedEventArgs> predicate)
        {
            var frame = new DispatcherFrame();
            var hasCalled = false;
            var result = false;
            
            ExecutedRoutedEventHandler onExecute = (s, e) =>
                {
                    hasCalled = true;
                    result = predicate(e);
                    frame.Continue = false;
                };

            this.commandContext.CommandBindings.Add(new CommandBinding(command, onExecute));

            using (new Timer(s => frame.Continue = false, null, 1000, Timeout.Infinite))
            {
                invocation();
                Dispatcher.PushFrame(frame);
            }

            hasCalled.ShouldBeTrue();
            result.ShouldBeTrue();
        }

        /// <summary>
        /// Asserts the command was not executed.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="invocation">The invocation.</param>
        private void AssertCommandWasNotExecuted(ICommand command, Action invocation)
        {
            var frame = new DispatcherFrame();
            var hasCalled = false;

            ExecutedRoutedEventHandler onExecute = (s, e) =>
            {
                hasCalled = true;
                frame.Continue = false;
            };

            this.commandContext.CommandBindings.Add(new CommandBinding(command, onExecute));

            using (new Timer(s => frame.Continue = false, null, 1000, Timeout.Infinite))
            {
                invocation();
                Dispatcher.PushFrame(frame);
            }

            hasCalled.ShouldBeFalse();
        }

        /// <summary>
        /// Generates the project selector loader.
        /// </summary>
        /// <returns>The loader instance.</returns>
        private ILoaderWithVolumeCheck GenerateLoaderAndAttachToService()
        {
            var loader = MockRepository.GenerateMock<ILoaderWithVolumeCheck>();

            this.projectSelectorService
                .Expect(pss => pss.CreateLoader(null))
                .IgnoreArguments()
                .Return(loader)
                .Repeat.Any();

            return loader;
        }

        /// <summary>
        /// Generates the project selector service.
        /// </summary>
        /// <returns>An instance of the project selector service.</returns>
        private IProjectSelectorService GenerateProjectSelectorService()
        {
            var selectorService = MockRepository.GenerateMock<IProjectSelectorService>();

            selectorService
                .Expect(ss => ss.GetLastProjectData())
                .WhenCalled(mi => mi.ReturnValue = this.lastProjectData)
                .Return(null)
                .Repeat.Any();

            return selectorService;
        }

        /// <summary>
        /// Initialises the view model with service and application controller.
        /// </summary>
        private void InitialiseViewModelWithNullProjectData()
        {
            this.InitialiseViewModelWithSpecifiedProjectData(null);
        }

        /// <summary>
        /// Initiaises the view model with service and project data.
        /// </summary>
        private void InitialiseViewModelWithProjectData()
        {
            this.InitialiseViewModelWithSpecifiedProjectData(ProjectDataGenerationHelper.GenerateProjectData());
        }

        /// <summary>
        /// Initialises the view model with specified project data.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        private void InitialiseViewModelWithSpecifiedProjectData(IProjectData projectData)
        {
            this.viewModelUnderTest = new ProjectSelectorViewModel(projectData, this.projectSelectorService, this.projectDataService);
        }

        /// <summary>
        /// Sets the property changed listener.
        /// </summary>
        private void SetPropertyChangedListener()
        {
            this.viewModelUnderTest.PropertyChanged += (s, e) => this.propertyChangedNotifcations.Add(e.PropertyName);
        }

        /// <summary>
        /// Asserts the last property changed was.
        /// </summary>
        /// <param name="expectedPropertyName">Expected name of the property.</param>
        private void AssertLastPropertyChangedWas(string expectedPropertyName)
        {
            Assert.AreEqual(this.propertyChangedNotifcations.Last(), expectedPropertyName);
        }

        /// <summary>
        /// Asserts the commands are generated.
        /// </summary>
        private void AssertCommandsAreGenerated()
        {
            this.viewModelUnderTest.CancelCommand.ShouldNotBeNull();
            this.viewModelUnderTest.LoadProjectDataCommand.ShouldNotBeNull();
            this.viewModelUnderTest.ShowProjectSelectorCommand.ShouldNotBeNull();
            this.viewModelUnderTest.EnsureProjectNodesLoadedCommand.ShouldNotBeNull();
        }

        /// <summary>
        /// Asserts the project path roots have been set.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        private void AssertProjectRootNodesHaveBeenSet(IProjectData projectData)
        {
            this.viewModelUnderTest.AreaRootNode.ShouldEqual(projectData.ProjectNodes[Settings.Default.AreaPathFieldName]);
            this.viewModelUnderTest.IterationRootNode.ShouldEqual(projectData.ProjectNodes[Settings.Default.IterationPathFieldName]);
        }

        /// <summary>
        /// Asserts the project path roots have been cleared.
        /// </summary>
        private void AssertProjectRootNodesAreNull()
        {
            this.viewModelUnderTest.AreaRootNode.ShouldBeNull();
            this.viewModelUnderTest.IterationRootNode.ShouldBeNull();
        }

        /// <summary>
        /// Asserts the project selection data has been set.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        private void AssertProjectSelectionDataHasBeenSet(IProjectData projectData)
        {
            this.viewModelUnderTest.ProjectData.ShouldEqual(projectData);
            this.viewModelUnderTest.AreaPath.ShouldEqual(projectData.ProjectAreaPath);
            this.viewModelUnderTest.IterationPath.ShouldEqual(projectData.ProjectIterationPath);
            this.viewModelUnderTest.SelectedProjectName.ShouldEqual(projectData.ProjectName);
            this.viewModelUnderTest.SelectedCollectionEndPoint.ShouldEqual(projectData.ProjectCollectionUrl);
        }

        /// <summary>
        /// Asserts the project data properties are cleared.
        /// </summary>
        private void AssertProjectDataPropertiesAreCleared()
        {
            this.viewModelUnderTest.ProjectData.ShouldBeNull();
            this.viewModelUnderTest.AreaPath.ShouldBeNull();
            this.viewModelUnderTest.IterationPath.ShouldBeNull();
            this.viewModelUnderTest.SelectedProjectName.ShouldBeNull();
            this.viewModelUnderTest.SelectedCollectionEndPoint.ShouldBeNull();
        }
    }
}
