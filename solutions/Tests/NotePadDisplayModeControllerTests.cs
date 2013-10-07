using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using NUnit.Framework;
using Rhino.Mocks;
using SharpArch.Testing.NUnit;
using TfsWorkbench.Core.DataObjects;
using TfsWorkbench.Core.EventArgObjects;
using TfsWorkbench.Core.Interfaces;
using TfsWorkbench.NotePadUI;
using TfsWorkbench.NotePadUI.Models;
using TfsWorkbench.NotePadUI.Services;
using TfsWorkbench.NotePadUI.UIElements;
using TfsWorkbench.Tests.Helpers;
using TfsWorkbench.UIElements;
using TfsWorkbench.UIElements.FilterObjects;

namespace TfsWorkbench.Tests
{
    [TestFixture]
    public class NotePadDisplayModeControllerTests
    {
        private IProjectDataService projectDataService;
        private IPadLayoutService padLayoutService;
        private IProjectData projectData;
        private IFilterCollection filterCollection;
        private IList<PadItemBase> padItemMapCollection;
        private DisplayMode displayMode;

        [SetUp]
        public void SetUp()
        {
            displayMode = new DisplayMode();
        }

        [Test]
        public void When_constructing_with_null_args_then_exception_is_thrown()
        {
            // Arrange
            TestDelegate methodToTestA = () => new DisplayModeController(null, null, null);
            TestDelegate methodToTestB = () => new DisplayModeController(displayMode, null, null);
            TestDelegate methodToTestC = () => new DisplayModeController(displayMode, projectDataService, null);

            // Act
            var resultA = Assert.Throws<ArgumentNullException>(methodToTestA).Message;
            var resultB = Assert.Throws<ArgumentNullException>(methodToTestB).Message;
            var resultC = Assert.Throws<ArgumentNullException>(methodToTestC).Message;

            // Assert
            Assert.IsTrue(resultA.Contains("displayMode"));
            Assert.IsTrue(resultB.Contains("projectDataService"));
            Assert.IsTrue(resultC.Contains("padLayoutService"));
        }

        [Test]
        public void When_constructing_with_single_arg_then_service_instance_is_called()
        {
            // Arrange
            GenerateDependencyServices();

            // Act
            var displayModeController = new DisplayModeController(displayMode);

            // Assert
            Assert.IsNotNull(displayModeController);
            ServiceManagerHelper.ClearDummyManager();
        }

