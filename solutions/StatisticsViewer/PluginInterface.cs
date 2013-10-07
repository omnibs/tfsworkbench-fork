// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginInterface.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the PluginInterface type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.StatisticsViewer
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.StatisticsViewer.Properties;

    /// <summary>
    /// The plugin interface class.
    /// </summary>
    [Export(typeof(IWorkbenchPlugin))]
    internal class PluginInterface : IWorkbenchPlugin
    {
        /// <summary>
        /// The statistics controller instance.
        /// </summary>
        private readonly IStatisticsController controller;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginInterface"/> class.
        /// </summary>
        public PluginInterface()
        {
            this.controller = StatisticsController.Instance;
            this.CommandBindings = new[]
                {
                    new CommandBinding(
                        LocalCommandLibrary.ShowStatisticsViewerCommand,
                        this.controller.OnShowStatistics,
                        this.controller.OnCanExecute)
                };

            this.ControlElement = new StatisticsViewerButton();
            this.MenuItem = new StatisticsViewerMenuItem();
        }

        /// <summary>
        /// Gets the display name of the service.
        /// </summary>
        /// <value>The display name of the service.</value>
        public string DisplayName
        {
            get
            {
                return Settings.Default.DisplayName;
            }
        }

        /// <summary>
        /// Gets the index of the control element position.
        /// </summary>
        /// <value>The index of the control element position.</value>
        public int DisplayPriority
        {
            get
            {
                return Settings.Default.DisplayPriority;
            }
        }

        /// <summary>
        /// Gets the menu item.
        /// </summary>
        /// <value>The menu item.</value>
        public MenuItem MenuItem { get; private set; }

        /// <summary>
        /// Gets the control element.
        /// </summary>
        /// <value>The control element.</value>
        public UIElement ControlElement { get; private set; }

        /// <summary>
        /// Gets the command bindings.
        /// </summary>
        /// <value>The command bindings.</value>
        public IEnumerable<CommandBinding> CommandBindings { get; private set; }
    }
}
