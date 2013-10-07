// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModelFixture.cs" company="None">
//   Crispin Parker 2011
// </copyright>
// <summary>
//   Defines the mainViewModelFixture type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.VersionCheck.Tests
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;

    using NUnit.Framework;

    using Rhino.Mocks;

    using SharpArch.Testing.NUnit;

    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.Core.Services;
    using TfsWorkbench.VersionCheck.Iterfaces;
    using TfsWorkbench.VersionCheck.Models;
    using TfsWorkbench.VersionCheck.Properties;
    using TfsWorkbench.VersionCheck.Services;
    using TfsWorkbench.VersionCheck.ViewModels;

    /// <summary>
    /// The menu option view model test fixture class.
    /// </summary>
    [TestFixture]
    public class MainViewModelFixture
    {
        /// <summary>
        /// The application context service instance.
        /// </summary>
        private IApplicationContextService applicationContextService;

        /// <summary>
        /// The version check service.
        /// </summary>
        private IVersionCheckService versionCheckService;

        /// <summary>
        /// The main view model instance.
        /// </summary>
        private MainViewModel viewModel;

        /// <summary>
        /// Sets up the test environemnt.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            this.applicationContextService = MockRepository.GenerateMock<IApplicationContextService>();
            this.versionCheckService = MockRepository.GenerateMock<IVersionCheckService>();
            this.viewModel = new MainViewModel(this.applicationContextService, this.versionCheckService);
        }

        /// <summary>
        /// Constructor, with no parameters, calls service manager.
        /// </summary>
        [Test]
        public void Constructor_WithNoParameters_CallsServiceManager()
        {
            // Arrange
            var serviceManager = MockRepository.GenerateMock<IServiceManager>();

            serviceManager
                .Expect(sm => sm.GetService<IApplicationContextService>())
                .Return(this.applicationContextService)
                .Repeat.Once();

            serviceManager
                .Expect(sm => sm.GetService<IVersionCheckService>())
                .Return(this.versionCheckService)
                .Repeat.Once();

            // Act
            ServiceManager.Instance = serviceManager;
            new MainViewModel();
            ServiceManager.Instance = null;

            // Assert
            serviceManager.VerifyAllExpectations();
        }

        /// <summary>
        /// Constructor, called with null parameters, throws an exception.
        /// </summary>
        [Test]
        public void Constructor_WithNullParameters_ThrowsException()
        {
            // Act
            try
            {
                new MainViewModel(null, null);
                Assert.Fail("Expected exception not thrown");
            }
            catch (ArgumentNullException)
            {
            }

            try
            {
                new MainViewModel(this.applicationContextService, null);
                Assert.Fail("Expected exception not thrown");
            }
            catch (ArgumentNullException)
            {
            }

            try
            {
                new MainViewModel(null, this.versionCheckService);
                Assert.Fail("Expected exception not thrown");
            }
            catch (ArgumentNullException)
            {
            }

            // Assert
            Assert.Pass();
        }

        /// <summary>
        /// Constructor, with valid parameters, generates the Version Check Command.
        /// </summary>
        [Test]
        public void Constructor_WithValidParameters_GeneratesVersionCheckCommand()
        {
            // Act
            var command = this.viewModel.VersionCheckCommand;

            // Assert
            command.ShouldNotBeNull();
            command.DisplayName.ShouldEqual(Resources.String007);
            command.Command.ShouldNotBeNull();
        }

        /// <summary>
        /// Constructor, with valid parameters, generates the Go To Download Page Command.
        /// </summary>
        [Test]
        public void Constructor_WithValidParameters_GeneratesGoToDownloadPageCommand()
        {
            // Act
            var command = this.viewModel.GoToDownloadPageCommand;

            // Assert
            command.ShouldNotBeNull();
            command.DisplayName.ShouldEqual(Resources.String008);
            command.Command.ShouldNotBeNull();
        }

        /// <summary>
        /// Executing the version check command, when the view model is ready, calls the version check service.
        /// </summary>
        [Test]
        public void ExecuteVersionCheckCommand_WhenModelReady_CallsVersionCheckService()
        {
            // Arrange
            this.SetVersionCheckServiceExpectations();

            // Act
            this.BeginVersionCheck();

            // Assert
            this.versionCheckService.VerifyAllExpectations();
        }

        /// <summary>
        /// Execute Version Check Command, When previous call is incomplete, does not call the VersionCheckService.
        /// </summary>
        [Test]
        public void ExecuteVersionCheckCommand_WhenPreviousCallIsIncomplete_DoesNotCallVersionCheckService()
        {
            // Arrange
            this.versionCheckService
                .Expect(vcs => vcs.BeginAsyncGetVersionStatus(null))
                .IgnoreArguments()
                .Repeat.Once();

            // Act
            this.BeginVersionCheck();

            // This call should not hit the service becuase the previous call has not called back.
            this.BeginVersionCheck();

            // Assert
            this.versionCheckService.VerifyAllExpectations();
        }

        /// <summary>
        /// Execute version check service, when previous call completed, calls the version check service.
        /// </summary>
        [Test]
        public void ExecuteVersionCheckCommand_WhenPreviousCallCompleted_CallsVersionCheckService()
        {
            // Arrange
            Action<VersionStatus, Exception> callBack = null;

            this.versionCheckService
                .Expect(vcs => vcs.BeginAsyncGetVersionStatus(null))
                .IgnoreArguments()
                .WhenCalled(mi => callBack = mi.Arguments.First() as Action<VersionStatus, Exception>)
                .Repeat.Twice();

            // Act
            this.BeginVersionCheck();
            callBack(new UnknownVersionStatus(), null);
            this.BeginVersionCheck();

            // Assert
            this.versionCheckService.VerifyAllExpectations();
        }

        /// <summary>
        /// Execute version check command, when successful, reports the returned status to the application.
        /// </summary>
        [Test]
        public void ExecuteVersionCheckCommand_WhenSuccesful_ReportsStatusToApplication()
        {
            // Arrange
            var messages = new Collection<string>();
            var versionStatus = new UnknownVersionStatus();
            this.SetVersionCheckServiceExpectations(versionStatus);

            this.applicationContextService
                .Expect(ac => ac.SendApplicationMessage(null))
                .IgnoreArguments()
                .WhenCalled(mi => messages.Add(mi.Arguments.First() as string))
                .Repeat.Any();

            // Act
            this.BeginVersionCheck();

            // Assert
            messages.Last().ShouldEqual(versionStatus.DisplayMessage);

            foreach (var message in messages)
            {
                Debug.WriteLine(message);
            }
        }

        /// <summary>
        /// Execute version check command, when version check fails, reports the exception to the application.
        /// </summary>
        [Test]
        public void ExecuteVersionCheckCommand_WhenVersionCheckFails_ReportsExceptionToApplication()
        {
            // Arrange
            var exceptionToReturn = new Exception();
            this.SetVersionCheckServiceExpectations(null, exceptionToReturn);

            Exception exception = null;

            this.applicationContextService
                .Expect(ac => ac.SendApplciationError(null))
                .IgnoreArguments()
                .WhenCalled(mi => exception = mi.Arguments.First() as Exception)
                .Repeat.Any();

            // Act
            this.BeginVersionCheck();

            // Assert
            exception.ShouldEqual(exceptionToReturn);

            Debug.WriteLine(exception.Message);
        }

        /// <summary>
        /// Execute version check command, when version out of date, offers open download page option.
        /// </summary>
        [Test]
        public void ExecuteVersionCheckCommand_WhenVersionIsOutOfDate_OffersOpenDownloadPageOption()
        {
            // Arrange
            this.SetVersionCheckServiceExpectations(new OutOfDateVersionStatus());
            this.applicationContextService.Expect(ac => ac.DoesUserWantToOpenDownloadPage(null)).IgnoreArguments().Repeat.Once();

            // Act
            this.BeginVersionCheck();

            // Assert
            this.applicationContextService.VerifyAllExpectations();
        }

        /// <summary>
        /// Execute version check command, when version is out of date and user disables start up check, sets start up check to false.
        /// </summary>
        [Test]
        public void ExecuteVersionCheckCommand_WhenUserDisablesStartUpCheckThroughDialog_SetsStartUpCheckToFalse()
        {
            // Arrange
            this.viewModel.CheckVersionOnStartUp = true;
            this.SetVersionCheckServiceExpectations(new OutOfDateVersionStatus());
            this.applicationContextService
                .Expect(ac => ac.DoesUserWantToOpenDownloadPage(null))
                .IgnoreArguments()
                .WhenCalled(mi => ((Action<bool, bool>)mi.Arguments.First())(false, false))
                .Repeat.Any();

            // Act
            this.BeginVersionCheck();

            // Assert
            this.viewModel.CheckVersionOnStartUp.ShouldEqual(false);
        }

        /// <summary>
        /// Execute version check command, when version out of date, handles user download page display choice.
        /// </summary>
        [Test]
        public void ExecuteVersionCheckCommand_WhenVersionIsOutOfDate_HandlesUserDownloadPageDisplayChoice()
        {
            // Arrange
            var userWantsToOpenDownloadPage = false;
            this.SetVersionCheckServiceExpectations(new OutOfDateVersionStatus());

            this.applicationContextService
                .Expect(ac => ac.DoesUserWantToOpenDownloadPage(null))
                .IgnoreArguments()
                .WhenCalled(mi => ((Action<bool, bool>)mi.Arguments.First())(userWantsToOpenDownloadPage = !userWantsToOpenDownloadPage, Settings.Default.CheckOnStartUp));

            this.applicationContextService
                .Expect(ac => ac.SendSystemShellCommand(null))
                .IgnoreArguments()
                .Repeat.Once();

            // Act
            
            // First call; user response is "True"
            this.BeginVersionCheck();

            // Second call; user response is "False"
            this.BeginVersionCheck();

            // Assert
            this.applicationContextService.VerifyAllExpectations();
        }

        /// <summary>
        /// Execute go to download page command, shells out to the expected url.
        /// </summary>
        [Test]
        public void ExecuteGoToDownloadPageCommand_ShellsOutToExpectedUrl()
        {
            // Arrange
            var openDownloadPageCommand = this.viewModel.GoToDownloadPageCommand;

            this.applicationContextService
                .Expect(ac => ac.SendSystemShellCommand(Settings.Default.DownloadUrl))
                .Repeat.Once();

            // Act
            openDownloadPageCommand.Command.Execute(null);

            // Assert
            this.applicationContextService.VerifyAllExpectations();
        }

        /// <summary>
        /// Version check command, when version check is in progress, cannnot be executed again.
        /// </summary>
        [Test]
        public void VersionCheckCommand_WhenVersionCheckInProgress_CannotBeExecuted()
        {
            // Arrange
            var versionCheckCommand = this.viewModel.VersionCheckCommand;
            Action<VersionStatus, Exception> callBack = null;

            this.versionCheckService
                .Expect(vcs => vcs.BeginAsyncGetVersionStatus(null))
                .IgnoreArguments()
                .WhenCalled(mi => callBack = mi.Arguments.First() as Action<VersionStatus, Exception>)
                .Repeat.Any();

            // Act
            var canExecuteBefore = versionCheckCommand.Command.CanExecute(null);
            this.BeginVersionCheck();
            var canExecuteDuring = versionCheckCommand.Command.CanExecute(null);
            
            // Call back to the view model to complete request.
            callBack(new UnknownVersionStatus(), null);

            var canExecuteAfter = versionCheckCommand.Command.CanExecute(null);

            // Assert
            canExecuteBefore.ShouldBeTrue();
            canExecuteDuring.ShouldBeFalse();
            canExecuteAfter.ShouldBeTrue();
        }

        /// <summary>
        /// Execute version check, suggests command requery, during and after check.
        /// </summary>
        [Test]
        public void ExecuteVersionCheck_SuggestsCommandRequery_DuringAndAfterCheck()
        {
            // Arrange
            this.applicationContextService
                .Expect(acs => acs.SuggestCommandRequery())
                .Repeat.Twice();

            Action<VersionStatus, Exception> callBack = null;

            this.versionCheckService
                .Expect(vcs => vcs.BeginAsyncGetVersionStatus(null))
                .IgnoreArguments()
                .WhenCalled(mi => callBack = mi.Arguments.First() as Action<VersionStatus, Exception>)
                .Repeat.Any();

            // Act
            this.BeginVersionCheck();

            // Call back to the view model to complete request.
            callBack(new UnknownVersionStatus(), null);

            // Assert
            this.applicationContextService.VerifyAllExpectations();
        }

        /// <summary>
        /// Check version on start up, when settings alternative value, updates the user settings.
        /// </summary>
        [Test]
        public void CheckVersionOnStartUp_WhenSettingAlterativeValuie_UpdatesSettings()
        {
            // Arrange
            var initialValue = Settings.Default.CheckOnStartUp;

            // Act
            this.viewModel.CheckVersionOnStartUp = !initialValue;
            var newValue = Settings.Default.CheckOnStartUp;
            Settings.Default.CheckOnStartUp = initialValue;

            // Assert
            newValue.ShouldEqual(!initialValue);
        }

        /// <summary>
        /// Check version on start up, when setting alternate value, raises the proerty changed event.
        /// </summary>
        [Test]
        public void CheckVersionOnStartUp_WhenSettingAlterativeValuie_RaisesPropertyChangedEvent()
        {
            // Arrange
            const string ExpectedPropertyName = "CheckVersionOnStartUp";
            var initialValue = Settings.Default.CheckOnStartUp;

            var hasRaisedEvent = false;
            var isExpectedPropertyName = false;

            this.viewModel.PropertyChanged += (s, e) =>
                {
                    hasRaisedEvent = true;
                    isExpectedPropertyName = e.PropertyName.Equals(ExpectedPropertyName);
                };

            // Act
            this.viewModel.CheckVersionOnStartUp = !initialValue;
            Settings.Default.CheckOnStartUp = initialValue;

            // Assert
            hasRaisedEvent.ShouldBeTrue();
            isExpectedPropertyName.ShouldBeTrue();
        }

        /// <summary>
        /// Check version on start up, when setting same value, does not raise the property change event.
        /// </summary>
        [Test]
        public void CheckVersionOnStartUp_WhenSettingSameValue_DoesNotRaisesPropertyChangedEvent()
        {
            // Arrange
            var initialValue = this.viewModel.CheckVersionOnStartUp;

            var hasRaisedEvent = false;

            this.viewModel.PropertyChanged += (s, e) => hasRaisedEvent = true;

            // Act
            this.viewModel.CheckVersionOnStartUp = initialValue;

            // Assert
            hasRaisedEvent.ShouldBeFalse();
        }

        /// <summary>
        /// Menu header text returns expected text.
        /// </summary>
        [Test]
        public void MenuHeaderText_ReturnsExpectedText()
        {
            // Arrange
            var expected = Resources.String012;

            // Act
            var result = this.viewModel.MenuHeaderText;

            // Assert
            result.ShouldEqual(expected);
        }

        /// <summary>
        /// Toggle Start Up Check HeaderText, returns expected text.
        /// </summary>
        [Test]
        public void ToggleStartUpCheckHeaderText_ReturnsExpectedText()
        {
            // Arrange
            var expected = Resources.String013;

            // Act
            var result = this.viewModel.ToggleStartUpCheckHeaderText;

            // Assert
            result.ShouldEqual(expected);
        }

        /// <summary>
        /// Buttons the tool tip, initial value, returns expected text.
        /// </summary>
        [Test]
        public void ButtonToolTip_InitialValue_ReturnsExpectedText()
        {
            // Arrange
            var expected = Resources.String007;

            // Act
            var result = this.viewModel.ButtonToolTip;

            // Assert
            result.ShouldEqual(expected);
        }

        /// <summary>
        /// Button tool tip, after successful version check, returns expected text.
        /// </summary>
        [Test]
        public void ButtonToolTip_AfterSuccessfulVersionCheck_ReturnsExpectedText()
        {
            // Arrange
            var versionStatus = new UptoDateVersionStatus();
            this.SetVersionCheckServiceExpectations(versionStatus);

            // Act
            this.BeginVersionCheck();
            var result = this.viewModel.ButtonToolTip;

            // Assert
            result.ShouldEqual(versionStatus.DisplayMessage);
        }

        /// <summary>
        /// Button tool tip, after unhandled version check exception, returns expected text.
        /// </summary>
        [Test]
        public void ButtonToolTip_AfterUnhandledVersionCheckException_ReturnsExpectedText()
        {
            // Arrange
            const string ExceptionMessage = "Exception Message";
            this.SetVersionCheckServiceExpectations(null, new Exception(ExceptionMessage));

            // Act
            this.BeginVersionCheck();
            var result = this.viewModel.ButtonToolTip;

            // Assert
            result.ShouldEqual(ExceptionMessage);
        }

        /// <summary>
        /// Button content text, returns expected text.
        /// </summary>
        [Test]
        public void ButtonContentText_ReturnsExpectedText()
        {
            // Arrange
            var expected = Resources.String001;

            // Act
            var result = this.viewModel.ButtonContentText;

            // Assert
            result.ShouldEqual(expected);
        }

        [Test]
        public void ShowButton_WhenSettingAlterativeValue_UpdatesSettings()
        {
            // Arrange
            var initialValue = Settings.Default.ShowButton;

            // Act
            this.viewModel.ShowButton = !initialValue;
            var newValue = Settings.Default.ShowButton;
            Settings.Default.ShowButton = initialValue;

            // Assert
            newValue.ShouldEqual(!initialValue);
        }

        [Test]
        public void ShowButton_WhenSettingAlterativeValuie_RaisesPropertyChangedEvent()
        {
            // Arrange
            const string ExpectedPropertyName = "ShowButton";
            var initialValue = Settings.Default.ShowButton;

            var hasRaisedEvent = false;
            var isExpectedPropertyName = false;

            this.viewModel.PropertyChanged += (s, e) =>
            {
                hasRaisedEvent = true;
                isExpectedPropertyName = e.PropertyName.Equals(ExpectedPropertyName);
            };

            // Act
            this.viewModel.ShowButton = !initialValue;
            Settings.Default.ShowButton = initialValue;

            // Assert
            hasRaisedEvent.ShouldBeTrue();
            isExpectedPropertyName.ShouldBeTrue();
        }

        /// <summary>
        /// Check version on start up, when setting same value, does not raise the property change event.
        /// </summary>
        [Test]
        public void ShowButton_WhenSettingSameValue_DoesNotRaisesPropertyChangedEvent()
        {
            // Arrange
            var initialValue = this.viewModel.ShowButton;

            var hasRaisedEvent = false;

            this.viewModel.PropertyChanged += (s, e) => hasRaisedEvent = true;

            // Act
            this.viewModel.ShowButton = initialValue;

            // Assert
            hasRaisedEvent.ShouldBeFalse();
        }
        
        [Test]
        public void ShowButtonHeaderText_ReturnsExpectedText()
        {
            // Arrange
            var expectedText = Resources.String016;

            // Act
            var actualText = this.viewModel.ShowButtonHeaderText;

            // Assert
            actualText.ShouldEqual(expectedText);
        }

        /// <summary>
        /// Creates the version check service.
        /// </summary>
        /// <param name="statusToReturn">The status to return.</param>
        /// <param name="exceptionToReturn">The exception to return.</param>
        /// <param name="expectedRepeats">The expected repeats.</param>
        private void SetVersionCheckServiceExpectations(VersionStatus statusToReturn = null, Exception exceptionToReturn = null, int expectedRepeats = 1)
        {
            this.versionCheckService
                .Expect(s => s.BeginAsyncGetVersionStatus(null))
                .IgnoreArguments()
                .WhenCalled(mi => ((Action<VersionStatus, Exception>)mi.Arguments.First())(statusToReturn ?? new UnknownVersionStatus(), exceptionToReturn))
                .Repeat.Times(expectedRepeats, expectedRepeats);
        }

        /// <summary>
        /// Begins the version check.
        /// </summary>
        private void BeginVersionCheck()
        {
            this.viewModel.ExecuteVersionCheck();
        }
    }
}
