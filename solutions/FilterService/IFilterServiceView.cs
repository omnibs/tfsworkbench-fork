// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFilterServiceView.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the IFilterView type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.FilterService
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Threading;

    /// <summary>
    /// The filter view interface.
    /// </summary>
    public interface IFilterServiceView : IInputElement
    {
        /// <summary>
        /// Gets the command bindings.
        /// </summary>
        /// <value>The command bindings.</value>
        CommandBindingCollection CommandBindings { get; }

        /// <summary>
        /// Gets or sets the selected WorkbenchFilter.
        /// </summary>
        /// <returns>The selected WorkbenchFilter.</returns>
        WorkbenchFilter WorkbenchFilter { get; set; }

        /// <summary>
        /// Gets or sets the controller.
        /// </summary>
        /// <value>The controller.</value>
        IFilterServiceController Controller { get; set; }

        /// <summary>
        /// Gets the dispatcher.
        /// </summary>
        /// <value>The dispatcher.</value>
        Dispatcher Dispatcher { get; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is enabled; otherwise, <c>false</c>.
        /// </value>
        new bool IsEnabled { get; set; }

        /// <summary>
        /// Gets the value entry control.
        /// </summary>
        /// <value>The value entry control.</value>
        ContentControl ValueCotnrol { get; }
    }
}