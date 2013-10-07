using System;
using System.Windows.Input;
using NUnit.Framework;
using TfsWorkbench.ScratchPadUI;

namespace TfsWorkbench.Tests
{
    [TestFixture]
    public class PadItemControllerTests
    {
        private PadItemBase uiPadItem;

        [SetUp]
        public void SetUp()
        {
            uiPadItem = new PadItemBase();
        }

        [Test]
        public void When_constructing_with_null_arg_then_exception_is_thrown()
        {
            // Arrange
            TestDelegate methodToTest = () => new PadItemController(default(PadItemBase));

            // Act
            var result = Assert.Throws<ArgumentNullException>(methodToTest).ParamName;

            // Assert
            Assert.AreEqual("padItem", result);
        }

        [Test]
        public void When_constructing_with_valid_arg_then_pad_item_is_set()
        {
            // Arrange
            var controller = CreatePadItemController();

            // Act
            var result = controller.PadItem;

            // Assert
            Assert.AreEqual(uiPadItem, result);
        }

        [Test]
        public void When_mouse_down_command_executed_with_null_then_no_exception_is_thrown_and_rotating_state_not_set()
        {
            // Arrange
            var controller = CreatePadItemController();

            TestDelegate methodToTest = () => controller.RotateMouseDownCommand.Execute(null);

            // Act
            Assert.DoesNotThrow(methodToTest);

            // Assert
            Assert.IsFalse(controller.IsRotating);
        }

        [Test]
        public void When_RotateMouseDownCommand_executed_with_mouse_args_then_rotating_state_is_set()
        {
            // Arrange
            var controller = CreatePadItemController();

            // Act
            controller.RotateMouseDownCommand.Execute(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left));

            // Assert
            Assert.IsTrue(controller.IsRotating);
        }

        [Test]
        public void When_RotateMouseUpCommand_executed_with_mouse_args_then_rotating_state_is_set()
        {
            // Arrange
            var controller = CreatePadItemController();

            controller.IsRotating = true;

            // Act
            controller.RotateMouseUpCommand.Execute(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left));

            // Assert
            Assert.IsFalse(controller.IsRotating);
        }

        [Test]
        public void When_RotateMouseMoveCommand_executed_with_mouse_args_then_rotating_angle_is_changed()
        {
            // Arrange
            var controller = CreatePadItemController();

            controller.IsRotating = true;

            var angleBefore = controller.PadItem.Angle;

            // Act
            controller.RotateMouseMoveCommand.Execute(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, MouseButton.Left));

            // Assert
            var angleAfter = controller.PadItem.Angle;
            Assert.AreNotEqual(angleBefore, angleAfter);
        }

        private PadItemController CreatePadItemController()
        {
            var controller = new PadItemController(uiPadItem);

            return controller;
        }
    }
}
