// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Factory.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the Factory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using DataObjects;

    using Interfaces;

    using Properties;

    /// <summary>
    /// Initializes instance of Factory
    /// </summary>
    public static class Factory
    {
        /// <summary>
        /// Builds the item.
        /// </summary>
        /// <param name="valueProvider">The value provider.</param>
        /// <returns>A new instance of the specified type.</returns>
        public static IWorkbenchItem BuildItem(IValueProvider valueProvider)
        {
            var output = new WorkbenchItem
                {
                    ValueProvider = valueProvider
                };

            output.ValueProvider.WorkbenchItem = output;

            return output;
        }

        /// <summary>
        /// Builds the project node.
        /// </summary>
        /// <param name="parentNode">The parent node.</param>
        /// <param name="nodeValue">The node value.</param>
        /// <returns>A new instance of a project node.</returns>
        public static IProjectNode BuildProjectNode(IProjectNode parentNode, string nodeValue)
        {
            var newNode = new ProjectNode(nodeValue, parentNode);

            if (parentNode != null)
            {
                parentNode.Children.Add(newNode);
            }

            return newNode;
        }

        /// <summary>
        /// Builds the project data instance.
        /// </summary>
        /// <param name="projectCollectionUri">The project collection URI.</param>
        /// <param name="projectCollectionGuid">The project collection GUID.</param>
        /// <param name="projectName">Name of the project.</param>
        /// <param name="viewMaps">The view maps.</param>
        /// <returns>A new project data instance.</returns>
        public static IProjectData BuildProjectData(Uri projectCollectionUri, Guid? projectCollectionGuid, string projectName, IEnumerable<ViewMap> viewMaps)
        {
            if (projectCollectionUri == null)
            {
                throw new ArgumentNullException("projectCollectionUri");
            }

            var projectData = new ProjectData
            {
                ProjectCollectionUrl = projectCollectionUri.AbsoluteUri,
                ProjectGuid = projectCollectionGuid,
                WebAccessUrl = BuildDefaultWebAccessUrl(projectCollectionUri).AbsoluteUri,
                ProjectName = projectName
            };

            if (viewMaps != null)
            {
                foreach (var map in viewMaps)
                {
                    projectData.ViewMaps.Add(map);
                }
            }

            return projectData;
        }

        /// <summary>
        /// Builds the default web access URL.
        /// </summary>
        /// <param name="projectCollectionUri">The project collection URI.</param>
        /// <returns>The default web access url.</returns>
        public static Uri BuildDefaultWebAccessUrl(Uri projectCollectionUri)
        {
            if (projectCollectionUri == null)
            {
                throw new ArgumentNullException("projectCollectionUri");
            }

            var webAccessUrl = string.Concat(
                projectCollectionUri.Scheme, "://", projectCollectionUri.Authority, "/tfs/web");

            return new Uri(webAccessUrl, UriKind.Absolute);
        }

        /// <summary>
        /// Builds a link item instance.
        /// </summary>
        /// <param name="linkName">Name of the link.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="child">The child.</param>
        /// <returns>A new instance of a link item interface.</returns>
        public static ILinkItem BuildLinkItem(string linkName, IWorkbenchItem parent, IWorkbenchItem child)
        {
            return new LinkItem { LinkName = linkName ?? string.Empty, Child = child, Parent = parent };
        }

        /// <summary>
        /// Builds the state colour.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <returns>An instance of the state colour type.</returns>
        public static StateColour BuildStateColour(string state)
        {
            if (state == null)
            {
                throw new ArgumentNullException("state");
            }

            var output = state.Equals(Settings.Default.ExclusionState) 
                ? new StateColour { Colour = Settings.Default.ExcludedStateColour, Value = state } 
                : new StateColour { Colour = Settings.Default.ItemColour, Value = state };

            return output;
        }
    }
}