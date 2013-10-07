// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationContextServiceFixture.cs" company="None">
//   Crispin Parker 2011
// </copyright>
// <summary>
//   Defines the ApplicationContextFactoryFixture type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.VersionCheck.Tests
{
    using System;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;

    using NUnit.Framework;

    using Rhino.Mocks;

    using SharpArch.Testing.NUnit;

    using TfsWorkbench.UIElements;
    using TfsWorkbench.VersionCheck.Iterfaces;
    using TfsWorkbench.VersionCheck.Services;

    /// <summary>
    /// The applciation context factory test fixture class.
    /// </summary>
    [TestFixture]
    [Serializable]
    public class ApplicationContextServiceFixture
    {
        /// <summary>
        /// The dummy application instance.
        /// </summary>
        private Application dummyApplication;

        /// <summary>
        /// Sets up the tests fixture.
        /// </summary>
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            // Sets up the Application.Current parameter if missing.
            this.dummyApplication = Application.Current ?? MockRepository.GenerateMock<Application>();
        }

        /// <summary>
        /// Constructor, with out parameter, uses current application.
        /// </summary>
        [Test]
        public void Constructor_WithOutParameter_UsesCurrentApplication()
        {
            // Arrange
            var window = MockRepository.GenerateMock<Window>();
            this.dummyApplication.MainWindow = window;

            // Act
            new ApplicationContextService();

            // Assert
            Assert.Pass();
        }

        /// <summary>
        /// Constructor, with null parameter, throws exception.
        /// </summary>
        [Test]
        public void Constructor_WithNullParameter_ThrowsException()
        {
            // Act
            try
            {
                new ApplicationContextService(null);
                Assert.Fail("Exception not thrown");
            }
            catch (ArgumentNullException)
            {
            }

            // Assert
            Assert.Pass();
        }

        /// <summary>
        /// Constructor, when no application main window, throws exception.
        /// </summary>
        [Test]
        public void Constructor_WhenNoApplicationMainWindow_ThrowsException()
        {
            // Arrange
            this.dummyApplication.MainWindow = null;

            // Act
            try
            {
                this.CreateDefaultApplicationContext();
                Assert.Fail("Exception not thrown");
            }
            catch (ArgumentException)
            {
            }

            // Assert
            Assert.Pass();
        }
        
        /// <summary>
        /// Send applciaiton message, when using the default context, should execute the expected application command.
        /// </summary>
        [Test]
        public void SendApplicaitonMessage_WhenUsingDefaultContext_ShouldExecuteCommand()
        {
            // Arrange
            const string MessageToSend = "Test Message";
            var messageReceived = string.Empty;

            // Act
            this.ExecuteCommandAndPushDispatcher(
                CommandLibrary.ApplicationMessageCommand, 
                e => messageReceived = e.Parameter as string, 
                a => a.SendApplicationMessage(MessageToSend));

            // Assert
            messageReceived.ShouldEqual(MessageToSend);
        }

        /// <summary>
        /// Send application exception, when using default context, should execute expected command.
        /// </summary>
        [Test]
        public void SendApplicaitonException_WhenUsingDefaultContext_ShouldExecuteCommand()
        {
            // Arrange
            var exception = new Exception("Test Exception");
            Exception exceptionReceived = null;

            // Act
            this.ExecuteCommandAndPushDispatcher(
                CommandLibrary.ApplicationExceptionCommand, 
                e => exceptionReceived = e.Parameter as Exception, 
                a => a.SendApplciationError(exception));

            // Assert
            exceptionReceived.ShouldEqual(exception);
        }

        /// <summary>
        /// Send system shell command, when using the default context, should execute the expected application command.
        /// </summary>
        [Test]
        public void SendSystemShellCommand_WhenUsingDefaultContext_ShouldExecuteCommand()
        {
            // Arrange
            const string ShellCommandToSend = "A shell command.";
            var shellCommandRecieved = string.Empty;

            // Act
            this.ExecuteCommandAndPushDispatcher(
                CommandLibrary.SystemShellCommand,
                e => shellCommandRecieved = e.Parameter as string,
                a => a.SendSystemShellCommand(ShellCommandToSend));

            // Assert
            shellCommandRecieved.ShouldEqual(ShellCommandToSend);
        }

        /// <summary>
        /// User wants to open download page, executes show dialog command.
        /// </summary>
        [Test]
        public void UserWantsToOpenDownloadPage_ExecutesShowDialogCommand()
        {
            // Arrange
            var hasCalledBack = false;
            Action<bool, bool> callBack = null;

            // Act
            this.ExecuteCommandAndPushDispatcher(
                CommandLibrary.ShowDialogCommand,
                e => { hasCalledBack = true; },
                a => a.DoesUserWantToOpenDownloadPage(callBack));

            // Assert
            hasCalledBack.ShouldBeTrue();
        }

        /// <summary>
        /// User wants to open download page, when user selects yes, calls back with positive value.
        /// </summary>
        [Test]
        public void UserWantsToOpenDownloadPage_WhenUserSelectsYes_CallsBackWithPositiveValue()
        {
            // Arrange
            var userDecision = false;
            Action<bool, bool> callBack = (decision, doStartUpCheck) => { userDecision = decision; };

            // Act
            this.ExecuteCommandAndPushDispatcher(
                CommandLibrary.ShowDialogCommand,
                e => InvokePrivateMethod(e.Parameter, "YesButton_OnClick"),
                a => a.DoesUserWantToOpenDownloadPage(callBack));

            // Assert
            userDecision.ShouldBeTrue();
        }

        /// <summary>
        /// User wants to open download page, when user selects no, calls back with negative value.
        /// </summary>
        [Test]
        public void UserWantsToOpenDownloadPage_WhenUserSelectsNo_CallsBackWithNegativeValue()
        {
            // Arrange
            var userDecision = true;
            Action<bool, bool> callBack = (decision, doStartUpCheck) => { userDecision = decision; };

            // Act
            this.ExecuteCommandAndPushDispatcher(
                CommandLibrary.ShowDialogCommand,
                e => InvokePrivateMethod(e.Parameter, "NoButton_OnClick"),
                a => a.DoesUserWantToOpenDownloadPage(callBack));

            // Assert
            userDecision.ShouldBeFalse();
        }

        /// <summary>
        /// User wants to open download page, when start up check altered, calls back with altered value.
        /// </summary>
        [Test]
        public void UserWantsToOpenDownloadPage_WhenStartUpCheckDisabled_CallsBackDisabledStatus()
        {
            // Arrange
            var doStartUpCheckResult = false;
            Action<bool, bool> callBack = (decision, doStartUpCheck) => { doStartUpCheckResult = doStartUpCheck; };

            // Act
            this.ExecuteCommandAndPushDispatcher(
                CommandLibrary.ShowDialogCommand,
                e =>
                    {
                        ((DecisionControl)e.Parameter).DoNotShowAgain = true;
                        InvokePrivateMethod(e.Parameter, "YesButton_OnClick");
                    },
                a => a.DoesUserWantToOpenDownloadPage(callBack));

            // Assert
            doStartUpCheckResult.ShouldBeTrue();
        }

        /// <summary>
        /// Suggests the command requery_ executes without error.
        /// </summary>
        [Test]
        public void SuggestCommandRequery_ExecutesWithoutError()
        {
            // Arrange
            this.dummyApplication.MainWindow = MockRepository.GenerateMock<Window>();
            var service = this.CreateDefaultApplicationContext();

            // Act
            service.SuggestCommandRequery();

            // Assert
            Assert.Pass();
        }

        /// <summary>
        /// Invokes the private method.
        /// </summary>
        /// <param name="containingObject">The containing object.</param>
        /// <param name="methodToInvoke">The method to invoke.</param>
        private static void InvokePrivateMethod(object containingObject, string methodToInvoke)
        {
            var methodInfo = containingObject.GetType().GetMethod(
                methodToInvoke, BindingFlags.NonPublic | BindingFlags.Instance);

            methodInfo.Invoke(containingObject, new object[] { null, null });
        }

        /// <summary>
        /// Gets the command binding.
        /// </summary>
        /// <param name="frame">The frame.</param>
        /// <param name="onCommandExecute">The on command execute.</param>
        /// <param name="commandToBind">The command to bind.</param>
        /// <returns>A command binding instance.</returns>
        private static CommandBinding CreateCommandBinding(
            DispatcherFrame frame,
            Action<ExecutedRoutedEventArgs> onCommandExecute, 
            ICommand commandToBind)
        {
            ExecutedRoutedEventHandler onExecute = (s, e) =>
                {
                    onCommandExecute(e);
                    frame.Continue = false;
                };

            return new CommandBinding(commandToBind, onExecute);
        }

        /// <summary>
        /// Creates the default application context.
        /// </summary>
        /// <returns>A new instance of the default applicaiton context type.</returns>
        private IApplicationContextService CreateDefaultApplicationContext()
        {
            return new ApplicationContextService(this.dummyApplication);
        }

        /// <summary>
        /// Invokes the specfied method with the default context.
        /// </summary>
        /// <param name="invocationMethod">The invocation method.</param>
        private void InvokeWithDefaultContext(Action<IApplicationContextService> invocationMethod)
        {
            invocationMethod(this.CreateDefaultApplicationContext());
        }

        /// <summary>
        /// Executes the command and pushes the dispatcher.
        /// </summary>
        /// <param name="commandToBind">The command to bind.</param>
        /// <param name="onCommandExecute">The on command execute.</param>
        /// <param name="invocationMethod">The invocation method.</param>
        private void ExecuteCommandAndPushDispatcher(
            ICommand commandToBind, 
            Action<ExecutedRoutedEventArgs> onCommandExecute, 
            Action<IApplicationContextService> invocationMethod)
        {
            var frameToPush = this.CreateFrame(commandToBind, onCommandExecute);

            this.InvokeWithDefaultContext(invocationMethod);

            Dispatcher.PushFrame(frameToPush);
        }

        /// <summary>
        /// Creates the frame.
        /// </summary>
        /// <param name="commandToBind">The command to bind.</param>
        /// <param name="onCommandExecute">The command execute handler.</param>
        /// <returns>The dispatcher frame to push.</returns>
        private DispatcherFrame CreateFrame(
            ICommand commandToBind,
            Action<ExecutedRoutedEventArgs> onCommandExecute)
        {
            var frame = new DispatcherFrame();

            this.SetUpTestWindow(CreateCommandBinding(frame, onCommandExecute, commandToBind));

            return frame;
        }

        /// <summary>
        /// Sets up the test window.
        /// </summary>
        /// <param name="commandBinding">The command binding.</param>
        private void SetUpTestWindow(CommandBinding commandBinding)
        {
            var window = new Window();
            window.CommandBindings.Add(commandBinding);
            this.dummyApplication.MainWindow = window;
        }
    }
}
