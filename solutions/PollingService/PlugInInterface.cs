// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PlugInInterface.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the PlugInInterface type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.PollingService
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.PollingService.Properties;

    /// <summary>
    /// The plugin interface class.
    /// </summary>
    [Export(typeof(IWorkbenchPlugin))]
    internal class PluginInterface : IWorkbenchPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PluginInterface"/> class.
        /// </summary>
        public PluginInterface()
            : this(new PollingServiceController())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PluginInterface"/> class.
        /// </summary>
        /// <param name="controller">The controller.</param>
        public PluginInterface(IPollingServiceController controller)
        {
            this.MenuItem = new PollingServiceMenuItem(controller);
            this.ControlElement = new PollingServiceButton(controller);
            this.CommandBindings = new[]
                {
                    new CommandBinding(
                        LocalCommandLibrary.ShowPollerDialogCommand,
                        (s, e) => controller.ShowChangePollerControl(e.Source as UIElement),
                        (s, e) => e.CanExecute = controller.CanShowDialog)
                };
        }

        /// <summary>
        /// Gets the display name of the service.
        /// </summary>
        /// <value>The display name of the service.</value>
        public string DisplayName
        {
            get
            {
                return Settings.Default.PluginName;
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
                return Settings.Default.PluginPriority;
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
