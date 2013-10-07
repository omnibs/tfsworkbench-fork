// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TaskBoardSortOrderFixture.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the TaskBoardSortOrderFixture type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Tests
{
    using NUnit.Framework;

    using Rhino.Mocks;

    using SharpArch.Testing.NUnit;

    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.TaskBoardUI;
    using TfsWorkbench.Tests.Helpers;

    [TestFixture]
    public class TaskBoardSortOrderFixture
    {
        /// <summary>
        /// View controller, when set sort fields, maintains sort field selection.
        /// </summary>
        [Test]
        public void ViewController_WhenSetSortFields_MaintainsSortFieldSelection()
        {
            // Arrange
            const string FieldName = "FieldA";
            var viewMap = DataObjectHelper.CreateViewMap();
            var projectData = DataObjectHelper.CreateProjectData().AddViewMap(viewMap);
            var viewControlUnderTest = new ViewControl { ProjectData = projectData };
            var viewControllerUnderTest = new ViewController(
                viewControlUnderTest, MockRepository.GenerateMock<ILinkManagerService>());

            // Act
            viewMap.ParentSorter.FieldName = FieldName;
            viewControllerUnderTest.SetSortFields();
            var result = viewMap.ParentSorter.FieldName;

            // Assert
            result.ShouldEqual(FieldName);
        }
    }
}
