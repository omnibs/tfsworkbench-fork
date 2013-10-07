// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectSetupUITests.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ProjectSetupUITests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Tests
{
    using System;
    using System.Collections.ObjectModel;

    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.Core.Services;
    using TfsWorkbench.ProjectSetupUI.DataObjects;
    using TfsWorkbench.ProjectSetupUI.Helpers;

    using NUnit.Framework;

    using Rhino.Mocks;

    using SharpArch.Testing.NUnit;

    using TfsWorkbench.Tests.Helpers;

    /// <summary>
    /// The project setup ui text fixture.
    /// </summary>
    [TestFixture]
    public class ProjectSetupUiTests
    {
        [Test]
        public void Setup_controller_helper_should_facilitate_child_workbench_item_creation()
        {
            // Arrange
            var projectDataService = MockRepository.GenerateMock<IProjectDataService>();
            projectDataService
                .Expect(pds => pds.CreateNewChild(null))
                .IgnoreArguments()
                .Return(DataObjectHelper.CreateWorkbenchItem());

            var projectNode = MockRepository.GenerateMock<IProjectNode>();
            projectNode.Expect(pn => pn.Children)
                .Return(new ObservableCollection<IProjectNode>())
                .Repeat.Any();
            
            var parentWorkItem =
                DataObjectHelper.CreateWorkbenchItem()
                .SetFieldValue(Core.Properties.Settings.Default.TypeFieldName, DataObjectHelper.ParentType);

            var parentNodeVisual = new ProjectNodeVisual(projectNode, null) { WorkbenchItem = parentWorkItem };
            var nodeVisual = new ProjectNodeVisual(projectNode, parentNodeVisual);

            IWorkbenchItem child;

            // Act
            ServiceManagerHelper.MockServiceManager(projectDataService);
            var result = SetupControllerHelper.TryCreateChildWorkbenchItem(
                new ProjectNodeVisual(projectNode, nodeVisual),
                DataObjectHelper.ParentType,
                DataObjectHelper.ChildType,
                out child);
            ServiceManagerHelper.ClearDummyManager();

            // Assert
            result.ShouldBeTrue();
            child.ShouldNotBeNull();
        }

        [Test]
        public void Setup_controller_helper_should_create_top_level_parent_item()
        {
            // Arrange
            var projectDataService = MockRepository.GenerateMock<IProjectDataService>();
            projectDataService
                .Expect(pds => pds.CreateNewItem(DataObjectHelper.ParentType))
                .Return(DataObjectHelper.CreateWorkbenchItem())
                .Repeat.Once();

            // Act
            ServiceManagerHelper.MockServiceManager(projectDataService);
            var result = SetupControllerHelper.CreateTopLevelWorkbenchItem(DataObjectHelper.ParentType);
            ServiceManagerHelper.ClearDummyManager();

            // Assert
            projectDataService.VerifyAllExpectations();
            result.ShouldNotBeNull();
        }

        [Test]
        public void Setup_controller_helper_should_validate_project_type()
        {
            // Arrange
            var scrumProject = DataObjectHelper.CreateProjectData()
                .AddItemTypeData(new Core.DataObjects.ItemTypeData { TypeName = ProjectSetupUI.Properties.Settings.Default.ReleaseType })
                .AddItemTypeData(new Core.DataObjects.ItemTypeData { TypeName = ProjectSetupUI.Properties.Settings.Default.SprintType })
                .AddItemTypeData(new Core.DataObjects.ItemTypeData { TypeName = ProjectSetupUI.Properties.Settings.Default.TeamType });

            var nonScrumProject = DataObjectHelper.CreateProjectData();

                // Act
            var resultA = SetupControllerHelper.IsValidScrumProject(scrumProject);
            var resultB = SetupControllerHelper.IsValidScrumProject(nonScrumProject);

            // Assert
            resultA.ShouldBeTrue();
            resultB.ShouldBeFalse();
        }

        [Test]
        public void Setup_controller_helper_should_add_worksreams()
        {
            // Arrange
            var projectSetup = new ProjectSetup("Test");

            // Act
            var result = SetupControllerHelper.AddWorkStream(projectSetup);

            // Assert
            result.WorkStreams.Count.ShouldEqual(1);
        }

        [Test]
        public void Setup_controller_helper_should_add_releases()
        {
            // Arrange
            var projectSetup = new ProjectSetup("Test") { StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(30) };

            // Act
            var result = SetupControllerHelper.AddRelease(projectSetup);

            // Assert
            result.Releases.Count.ShouldEqual(1);
        }

        [Test]
        public void Setup_controller_helper_should_add_teams()
        {
            // Arrange
            var projectSetup = new ProjectSetup("Test");

            // Act
            var result = SetupControllerHelper.AddTeam(projectSetup);

            // Assert
            result.Teams.Count.ShouldEqual(1);
        }

        [Test]
        public void Setup_controller_helper_should_test_if_root_path_is_loaded()
        {
            // Arrange
            var projectData = MockRepository.GenerateStub<IProjectData>();
            projectData.ProjectName = "Project Name";

            // Act
            projectData.ProjectIterationPath = "Project Name";
            var resultA = SetupControllerHelper.HasRootPathLoaded(projectData);
            projectData.ProjectIterationPath = "Project Name/Sub Node";
            var resultB = SetupControllerHelper.HasRootPathLoaded(projectData);

            // Assert
            resultA.ShouldBeTrue();
            resultB.ShouldBeFalse();
        }

        [Test]
        public void Setup_controller_helper_should_generate_numbered_names()
        {
            // Arrange
            var namedObjects = new System.Collections.Generic.List<INamedItem>();

            for (var i = 2; i < 5; i++)
            {
                var namedItem = MockRepository.GenerateMock<INamedItem>();
                namedItem.Expect(ni => ni.Name).Return(string.Concat("Item 0", i)).Repeat.Any();
                namedObjects.Add(namedItem);
            }

            // Act
            var resultA = SetupControllerHelper.GetNextName(namedObjects, "Item");

            var resultItem = MockRepository.GenerateMock<INamedItem>();
            resultItem.Expect(ni => ni.Name).Return(resultA).Repeat.Any();
            namedObjects.Add(resultItem);

            var resultB = SetupControllerHelper.GetNextName(namedObjects, "Item");

            // Assert
            resultA.ShouldEqual("Item 01");
            resultB.ShouldEqual("Item 05");
        }
    }
}
