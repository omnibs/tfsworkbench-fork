// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataProviderController.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the IDataProviderController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.WpfUI.Controllers
{
    using System;
    using System.Windows;

    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.WpfUI.Controls;

    /// <summary>
    /// The data provider controller interface.
    /// </summary>
    public interface IDataProviderController : IDisposable
    {
        /// <summary>
        /// Clears the control cache.
        /// </summary>
        void ClearControlCache();

        /// <summary>
        /// Resets the project layout.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <returns>The reset project data.</returns>
        IProjectData ResetProjectLayout(IProjectData projectData);

        /// <summary>
        /// Resets the project data.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <returns>The reset project data.</returns>
        IProjectData RefreshProjectData(IProjectData projectData);

        /// <summary>
        /// Begins the save project data event.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        void BeginSaveProjectData(IProjectData projectData);

        /// <summary>
        /// Generates the project load control.
        /// </summary>
        /// <returns>A new instacne of the project load control.</returns>
        FrameworkElement GenerateProjectLoadControl();

        /// <summary>
        /// Gets the control items.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <returns>The associated control item collection.</returns>
        IControlItemGroup GetControlItems(IWorkbenchItem workbenchItem);
    }
}