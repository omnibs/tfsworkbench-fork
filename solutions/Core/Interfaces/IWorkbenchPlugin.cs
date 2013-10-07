// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkbenchPlugin.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the IWorkbenchService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.Interfaces
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// The workbench service interface.
    /// </summary>
    public interface IWorkbenchPlugin
    {
        /// <summary>
        /// Gets the display name of the service.
        /// </summary>
        /// <value>The display name of the service.</value>
        string DisplayName { get; }

        /// <summary>
        /// Gets the index of the control element position.
        /// </summary>
        /// <value>The index of the control element position.</value>
        int DisplayPriority { get; }

        /// <summary>
        /// Gets the menu item.
        /// </summary>
        /// <value>The menu item.</value>
        MenuItem MenuItem { get;  }

        /// <summary>
        /// Gets the control element.
        /// </summary>
        /// <value>The control element.</value>
        UIElement ControlElement { get; }

        /// <summary>
        /// Gets the command bindings.
        /// </summary>
        /// <value>The command bindings.</value>
        IEnumerable<CommandBinding> CommandBindings { get; }
    }
}