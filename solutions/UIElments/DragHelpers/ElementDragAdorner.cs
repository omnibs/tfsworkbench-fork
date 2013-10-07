// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ElementDragAdorner.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the DragAdorner type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.UIElements.DragHelpers
{
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Shapes;

    /// <summary>
    /// Defines the DragAdorner type.
    /// </summary>
    public class ElementDragAdorner : Adorner
    {
        /// <summary>
        /// The top offset.
        /// </summary>
        private double topOffset;

        /// <summary>
        /// The left offset.
        /// </summary>
        private double leftOffset;

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementDragAdorner"/> class.
        /// </summary>
        /// <param name="owner">
        /// The owner.
        /// </param>
        public ElementDragAdorner(UIElement owner) : base(owner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementDragAdorner"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="adornElement">The adorn element.</param>
        /// <param name="useVisualBrush">The use visual brush.</param>
        /// <param name="opacity">The opacity.</param>
        public ElementDragAdorner(UIElement owner, UIElement adornElement, bool useVisualBrush, double opacity) : base(owner)
        {
            this.Owner = owner;
            if (useVisualBrush)
            {
                this.Brush = new VisualBrush(adornElement) { Opacity = opacity };
                var r = new Rectangle
                    {
                        RadiusX = 3,
                        RadiusY = 3,
                        Width = adornElement.DesiredSize.Width,
                        Height = adornElement.DesiredSize.Height
                    };

                this.XCenter = adornElement.DesiredSize.Width / 2;
                this.YCenter = adornElement.DesiredSize.Height / 2;

                r.Fill = this.Brush;
                this.Child = r;
            }
            else
            {
                this.Child = adornElement;
            }
        }

        /// <summary>
        /// Gets or sets the scale.
        /// </summary>
        /// <value>The scale.</value>
        public double Scale
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets LeftOffset.
        /// </summary>
        /// <value>The left offset.</value>
        public double LeftOffset
        {
            get
            {
                return this.leftOffset;
            }

            set
            {
                this.leftOffset = value - this.XCenter;
                this.UpdatePosition();
            }
        }

        /// <summary>
        /// Gets or sets TopOffset.
        /// </summary>
        /// <value>The top offset.</value>
        public double TopOffset
        {
            get
            {
                return this.topOffset;
            }

            set
            {
                this.topOffset = value - this.YCenter;

                this.UpdatePosition();
            }
        }

        /// <summary>
        /// Gets or sets the visual brush.
        /// </summary>
        /// <value>The brush.</value>
        protected VisualBrush Brush 
        {
            get; 
            set;
        }

        /// <summary>
        /// Gets or sets the child.
        /// </summary>
        /// <value>The child.</value>
        protected UIElement Child
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the owner.
        /// </summary>
        /// <value>The owner.</value>
        protected UIElement Owner
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the X center.
        /// </summary>
        /// <value>The X center.</value>
        protected double XCenter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the Y center.
        /// </summary>
        /// <value>The Y center.</value>
        protected double YCenter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets VisualChildrenCount.
        /// </summary>
        /// <value>
        /// The visual children count.
        /// </value>
        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Returns a <see cref="T1:System.Windows.Media.Transform"/> for the adorner, based on the transform that is currently applied to the adorned element.
        /// </summary>
        /// <param name="transform">The transform.</param>
        /// <returns>A transform to apply to the adorner.</returns>
        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            var result = new GeneralTransformGroup();

            result.Children.Add(base.GetDesiredTransform(transform));
            result.Children.Add(new TranslateTransform(this.leftOffset, this.topOffset));
            return result;
        }

        /// <summary>
        /// When overridden in a derived class, positions child elements and determines a size for a <see cref="T1:System.Windows.FrameworkElement"/> derived class.
        /// </summary>
        /// <param name="finalSize">The final size.</param>
        /// <returns>The actual size used.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            this.Child.Arrange(new Rect(this.Child.DesiredSize));
            return finalSize;
        }

        /// <summary>
        /// Overrides <see cref="M:System.Windows.Media.Visual.GetVisualChild(System.Int32)"/>, and returns a child at the specified index from a collection of child elements.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>
        /// The requested child element. This should not return null; if the provided index is out of range, an exception is thrown.
        /// </returns>
        protected override Visual GetVisualChild(int index)
        {
            return this.Child;
        }

        /// <summary>
        /// Measures the override.
        /// </summary>
        /// <param name="constraint">The final size.</param>
        /// <returns>Measure override.</returns>
        protected override Size MeasureOverride(Size constraint)
        {
            this.Child.Measure(constraint);
            return this.Child.DesiredSize;
        }

        /// <summary>
        /// Updates the position.
        /// </summary>
        private void UpdatePosition()
        {
            var adorner = (AdornerLayer)this.Parent;
            if (adorner != null)
            {
                adorner.Update(this.AdornedElement);
            }
        }
    }
}