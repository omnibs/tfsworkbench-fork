// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MenuOptionViewModelFixture.cs" company="None">
//   Crispin Parker 2011
// </copyright>
// <summary>
//   Defines the MenuOptionViewModelFixture type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.VersionCheck.Tests
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;

    using NUnit.Framework;

    using Rhino.Mocks;

    using SharpArch.Testing.NUnit;

    using TfsWorkbench.VersionCheck.Iterfaces;
    using TfsWorkbench.VersionCheck.Properties;
    using TfsWorkbench.VersionCheck.ViewModel;

    /// <summary>
    /// The menu option view model test fixture class.
    /// </summary>
    [TestFixture]
    public class MenuOptionViewModelFixture
    {
        /// <summary>
        /// Constructure, called with null parameters, throws an exception.
        /// </summary>
        [Test]
        public void Constructor_WithNullParameters_ThrowsException()
        {
            // Arrange
            var service = MockRepository.GenerateMock<IVersionCheckService>();
            var appContext = MockRepository.GenerateMock<IAppContext>();

            // Act
            try
            {
                new MenuOptionViewModel(null, null);
                Assert.Fail("Expected exception not thrown");
            }
            catch (ArgumentNullException)
            {
            }

            try
            {
                new MenuOptionViewModel(service, null);
                Assert.Fail("Expected exception not thrown");
            }
            catch (ArgumentNullException)
            {
            }

            try
            {
                new MenuOptionViewModel(null, appContext);
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
            // Arrange
            var service = MockRepository.GenerateMock<IVersionCheckService>();
            var appContext = MockRepository.GenerateMock<IAppContext>();
            var viewModel = new MenuOptionViewModel(service, appContext);

            // Act
            var command = viewModel.VersionCheckCommand;

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
            // Arrange
            var service = MockRepository.GenerateMock<IVersionCheckService>();
            var appContext = MockRepository.GenerateMock<IAppContext>();
            var viewModel = new MenuOptionViewModel(service, appContext);

            // Act
            var command = viewModel.GoToDownloadPageCommand;

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
            var service = MockRepository.GenerateMock<IVersionCheckService>();
            var appContext = MockRepository.GenerateMock<IAppContext>();
            var menuOptionViewModel = new MenuOptionViewModel(service, appContext);
            var command = menuOptionViewModel.VersionCheckCommand;

            service
                .Expect(s => s.GetVersionStatus())
                .Return(null)
                .Repeat.Once();

            // Act
            command.Command.Execute(null);

            WaitForVersionCheckToFinish(menuOptionViewModel);

            // Assert
            service.VerifyAllExpectations();
        }

        /// <summary>
        /// Execute Version Check Command, When previous call is incomplete, does not call the VersionCheckService.
        /// </summary>
        [Test]
        public void ExecuteVersionCheckCommand_WhenPreviousCallIsIncomplete_DoesNotCallVersionCheckService()
        {
            // Arrange
            var service = MockRepository.GenerateMock<IVersionCheckService>();
            var appContext = MockRepository.GenerateMock<IAppContext>();
            var menuOptionViewModel = new MenuOptionViewModel(service, appContext);
            var versionCheckCommand = menuOptionViewModel.VersionCheckCommand;
            var viewModelWaitHandle = new AutoResetEvent(false);

            service
                .Expect(s => s.GetVersionStatus())
                .WhenCalled(mi => viewModelWaitHandle.Set())
                .Return(null)
                .Repeat.Once();

            // Act
            versionCheckCommand.Command.Execute(null);

            // Wait for the service to be called.
            viewModelWaitHandle.WaitOne(1000);

            // Second call should not be passed to service because first call has not yet returned.
            versionCheckCommand.Command.Execute(null);

            WaitForVersionCheckToFinish(menuOptionViewModel);

            // Assert
            service.VerifyAllExpectations();
        }

        /// <summary>
        /// Execute version check service, when previous call completed, calls the version check service.
        /// </summary>
        [Test]
        public void ExecuteVersionCheckCommand_WhenPreviousCallCompleted_CallsVersionCheckService()
        {
            // Arrange
            var service = MockRepository.GenerateMock<IVersionCheckService>();
            var appContext = MockRepository.GenerateMock<IAppContext>();
            var menuOptionViewModel = new MenuOptionViewModel(service, appContext);
            var versionCheckCommand = menuOptionViewModel.VersionCheckCommand;

            service
                .Expect(s => s.GetVersionStatus())
                .Return(null)
                .Repeat.Twice();

            // Act
            versionCheckCommand.Command.Execute(null);
            WaitForVersionCheckToFinish(menuOptionViewModel); 
            versionCheckCommand.Command.Execute(null);
            WaitForVersionCheckToFinish(menuOptionViewModel);

            // Assert
            service.VerifyAllExpectations();
        }

        /// <summary>
        /// Execute version check command, when successful, reports the returned status to the application.
        /// </summary>
        [Test]
        public void ExecuteVersionCheckCommand_WhenSuccesful_ReportsStatusToApplication()
        {
            // Arrange
            const string StatusMessage = "Returned Status Message";
            var messages = new Collection<string>();
            var service = MockRepository.GenerateMock<IVersionCheckService>();
            var appContext = MockRepository.GenerateMock<IAppContext>();
            var menuOptionViewModel = new MenuOptionViewModel(service, appContext);
            var versionCheckCommand = menuOptionViewModel.VersionCheckCommand;

            service
                .Expect(s => s.GetVersionStatus())
                .Return(new Model.VersionStatus { DisplayMessage = StatusMessage })
                .Repeat.Any();

            appContext
                .Expect(ac => ac.SendApplicationMessage(null))
                .IgnoreArguments()
                .WhenCalled(mi => messages.Add(mi.Arguments.First() as string))
                .Repeat.Any();

            // Act
            versionCheckCommand.Command.Execute(null);
            WaitForVersionCheckToFinish(menuOptionViewModel);

            // Assert
            messages.Last().ShouldEqual(StatusMessage);

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
            const string ExceptionMessage = "Unhandled Exception In Service";
            var service = MockRepository.GenerateMock<IVersionCheckService>();
            var appContext = MockRepository.GenerateMock<IAppContext>();
            var menuOptionViewModel = new MenuOptionViewModel(service, appContext);
            var versionCheckCommand = menuOptionViewModel.VersionCheckCommand;

            service
                .Expect(s => s.GetVersionStatus())
                .Throw(new Exception(ExceptionMessage));

            Exception exception = null;

            appContext
                .Expect(ac => ac.SendApplciationError(null))
                .IgnoreArguments()
                .WhenCalled(mi => exception = mi.Arguments.First() as Exception)
                .Repeat.Any();

            // Act
            versionCheckCommand.Command.Execute(null);
            WaitForVersionCheckToFinish(menuOptionViewModel);

            // Assert
            exception.ShouldNotBeNull();
            exception.Message.ShouldEqual(ExceptionMessage);

            Debug.WriteLine(exception.Message);
        }

        /// <summary>
        /// Execute version check command, when version out of date, offers open download page option.
        /// </summary>
        [Test]
        public void ExecuteVersionCheckCommand_WhenVersionIsOutOfDate_OffersOpenDownloadPageOption()
        {
            // Arrange
            var service = MockRepository.GenerateMock<IVersionCheckService>();
            var appContext = MockRepository.GenerateMock<IAppContext>();
            var menuOptionViewModel = new MenuOptionViewModel(service, appContext);
            var versionCheckCommand = menuOptionViewModel.VersionCheckCommand;

            service
                .Expect(s => s.GetVersionStatus())
                .Return(new Model.VersionStatus { Status = Service.VersionStatusOption.OutDated });

            appContext.Expect(ac => ac.UserWantsToOpenDownloadPage()).Return(false).Repeat.Once();

            // Act
            versionCheckCommand.Command.Execute(null);
            WaitForVersionCheckToFinish(menuOptionViewModel);

            // Assert
            appContext.VerifyAllExpectations();
        }

        /// <summary>
        /// Execute version check command, when version out of date, handles user download page display choice.
        /// </summary>
        [Test]
        public void ExecuteVersionCheckCommand_WhenVersionIsOutOfDate_HandlesUserDownloadPageDisplayChoice()
        {
            // Arrange
            var service = MockRepository.GenerateMock<IVersionCheckService>();
            var appContext = MockRepository.GenerateMock<IAppContext>();
            var menuOptionViewModel = new MenuOptionViewModel(service, appContext);
            var versionCheckCommand = menuOptionViewModel.VersionCheckCommand;

            service
                .Expect(s => s.GetVersionStatus())
                .Return(new Model.VersionStatus { Status = Service.VersionStatusOption.OutDated })
                .Repeat.Any();

            appContext
                .Expect(ac => ac.UserWantsToOpenDownloadPage())
                .WhenCalled(mi => mi.ReturnValue = !(bool)mi.ReturnValue)
                .Return(false);

            appContext
                .Expect(ac => ac.SendSystemShellCommand(null))
                .IgnoreArguments()
                .Repeat.Once();

            // Act
            
            // First call; user response is "True"
            versionCheckCommand.Command.Execute(null);
            WaitForVersionCheckToFinish(menuOptionViewModel);

            // Second call; user response is "False"
            versionCheckCommand.Command.Execute(null);
            WaitForVersionCheckToFinish(menuOptionViewModel);

            // Assert
            appContext.VerifyAllExpectations();
        }

        /// <summary>
        /// Execute go to download page command, shells out to the expected url.
        /// </summary>
        [Test]
        public void ExecuteGoToDownloadPageCommand_ShellsOutToExpectedUrl()
        {
            // Arrange
            var service = MockRepository.GenerateMock<IVersionCheckService>();
            var appContext = MockRepository.GenerateMock<IAppContext>();
            var openDownloadPageCommand = new MenuOptionViewModel(service, appContext).GoToDownloadPageCommand;

            appContext
                .Expect(ac => ac.SendSystemShellCommand(Settings.Default.DownloadUrl))
                .Repeat.Once();

            // Act
            openDownloadPageCommand.Command.Execute(null);

            // Assert
            appContext.VerifyAllExpectations();
        }

        /// <summary>
        /// Waits for version check to finish.
        /// </summary>
        /// <param name="menuOptionViewModel">The menu option view model.</param>
        private static void WaitForVersionCheckToFinish(MenuOptionViewModel menuOptionViewModel)
        {
            const int LoopLimit = 10;
            var loopCount = 0;

            while (menuOptionViewModel.VersionCheckInProgress && loopCount < LoopLimit)
            {
                Thread.Sleep(100);
                loopCount++;
            }
        }
    }
}
