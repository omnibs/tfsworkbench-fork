// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwimLaneTests.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the SwimLaneObjectTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Tests
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;

    using NUnit.Framework;

    using Rhino.Mocks;

    using SharpArch.Testing.NUnit;

    using TfsWorkbench.Core.DataObjects;
    using TfsWorkbench.Core.EventArgObjects;
    using TfsWorkbench.Core.Helpers;
    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.Core.Properties;
    using TfsWorkbench.Core.Services;
    using TfsWorkbench.TaskBoardUI.DataObjects;
    using TfsWorkbench.TaskBoardUI.Helpers;
    using TfsWorkbench.Tests.Helpers;

    /// <summary>
    /// The swim lane object test fixture.
    /// </summary>
    [TestFixture]
    public class SwimLaneTests
    {
        [Test]
        public void Task_board_ui_should_contain_swimlane_object_definitions()
        {
            // Arrange
            var viewMap = new ViewMap();

            // Act
            var swimLane = new SwimLaneView(viewMap);
            var swimLaneRow = new SwimLaneRow(null, new string[] { }, null, null, 0);
            var stateCollection = new StateCollection(string.Empty, string.Empty);

            // Assert
            swimLane.ShouldNotBeNull();
            swimLaneRow.ShouldNotBeNull();
            stateCollection.ShouldNotBeNull();
        }

        [Test]
        public void Swim_lane_view_should_be_initialised_with_a_non_null_view_map()
        {
            // Arrange
            var viewMap = new ViewMap();

            // Act
            try
            {
                new TaskBoardUI.DataObjects.SwimLaneView(null);
                Assert.Fail("Exception not thrown");
            }
            catch (ArgumentNullException)
            {
            }

            var swimLaneView = new SwimLaneView(viewMap);

            // Asert
            swimLaneView.ViewMap.ShouldEqual(viewMap);
        }

        [Test]
        public void Task_board_ui_should_provide_a_singleton_swimlane_repository_service()
        {
            // Arrange

            // Act
            var service = SwimLaneService.Instance;

            try
            {
                Activator.CreateInstance<SwimLaneService>();
                Assert.Fail("Exception not thrown");
            }
            catch (MissingMethodException)
            {
            }

            // Assert
            service.ShouldNotBeNull();
            service.ShouldEqual(SwimLaneService.Instance);
        }

        [Test]
        public void Swim_lane_service_should_provide_an_observable_collection_of_swimlanes()
        {
            // Arrange
            var service = SwimLaneService.Instance;

            // Act
            var views = service.SwimLaneViews;

            // Asert
            views.ShouldBeOfType(typeof(ObservableCollection<SwimLaneView>));
        }

        [Test]
        public void Swim_lane_service_should_populate_views_when_given_project_data()
        {
            // Arrange
            var projectData = DataObjectHelper
                .CreateProjectData()
                .AddViewMap(new ViewMap())
                .AddViewMap(new ViewMap())
                .AddViewMap(new ViewMap());

            var service = SwimLaneService.Instance;

            service.Initialise(projectData);

            // Act
            var swimLaneViews = service.SwimLaneViews;

            // Assert
            swimLaneViews.Count().ShouldEqual(3);

            for (int i = 0; i < swimLaneViews.Count(); i++)
            {
                swimLaneViews[i].ViewMap.ShouldEqual(projectData.ViewMaps[i]);
            }
        }

        [Test]
        public void Swim_lane_service_should_listern_to_project_changes_and_update_swim_lane_views_accordingly()
        {
            // Arrange
            var projectData = DataObjectHelper
                .CreateProjectData()
                .AddViewMap(new ViewMap())
                .AddViewMap(new ViewMap())
                .AddViewMap(new ViewMap());


            var service = SwimLaneService.Instance;

            service.Initialise(projectData);

            // Act
            projectData.ViewMaps.Add(new ViewMap());

            var swimLaneViews = service.SwimLaneViews;

            // Assert
            swimLaneViews.Count().ShouldEqual(projectData.ViewMaps.Count());

            for (int i = 0; i < swimLaneViews.Count(); i++)
            {
                swimLaneViews[i].ViewMap.ShouldEqual(projectData.ViewMaps[i]);
            }
        }

        [Test]
        public void Swim_lane_service_should_stop_listerning_to_original_project_changes_when_alternative_project_provided()
        {
            // Arrange
            var projectData1 = DataObjectHelper.CreateProjectData();
            var projectData2 = DataObjectHelper.CreateProjectData();

            var service = SwimLaneService.Instance;

            service.Initialise(projectData1);
            service.Initialise(projectData2);

            // Act
            projectData1.AddViewMap(new ViewMap());
            projectData2.AddViewMap(new ViewMap());

            // Assert
            service.SwimLaneViews.Count().ShouldEqual(1);
            service.SwimLaneViews[0].ViewMap.ShouldEqual(projectData2.ViewMaps[0]);
        }

        [Test]
        public void Swim_lane_service_should_stop_listerning_to_project_changes_when_resources_are_released()
        {
            // Arrange
            var projectData = DataObjectHelper.CreateProjectData();

            var service = SwimLaneService.Instance;

            service.Initialise(projectData);

            // Act
            service.ReleaseResources();

            projectData.AddViewMap(new ViewMap());

            // Assert
            service.SwimLaneViews.Count().ShouldEqual(0);
        }

        [Test]
        public void Swim_lanes_should_be_populated_with_loaded_work_items_on_view_generation()
        {
            // Arrange
            var viewMap1 = DataObjectHelper.CreateViewMap();

            var viewMap2 = DataObjectHelper.CreateViewMap(new[] { "Type 3" }, "Type 4", "Link.Name.2");

            var item1 = DataObjectHelper.CreateWorkbenchItem()
                .SetFieldValue(Settings.Default.TypeFieldName, DataObjectHelper.ParentType)
                .SetFieldValue(Settings.Default.StateFieldName, DataObjectHelper.State1);

            var item2 = DataObjectHelper.CreateWorkbenchItem()
                .SetFieldValue(Settings.Default.TypeFieldName, DataObjectHelper.ChildType)
                .SetFieldValue(Settings.Default.StateFieldName, DataObjectHelper.State1);

            var projectData =
                DataObjectHelper.CreateProjectData()
                .AddViewMap(viewMap1)
                .AddViewMap(viewMap2)
                .AddWorkItem(item1)
                .AddWorkItem(item2);

            var service = SwimLaneService.Instance;

            // Act
            service.Initialise(projectData);

            // Assert
            service.SwimLaneViews.Count().ShouldEqual(2);
            service.SwimLaneViews[0].SwimLaneRows.Count().ShouldEqual(1);
        }

        [Test]
        public void Swim_lane_view_should_have_a_header_for_each_swim_lane_state()
        {
            // Arrange
            var viewMap1 = new ViewMap();

            viewMap1.SwimLaneStates.Add(DataObjectHelper.State1);
            viewMap1.SwimLaneStates.Add("State 2");
            viewMap1.SwimLaneStates.Add("State 3");

            // Act
            var swimLaneView = new SwimLaneView(viewMap1);

            // Assert
            swimLaneView.RowHeaders.Count().ShouldEqual(4);
            swimLaneView.RowHeaders[0].ShouldBeEmpty();
            swimLaneView.RowHeaders[1].ShouldEqual(DataObjectHelper.State1);
            swimLaneView.RowHeaders[2].ShouldEqual("State 2");
            swimLaneView.RowHeaders[3].ShouldEqual("State 3");
        }

        [Test]
        public void Swim_lane_view_should_have_a_bucket_state_container_for_each_view_map_bucket_state()
        {
            // Arrange
            var viewMap1 = new ViewMap();

            viewMap1.BucketStates.Add(DataObjectHelper.State1);
            viewMap1.BucketStates.Add("State 2");
            viewMap1.BucketStates.Add("State 3");

            // Act
            var swimLaneView = new SwimLaneView(viewMap1);

            // Assert
            swimLaneView.BucketStates.Count().ShouldEqual(3);
            swimLaneView.BucketStates[0].State.ShouldEqual(DataObjectHelper.State1);
            swimLaneView.BucketStates[1].State.ShouldEqual("State 2");
            swimLaneView.BucketStates[2].State.ShouldEqual("State 3");
        }

        [Test]
        public void Swim_lane_view_should_have_an_orphans_container()
        {
            // Arrange
            var viewMap1 = new ViewMap();

            // Act
            var swimLaneView = new SwimLaneView(viewMap1);

            // Assert
            swimLaneView.Orphans.ShouldNotBeNull();
        }

        [Test]
        public void Swim_lane_rows_should_be_populated_with_child_items()
        {
            // Arrange
            var viewMap1 = DataObjectHelper.CreateViewMap();

            viewMap1.SwimLaneStates.Add(DataObjectHelper.State1);

            var childItem = DataObjectHelper.CreateWorkbenchItem()
                .SetFieldValue(Settings.Default.TypeFieldName, DataObjectHelper.ChildType)
                .SetFieldValue(Settings.Default.StateFieldName, DataObjectHelper.State1);

            var parentItem = DataObjectHelper.CreateWorkbenchItem()
                .SetFieldValue(Settings.Default.TypeFieldName, DataObjectHelper.ParentType)
                .SetFieldValue(Settings.Default.StateFieldName, DataObjectHelper.State1)
                .LinkChild(childItem, DataObjectHelper.LinkType1);

            var projectData =
                DataObjectHelper.CreateProjectData()
                .AddViewMap(viewMap1)
                .AddWorkItem(parentItem)
                .AddWorkItem(childItem);

            var service = SwimLaneService.Instance;

            // Act
            service.Initialise(projectData);

            // Assert
            service.SwimLaneViews.Count().ShouldEqual(1);
            service.SwimLaneViews[0].SwimLaneRows.Count().ShouldEqual(1);
            
            var swimLaneRow = service.SwimLaneViews[0].SwimLaneRows[0];

            swimLaneRow.Parent.ShouldEqual(parentItem);
            swimLaneRow.SwimLaneColumns.Count().ShouldEqual(1);
            swimLaneRow[DataObjectHelper.State1].Count().ShouldEqual(1);
            swimLaneRow[DataObjectHelper.State1].ElementAt(0).ShouldEqual(childItem);
        }

        [Test]
        public void Service_should_handle_workbench_item_state_change_event()
        {
            // Arrange
            var projectData = DataObjectHelper.GenerateProjectData(1, 3);
            var service = SwimLaneService.Instance;
            service.Initialise(projectData);
            var swimLaneView = service.SwimLaneViews[0];

            // Act
            var column1CountA = swimLaneView.SwimLaneRows[0][DataObjectHelper.State1].Count();
            var column2CountA = swimLaneView.SwimLaneRows[0][DataObjectHelper.State2].Count();
            var bucketStateCountA = swimLaneView.BucketStates[0].Count();

            var childItem1 = projectData.WorkbenchItems.Where(w => w.GetTypeName().Equals(DataObjectHelper.ChildType)).ElementAt(0);
            this.RaiseStateChangedEvent(projectData, childItem1, DataObjectHelper.State1, DataObjectHelper.State2);

            var column1CountB = swimLaneView.SwimLaneRows[0][DataObjectHelper.State1].Count();
            var column2CountB = swimLaneView.SwimLaneRows[0][DataObjectHelper.State2].Count();
            var bucketStateCountB = swimLaneView.BucketStates[0].Count();

            var childItem2 = projectData.WorkbenchItems.Where(w => w.GetTypeName().Equals(DataObjectHelper.ChildType)).ElementAt(1);
            this.RaiseStateChangedEvent(projectData, childItem2, DataObjectHelper.State1, DataObjectHelper.BucketState);

            var column1CountC = swimLaneView.SwimLaneRows[0][DataObjectHelper.State1].Count();
            var column2CountC = swimLaneView.SwimLaneRows[0][DataObjectHelper.State2].Count();
            var bucketStateCountC = swimLaneView.BucketStates[0].Count();

            // Assert
            bucketStateCountA.ShouldEqual(0);
            column1CountA.ShouldEqual(3);
            column2CountA.ShouldEqual(0);

            column1CountB.ShouldEqual(2);
            column2CountB.ShouldEqual(1);
            bucketStateCountB.ShouldEqual(0);

            column1CountC.ShouldEqual(1);
            column2CountC.ShouldEqual(1);
            bucketStateCountC.ShouldEqual(1);
        }

        [Test]
        public void Service_should_handle_Links_added_events()
        {
            // Arrange
            var projectData = DataObjectHelper.GenerateProjectData(1, 0, 1);
            var parentItem = projectData.WorkbenchItems.First(w => w.GetTypeName().Equals(DataObjectHelper.ParentType));
            var childItem = projectData.WorkbenchItems.First(w => w.GetTypeName().Equals(DataObjectHelper.ChildType));
            var service = SwimLaneService.Instance;
            service.Initialise(projectData);
            var swimLaneView = service.SwimLaneViews[0];

            // Act
            var column1CountA = swimLaneView.SwimLaneRows[0][DataObjectHelper.State1].Count();
            var orphanCountA = swimLaneView.Orphans.Count();

            parentItem.LinkChild(childItem, DataObjectHelper.LinkType1);
            this.RaiseLinkChangedEvent(projectData, parentItem, null, parentItem.ChildLinks.First());

            var column1CountB = swimLaneView.SwimLaneRows[0][DataObjectHelper.State1].Count();
            var orphanCountB = swimLaneView.Orphans.Count();

            // Assert
            column1CountA.ShouldEqual(0);
            orphanCountA.ShouldEqual(1);

            column1CountB.ShouldEqual(1);
            orphanCountB.ShouldEqual(0);
        }

        [Test]
        public void Service_should_handle_links_removed_event()
        {
            // Arrange
            var projectData = DataObjectHelper.GenerateProjectData(1, 1);
            var parentItem = projectData.WorkbenchItems.First(w => w.GetTypeName().Equals(DataObjectHelper.ParentType));
            var service = SwimLaneService.Instance;
            service.Initialise(projectData);
            var swimLaneView = service.SwimLaneViews[0];

            // Act
            var column1CountA = swimLaneView.SwimLaneRows[0][DataObjectHelper.State1].Count();
            var orphanCountA = swimLaneView.Orphans.Count();

            this.RaiseLinkChangedEvent(projectData, parentItem, parentItem.ChildLinks.First(), null);

            var column1CountB = swimLaneView.SwimLaneRows[0][DataObjectHelper.State1].Count();
            var orphanCountB = swimLaneView.Orphans.Count();

            // Assert
            column1CountA.ShouldEqual(1);
            orphanCountA.ShouldEqual(0);

            column1CountB.ShouldEqual(0);
            orphanCountB.ShouldEqual(1);
        }

        [Test]
        public void Service_should_handle_workbench_item_added_event_for_parent_types()
        {
            // Arrange
            var projectData = DataObjectHelper.GenerateProjectData();
            var service = SwimLaneService.Instance;
            service.Initialise(projectData);
            var swimLaneView = service.SwimLaneViews[0];

            var parentItem = DataObjectHelper.CreateWorkbenchItem()
                .SetFieldValue(Settings.Default.TypeFieldName, DataObjectHelper.ParentType)
                .SetFieldValue(Settings.Default.StateFieldName, DataObjectHelper.State1);

            // Act
            var rowCountA = swimLaneView.SwimLaneRows.Count();

            this.RaiseItemAddedEvent(projectData, parentItem);

            var rowCountB = swimLaneView.SwimLaneRows.Count();

            // Assert
            rowCountA.ShouldEqual(0);
            rowCountB.ShouldEqual(1);
        }


        [Test]
        public void Service_should_handle_workbench_item_added_event_for_child_types()
        {
            // Arrange
            var projectData = DataObjectHelper.GenerateProjectData(1);
            var service = SwimLaneService.Instance;
            service.Initialise(projectData);
            var swimLaneView = service.SwimLaneViews[0];

            var parentItem = projectData.WorkbenchItems.First();
                
            var linkedSwimLaneChild = DataObjectHelper.CreateWorkbenchItem()
                .SetFieldValue(Settings.Default.TypeFieldName, DataObjectHelper.ChildType)
                .SetFieldValue(Settings.Default.StateFieldName, DataObjectHelper.State1);

            var linkedBucketChild = DataObjectHelper.CreateWorkbenchItem()
                .SetFieldValue(Settings.Default.TypeFieldName, DataObjectHelper.ChildType)
                .SetFieldValue(Settings.Default.StateFieldName, DataObjectHelper.BucketState);

            parentItem.LinkChild(linkedSwimLaneChild, DataObjectHelper.LinkType1).LinkChild(linkedBucketChild, DataObjectHelper.LinkType1);

            var orphanChild = DataObjectHelper.CreateWorkbenchItem()
                .SetFieldValue(Settings.Default.TypeFieldName, DataObjectHelper.ChildType)
                .SetFieldValue(Settings.Default.StateFieldName, DataObjectHelper.State1);

            // Act
            var orphanCountA = swimLaneView.Orphans.Count();
            var bucketCountA = swimLaneView.BucketStates[0].Count();
            var column1CountA = swimLaneView.SwimLaneRows[0].SwimLaneColumns[0].Count();

            this.RaiseItemAddedEvent(projectData, linkedSwimLaneChild);
            this.RaiseItemAddedEvent(projectData, linkedBucketChild);
            this.RaiseItemAddedEvent(projectData, orphanChild);

            var orphanCountB = swimLaneView.Orphans.Count();
            var bucketCountB = swimLaneView.BucketStates[0].Count();
            var column1CountB = swimLaneView.SwimLaneRows[0].SwimLaneColumns[0].Count();

            // Assert
            orphanCountA.ShouldEqual(0);
            bucketCountA.ShouldEqual(0);
            column1CountA.ShouldEqual(0);

            orphanCountB.ShouldEqual(1);
            bucketCountB.ShouldEqual(1);
            column1CountB.ShouldEqual(1);
        }

        [Test]
        public void Service_should_handle_workbench_item_removed_event_for_parent_types()
        {
            // Arrange
            var projectData = DataObjectHelper.GenerateProjectData(1);
            var service = SwimLaneService.Instance;
            service.Initialise(projectData);
            var swimLaneView = service.SwimLaneViews.First();
            var parentItem = projectData.WorkbenchItems.First();

            // Act
            var rowCountA = swimLaneView.SwimLaneRows.Count();

            this.RaiseItemRemovedEvent(projectData, parentItem);

            var rowCountB = swimLaneView.SwimLaneRows.Count();
            
            // Assert
            rowCountA.ShouldEqual(1);

            rowCountB.ShouldEqual(0);
        }

        [Test]
        public void Service_should_handle_workbench_item_removed_event_for_Child_types()
        {
            // Arrange
            var projectData = DataObjectHelper.GenerateProjectData(1, 1, 1, 1);
            var service = SwimLaneService.Instance;
            service.Initialise(projectData);
            var swimLaneView = service.SwimLaneViews[0];

            // Act
            var orphanCountA = swimLaneView.Orphans.Count();
            var bucketCountA = swimLaneView.BucketStates[0].Count();
            var column1CountA = swimLaneView.SwimLaneRows[0].SwimLaneColumns[0].Count();

            this.RaiseItemRemovedEvent(projectData, swimLaneView.Orphans.First());
            this.RaiseItemRemovedEvent(projectData, swimLaneView.SwimLaneRows[0].SwimLaneColumns[0].First());
            this.RaiseItemRemovedEvent(projectData, swimLaneView.BucketStates[0].First());

            var orphanCountB = swimLaneView.Orphans.Count();
            var bucketCountB = swimLaneView.BucketStates[0].Count();
            var column1CountB = swimLaneView.SwimLaneRows[0].SwimLaneColumns[0].Count();

            // Assert
            orphanCountA.ShouldEqual(1);
            bucketCountA.ShouldEqual(1);
            column1CountA.ShouldEqual(1);

            orphanCountB.ShouldEqual(0);
            bucketCountB.ShouldEqual(0);
            column1CountB.ShouldEqual(0);
        }

        [Test]
        public void Swim_lane_view_should_handle_view_data_layout_change_events()
        {
            // Arrange
            var projectData = DataObjectHelper.GenerateProjectData(1, 1, 1, 1);
            var viewMap = projectData.ViewMaps.First();
            var service = SwimLaneService.Instance;
            service.Initialise(projectData);
            var swimLaneView = service.SwimLaneViews.First();

            // Act
            viewMap.SwimLaneStates.Add("State 3");
            viewMap.BucketStates.Add("Bucket State 2");
            viewMap.OnLayoutUpdated();

            var headerCount = swimLaneView.RowHeaders.Count();
            var bucketCount = swimLaneView.BucketStates.Count();

            var rowCount = swimLaneView.SwimLaneRows.Count();
            var orphanCount = swimLaneView.Orphans.Count();
            var column1Count = swimLaneView.SwimLaneRows[0].SwimLaneColumns[0].Count();
            var bucket1Count = swimLaneView.BucketStates[0].Count();

            // Assert
            headerCount.ShouldEqual(4);
            bucketCount.ShouldEqual(2);

            rowCount.ShouldEqual(1);
            orphanCount.ShouldEqual(1);
            column1Count.ShouldEqual(1);
            bucket1Count.ShouldEqual(1);
        }

        [Test]
        public void When_row_parent_removed_children_should_become_orphans()
        {
            // Arrange
            var projectData = DataObjectHelper.GenerateProjectData(1, 1);
            var service = SwimLaneService.Instance;
            service.Initialise(projectData);
            var swimLaneView = service.SwimLaneViews.First();
            var parentItem = projectData.WorkbenchItems.First();
            var childItem = parentItem.ChildLinks.First().Child;

            // Act
            var rowCountA = swimLaneView.SwimLaneRows.Count();
            var orpahnCountA = swimLaneView.Orphans.Count();

            //LinkChangeHelper.RemoveLink(childItem.ParentLinks.First());
            this.RaiseItemRemovedEvent(projectData, parentItem);

            var rowCountB = swimLaneView.SwimLaneRows.Count();
            var orpahnCountB = swimLaneView.Orphans.Count();

            // Assert
            rowCountA.ShouldEqual(1);
            orpahnCountA.ShouldEqual(0);

            rowCountB.ShouldEqual(0);
            orpahnCountB.ShouldEqual(1);
        }

        [Test]
        public void Child_should_be_ignored_when_added_before_parent_row()
        {
            // Arrange
            var projectData = DataObjectHelper.GenerateProjectData();
            var service = SwimLaneService.Instance;
            service.Initialise(projectData);
            var swimLaneView = service.SwimLaneViews.First();
            var childItem = DataObjectHelper.CreateWorkbenchItem()
                .SetFieldValue(Settings.Default.TypeFieldName, DataObjectHelper.ChildType)
                .SetFieldValue(Settings.Default.StateFieldName, DataObjectHelper.State1);

            var parentItem = DataObjectHelper.CreateWorkbenchItem()
                .SetFieldValue(Settings.Default.TypeFieldName, DataObjectHelper.ParentType)
                .SetFieldValue(Settings.Default.StateFieldName, DataObjectHelper.State1)
                .LinkChild(childItem, DataObjectHelper.LinkType1);

            // Act
            var rowCountA = swimLaneView.SwimLaneRows.Count();
            var orpahnCountA = swimLaneView.Orphans.Count();

            this.RaiseItemAddedEvent(projectData, childItem);

            var rowCountB = swimLaneView.SwimLaneRows.Count();
            var orpahnCountB = swimLaneView.Orphans.Count();

            // Assert
            rowCountA.ShouldEqual(0);
            orpahnCountA.ShouldEqual(0);

            rowCountB.ShouldEqual(0);
            orpahnCountB.ShouldEqual(0);
        }

        [Test]
        public void Adding_row_should_also_add_children()
        {
            // Arrange
            var projectData = DataObjectHelper.GenerateProjectData();
            var service = SwimLaneService.Instance;
            service.Initialise(projectData);

            var swimLaneView = service.SwimLaneViews.First();

            var childItem1 = DataObjectHelper.CreateWorkbenchItem()
                .SetFieldValue(Settings.Default.TypeFieldName, DataObjectHelper.ChildType)
                .SetFieldValue(Settings.Default.StateFieldName, DataObjectHelper.State1);

            var childItem2 = DataObjectHelper.CreateWorkbenchItem()
                .SetFieldValue(Settings.Default.TypeFieldName, DataObjectHelper.ChildType)
                .SetFieldValue(Settings.Default.StateFieldName, DataObjectHelper.State2);

            var childItem3 = DataObjectHelper.CreateWorkbenchItem()
                .SetFieldValue(Settings.Default.TypeFieldName, DataObjectHelper.ChildType)
                .SetFieldValue(Settings.Default.StateFieldName, DataObjectHelper.BucketState);

            var parentItem = DataObjectHelper.CreateWorkbenchItem()
                .SetFieldValue(Settings.Default.TypeFieldName, DataObjectHelper.ParentType)
                .SetFieldValue(Settings.Default.StateFieldName, DataObjectHelper.State1)
                .LinkChild(childItem1, DataObjectHelper.LinkType1)
                .LinkChild(childItem2, DataObjectHelper.LinkType1)
                .LinkChild(childItem3, DataObjectHelper.LinkType1);

            // Act
            var rowCountA = swimLaneView.SwimLaneRows.Count();

            this.RaiseItemAddedEvent(projectData, parentItem);

            var rowCountB = swimLaneView.SwimLaneRows.Count();
            var orpahnCountB = swimLaneView.Orphans.Count();
            var swimLaneRow = swimLaneView.SwimLaneRows[0];
            var column1Count = swimLaneRow[DataObjectHelper.State1].Count();
            var column2Count = swimLaneRow[DataObjectHelper.State2].Count();
            var bucketCount = swimLaneView.BucketStates[0].Count();

            // Assert
            rowCountA.ShouldEqual(0);

            rowCountB.ShouldEqual(1);
            orpahnCountB.ShouldEqual(0);
            column1Count.ShouldEqual(1);
            column2Count.ShouldEqual(1);
            bucketCount.ShouldEqual(1);
        }

        [Test]
        public void Moving_a_child_into_the_orphan_container_should_remove_parent_link()
        {
            // Arrange
            var projectData = DataObjectHelper.GenerateProjectData(1, 1);
            var service = SwimLaneService.Instance;
            service.Initialise(projectData);

            var swimLaneView = service.SwimLaneViews.First();

            var child = projectData.WorkbenchItems.First(w => Equals(w.GetTypeName(), DataObjectHelper.ChildType));
            var parent = projectData.WorkbenchItems.First(w => Equals(w.GetTypeName(), DataObjectHelper.ParentType));

            // Act
            var childLinkCountA = child.ParentLinks.Count();
            var parentLinkCountA = parent.ChildLinks.Count();

            swimLaneView.SwimLaneRows.First()[DataObjectHelper.State1].Remove(child);
            swimLaneView.Orphans.Add(child);

            var childLinkCountB = child.ParentLinks.Count();
            var parentLinkCountB = parent.ChildLinks.Count();

            // Assert
            childLinkCountA.ShouldEqual(1);
            parentLinkCountA.ShouldEqual(1);
            childLinkCountB.ShouldEqual(0);
            parentLinkCountB.ShouldEqual(0);
        }

        [Test]
        public void Moving_a_child_from_a_bucket_state_into_the_orphan_container_should_remove_parent_link()
        {
            // Arrange
            var projectData = DataObjectHelper.GenerateProjectData(1, 0, 0, 1);
            var service = SwimLaneService.Instance;
            service.Initialise(projectData);

            var swimLaneView = service.SwimLaneViews.First();

            var child = projectData.WorkbenchItems.First(w => Equals(w.GetTypeName(), DataObjectHelper.ChildType));
            var parent = projectData.WorkbenchItems.First(w => Equals(w.GetTypeName(), DataObjectHelper.ParentType));

            // Act
            var childLinkCountA = child.ParentLinks.Count();
            var parentLinkCountA = parent.ChildLinks.Count();

            swimLaneView.BucketStates.First().Remove(child);
            swimLaneView.Orphans.Add(child);

            var childLinkCountB = child.ParentLinks.Count();
            var parentLinkCountB = parent.ChildLinks.Count();

            // Assert
            childLinkCountA.ShouldEqual(1);
            parentLinkCountA.ShouldEqual(1);
            childLinkCountB.ShouldEqual(0);
            parentLinkCountB.ShouldEqual(0);
        }

        [Test]
        public void Syncronise_views_should_remove_missing_items()
        {
            // Arrange
            var projectData = DataObjectHelper.GenerateProjectData(2, 2, 2, 1);
            var service = SwimLaneService.Instance;
            service.Initialise(projectData);

            var swimLaneView = service.SwimLaneViews.First();

            // Act
            var rowCountBefore = swimLaneView.SwimLaneRows.Count();
            var rowChildCountBefore = swimLaneView.SwimLaneRows.First().SwimLaneColumns.First().Count();
            var orphanCountBefore = swimLaneView.Orphans.Count();
            var bucketCountBefore = swimLaneView.BucketStates.First().Count();

            var filteredList = new[]
                {
                    swimLaneView.SwimLaneRows.First().Parent,
                    swimLaneView.SwimLaneRows.First().Parent.ChildLinks.Select(l => l.Child).First(),
                    swimLaneView.BucketStates.First().First(),
                    swimLaneView.Orphans.First()
                };

            SwimLaneHelper.SyncroniseViewItems(swimLaneView, filteredList.ToArray());

            var rowCountAfter = swimLaneView.SwimLaneRows.Count();
            var rowChildCountAfter = swimLaneView.SwimLaneRows.First().SwimLaneColumns.First().Count();
            var orphanCountAfter = swimLaneView.Orphans.Count();
            var bucketCountAfter = swimLaneView.BucketStates.First().Count();

            // Assert
            rowCountBefore.ShouldEqual(2);
            rowChildCountBefore.ShouldEqual(2);
            orphanCountBefore.ShouldEqual(2);
            bucketCountBefore.ShouldEqual(2);

            rowCountAfter.ShouldEqual(1);
            rowChildCountAfter.ShouldEqual(1);
            orphanCountAfter.ShouldEqual(1);
            bucketCountAfter.ShouldEqual(1);
        }
        
        [Test]
        public void Moving_child_between_swimlane_states_should_not_cause_stack_overflow()
        {
            // Arrange
            const int ExpectedNumberOfLinkChanges = 1;
            var projectData = DataObjectHelper.GenerateProjectData(1, 1);
            var service = SwimLaneService.Instance;
            service.Initialise(projectData);
            var row = service.SwimLaneViews.First().SwimLaneRows.First();
            var columnA = row.SwimLaneColumns.First();
            var columnB = row.SwimLaneColumns.ElementAt(1);
            var child = columnA.First();

            var linkAddCount = 0;
            var linkRemoveCount = 0;

            EventHandler<LinkChangeEventArgs> onLinkAdded = (s, lcea) =>
                {
                    if (++linkAddCount > ExpectedNumberOfLinkChanges)
                    {
                        Assert.Fail("Number of link added events exceeds expectations.");
                    }

                    this.RaiseLinkChangedEvent(projectData, child, null, lcea.Context);
                };

            EventHandler<LinkChangeEventArgs> onLinkRemoved = (s, lcea) =>
                {
                    if (++linkRemoveCount > ExpectedNumberOfLinkChanges)
                    {
                        Assert.Fail("Number of link removed events exceeds expectations.");
                    }

                    this.RaiseLinkChangedEvent(projectData, child, lcea.Context, null);
                };

            Action<MethodInvocation> setStateAndRaiseChangeEvent = mi =>
                {
                    var newState = (string)mi.Arguments.ElementAt(1);
                    var previousState = child.GetState();

                    if (newState.Equals(previousState))
                    {
                        return;
                    }

                    child.SetFieldValue((string)mi.Arguments.First(), newState);

                    this.RaiseStateChangedEvent(projectData, child, previousState, newState);
                };

            child.Expect(c => c["System.State"] = null)
                .IgnoreArguments()
                .WhenCalled(setStateAndRaiseChangeEvent)
                .Repeat.Any();

            // Act
            var linkManagerService = ServiceManager.Instance.GetService<ILinkManagerService>();
            linkManagerService.LinkAdded += onLinkAdded;
            linkManagerService.LinkRemoved += onLinkRemoved;
            columnA.Remove(child);
            columnB.Add(child);
            linkManagerService.LinkAdded -= onLinkAdded;
            linkManagerService.LinkRemoved -= onLinkRemoved;

            // Assert
            linkAddCount.ShouldEqual(ExpectedNumberOfLinkChanges);
            linkRemoveCount.ShouldEqual(ExpectedNumberOfLinkChanges);
            columnA.Count().ShouldEqual(0);
            columnB.Count().ShouldEqual(1);
            child.GetState().ShouldEqual(DataObjectHelper.State2);
        }

        /// <summary>
        /// Raises the item removed event.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <param name="workbenchItem">The workbench item.</param>
        private void RaiseItemRemovedEvent(IProjectData projectData, IWorkbenchItem workbenchItem)
        {
            projectData.WorkbenchItems.Raise(
                repository => 
                    repository.CollectionChanged += null, 
                    this, 
                    new RepositoryChangedEventArgs<IWorkbenchItem>(ChangeActionOption.Remove, new[] { workbenchItem} ));
        }

        /// <summary>
        /// Raises the item added event.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <param name="workbenchItem">The workbench item.</param>
        private void RaiseItemAddedEvent(IProjectData projectData, IWorkbenchItem workbenchItem)
        {
            projectData.WorkbenchItems.Raise(
                repository =>
                    repository.CollectionChanged += null,
                    this,
                    new RepositoryChangedEventArgs<IWorkbenchItem>(ChangeActionOption.Add, new[] { workbenchItem }));
        }

        /// <summary>
        /// Raises the state changed event.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <param name="stateFrom">The state from.</param>
        /// <param name="stateTo">The state to.</param>
        private void RaiseStateChangedEvent(IProjectData projectData, IWorkbenchItem workbenchItem, string stateFrom, string stateTo)
        {
            projectData.WorkbenchItems.Raise(
                repository =>
                    repository.ItemStateChanged += null,
                    this,
                    new ItemStateChangeEventArgs(workbenchItem, stateFrom, stateTo));
        }

        /// <summary>
        /// Raises the link changed event.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <param name="oldLink">The old link.</param>
        /// <param name="newLink">The new link.</param>
        private void RaiseLinkChangedEvent(IProjectData projectData, IWorkbenchItem workbenchItem, ILinkItem oldLink, ILinkItem newLink)
        {
            projectData.WorkbenchItems.Raise(
                repository =>
                    repository.LinkChanged += null,
                    this,
                    new ItemLinkChangeEventArgs(workbenchItem, oldLink, newLink));
        }
    }
}
