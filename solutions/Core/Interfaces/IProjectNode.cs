// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IProjectNode.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the IProjectNode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.Interfaces
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    /// <summary>
    /// The project node interface
    /// </summary>
    public interface IProjectNode : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        ObservableCollection<IProjectNode> Children { get; }

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <value>The project node path.</value>
        string Path { get; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The node name.</value>
        string Name { get; set; }

        /// <summary>
        /// Clears the children.
        /// </summary>
        void ClearChildren();
    }
}