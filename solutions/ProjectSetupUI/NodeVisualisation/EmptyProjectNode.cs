// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmptyProjectNode.cs" company="None">
//   None
// </copyright>
// <summary>
//   The empty project node class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.ProjectSetupUI.NodeVisualisation
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    using TfsWorkbench.Core.Interfaces;

    /// <summary>
    /// The empty project node class.
    /// Create to allow bound controls to be cleared.
    /// </summary>
    internal sealed class EmptyProjectNode : IProjectNode
    {
        /// <summary>
        /// The node name.
        /// </summary>
        private string name;

        /// <summary>
        /// Prevents a default instance of the <see cref="EmptyProjectNode"/> class from being created.
        /// </summary>
        private EmptyProjectNode()
        {
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        public ObservableCollection<IProjectNode> Children
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <value>The project node path.</value>
        public string Path
        {
            get { return null; }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The node name.</value>
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = value;

                this.OnPropertyChanged("Name");
            }
        }

        /// <summary>
        /// Creates an instance of the empty project node class.
        /// </summary>
        /// <returns>An instance of the empty project node class.</returns>
        public static EmptyProjectNode CreateInstance()
        {
            return new EmptyProjectNode();
        }

        /// <summary>
        /// Clears the children.
        /// </summary>
        public void ClearChildren()
        {
            foreach (var child in this.Children)
            {
                child.ClearChildren();
            }

            this.Children.Clear();
        }

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged == null)
            {
                return;
            }

            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
