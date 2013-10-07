// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewEditPanelTests.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ViewEditPanelTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Tests
{
    using System.Linq;

    using TfsWorkbench.Core.DataObjects;
    using TfsWorkbench.Tests.Helpers;
    using TfsWorkbench.WpfUI.Controls;

    using NUnit.Framework;

    using SharpArch.Testing.NUnit;

    /// <summary>
    /// The view edit pnale test fixture class.
    /// </summary>
    [TestFixture]
    public class ViewEditPanelTests
    {
        [Test]
        public void View_edit_panel_should_only_list_valid_child_item_states()
        {
            // Arrange
            var projectData = DataObjectHelper.GenerateProjectData();
            projectData.ItemTypes[DataObjectHelper.ChildType].States.Add("New state 1");
            projectData.ItemTypes[DataObjectHelper.ChildType].States.Add("New state 2");
            projectData.ItemTypes[DataObjectHelper.ChildType].States.Add("New state 3");

            var viewEditPanel = new ViewEditorControl();

            // Act
            viewEditPanel.ProjectData = projectData;
            viewEditPanel.ViewMap = projectData.ViewMaps[0];

            // Assert
            viewEditPanel.BucketStates.Items.Count.ShouldEqual(0);
            viewEditPanel.SwimLaneStates.Items.Count.ShouldEqual(0);
            viewEditPanel.UnassignedStates.Items.Count.ShouldEqual(3);
        }

        [Test]
        public void Changing_the_child_item_should_update_the_edit_control_lists()
        {
            // Arrange
            var projectData = DataObjectHelper.GenerateProjectData();
            projectData.ItemTypes[DataObjectHelper.ParentType].States.Add("Deleted");
            projectData.ItemTypes[DataObjectHelper.ParentType].States.Add("New state 2");
            projectData.ItemTypes[DataObjectHelper.ParentType].States.Add("New state 3");

            var viewEditPanel = new ViewEditorControl();

            // Act
            viewEditPanel.ProjectData = projectData;
            viewEditPanel.ViewMap = projectData.ViewMaps[0];

            var bucketStateCountA = viewEditPanel.BucketStates.Items.Count;
            var swimLaneStatesCountA = viewEditPanel.SwimLaneStates.Items.Count;
            var unassignedStateCountA = viewEditPanel.UnassignedStates.Items.Count;

            viewEditPanel.ChildType.SelectedItem = DataObjectHelper.ParentType;

            var bucketStateCountB = viewEditPanel.BucketStates.Items.Count;
            var swimLaneStatesCountB = viewEditPanel.SwimLaneStates.Items.Count;
            var unassignedStateCountB = viewEditPanel.UnassignedStates.Items.Count;

            // Assert
            bucketStateCountA.ShouldEqual(0);
            swimLaneStatesCountA.ShouldEqual(0);
            unassignedStateCountA.ShouldEqual(0);

            bucketStateCountB.ShouldEqual(1);
            swimLaneStatesCountB.ShouldEqual(2);
            unassignedStateCountB.ShouldEqual(0);
        }

        [Test]
        public void Appying_a_view_without_states_should_list_all_states_as_unassigned()
        {
            // Arrange
            var projectData = DataObjectHelper.GenerateProjectData();
            projectData.ItemTypes[DataObjectHelper.ChildType].States.Add("New state 2");
            projectData.ItemTypes[DataObjectHelper.ChildType].States.Add("New state 2");
            projectData.ItemTypes[DataObjectHelper.ChildType].States.Add("New state 3");

            var viewEditPanel = new ViewEditorControl();
            var newView = new ViewMap { ChildType = DataObjectHelper.ChildType };
            newView.ParentTypes.Add(DataObjectHelper.ParentType);

            // Act
            viewEditPanel.ProjectData = projectData;
            viewEditPanel.ViewMap = newView;

            var bucketStateCountA = viewEditPanel.BucketStates.Items.Count;
            var swimLaneStatesCountA = viewEditPanel.SwimLaneStates.Items.Count;
            var unassignedStateCountA = viewEditPanel.UnassignedStates.Items.Count;

            // Assert
            bucketStateCountA.ShouldEqual(0);
            swimLaneStatesCountA.ShouldEqual(0);
            unassignedStateCountA.ShouldEqual(3);
        }
    }
}
