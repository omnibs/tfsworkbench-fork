// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetupControllerHelper.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the SetupControllerHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.ProjectSetupUI.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Windows;

    using Core.Helpers;
    using Core.Interfaces;
    using DataObjects;
    using Properties;

    using TfsWorkbench.Core.DataObjects;
    using TfsWorkbench.Core.Services;
    using TfsWorkbench.UIElements;

    /// <summary>
    /// Initializes instance of SetupControllerHelper
    /// </summary>
    internal class SetupControllerHelper
    {
        /// <summary>
        /// Gets the project data service.
        /// </summary>
        /// <value>The project data service.</value>
        private static IProjectDataService ProjectDataService
        {
            get
            {
                return ServiceManager.Instance.GetService<IProjectDataService>();
            }
        }

        /// <summary>
        /// Tries to create a child workbench item.
        /// </summary>
        /// <param name="node">The releated node.</param>
        /// <param name="parentType">Type of the parent.</param>
        /// <param name="childType">Type of the child.</param>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <returns><c>True</c> if the child workbench item is created; otherwise <c>false</c>.</returns>
        public static bool TryCreateChildWorkbenchItem(ProjectNodeVisual node, string parentType, string childType, out IWorkbenchItem workbenchItem)
        {
            workbenchItem = null;
            IWorkbenchItem parentWorkbenchItem = null;
            var parent = node.Parent;
            while (parent != null)
            {
                if (parent.WorkbenchItem != null && parent.WorkbenchItem.GetTypeName().Equals(parentType))
                {
                    parentWorkbenchItem = parent.WorkbenchItem;
                    break;
                }

                parent = parent.Parent;
            }

            if (parentWorkbenchItem != null)
            {
                workbenchItem = CreateChildItem(parentWorkbenchItem, childType);
            }

            return workbenchItem != null;
        }

        /// <summary>
        /// Creates the top level workbench item.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <returns>The new workbench item.</returns>
        public static IWorkbenchItem CreateTopLevelWorkbenchItem(string typeName)
        {
            return ProjectDataService.CreateNewItem(typeName);
        }

        /// <summary>
        /// Determines whether [the specified project data] [is a valid scrum project].
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <returns>
        /// <c>true</c> if [the specified project data] [is a valid scrum project]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidScrumProject(IProjectData projectData)
        {
            if (projectData == null)
            {
                return false;
            }

            var isValidProject = projectData.ItemTypes[Settings.Default.ReleaseType] != null &&
                                 projectData.ItemTypes[Settings.Default.SprintType] != null &&
                                 projectData.ItemTypes[Settings.Default.TeamType] != null;

            return isValidProject;
        }

        /// <summary>
        /// Determines whether [the specified project data] [has root path loaded].
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <returns>
        /// <c>True</c> if [the specified project data] [has root path loaded]; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasRootPathLoaded(IProjectData projectData)
        {
            return projectData.ProjectIterationPath.Equals(projectData.ProjectName);
        }

        /// <summary>
        /// Adds the work stream.
        /// </summary>
        /// <param name="projectSetup">The project setup.</param>
        /// <returns>The project setup instance including a new work stream element.</returns>
        public static ProjectSetup AddWorkStream(ProjectSetup projectSetup)
        {
            var workStream = new WorkStream
            {
                Cadance = Settings.Default.DefaultWorkStreamCadance,
                Name = GetNextName(projectSetup.WorkStreams, "WorkStream")
            };

            projectSetup.WorkStreams.Add(workStream);

            return projectSetup;
        }

        /// <summary>
        /// Adds the release.
        /// </summary>
        /// <param name="projectSetup">The project setup.</param>
        /// <returns>
        /// The ProjectAdvancedSetup including a new release instance.
        /// </returns>
        public static ProjectSetup AddRelease(ProjectSetup projectSetup)
        {
            var release =
                new Release
                {
                    Name = GetNextName(projectSetup.Releases, Settings.Default.ReleaseType)
                };

            // Use the last release for duration info.
            var previousRelease = projectSetup.Releases.OrderBy(r => r.EndDate).LastOrDefault();

            DateTime? startDate, endDate;

            if (previousRelease == null)
            {
                startDate = projectSetup.StartDate;
                endDate = projectSetup.EndDate;
            }
            else
            {
                startDate = previousRelease.StartDate;
                endDate = previousRelease.EndDate;
            }

            if (!startDate.HasValue || !endDate.HasValue)
            {
                throw new NullReferenceException("The dates are not valid.");
            }

            if (ValidationHelper.IsValidDateRange(startDate, endDate))
            {
                var duration = endDate.Value.Subtract(startDate.Value);

                var isInitialRelease = projectSetup.Releases.Count() == 0;

                if (isInitialRelease)
                {
                    release.StartDate = startDate;
                    release.EndDate = endDate;
                }
                else
                {
                    release.StartDate = endDate.Value.AddDays(1);
                    release.EndDate = endDate.Value.AddDays(duration.Days + 1);
                }
            }

            projectSetup.Releases.Add(release);

            return projectSetup;
        }

        /// <summary>
        /// Adds the team.
        /// </summary>
        /// <param name="projectSetup">The project setup.</param>
        /// <returns>
        /// The project setup instance includung a new team.
        /// </returns>
        public static ProjectSetup AddTeam(ProjectSetup projectSetup)
        {
            var team = new Team
            {
                WorkStream = projectSetup.WorkStreams.FirstOrDefault(),
                Name = GetNextName(projectSetup.Teams, "Team"),
                Capacity = Settings.Default.DefaultTeamCapacity
            };

            projectSetup.Teams.Add(team);

            return projectSetup;
        }

        /// <summary>
        /// Gets the next name in sequence.
        /// </summary>
        /// <param name="namedItems">The named items.</param>
        /// <param name="prefix">The prefix.</param>
        /// <returns>An instance of a numbered name.</returns>
        public static string GetNextName(IEnumerable<INamedItem> namedItems, string prefix)
        {
            var suffixCount = 1;

            Func<int, string> getName = i => string.Format(CultureInfo.InvariantCulture, "{0} {1:00}", prefix, i);
            Func<string, bool> isUnique = s => !namedItems.Any(n => n.Name.Equals(s));

            var newName = getName(suffixCount);

            while (!isUnique(newName))
            {
                newName = getName(++suffixCount);
            }

            return newName;
        }

        /// <summary>
        /// Populates the releases.
        /// </summary>
        /// <param name="projectSetup">The project setup.</param>
        /// <returns>A collection of populated release objects.</returns>
        public static IEnumerable<Release> CreateProjectStructure(ProjectSetup projectSetup)
        {
            var root = projectSetup.ProjectNode;

            root.Children.Clear();

            var releases = projectSetup.Releases.Clone().ToArray();

            foreach (var release in releases)
            {
                var releaseStartDate = release.StartDate;
                var releaseEndDate = release.EndDate;

                if (!releaseEndDate.HasValue || !releaseStartDate.HasValue)
                {
                    throw new Exception(
                        "The specified release object is not valid. A release must have start end dates and a least one valid work stream.");
                }

                var releaseNode = Factory.BuildProjectNode(root, release.Name);
                release.IterationPath = releaseNode.Path;

                // Add the workstreams to the release.
                foreach (var workStream in projectSetup.WorkStreams.Clone())
                {
                    release.WorkStreams.Add(workStream);
                }

                foreach (var workStream in release.WorkStreams)
                {
                    var workStreamNode = Factory.BuildProjectNode(releaseNode, workStream.Name);
                    workStream.IterationPath = workStreamNode.Path;

                    var workingDate = releaseStartDate;

                    while (workingDate.Value.Ticks < releaseEndDate.Value.Ticks)
                    {
                        // Create a new sprint.
                        var sprint = new Sprint
                        {
                            Name = GetNextName(workStream.Sprints, "Sprint"),
                            StartDate = workingDate.Value,
                            EndDate = workingDate.Value.AddDays(workStream.Cadance.Value - 1)
                        };

                        var sprintNode = Factory.BuildProjectNode(workStreamNode, sprint.Name);

                        sprint.IterationPath = sprintNode.Path;

                        workStream.Sprints.Add(sprint);

                        var stream = workStream;
                        Func<Team, bool> teamPredicate =
                            t => t.WorkStream.Name.Equals(stream.Name) && t.WorkStream.Cadance.Equals(stream.Cadance);

                        foreach (var team in projectSetup.Teams.Where(teamPredicate).Clone())
                        {
                            team.StartDate = sprint.StartDate;
                            team.EndDate = sprint.EndDate;
                            team.IterationPath = Factory.BuildProjectNode(sprintNode, team.Name).Path;

                            sprint.Teams.Add(team);
                        }

                        workingDate = workingDate.Value.AddDays(workStream.Cadance.Value);
                    }
                }
            }

            return releases;
        }

        /// <summary>
        /// Clears the existing items.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        public static void ClearExistingItems(IProjectData projectData)
        {
            Func<IWorkbenchItem, bool> predicate = tbi =>
            {
                var typeName = tbi.GetTypeName();
                var state = tbi.GetState();

                if (state.Equals(Core.Properties.Settings.Default.ExclusionState))
                {
                    return false;
                }

                return typeName.Equals(Settings.Default.ReleaseType) ||
                       typeName.Equals(Settings.Default.SprintType) ||
                       typeName.Equals(Settings.Default.TeamType);
            };

            foreach (var workbenchItem in projectData.WorkbenchItems.Where(predicate).ToArray())
            {
                if (workbenchItem.ValueProvider.IsNew)
                {
                    projectData.WorkbenchItems.Remove(workbenchItem);
                    continue;
                }

                workbenchItem.ValueProvider.SyncToLatest();
                workbenchItem[Core.Properties.Settings.Default.IterationPathFieldName] = projectData.ProjectName;
                workbenchItem.SetState(Core.Properties.Settings.Default.ExclusionState);
            }
        }

        /// <summary>
        /// Adds the new structure items.
        /// </summary>
        /// <param name="releases">The releases.</param>
        /// <param name="dataProvider">The data provider.</param>
        /// <param name="projectData">The project data.</param>
        public static void AddNewStructureItems(IEnumerable<Release> releases, IDataProvider dataProvider, IProjectData projectData)
        {
            foreach (var release in releases)
            {
                var tbiRelease = ProjectDataService.CreateNewItem(Settings.Default.ReleaseType);

                tbiRelease[Core.Properties.Settings.Default.IterationPathFieldName] = release.IterationPath;
                tbiRelease[Settings.Default.ReleaseStartDateFieldName] = release.StartDate;
                tbiRelease[Settings.Default.ReleaseEndDateFieldName] = release.EndDate;

                projectData.WorkbenchItems.Add(tbiRelease);

                foreach (var sprint in release.WorkStreams.SelectMany(w => w.Sprints))
                {
                    var tbiSprint = CreateChildItem(tbiRelease, Settings.Default.SprintType);

                    tbiSprint[Core.Properties.Settings.Default.IterationPathFieldName] = sprint.IterationPath;
                    tbiSprint[Settings.Default.SprintStartDateFieldName] = sprint.StartDate;
                    tbiSprint[Settings.Default.SprintEndDateFieldName] = sprint.EndDate;

                    projectData.WorkbenchItems.Add(tbiSprint);

                    foreach (var team in sprint.Teams)
                    {
                        var tbiTeam = CreateChildItem(tbiSprint, Settings.Default.TeamType);

                        tbiTeam[Core.Properties.Settings.Default.IterationPathFieldName] = team.IterationPath;
                        tbiTeam[Settings.Default.TeamStartDateFieldName] = team.StartDate;
                        tbiTeam[Settings.Default.TeamEndDateFieldName] = team.EndDate;
                        tbiTeam[Settings.Default.TeamCapacityField] = team.Capacity;

                        projectData.WorkbenchItems.Add(tbiTeam);

                        CommandLibrary.SaveItemCommand.Execute(tbiTeam, Application.Current.MainWindow);
                    }

                    CommandLibrary.SaveItemCommand.Execute(tbiSprint, Application.Current.MainWindow);
                }

                CommandLibrary.SaveItemCommand.Execute(tbiRelease, Application.Current.MainWindow);
            }
        }

        /// <summary>
        /// Creates the child item.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="childTypeName">Name of the child type.</param>
        /// <returns>A new child instance.</returns>
        private static IWorkbenchItem CreateChildItem(IWorkbenchItem parent, string childTypeName)
        {
            var childCreationParameters = new ChildCreationParameters
                {
                    ChildTypeName = childTypeName,
                    LinkTypeName = Settings.Default.RelationLinkName,
                    Parent = parent
                };

            return ProjectDataService.CreateNewChild(childCreationParameters);
        }
    }
}
