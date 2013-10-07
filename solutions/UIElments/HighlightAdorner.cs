// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HighlightAdorner.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the HighlightAdorner type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.UIElements
{
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Media;

    /// <summary>
    /// The highlight adorner.
    /// </summary>
    public class HighlightAdorner : Adorner
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HighlightAdorner"/> class.
        /// </summary>
        /// <param name="adornedElement">The element to bind the adorner to.</param>
        /// <exception cref="T:System.ArgumentNullException">adornedElement is null.</exception>
        public HighlightAdorner(UIElement adornedElement) : base(adornedElement)
        {
            this.IsHitTestVisible = false;
        }

        /// <summary>
        /// When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            var renderSize = this.AdornedElement.RenderSize;
            var desiredSize = this.AdornedElement.DesiredSize;

            var size = new Size
                {
                    Height =
                        renderSize.Height > desiredSize.Height
                            ? renderSize.Height
                            : desiredSize.Height,
                    Width =
                        renderSize.Width > desiredSize.Width
                            ? renderSize.Width
                            : desiredSize.Width
                };

            var targetRect = new Rect(size);

            var fillBrush = new SolidColorBrush(Colors.Red) { Opacity = 0.1 };

            var pen = new Pen(new SolidColorBrush(Colors.Red) { Opacity = 0.5 }, 4);

            drawingContext.DrawRectangle(fillBrush, pen, targetRect);
        }
    }
}
