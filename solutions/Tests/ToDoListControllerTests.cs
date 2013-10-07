using System;
using System.Linq;
using NUnit.Framework;
using TfsWorkbench.NotePadUI;
using TfsWorkbench.NotePadUI.Models;

namespace TfsWorkbench.Tests
{
    [TestFixture]
    public class ToDoListControllerTests
    {
        [Test]
        public void When_constructing_with_null_args_then_expection_thrown()
        {
            // Arrange
            TestDelegate methodToTest = () => new ToDoListController(default(ToDoList));

            // Act
            var result = Assert.Throws<ArgumentNullException>(methodToTest).ParamName;

            // Assert
            Assert.AreEqual("toDoList", result);
        }

        [Test]
        public void when_constructing_with_valid_args_then_collection_is_initialised()
        {
            // Arrange
            var toDoList = new ToDoList();

            toDoList.ToDoItems.Add(new ToDoItem());
            toDoList.ToDoItems.Add(new ToDoItem());

            var controller = new ToDoListController(toDoList);

            // Act
            var result = controller.ToDoItems;

            // Assert
            Assert.AreEqual(toDoList.ToDoItems.Count(), result.Count());
        }

        [Test]
        public void When_removing_an_item_then_source_list_updated()
        {
            // Arrange
            var toDoList = new ToDoList();

            var toDoItem = new ToDoItem();
            toDoList.ToDoItems.Add(toDoItem);

            var controller = new ToDoListController(toDoList);

            // Act
            controller.ToDoItems.Remove(toDoItem);

            // Assert
            Assert.AreEqual(0, toDoList.ToDoItems.Count());
        }

        [Test]
        public void When_adding_an_item_then_source_list_updated()
        {
            // Arrange
            var toDoList = new ToDoList();

            var toDoItem = new ToDoItem();

            var controller = new ToDoListController(toDoList);

            // Act
            controller.ToDoItems.Add(toDoItem);

            // Assert
            Assert.AreEqual(1, toDoList.ToDoItems.Count());
        }

        [Test]
        public void When_clearing_the_ToDoItems_then_list_is_cleared()
        {
            // Arrange
            var toDoList = new ToDoList();

            toDoList.ToDoItems.Add(new ToDoItem());
            toDoList.ToDoItems.Add(new ToDoItem());
            toDoList.ToDoItems.Add(new ToDoItem());

            var controller = new ToDoListController(toDoList);

            // Act
            controller.ToDoItems.Clear();

            // Assert
            Assert.AreEqual(0, toDoList.ToDoItems.Count());
        }

        [Test]
        public void When_setting_item_as_done_then_property_changed_event_is_raised()
        {
            // Arrange
            var toDoItem = new ToDoItem();

            bool hasRaised = false;

            toDoItem.PropertyChanged += (sender, args) => hasRaised = args.PropertyName == "IsDone";

            // Act
            toDoItem.IsDone = true;

            // Assert
            Assert.IsTrue(hasRaised);
        }

        [Test]
        public void When_deleting_item_then_item_is_removed_from_list()
        {
            // Arrange
            var toDoList = new ToDoList();

            var itemToRemove = new ToDoItem();

            toDoList.ToDoItems.Add(itemToRemove);

            var controller = new ToDoListController(toDoList);

            // Act
            controller.DeleteCommand.Execute(itemToRemove);

            // Assert
            Assert.AreEqual(0, toDoList.ToDoItems.Count());
        }

        [Test]
        public void When_deleting_with_null_arg_then_no_action_is_taken()
        {
            // Arrange
            var controller = new ToDoListController(new ToDoList());

            // Act
            controller.DeleteCommand.Execute(null);

            // Assert
            Assert.Pass("No action taken");
        }

        [Test]
        public void When_adding_new_item_then_new_item_added_to_list()
        {
            // Arrange
            var toDoList = new ToDoList();

            var controller= new ToDoListController(toDoList);

            // Act
            controller.AddCommand.Execute(null);

            // Assert
            Assert.AreEqual(1, toDoList.ToDoItems.Count());
        }
    }
}
