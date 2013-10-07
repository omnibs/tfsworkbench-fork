namespace TfsWorkbench.Tests.WpfUiProjectSelector
{
    using System.Collections.Generic;
    using System.Linq;

    using Rhino.Mocks;

    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.Core.Properties;

    public class ProjectDataGenerationHelper
    {
        /// <summary>
        /// The collection end point string.
        /// </summary>
        public const string CollectionEndPoint = "http://host:8080/tfs/defaultcollection";

        /// <summary>
        /// The test project name.
        /// </summary>
        public const string ProjectName = "Project Name";

        /// <summary>
        /// Gets the project with project nodes.
        /// </summary>
        /// <returns>An instacne of the project data with project nodes.</returns>
        public static IProjectData GenerateProjectData()
        {
            var projectData = GenerateProjectDataWithoutNodes();

            var rootAreaNode = MockRepository.GenerateMock<IProjectNode>();
            var rootIterationNode = MockRepository.GenerateMock<IProjectNode>();

            var projectNodes = new Dictionary<string, IProjectNode>
                {
                    { Settings.Default.AreaPathFieldName, rootAreaNode },
                    { Settings.Default.IterationPathFieldName, rootIterationNode }
                };

            projectData.Expect(pd => pd.ProjectNodes)
                .Return(projectNodes)
                .Repeat.Any();

            return projectData;
        }

        /// <summary>
        /// Generates the project data without nodes.
        /// </summary>
        /// <returns>An instance of project data without any node object.</returns>
        public static IProjectData GenerateProjectDataWithoutNodes()
        {
            var projectData = MockRepository.GenerateMock<IProjectData>();

            projectData
                .Expect(pd => pd.ProjectCollectionUrl)
                .Return(CollectionEndPoint)
                .Repeat.Any();

            projectData
                .Expect(pd => pd.ProjectName)
                .Return(ProjectName)
                .Repeat.Any();
            
            SetupAreaPath(projectData);
            SetupIterationPath(projectData);

            return projectData;
        }

        /// <summary>
        /// Setups the area path.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        private static void SetupAreaPath(IProjectData projectData)
        {
            string areaPath = null;
            projectData
                .Expect(pd => pd.ProjectAreaPath = null)
                .IgnoreArguments()
                .WhenCalled(mi => areaPath = mi.Arguments.First() as string)
                .Repeat.Any();

            projectData
                .Expect(pd => pd.ProjectAreaPath)
                .WhenCalled(mi => mi.ReturnValue = areaPath)
                .Return(null)
                .Repeat.Any();
        }

        /// <summary>
        /// Setups the iteration path.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        private static void SetupIterationPath(IProjectData projectData)
        {
            string iterationPath = null;

            projectData
                .Expect(pd => pd.ProjectIterationPath = null)
                .IgnoreArguments()
                .WhenCalled(mi => iterationPath = mi.Arguments.First() as string)
                .Repeat.Any();

            projectData
                .Expect(pd => pd.ProjectIterationPath)
                .WhenCalled(mi => mi.ReturnValue = iterationPath)
                .Return(null)
                .Repeat.Any();
        }
    }
}