        [Test]
        public void When_constructed_then_filters_collection_is_not_null()
        {
            // Arrange
            GenerateDependencyServices();
            var controller = new DisplayModeController(displayMode, projectDataService, padLayoutService);

            // Act
            var result = controller.FilterCollection as FilterCollection;

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void When_project_data_changed_then_filters_collection_is_initialised()
        {
            // Arrange
            GenerateDependencyServices();

            padLayoutService
                .Expect(pls => pls.GetWorkspaceLayout(projectData))
                .Return(new List<PadItemBase>())
                .Repeat.Any();

            filterCollection.Expect(f => f.Initialise(projectData)).Repeat.Once();

            CreateControllerWithMockedDependencies();

            // Act
            RaiseProjectDataChangedEvent();

            // Assert
            filterCollection.VerifyAllExpectations();
        }

        [Test]
        public void When_project_data_changed_then_layout_service_is_quered()
        {
            // Arrange
            GenerateDependencyServices();

            CreateControllerWithMockedDependencies();

            // Act
            RaiseProjectDataChangedEvent();

            // Assert
            padLayoutService.VerifyAllExpectations();
        }

        [Test]
        public void When_project_data_changed_then_layout_service_items_are_added_to_display_elements()
        {
            // Arrange
            GenerateDependencyServices();

            var controlller = CreateControllerWithMockedDependencies();

            // Act
            RaiseProjectDataChangedEvent();

            // Assert
            Assert.IsTrue(padItemMapCollection.All(pi => controlller.PadUiItems.Any(ui => ui.DataContext == pi)));
        }

        [Test]
        public void When_project_data_changed_then_filter_selections_are_set_accordingly()
        {
            // Arrange
            GenerateDependencyServices();

            Predicate<IEnumerable<IWorkbenchItem>> predicate = o => padItemMapCollection.OfType<WorkbenchPadItem>().Select(i => i.WorkbenchItem).All(o.Contains);

            filterCollection
                .Expect(fc => fc.SelectItems(null))
                .IgnoreArguments()
                .Constraints(Rhino.Mocks.Constraints.Is.Matching(predicate))
                .Repeat.Once();

            CreateControllerWithMockedDependencies();

            // Act
            RaiseProjectDataChangedEvent();

            // Assert
            filterCollection.VerifyAllExpectations();
        }

        [Test]
        public void When_filters_are_selected_then_pad_items_are_updated()
        {
            var itemCount = 0;

            Action test = () =>
                {
                    // Arrange
                    GenerateDependencyServices();
                    displayMode = new DisplayMode();
                    var controller = CreateControllerWithMockedDependencies();

                    padLayoutService
                        .Expect(pls => pls.GetWorkspaceLayout(projectData, filterCollection.IsMatch))
                        .Return(padItemMapCollection)                        
                        .Repeat.Any();

                    // Act
                    filterCollection.Raise(fc => fc.SelectionChanged += null, filterCollection, EventArgs.Empty);

                    Dispatcher.CurrentDispatcher
                        .BeginInvoke(
                            DispatcherPriority.Background,
                            new Action(() => itemCount = controller.PadUiItems.Count()));
                };

            DispatcherHelper.ExecuteOnDispatcherThread(test);

            // Assert
            Assert.AreEqual(2, itemCount);
        }

        [Test]
        public void When_pad_items_collection_changed_then_layout_canvas_updated()
        {
            // Arrange
            GenerateDependencyServices();
            var controller = CreateControllerWithMockedDependencies();

            // Act
            controller.PadUiItems.Add(new UIPadItem());

            // Assert
            Assert.AreEqual(1, displayMode.PART_LayoutCanvas.Children.Count);
        }

        [Test]
        public void When_reset_command_is_executed_then_user_is_asked_to_confirm_reset()
        {
            // Arrange
            GenerateDependencyServices();
            var controller = CreateControllerWithMockedDependencies();

            // Act
            Action invocation = () => controller.ResetCommand.Execute(null);
            Predicate<ExecutedRoutedEventArgs> predicate = e =>
                {
                    var decisionControl = e.Parameter as DecisionControl;

                    if (decisionControl == null)
                    {
                        return false;
                    }

                    var isAsExpected = decisionControl.Caption == "Confirm Reset All Notes";
                    isAsExpected &= decisionControl.Message == "Are you sure you want to reset all notes?";
                    isAsExpected &= decisionControl.HideDoNotShowAgain;

                    return isAsExpected;
                };

            // Assert
            AssertCommandWasExecuted(CommandLibrary.ShowDialogCommand, invocation, predicate);
        }

        [Test]
        public void When_user_confirms_reset_command_then_all_items_are_reset()
        {
            // Arrange
            GenerateDependencyServices();
            var controller = CreateControllerWithMockedDependencies();
            controller.PadUiItems.Add(new UIPadItem());

            // Act
            Action invocation = () => controller.ResetCommand.Execute(null);
            Predicate<ExecutedRoutedEventArgs> predicate = e =>
            {
                var decisionControl = e.Parameter as DecisionControl;

                if (decisionControl == null)
                {
                    return false;
                }

                decisionControl.IsYes = true;
                decisionControl.OnDecisionMade();

                return !controller.PadUiItems.Any();
            };

            // Assert
            AssertCommandWasExecuted(CommandLibrary.ShowDialogCommand, invocation, predicate);
        }

        [Test]
        public void When_user_rejects_reset_command_then_items_are_not_reset()
        {
            // Arrange
            GenerateDependencyServices();
            var controller = CreateControllerWithMockedDependencies();
            controller.PadUiItems.Add(new UIPadItem());

            // Act
            Action invocation = () => controller.ResetCommand.Execute(null);
            Predicate<ExecutedRoutedEventArgs> predicate = e =>
            {
                var decisionControl = e.Parameter as DecisionControl;

                if (decisionControl == null)
                {
                    return false;
                }

                decisionControl.IsYes = false;
                decisionControl.OnDecisionMade();

                return controller.PadUiItems.Count() == 1;
            };

            // Assert
            AssertCommandWasExecuted(CommandLibrary.ShowDialogCommand, invocation, predicate);
        }

        [Test]
        public void When_user_confirms_reset_command_then_layout_is_saved()
        {
            // Arrange
            GenerateDependencyServices();
            var controller = CreateControllerWithMockedDependencies();
            controller.PadUiItems.Add(new UIPadItem());

            // Act
            Action invocation = () => controller.ResetCommand.Execute(null);
            Predicate<ExecutedRoutedEventArgs> predicate = e =>
            {
                var decisionControl = e.Parameter as DecisionControl;

                if (decisionControl == null)
                {
                    return false;
                }

                decisionControl.IsYes = true;
                decisionControl.OnDecisionMade();

                padLayoutService.AssertWasCalled(s => s.Save());

                return true;
            };

            // Assert
            AssertCommandWasExecuted(CommandLibrary.ShowDialogCommand, invocation, predicate);
        }

        [Test]
        public void When_highlighting_with_null_arg_then_no_action_is_taken()
        {
            // Arrange
            GenerateDependencyServices();
            var controller = CreateControllerWithMockedDependencies();

            TestDelegate methodToTest = () => controller.Highlight(null);

            // Act
            Assert.DoesNotThrow(methodToTest);

            // Assert
            Assert.Pass("No exception was thrown");
        }

        [Test]
        public void When_highlight_is_called_with_new_item_then_item_is_added_to_canvas()
        {
            var hasHighlighted = false;

            Action test = () =>
            {
                // Arrange
                GenerateDependencyServices();
                displayMode = new DisplayMode();
                var controller = CreateControllerWithMockedDependencies();

                var item = MockRepository.GenerateMock<IWorkbenchItem>();
                
                padItemMapCollection.Add(new WorkbenchPadItem { WorkbenchItem = item });

                padLayoutService
                    .Expect(pls => pls.GetWorkspaceLayout(projectData, filterCollection.IsMatch))
                    .Return(padItemMapCollection)
                    .Repeat.Any();

                filterCollection
                    .Expect(fc => fc.SelectItems(null))
                    .IgnoreArguments()
                    .WhenCalled(mi => filterCollection.Raise(fc => fc.SelectionChanged += null, filterCollection, EventArgs.Empty))
                    .Repeat.Any();

                // Act
                controller.Highlight(item);

                Action callbackMethod = () =>
                    hasHighlighted = displayMode.PART_LayoutCanvas
                        .Children
                        .OfType<UIPadItem>()
                        .Select(ui => ui.DataContext)
                        .OfType<WorkbenchPadItem>()
                        .Any(wi => wi.WorkbenchItem == item);

                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, callbackMethod);
            };

            DispatcherHelper.ExecuteOnDispatcherThread(test);

            // Assert
            Assert.IsTrue(hasHighlighted);
        }

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

