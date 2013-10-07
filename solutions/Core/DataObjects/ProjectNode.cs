// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectNode.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ProjectNode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.DataObjects
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    using Interfaces;

    /// <summary>
    /// Initializes instance of ProjectNode
    /// </summary>
    internal class ProjectNode : IProjectNode
    {
        /// <summary>
        /// The parent node.
        /// </summary>
        private readonly IProjectNode parent;

        /// <summary>
        /// The internal childred collection.
        /// </summary>
        private readonly ObservableCollection<IProjectNode> children = new ObservableCollection<IProjectNode>();

        /// <summary>
        /// The node name.
        /// </summary>
        private string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectNode"/> class.
        /// </summary>
        /// <param name="name">The node name.</param>
        /// <param name="parent">The parent.</param>
        public ProjectNode(string name, IProjectNode parent)
        {
            this.parent = parent;
            this.Name = name;
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

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
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        public ObservableCollection<IProjectNode> Children
        {
            get { return this.children; }
        }

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <value>The project node path.</value>
        public string Path
        {
            get
            {
                var parentPath = this.parent == null ? string.Empty : string.Concat(this.parent.Path, "\\");
                return string.Concat(parentPath, this.Name);
            }
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

            this.children.Clear();
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
