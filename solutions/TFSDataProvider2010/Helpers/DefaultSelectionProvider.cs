// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultSelectionProvider.cs" company="None">
//   None
// </copyright>
// <summary>
//   Initialises and instance of TfsWorkbench.TFSDataProvider.DefaultSelectionProvider
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using TfsWorkbench.TFSDataProvider2010.Properties;
using System;
using System.Collections.Generic;
using Microsoft.TeamFoundation.Client;

namespace TfsWorkbench.TFSDataProvider2010.Helpers
{
    /// <summary>
    /// Initialises and instance of TfsWorkbench.TFSDataProvider.DefaultSelectionProvider
    /// </summary>
    internal class DefaultSelectionProvider : ITeamProjectPickerDefaultSelectionProvider
    {
        /// <summary>
        /// Gets the default server URI.
        /// </summary>
        /// <returns>The last tfs URI if exists; otherwise null</returns>
        public Uri GetDefaultServerUri()
        {
            Uri tfsUri;

            return Uri.TryCreate(Settings.Default.LastTfsUrl, UriKind.Absolute, out tfsUri)
                ? tfsUri
                : null;
        }

        /// <summary>
        /// Gets the default collection id.
        /// </summary>
        /// <param name="instanceUri">The instance URI.</param>
        /// <returns>Not implemented</returns>
        public Guid? GetDefaultCollectionId(Uri instanceUri)
        {
            if (string.IsNullOrEmpty(Settings.Default.LastCollectionGuid))
            {
                return null;
            }

            return new Guid(Settings.Default.LastCollectionGuid);
        }

        /// <summary>
        /// Gets the default projects.
        /// </summary>
        /// <param name="collectionId">The collection id.</param>
        /// <returns>A list of the project names.</returns>
        public IEnumerable<string> GetDefaultProjects(Guid collectionId)
        {
            return new List<string> { Settings.Default.LastProjectName };
        }
    }
}