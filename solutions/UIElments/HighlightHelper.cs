// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HighlightHelper.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the HighlightHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.UIElements
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Documents;

    /// <summary>
    /// The highlight helper class.
    /// </summary>
    public static class HighlightHelper
    {
        /// <summary>
        /// The adorned element collection.
        /// </summary>
        private static readonly ICollection<FrameworkElement> adornedElements = new Collection<FrameworkElement>();

        /// <summary>
        /// Clears all highlights.
        /// </summary>
        public static void ClearAllHighlights()
        {
            foreach (var adornedElement in adornedElements)
            {
                ClearHighlight(adornedElement);
            }

            adornedElements.Clear();
        }

        /// <summary>
        /// Hightlights the specified element.
        /// </summary>
        /// <param name="elementToHighlight">The element to highlight.</param>
        public static void Highlight(FrameworkElement elementToHighlight)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(elementToHighlight);

            if (adornerLayer == null)
            {
                return;
            }

            var highlightAdorner = new HighlightAdorner(elementToHighlight);

            adornerLayer.Add(highlightAdorner);

            adornedElements.Add(elementToHighlight);
        }

        /// <summary>
        /// Clears the highlights.
        /// </summary>
        /// <param name="element">The element.</param>
        private static void ClearHighlight(UIElement element)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(element);

            if (adornerLayer == null)
            {
                return;
            }

            var adorners = adornerLayer.GetAdorners(element);

            if (adorners == null)
            {
                return;
            }

            foreach (var highlight in adorners.OfType<HighlightAdorner>())
            {
                adornerLayer.Remove(highlight);
            }
        }
    }
}
