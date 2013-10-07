// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatisticsServiceTests.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the StatisticsServiceTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Tests
{
    using System.Linq;

    using NUnit.Framework;

    using Rhino.Mocks;

    using SharpArch.Testing.NUnit;

    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.Core.Services;
    using TfsWorkbench.StatisticsViewer;
    using TfsWorkbench.StatisticsViewer.Properties;
    using TfsWorkbench.StatisticsViewer.StatisticsGroups;
    using TfsWorkbench.Tests.Helpers;

    /// <summary>
    /// The statistics service test fixture class.
    /// </summary>
    [TestFixture]
    public class StatisticsServiceTests
    {
        private IProjectData projectData;

        [SetUp]
        public void SetUp()
        {
            this.GenerateProjectDataService();
        }

        [TearDown]
        public void TearDown()
        {
            ServiceManagerHelper.ClearDummyManager();
        }

        /// <summary>
        /// Test: Statistics_service_should_indicate_validity_status.
        /// </summary>
        [Test]
        public void Statistics_service_should_indicate_validity_status()
        {
            // Arrange
            var service = new StatisticsService();

            // Act
            this.projectData = null;
            var resultA = service.IsValid;

            this.projectData = MockRepository.GenerateMock<IProjectData>();
            var resultB = service.IsValid;

            // Assert
            resultA.ShouldBeFalse();
            resultB.ShouldBeTrue();
        }

        /// <summary>
        /// Test: Statistics_service_should_provide_list_of_statistics.
        /// </summary>
        [Test]
        public void Statistics_service_should_provide_list_of_statistics()
        {
            // Arrange
            var service = new StatisticsService();

            // Act
            this.projectData = null;
            var resultsA = service.GetStatistics();

            this.projectData = DataObjectHelper.GenerateProjectData();
            var resultsB = service.GetStatistics();

            // Assert
            resultsA.ShouldNotBeNull();
            resultsA.Count().ShouldEqual(0);

            resultsB.ShouldNotBeNull();
            resultsB.Count().ShouldBeGreaterThan(0);
        }

        /// <summary>
        /// Test: Workspace_statistics_should_include_a_header_and_a_line_item_for_each_work_item_type.
        /// </summary>
        [Test]
        public void Workspace_statistics_should_include_a_header_and_a_line_item_for_each_work_item_type()
        {
            // Arrange
            this.projectData = DataObjectHelper.GenerateProjectData(2, 2);

            var statistics = new WorkspaceGroup(this.projectData);

            // Act
            var header = statistics.Header;
            var columnHeaders = statistics.ColumnHeaders;
            var lineItems = statistics.Lines;

            // Assert
            header.ShouldEqual(Resources.String008);
            columnHeaders.Count().ShouldEqual(2);
            columnHeaders.ElementAt(0).ShouldEqual(Resources.String002);
            columnHeaders.ElementAt(1).ShouldEqual(Resources.String003);
            lineItems.Count().ShouldEqual(3);

            var parentLine = lineItems.FirstOrDefault(li => li.RowHeader.Equals(DataObjectHelper.ParentType));
            var childLine = lineItems.FirstOrDefault(li => li.RowHeader.Equals(DataObjectHelper.ChildType));
            var totalLine = lineItems.Last();

            parentLine.ShouldNotBeNull();
            childLine.ShouldNotBeNull();

            parentLine.Values.Count().ShouldEqual(2);
            parentLine.Values.ElementAt(0).ShouldEqual("2 / (2)");
            childLine.Values.Count().ShouldEqual(2);
            childLine.Values.ElementAt(0).ShouldEqual("4 / (4)");

            totalLine.Values.Count().ShouldEqual(2);
            totalLine.Values.ElementAt(0).ShouldEqual("6 / (6)");
        }

        /// <summary>
        /// Generates the project data service.
        /// </summary>
        private void GenerateProjectDataService()
        {
            var projectDataService = MockRepository.GenerateMock<IProjectDataService>();

            projectDataService
                .Expect(pds => pds.CurrentProjectData)
                .WhenCalled(mi => mi.ReturnValue = this.projectData)
                .Return(null)
                .Repeat.Any();

            ServiceManagerHelper.MockServiceManager(projectDataService);
            ILinkManagerService linkManagerService = new LinkManagerService();
            ServiceManagerHelper.RegisterServiceInstance(linkManagerService);
        }
    }
}
