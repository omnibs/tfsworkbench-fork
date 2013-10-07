// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandControllerTests.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the CommandControllerTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Tests
{
    using System;
    using System.Linq;
    using System.Windows.Input;
    using System.Windows.Threading;

    using TfsWorkbench.Core.DataObjects;
    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.Tests.Helpers;
    using TfsWorkbench.UIElements;
    using TfsWorkbench.WpfUI;

    using NUnit.Framework;

    using Rhino.Mocks;

    using SharpArch.Testing.NUnit;

    using TfsWorkbench.WpfUI.Controllers;

    /// <summary>
    /// The command controller test fixture class.
    /// </summary>
    [TestFixture]
    public class CommandControllerTests
    {
        /// <summary>
        /// Sets up the command bindings.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            var applicationController = MockRepository.GenerateMock<IApplicationController>();
            var mainAppWindow = new MainAppWindow();
            var dialogController = MockRepository.GenerateMock<IDialogController>();
            applicationController.Expect(ac => ac.MainWindow).Return(mainAppWindow).Repeat.Any();
            applicationController.Expect(ac => ac.DialogController).Return(dialogController).Repeat.Any();

            ApplicationController.Instance = applicationController;

            CommandController.SetupCommandBindings();
        }

        /// <summary>
        /// Test: Add_view_command_should_create_new_view_defininition_with_expected_default_values.
        /// </summary>
        [Test]
        public void Add_view_command_should_create_new_view_defininition_with_expected_default_values()
        {
            // Arrange
            var projectData = DataObjectHelper.GenerateProjectData();
            var projectDataService = MockRepository.GenerateMock<IProjectDataService>();

            projectDataService
                .Expect(pds => pds.CurrentProjectData)
                .Return(projectData)
                .Repeat.Any();

            // Act
            ServiceManagerHelper.MockServiceManager(projectDataService);
            CommandLibrary.AddViewCommand.Execute(null, ApplicationController.Instance.MainWindow);
            ServiceManagerHelper.ClearDummyManager();

            // Assert
            projectData.ViewMaps.Count().ShouldEqual(2);
            var newView = projectData.ViewMaps.ElementAt(1);

            newView.Title.ShouldEqual("New View");
            newView.Description.ShouldEqual("new view description");
            newView.ParentTypes.ShouldContain(DataObjectHelper.ParentType);
            newView.ChildType.ShouldEqual(DataObjectHelper.ParentType);
            newView.LinkName.ShouldEqual(DataObjectHelper.LinkType1);
            newView.DisplayOrder.ShouldEqual(1);
        }

        [Test]
        public void Refresh_parent_and_children_command_should_report_incorrect_parameters()
        {
            // Arrange
            var hasReportedException = false;

            ExecutedRoutedEventHandler commandHandler = (s, e) => hasReportedException = true;

            this.ReplaceCommandHandler(CommandLibrary.ApplicationExceptionCommand, commandHandler);

            // Act
            CommandLibrary.RefreshItemAndViewChildren.Execute(new object(), ApplicationController.Instance.MainWindow);

            // Assert
            hasReportedException.ShouldBeTrue();
        }


        [Test]
        public void Refresh_parent_and_children_command_should_accept_valid_parameter()
        {
            // Arrange
            var hasReportedException = false;

            ExecutedRoutedEventHandler commandHandler = (s, e) => hasReportedException = true;

            this.ReplaceCommandHandler(CommandLibrary.ApplicationExceptionCommand, commandHandler);

            var childCreationParameters = new ChildCreationParameters
                {
                    Parent = DataObjectHelper.CreateWorkbenchItem().SetFieldValue(Core.Properties.Settings.Default.IdFieldName, 1),
                    ChildTypeName = DataObjectHelper.ChildType,
                    LinkTypeName = DataObjectHelper.LinkType1
                };

            // Act
            CommandLibrary.RefreshItemAndViewChildren.Execute(childCreationParameters, ApplicationController.Instance.MainWindow);

            // Assert
            hasReportedException.ShouldBeFalse();
        }
        
        [Test]
        public void Refresh_parent_and_children_command_should_only_refresh_linked_items_in_specified_view()
        {
            // Arrange
            var childA = DataObjectHelper.CreateWorkbenchItem()
                .SetFieldValue(Core.Properties.Settings.Default.TypeFieldName, DataObjectHelper.ChildType)
                .SetFieldValue(Core.Properties.Settings.Default.IdFieldName, 2);

            var childB = DataObjectHelper.CreateWorkbenchItem()
                .SetFieldValue(Core.Properties.Settings.Default.TypeFieldName, DataObjectHelper.ChildType)
                .SetFieldValue(Core.Properties.Settings.Default.IdFieldName, 3);

            var childC = DataObjectHelper.CreateWorkbenchItem()
                .SetFieldValue(Core.Properties.Settings.Default.TypeFieldName, DataObjectHelper.ChildType)
                .SetFieldValue(Core.Properties.Settings.Default.IdFieldName, 4);

            var childD = DataObjectHelper.CreateWorkbenchItem()
                .SetFieldValue(Core.Properties.Settings.Default.TypeFieldName, "Alternative Child Type")
                .SetFieldValue(Core.Properties.Settings.Default.IdFieldName, 4);

            var parent = DataObjectHelper.CreateWorkbenchItem()
                .SetFieldValue(Core.Properties.Settings.Default.IdFieldName, 1)
                .LinkChild(childA, DataObjectHelper.LinkType1)
                .LinkChild(childB, DataObjectHelper.LinkType1)
                .LinkChild(childC, "Alternative Link Type")
                .LinkChild(childD, DataObjectHelper.LinkType1);

            parent.ValueProvider.Expect(vp => vp.SyncToLatest()).Repeat.Once();
            childA.ValueProvider.Expect(vp => vp.SyncToLatest()).Repeat.Once();
            childB.ValueProvider.Expect(vp => vp.SyncToLatest()).Repeat.Once();
            childC.ValueProvider.Expect(vp => vp.SyncToLatest()).Repeat.Never();
            childD.ValueProvider.Expect(vp => vp.SyncToLatest()).Repeat.Never();

            var commandParameters = new ChildCreationParameters
            {
                Parent = parent,
                ChildTypeName = DataObjectHelper.ChildType,
                LinkTypeName = DataObjectHelper.LinkType1
            };

            // Act
            CommandLibrary.RefreshItemAndViewChildren.Execute(commandParameters, ApplicationController.Instance.MainWindow);

            // Assert
            parent.VerifyAllExpectations();
            childA.VerifyAllExpectations();
            childB.VerifyAllExpectations();
            childC.VerifyAllExpectations();
            childD.VerifyAllExpectations();
        }

        [Test]
        public void Refresh_parent_and_children_command_should_disable_user_input_during_process()
        {
            // Arrange
            var userInputWasDisabled = false;
            var userInputWasEnablded = false;

            ExecutedRoutedEventHandler commandHandler = (s, e) =>
            {
                var enable = (bool)e.Parameter;

                if (enable)
                {
                    userInputWasEnablded = true;
                }
                else
                {
                    userInputWasDisabled = true;
                }
            };

            this.ReplaceCommandHandler(CommandLibrary.DisableUserInputCommand, commandHandler);

            var commandParameters = new ChildCreationParameters
            {
                Parent = DataObjectHelper.CreateWorkbenchItem().SetFieldValue(Core.Properties.Settings.Default.IdFieldName, 1),
                ChildTypeName = DataObjectHelper.ChildType,
                LinkTypeName = DataObjectHelper.LinkType1
            };

            // Act
            CommandLibrary.RefreshItemAndViewChildren.Execute(commandParameters, ApplicationController.Instance.MainWindow);

            // Add a low priority action to wait for processing...
            Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.ApplicationIdle, new Action(() => { }));

            // Assert
            userInputWasDisabled.ShouldBeTrue();
            userInputWasEnablded.ShouldBeTrue();
        }

        [Test]
        public void Refresh_parent_and_children_command_should_display_start_and_finish_messages()
        {
            // Arrange
            ApplicationController.Instance.Expect(ac => ac.SetStatusMessage(null))
                .IgnoreArguments()
                .Repeat.Twice();

            var commandParameters = new ChildCreationParameters
            {
                Parent = DataObjectHelper.CreateWorkbenchItem().SetFieldValue(Core.Properties.Settings.Default.IdFieldName, 1),
                ChildTypeName = DataObjectHelper.ChildType,
                LinkTypeName = DataObjectHelper.LinkType1
            };

            // Act
            CommandLibrary.RefreshItemAndViewChildren.Execute(commandParameters, ApplicationController.Instance.MainWindow);

            // Add a low priority action to wait for processing...
            Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.SystemIdle, new Action(() => { }));

            // Assert
            ApplicationController.Instance.VerifyAllExpectations();
        }

        /// <summary>
        /// Replaces the command handler.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="replacementHandler">The replacement handler.</param>
        private void ReplaceCommandHandler(RoutedUICommand command, ExecutedRoutedEventHandler replacementHandler)
        {
            var mainAppWindow = ApplicationController.Instance.MainWindow;

            var commandBinding = mainAppWindow.CommandBindings.OfType<CommandBinding>().FirstOrDefault(cb => cb.Command == command);

            if (commandBinding == null)
            {
                throw new NullReferenceException(string.Format("Cannot find command handler for '{0}'", command.Name));
            }

            mainAppWindow.CommandBindings.Remove(commandBinding);

            mainAppWindow.CommandBindings.Add(new CommandBinding(command, replacementHandler));
        }
    }
}
