// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MouseWheelScrollBubbler.cs" company="None">
//   None
// </copyright>
// <summary>
//   Initialises and instance of TfsWorkbench.UIElements.MouseWheelScrollBubbler
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.UIElements
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Initialises and instance of TfsWorkbench.UIElements.MouseWheelScrollBubbler
    /// </summary>
    public static class MouseWheelScrollBubbler
    {
        /// <summary>
        /// Handles the preview mouse wheel.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="MouseWheelEventArgs"/> instance containing the event data.</param>
        public static void HandleMouseWheel(object sender, RoutedEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                return;
            }

            var element = e.OriginalSource as FrameworkElement;
            var mouseEventArgs = e as MouseWheelEventArgs;

            if (element == null || mouseEventArgs == null || e.Handled)
            {
                return;
            }

            var scrollControl = element.GetFirstChildElementOfType<ScrollViewer>();

            if (scrollControl == null)
            {
                return;
            }

            var isScrolledPastTop = mouseEventArgs.Delta > 0 && scrollControl.VerticalOffset == 0;
            var isScrolledPastBottom = mouseEventArgs.Delta <= 0 && scrollControl.VerticalOffset >= scrollControl.ExtentHeight - scrollControl.ViewportHeight;

            if (!isScrolledPastTop && !isScrolledPastBottom)
            {
                return;
            }

            e.Handled = true;

            var parent = element.Parent as UIElement;
            if (parent == null)
            {
                return;
            }

            var eventArg = new MouseWheelEventArgs(mouseEventArgs.MouseDevice, mouseEventArgs.Timestamp, mouseEventArgs.Delta)
                {
                    RoutedEvent = UIElement.MouseWheelEvent,
                    Source = sender
                };

            parent.RaiseEvent(eventArg);
        }
    }
}
