// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectSelectorViewModel.cs" company="None">
//   None
// </copyright>
// <summary>
//   The project selector view model interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.WpfUI.ProjectSelector
{
    using System.ComponentModel;
    using System.Windows.Input;

    using TfsWorkbench.Core.Interfaces;

    /// <summary>
    /// The project selector view model interface.
    /// </summary>
    public interface IProjectSelectorViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the show project selector command.
        /// </summary>
        /// <value>The show project selector command.</value>
        ICommand ShowProjectSelectorCommand { get; }

        /// <summary>
        /// Gets the load project data command.
        /// </summary>
        /// <value>The load project data command.</value>
        ICommand LoadProjectDataCommand { get; }

        /// <summary>
        /// Gets the cancel command.
        /// </summary>
        /// <value>The cancel command.</value>
        ICommand CancelCommand { get; }

        /// <summary>
        /// Gets the ensure project nodes loaded.
        /// </summary>
        /// <value>The ensure project nodes loaded.</value>
        ICommand EnsureProjectNodesLoadedCommand { get; }

        /// <summary>
        /// Gets the name of the selected project.
        /// </summary>
        /// <value>The name of the selected project.</value>
        string SelectedProjectName { get; }

        /// <summary>
        /// Gets the selected collectionend point.
        /// </summary>
        /// <value>The selected collectionend point.</value>
        string SelectedCollectionEndPoint { get; }

        /// <summary>
        /// Gets or sets the iteration path.
        /// </summary>
        /// <value>The iteration path.</value>
        string IterationPath { get; set; }

        /// <summary>
        /// Gets or sets AreaPath.
        /// </summary>
        string AreaPath { get; set; }

        /// <summary>
        /// Gets the area root node.
        /// </summary>
        /// <value>The area root node.</value>
        IProjectNode AreaRootNode { get; }

        /// <summary>
        /// Gets the iteration root node.
        /// </summary>
        /// <value>The iteration root node.</value>
        IProjectNode IterationRootNode { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is busy.
        /// </summary>
        /// <value><c>true</c> if this instance is busy; otherwise, <c>false</c>.</value>
        bool IsBusy { get; }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        /// <value>The error message.</value>
        string ErrorMessage { get; }
    }
}