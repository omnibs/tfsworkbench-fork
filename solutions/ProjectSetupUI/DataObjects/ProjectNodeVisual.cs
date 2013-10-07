// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectNodeVisual.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ProjectNodeVisual type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.ProjectSetupUI.DataObjects
{
    using System;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Shapes;

    using Core.Interfaces;

    using NodeVisualisation;

    /// <summary>
    /// Initializes instance of ProjectNodeVisual
    /// </summary>
    internal class ProjectNodeVisual : INotifyPropertyChanged, IProjectNode
    {
        /// <summary>
        /// The child collection.
        /// </summary>
        private readonly ObservableCollection<IProjectNode> children = new ObservableCollection<IProjectNode>();

        /// <summary>
        /// The source node property.
        /// </summary>
        private readonly IProjectNode sourceNode;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectNodeVisual"/> class.
        /// </summary>
        /// <param name="sourceNode">The source node.</param>
        /// <param name="parent">The parent.</param>
        public ProjectNodeVisual(IProjectNode sourceNode, ProjectNodeVisual parent)
        {
            this.Parent = parent;
            this.sourceNode = sourceNode;

            foreach (var child in sourceNode.Children)
            {
                this.children.Add(new ProjectNodeVisual(child, this));
            }
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the source node.
        /// </summary>
        /// <value>The source node.</value>
        public IProjectNode SourceNode 
        { 
            get { return this.sourceNode; } 
        }

        /// <summary>
        /// Gets or sets the task board item.
        /// </summary>
        /// <value>The task board item.</value>
        public IWorkbenchItem WorkbenchItem { get; set; }

        /// <summary>
        /// Gets the in point.
        /// </summary>
        /// <value>The in point.</value>
        public Vector InPoint
        {
            get
            {
                Vector output;
                if (this.Visual == null)
                {
                    output = new Vector(0, 0);
                }
                else
                {
                    output = VisualTreeHelper.GetOffset(this.Visual);
                    output.X += this.Visual.DesiredSize.Width / 2;
                }

                return output;
            }
        }

        /// <summary>
        /// Gets the in point.
        /// </summary>
        /// <value>The in point.</value>
        public Vector OutPoint
        {
            get
            {
                Vector output;
                if (this.Visual == null)
                {
                    output = new Vector(0, 0);
                }
                else
                {
                    output = VisualTreeHelper.GetOffset(this.Visual);
                    output.X += this.Visual.DesiredSize.Width / 2;
                    output.Y += this.Visual.DesiredSize.Height;
                }

                return output;
            }
        }

        /// <summary>
        /// Gets the depth.
        /// </summary>
        /// <value>The depth.</value>
        public int Depth
        {
            get
            {
                var depth = 0;
                var parent = this.Parent;

                while (parent != null)
                {
                    depth++;
                    parent = parent.Parent;
                }

                return depth;
            }
        }

        /// <summary>
        /// Gets or sets the connection point.
        /// </summary>
        public Ellipse InPointVisual { get; set; }

        /// <summary>
        /// Gets or sets the connection point.
        /// </summary>
        public Ellipse OutPointVisual { get; set; }

        /// <summary>
        /// Gets or sets the parent line.
        /// </summary>
        /// <value>The parent line.</value>
        public Line ParentLine { get; set; }

        /// <summary>
        /// Gets or sets the Visual offset.
        /// </summary>
        public FrameworkElement Visual { get; set; }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public ProjectNodeVisual Parent { get; private set; }

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
            get { return this.sourceNode.Path; }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The node name.</value>
        public string Name
        {
            get
            {
                return this.sourceNode.Name;
            }

            set
            {
                this.sourceNode.Name = value;

                if (this.PropertyChanged == null)
                {
                    return;
                }

                this.PropertyChanged(this, new PropertyChangedEventArgs("Name"));
                this.PropertyChanged(this, new PropertyChangedEventArgs("Path"));
            }
        }

        /// <summary>
        /// Clears the children.
        /// </summary>
        public void ClearChildren()
        {
            foreach (var child in this.children)
            {
                child.ClearChildren();
            }

            this.children.Clear();
        }
    }
}
