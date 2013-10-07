// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDisplayMode.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the IDisplayMode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.Interfaces
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// The task board dock item interface.
    /// </summary>
    public interface IDisplayMode : IHighlightProvider
    {
        /// <summary>
        /// Gets the display priority.
        /// </summary>
        /// <value>The display priority.</value>
        int DisplayPriority { get;  }

        /// <summary>
        /// Gets the menu control.
        /// </summary>
        /// <value>The menu control.</value>
        MenuItem MenuControl { get; }

        /// <summary>
        /// Gets the visibility.
        /// </summary>
        /// <value>The visibility.</value>
        Visibility Visibility { get; }
    }
}