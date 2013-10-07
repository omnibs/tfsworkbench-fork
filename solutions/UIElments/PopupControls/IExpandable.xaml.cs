// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IExpandable.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the IExpandable type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.UIElements.PopupControls
{
    /// <summary>
    /// The expandable interface.
    /// </summary>
    public interface IExpandable
    {
        /// <summary>
        /// Gets a value indicating whether this instance is expanded.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is expanded; otherwise, <c>false</c>.
        /// </value>
        bool IsExpanded { get; }

        /// <summary>
        /// Expands this instance.
        /// </summary>
        void Expand();
    }
}