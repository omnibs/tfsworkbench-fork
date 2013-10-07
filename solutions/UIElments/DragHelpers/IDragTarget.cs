// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDragTarget.cs" company="None">
//   None
// </copyright>
// <summary>
//   Default interface for ItemsControls that can accept drag drop.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.UIElements.DragHelpers
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Default interface for ItemsControls that can accept drag drop.
    /// </summary>
    /// <typeparam name="TElement">The type of the element.</typeparam>
    public interface IDragTarget<TElement>
    {
        /// <summary>
        /// Occurs when [preview mouse left button down].
        /// </summary>
        event MouseButtonEventHandler PreviewMouseLeftButtonDown;

        /// <summary>
        /// Occurs when [preview mouse left button up].
        /// </summary>
        event MouseButtonEventHandler PreviewMouseLeftButtonUp;

        /// <summary>
        /// Occurs when [preview mouse move].
        /// </summary>
        event MouseEventHandler PreviewMouseMove;

        /// <summary>
        /// Occurs when [drop].
        /// </summary>
        event DragEventHandler Drop;

        /// <summary>
        /// Occurs when [drag over].
        /// </summary>
        event DragEventHandler DragOver;

        /// <summary>
        /// Gets or sets a value indicating whether [allow drop] is enabled.
        /// </summary>
        /// <value>
        /// <c>True</c> if [allow drop]; otherwise, <c>false</c>.
        /// </value>
        bool AllowDrop
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the actual width.
        /// </summary>
        /// <value>
        /// The actual width.
        /// </value>
        double ActualWidth
        {
            get;
        }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>
        /// The items.
        /// </value>
        ItemCollection Items
        {
            get;
        }

        /// <summary>
        /// Gets the item container generator.
        /// </summary>
        /// <value>The item container generator.</value>
        ItemContainerGenerator ItemContainerGenerator
        {
            get;
        }

        /// <summary>
        /// Gets the elements to drag.
        /// </summary>
        /// <value>
        /// The elements to drag.
        /// </value>
        IEnumerable<TElement> ElementsToDrag
        {
            get;
        }

        /// <summary>
        /// Gets the items source.
        /// </summary>
        /// <value>The items source.</value>
        IEnumerable ItemsSource { get; }

        /// <summary>
        /// Returns the input element locatated at the point.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>Input element found at specified point.</returns>
        IInputElement InputHitTest(Point point);

        /// <summary>
        /// Determines whether this instance [is valid drop location] for [the specified drop items].
        /// </summary>
        /// <param name="dropItems">The drop items.</param>
        /// <returns>
        /// <c>True</c> if this instance [is valid drop location] for [the specified drop items]; otherwise, <c>false</c>.
        /// </returns>
        bool TestDropLocation(IEnumerable<TElement> dropItems);

        /// <summary>
        /// Creates the adorner element.
        /// </summary>
        /// <param name="elements">The dragged elements.</param>
        /// <returns>A uielement based on the dragged items.</returns>
        UIElement CreateAdornerElement(IEnumerable<TElement> elements);
    }
}