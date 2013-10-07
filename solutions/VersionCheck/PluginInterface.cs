// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginInterface.cs" company="None">
//   Crispin Parker 2011
// </copyright>
// <summary>
//   Defines the PluginInterface type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.VersionCheck
{
    using System.Collections.Generic;
    using System.ComponentModel.Composition;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Core.Interfaces;

    using Properties;

    using TfsWorkbench.Core.Services;
    using TfsWorkbench.VersionCheck.Iterfaces;
    using TfsWorkbench.VersionCheck.Services;
    using TfsWorkbench.VersionCheck.Views;

    /// <summary>
    /// The plugin interface class.
    /// </summary>
    [Export(typeof(IWorkbenchPlugin))]
    internal class PluginInterface : IWorkbenchPlugin
    {
        /// <summary>
        /// The is initialised flag.
        /// </summary>
        private bool isInitialised;

        /// <summary>
        /// The menu item.
        /// </summary>
        private MenuItem menuItem;

        /// <summary>
        /// The plugin button.
        /// </summary>
        private UIElement button;

        /// <summary>
        /// Gets the command bindings.
        /// </summary>
        /// <value>The command bindings.</value>
        public IEnumerable<CommandBinding> CommandBindings
        {
            get { return new CommandBinding[] { }; }
        }

        /// <summary>
        /// Gets the control element.
        /// </summary>
        /// <value>The control element.</value>
        public UIElement ControlElement
        {
            get
            {
                if (!this.isInitialised)
                {
                    this.InitailiseAndDoStartupCheck();
                }

                return this.button;
            }
        }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName
        {
            get { return Resources.String001; }
        }

        /// <summary>
        /// Gets the display priority.
        /// </summary>
        /// <value>The display priority.</value>
        public int DisplayPriority
        {
            get { return Settings.Default.DisplayPriority; }
        }

        /// <summary>
        /// Gets the menu item.
        /// </summary>
        /// <value>The menu item.</value>
        public MenuItem MenuItem
        {
            get
            {
                if (!this.isInitialised)
                {
                    this.InitailiseAndDoStartupCheck();
                }

                return this.menuItem;
            }
        }

        /// <summary>
        /// Initialises the controls and executes the version startup check.
        /// </summary>
        private void InitailiseAndDoStartupCheck()
        {
            this.isInitialised = true;

            ServiceRegistor.RegisterConstructors(ServiceManager.Instance);

            var mainViewModel = ServiceManager.Instance.GetService<IMainViewModel>();

            this.menuItem = new VersionCheckMenuItem { DataContext = mainViewModel };

            this.button = new VersionCheckButton { DataContext = mainViewModel };

            if (mainViewModel.CheckVersionOnStartUp)
            {
                mainViewModel.ExecuteVersionCheck();
            }
        }
    }
}