            displayMode.CommandBindings.Add(new CommandBinding(command, onExecute));

            using (new Timer(state => frame.Continue = false, null, 1000, Timeout.Infinite))
            {
                invocation();
                Dispatcher.PushFrame(frame);
            }

            hasCalled.ShouldBeTrue();
            result.ShouldBeTrue();
        }

        private void RaiseProjectDataChangedEvent()
        {
            projectDataService.Raise(pds => pds.ProjectDataChanged += null, projectDataService,
                                     new ProjectDataChangedEventArgs(null, projectData));
        }

        private DisplayModeController CreateControllerWithMockedDependencies()
        {
            var displayModeController = new DisplayModeController(displayMode, projectDataService, padLayoutService)
                {
                    FilterCollection = filterCollection
                };

            return displayModeController;
        }

        private void GenerateDependencyServices()
        {
            padLayoutService = MockRepository.GenerateMock<IPadLayoutService>();
            filterCollection = MockRepository.GenerateMock<IFilterCollection>();            

            projectDataService = MockRepository.GenerateMock<IProjectDataService>();
            projectData = MockRepository.GenerateMock<IProjectData>();

            projectData
                .Expect(pd => pd.ItemTypes)
                .Return(new ItemTypeDataCollection())
                .Repeat.Any();

            projectDataService
                .Expect(pds => pds.CurrentProjectData)
                .WhenCalled(mi => mi.ReturnValue = projectData)
                .Return(null)
                .Repeat.Any();

            padItemMapCollection = new List<PadItemBase>(new[] 
                {
                    new WorkbenchPadItem { WorkbenchItem = MockRepository.GenerateMock<IWorkbenchItem>() }, 
                    new WorkbenchPadItem { WorkbenchItem = MockRepository.GenerateMock<IWorkbenchItem>() }
                });

            padLayoutService
                .Expect(pls => pls.GetWorkspaceLayout(projectData))
                .Return(padItemMapCollection)
                .Repeat.Once();

            ServiceManagerHelper.MockServiceManager(projectDataService);
        }
    }
}