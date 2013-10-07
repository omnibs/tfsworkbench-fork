// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginInterface.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the Service type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.ReportViewer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Core.Interfaces;

    using TfsWorkbench.ReportViewer.Properties;

    /// <summary>
    /// The public service class.
    /// </summary>
    [Export(typeof(IWorkbenchPlugin))]
    internal class PluginInterface : IWorkbenchPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginInterface"/> class.
        /// </summary>
        public PluginInterface()
            : this(new ReportController())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginInterface"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public PluginInterface(IReportController controller)
        {
            this.MenuItem = new ReportViewerMenuItem(controller);
            this.ControlElement = new ReportViewerButton(controller);
            this.CommandBindings = new[]
                {
                    new CommandBinding(
                        LocalCommandLibrary.ShowReportViewerCommand,
                        (s, e) => controller.ShowReportViewer(e.Source as UIElement),
                        (s, e) => e.CanExecute = controller.HasLoadedReportList)
                };
        }

        /// <summary>
        /// Gets the display name of the plug in.
        /// </summary>
        /// <value>The display name of the plug in.</value>
        public string DisplayName
        {
            get
            {
                return Settings.Default.PluginName;
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
    }
}
