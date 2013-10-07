// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskBoardControlTests.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the TaskBoardControlTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Tests
{
    using System.Collections.ObjectModel;
    using System.Windows.Controls;

    using TfsWorkbench.Core.DataObjects;
    using TfsWorkbench.Core.EventArgObjects;
    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.TaskBoardUI;
    using TfsWorkbench.TaskBoardUI.DataObjects;
    using TfsWorkbench.TaskBoardUI.Helpers;

    using NUnit.Framework;

    using Rhino.Mocks;

    using SharpArch.Testing.NUnit;

    using TfsWorkbench.Tests.Helpers;

    [TestFixture]
    public class TaskBoardControlTests
    {
        [Test]
        public void Display_mode_controller_should_build_tabs_when_project_data_changed()
        {
            // Arrange
            var service = MockRepository.GenerateMock<ISwimLaneService>();

            var viewMap = new ViewMap();

            viewMap.ParentTypes.Add(DataObjectHelper.ParentType);

            var itemTypeData = new ItemTypeData { TypeName = DataObjectHelper.ParentType };

            var projectData =
                DataObjectHelper.CreateProjectData().AddViewMap(viewMap).AddItemTypeData(itemTypeData);

            var swimLaneViews = new ObservableCollection<SwimLaneView>();

            System.Action initialseSwimLaneViews = () => swimLaneViews.Add(new SwimLaneView(viewMap));

            service.Expect(s => s.Initialise(null)).IgnoreArguments()
                .WhenCalled(mo => initialseSwimLaneViews()).Repeat.AtLeastOnce();

            service.Expect(s => s.SwimLaneViews).Return(swimLaneViews).Repeat.AtLeastOnce();

            var projectDataService = MockRepository.GenerateStub<IProjectDataService>();
            projectDataService.CurrentProjectData = projectData;

            var displayMode = new DisplayMode();
            new DisplayModeController(displayMode, service, projectDataService);

            // Act
            projectDataService.Raise(
                pds => pds.ProjectDataChanged += null, null, new ProjectDataChangedEventArgs(null, projectData));

            var mainTabControl = displayMode.PART_MainTabControl;

            // Assert
            service.VerifyAllExpectations();

            mainTabControl.Items.Count.ShouldEqual(1);
        }

        [Test]
        public void Main_tab_control_should_add_tab_when_view_added()
        {
            // Arrange
            var service = MockRepository.GenerateMock<ISwimLaneService>();

            var viewMap = DataObjectHelper.CreateViewMap();
            var viewMap2 = DataObjectHelper.CreateViewMap(new[] { "Parent Type 2" });

            var itemTypeData1 = new ItemTypeData { TypeName = DataObjectHelper.ParentType };
            var itemTypeData2 = new ItemTypeData { TypeName = "Parent Type 2" };

            var projectData =
                DataObjectHelper.CreateProjectData()
                .AddViewMap(viewMap)
                .AddItemTypeData(itemTypeData1)
                .AddItemTypeData(itemTypeData2);

            var swimLaneViews = new ObservableCollection<SwimLaneView>();

            System.Action initialseSwimLaneViews = () => swimLaneViews.Add(new SwimLaneView(viewMap));

            service.Expect(s => s.Initialise(null)).IgnoreArguments()
                .WhenCalled(mo => initialseSwimLaneViews()).Repeat.AtLeastOnce();

            service.Expect(s => s.SwimLaneViews).Return(swimLaneViews).Repeat.Any();

            var projectDataService = MockRepository.GenerateStub<IProjectDataService>();
            projectDataService.CurrentProjectData = projectData;

            var viewTabsControl = new DisplayMode();
            new DisplayModeController(viewTabsControl, service, projectDataService);

            // Act
            projectDataService.Raise(
                pds => pds.ProjectDataChanged += null, null, new ProjectDataChangedEventArgs(null, projectData));

            swimLaneViews.Add(new SwimLaneView(viewMap2));

            var mainTabControl = viewTabsControl.PART_MainTabControl;

            // Assert
            mainTabControl.Items.Count.ShouldEqual(2);
        }

        [Test]
        public void Main_tab_control_should_remove_tab_when_view_removed()
        {
            // Arrange
            var service = MockRepository.GenerateMock<ISwimLaneService>();

            var viewMap = DataObjectHelper.CreateViewMap();
            var viewMap2 = DataObjectHelper.CreateViewMap(new[] { "Parent Type 2" });

            var itemTypeData1 = new ItemTypeData { TypeName = DataObjectHelper.ParentType };
            var itemTypeData2 = new ItemTypeData { TypeName = "Parent Type 2" };

            var projectData =
                DataObjectHelper.CreateProjectData()
                .AddViewMap(viewMap)
                .AddItemTypeData(itemTypeData1)
                .AddItemTypeData(itemTypeData2);

            var swimLaneViews = new ObservableCollection<SwimLaneView>();

            System.Action initialseSwimLaneViews = () =>
                {
                    swimLaneViews.Add(new SwimLaneView(viewMap));
                    swimLaneViews.Add(new SwimLaneView(viewMap2));
                };

            service.Expect(s => s.Initialise(null)).IgnoreArguments()
                .WhenCalled(mo => initialseSwimLaneViews()).Repeat.AtLeastOnce();

            service.Expect(s => s.SwimLaneViews).Return(swimLaneViews).Repeat.Any();

            var projectDataService = MockRepository.GenerateStub<IProjectDataService>();
            projectDataService.CurrentProjectData = projectData;

            var viewTabsControl = new DisplayMode();
            new DisplayModeController(viewTabsControl, service, projectDataService);

            var mainTabControl = viewTabsControl.PART_MainTabControl;

            // Act
            projectDataService.Raise(
                pds => pds.ProjectDataChanged += null, null, new ProjectDataChangedEventArgs(null, projectData));

            swimLaneViews.Remove(swimLaneViews[0]);

            // Assert
            mainTabControl.Items.Count.ShouldEqual(1);
        }

        [Test]
        public void Main_tab_control_should_render_tabs_in_ascending_display_order()
        {
            // Arrange
            var service = MockRepository.GenerateMock<ISwimLaneService>();

            var swimLaneView1 = new SwimLaneView(DataObjectHelper.CreateViewMap(null, null, null, 2));
            var swimLaneView2 = new SwimLaneView(DataObjectHelper.CreateViewMap(new[] { "Parent Type 2" }, null, null, 3));
            var swimLaneView3 = new SwimLaneView(DataObjectHelper.CreateViewMap(new[] { "Parent Type 3" }, null, null, 1));

            var itemTypeData1 = new ItemTypeData { TypeName = DataObjectHelper.ParentType };
            var itemTypeData2 = new ItemTypeData { TypeName = "Parent Type 2" };
            var itemTypeData3 = new ItemTypeData { TypeName = "Parent Type 3" };

            var projectData =
                DataObjectHelper.CreateProjectData()
                .AddItemTypeData(itemTypeData1)
                .AddItemTypeData(itemTypeData2)
                .AddItemTypeData(itemTypeData3);

            var swimLaneViews = new ObservableCollection<SwimLaneView>();

            System.Action initialseSwimLaneViews = () =>
            {
                swimLaneViews.Add(swimLaneView1);
                swimLaneViews.Add(swimLaneView2);
                swimLaneViews.Add(swimLaneView3);
            };

            service.Expect(s => s.Initialise(null)).IgnoreArguments()
                .WhenCalled(mo => initialseSwimLaneViews()).Repeat.AtLeastOnce();

            service.Expect(s => s.SwimLaneViews).Return(swimLaneViews).Repeat.Any();

            var projectDataService = MockRepository.GenerateStub<IProjectDataService>();
            projectDataService.CurrentProjectData = projectData;

            var viewTabsControl = new DisplayMode();
            new DisplayModeController(viewTabsControl, service, projectDataService);

            var mainTabControl = viewTabsControl.PART_MainTabControl;

            // Act
            projectDataService.Raise(
                            pds => pds.ProjectDataChanged += null, null, new ProjectDataChangedEventArgs(null, projectData)); 
            
            var tab1 = mainTabControl.Items[0] as TabItem;
            var tab2 = mainTabControl.Items[1] as TabItem;
            var tab3 = mainTabControl.Items[2] as TabItem;

            // Assert
            tab1.ShouldNotBeNull();
            tab2.ShouldNotBeNull();
            tab3.ShouldNotBeNull();

            if (tab1 == null || tab2 == null || tab3 == null)
            {
                return;
            }

            tab1.DataContext.ShouldEqual(swimLaneView3);
            tab2.DataContext.ShouldEqual(swimLaneView1);
            tab3.DataContext.ShouldEqual(swimLaneView2);
        }
    }
}
