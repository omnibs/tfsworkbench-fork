// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HierarchyElementBase.cs" company="None">
//   None
// </copyright>
// <summary>
//   The hierarchy visaul base class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.HierarchyUI.HierarchyObjects
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;

    using TfsWorkbench.HierarchyUI.Helpers;

    /// <summary>
    /// The hierarchy visaul base class.
    /// </summary>
    public abstract class HierarchyElementBase
    {
        /// <summary>
        /// The padding between element levels.
        /// </summary>
        protected const double ElementPadding = 50;

        /// <summary>
        /// The elipse radius;
        /// </summary>
        private const double EllipseRadius = 2.5;

        /// <summary>
        /// The ellipse in.
        /// </summary>
        private Ellipse ellipseIn;

        /// <summary>
        /// The ellipse out.
        /// </summary>
        private Ellipse ellipseOut;

        /// <summary>
        /// The parent line.
        /// </summary>
        private Line parentLine;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchyElementBase"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        protected HierarchyElementBase(HierarchyElementBase parent)
        {
            this.Parent = parent;
        }

        /// <summary>
        /// Gets or sets the control.
        /// </summary>
        /// <value>The control.</value>
        public UIElement VisualElement { get; set; }

        /// <summary>
        /// Gets or sets the entry point.
        /// </summary>
        /// <value>The entry point.</value>
        public Point EntryPoint { get; set; }

        /// <summary>
        /// Gets or sets the exit point.
        /// </summary>
        /// <value>The exit point.</value>
        public Point ExitPoint { get; set; }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public HierarchyElementBase Parent { get; set; }

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <value>The children.</value>
        public abstract IEnumerable<HierarchyElementBase> Children { get; }

        /// <summary>
        /// Gets the height of the element.
        /// </summary>
        /// <value>The height of the element.</value>
        protected abstract double ElementHeight { get; }

        /// <summary>
        /// Gets the width of the element.
        /// </summary>
        /// <value>The width of the element.</value>
        protected abstract double ElementWidth { get; }

        /// <summary>
        /// Renders this instance to the specified canvas.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="orientation">The orientation.</param>
        /// <returns>The offset of the rendered item.</returns>
        public Point Render(Canvas canvas, Point offset, Orientation orientation)
        {
            if (this.VisualElement == null)
            {
                this.VisualElement = this.CreateVisualElement(orientation);
                canvas.Children.Add(this.VisualElement);
            }

            var desiredSize = this.GetDesiredSize(orientation);
            
            if (orientation == Orientation.Horizontal)
            {
                var centreX = offset.X + (desiredSize.Width / 2);
                this.EntryPoint = new Point(centreX, offset.Y);
                this.ExitPoint = new Point(centreX, offset.Y + this.ElementHeight);
                this.VisualElement.SetValue(Canvas.LeftProperty, this.EntryPoint.X - (this.ElementWidth / 2));
                this.VisualElement.SetValue(Canvas.TopProperty, this.EntryPoint.Y);
            }
            else
            {
                var centreY = offset.Y + (desiredSize.Height / 2);
                this.EntryPoint = new Point(offset.X, centreY);
                this.ExitPoint = new Point(offset.X + this.ElementWidth, centreY);
                this.VisualElement.SetValue(Canvas.LeftProperty, this.EntryPoint.X);
                this.VisualElement.SetValue(Canvas.TopProperty, this.EntryPoint.Y - (this.ElementHeight / 2));
            }

            var childOffset = orientation == Orientation.Horizontal
                                  ? new Point(offset.X, this.ExitPoint.Y + (ElementPadding * 1.5))
                                  : new Point(this.ExitPoint.X + (ElementPadding * 1.5), offset.Y);

            this.Children.Aggregate(childOffset, (current, child) => child.Render(canvas, current, orientation));

            this.DrawConnections(canvas);

            return orientation == Orientation.Horizontal 
                ? new Point(desiredSize.Width + offset.X, offset.Y)
                : new Point(offset.X, desiredSize.Height + offset.Y);
        }

        /// <summary>
        /// Draws the connections.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        public void DrawConnections(Panel canvas)
        {
            if (this.Children.Any())
            {
                if (this.ellipseOut == null)
                {
                    this.ellipseOut = new Ellipse
                        {
                            Width = EllipseRadius * 2,
                            Height = EllipseRadius * 2,
                            Fill = Brushes.Black
                        };
                    
                    canvas.Children.Add(this.ellipseOut);
                }

                this.ellipseOut.SetValue(Canvas.LeftProperty, this.ExitPoint.X - EllipseRadius);
                this.ellipseOut.SetValue(Canvas.TopProperty, this.ExitPoint.Y - EllipseRadius);
            }

            if (this.Parent == null)
            {
                return;
            }

            if (this.parentLine == null)
            {
                this.parentLine = new Line { Stroke = LayoutHelper.ConnectorBrush };

                canvas.Children.Add(this.parentLine);
            }

            this.parentLine.X1 = this.Parent.ExitPoint.X;
            this.parentLine.X2 = this.EntryPoint.X;
            this.parentLine.Y1 = this.Parent.ExitPoint.Y;
            this.parentLine.Y2 = this.EntryPoint.Y;

            if (this.ellipseIn == null)
            {
                this.ellipseIn = new Ellipse
                    {
                        Width = EllipseRadius * 2,
                        Height = EllipseRadius * 2,
                        Fill = Brushes.Black
                    };

                canvas.Children.Add(this.ellipseIn);
            }

            this.ellipseIn.SetValue(Canvas.LeftProperty, this.EntryPoint.X - EllipseRadius);
            this.ellipseIn.SetValue(Canvas.TopProperty, this.EntryPoint.Y - EllipseRadius);
        }

        /// <summary>
        /// Removes the visuals.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        public void RemoveVisuals(Canvas canvas)
        {
            foreach (var element in this.Children)
            {
                element.RemoveVisuals(canvas);
            }

            if (this.VisualElement != null)
            {
                RemoveIfPresent(canvas, this.VisualElement);

                this.VisualElement = null;
            }

            if (this.ellipseIn != null)
            {
                RemoveIfPresent(canvas, this.ellipseIn);

                this.ellipseIn = null;
            }

            if (this.ellipseOut != null)
            {
                RemoveIfPresent(canvas, this.ellipseOut);

                this.ellipseOut = null;
            }

            if (this.parentLine != null)
            {
                RemoveIfPresent(canvas, this.parentLine);

                this.parentLine = null;
            }
        }

        /// <summary>
        /// Gets the visual element.
        /// </summary>
        /// <param name="orientation">The orientation.</param>
        /// <returns>The visual element.</returns>
        protected abstract FrameworkElement CreateVisualElement(Orientation orientation);

        /// <summary>
        /// Removes if present.
        /// </summary>
        /// <param name="panel">The panel.</param>
        /// <param name="uiElement">The UI element.</param>
        private static void RemoveIfPresent(Panel panel, UIElement uiElement)
        {
            if (panel.Children.Contains(uiElement))
            {
                panel.Children.Remove(uiElement);
            }
        }

        /// <summary>
        /// Gets the desired size of the item.
        /// </summary>
        /// <param name="orientation">The orientation.</param>
        /// <returns>The desired size of the item and all its children.</returns>
        private Size GetDesiredSize(Orientation orientation)
        {
            var parentSize = this.Parent == null ? null : (Size?)new Size(this.Parent.ElementWidth, this.Parent.ElementHeight);

            var output = new Size();

            output.Height = parentSize.HasValue && parentSize.Value.Height > this.ElementHeight
                                ? parentSize.Value.Height
                                : this.ElementHeight;

            output.Width = parentSize.HasValue && parentSize.Value.Width > this.ElementWidth
                               ? parentSize.Value.Width
                               : this.ElementWidth;

            var childDesiredSizes = this.Children.Any()
                                        ? this.Children.Select(child => child.GetDesiredSize(orientation))
                                        : new[] { new Size(0, 0) };

            if (orientation == Orientation.Horizontal)
            {
                output.Width += ElementPadding / 2;

                var sumChildWidth = childDesiredSizes.Sum(s => s.Width);

                if (sumChildWidth > output.Width)
                {
                    output.Width = sumChildWidth;
                }

                if (parentSize.HasValue && output.Width < parentSize.Value.Width)
                {
                    output.Width = parentSize.Value.Width;
                }
            }
            else
            {
                output.Height += ElementPadding / 2;

                var sumChildHeight = childDesiredSizes.Sum(s => s.Height);

                if (sumChildHeight > output.Height)
                {
                    output.Height = sumChildHeight;
                }

                if (parentSize.HasValue && output.Height < parentSize.Value.Height)
                {
                    output.Height = parentSize.Value.Height;
                }
            }

            return output;
        }
    }
}