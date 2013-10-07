// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ElementDragHelper.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ElementDragHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TfsWorkbench.HierarchyUI.HierarchyObjects;
using TfsWorkbench.UIElements;

namespace TfsWorkbench.HierarchyUI.Helpers
{
    /// <summary>
    /// The element drag helper class.
    /// </summary>
    internal static class ElementDragHelper
    {
        /// <summary>
        /// The offset point.
        /// </summary>
        private static Point offset;

        /// <summary>
        /// The selected visual.
        /// </summary>
        private static ContentControl selectedVisual;

        /// <summary>
        /// Registers the scroll viewer.
        /// </summary>
        /// <param name="scrollViewer">The scroll viewer.</param>
        public static void RegisterScrollViewer(ScrollViewer scrollViewer)
        {
            scrollViewer.PreviewMouseDown += OnPreviewMouseDown;
            scrollViewer.PreviewMouseUp += OnPreviewMouseUp;
            scrollViewer.PreviewMouseMove += OnPreviewMouseMove;
        }

        /// <summary>
        /// Called when [preview mouse up].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private static void OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (selectedVisual != null)
            {
                var canvas = selectedVisual.GetParentOfType<Canvas>();
                if (canvas != null)
                {
                    canvas.UpdateLayout();
                    LayoutHelper.MoveElementsToTopLeft(canvas);
                    LayoutHelper.ResizeCanvasToContent(canvas);
                }
            }

            selectedVisual = null;

            Mouse.OverrideCursor = null;
        }
        
        /// <summary>
        /// Called when [preview mouse down].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private static void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed || e.ClickCount != 1)
            {
                selectedVisual = null;
                return;
            }

            selectedVisual = e.Source as ContentControl;

            if (selectedVisual == null)
            {
                return;
            }

            var canvas = selectedVisual.GetParentOfType<Canvas>();
            if (canvas == null)
            {
                return;
            }

            var vector = VisualTreeHelper.GetOffset(selectedVisual);
            var position = Mouse.GetPosition(canvas);
            offset = new Point(position.X - vector.X, position.Y - vector.Y);

            Mouse.OverrideCursor = CustomCursors.MoveHand;
        }

        /// <summary>
        /// Called when [preview mouse move].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private static void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (selectedVisual == null)
            {
                return;
            }

            HierarchyElementBase hierarchyElement = null;

            if (selectedVisual is HierarchyItemNode)
            {
                hierarchyElement = ((HierarchyItemNode)selectedVisual).HierarchyItem;
            }

            if (selectedVisual is HierarchyViewNode)
            {
                hierarchyElement = ((HierarchyViewNode)selectedVisual).HierarchyView;
            }

            if (hierarchyElement == null)
            {
                return;
            }

            var canvas = selectedVisual.GetParentOfType<Canvas>();
            if (canvas == null)
            {
                return;
            }

            var position = Mouse.GetPosition(canvas);
            var offsetPosition = new Point(position.X - offset.X, position.Y - offset.Y);
            var currentPosition = VisualTreeHelper.GetOffset(selectedVisual);
            var delta = new Point(currentPosition.X - offsetPosition.X, currentPosition.Y - offsetPosition.Y);

            MoveVisual(canvas, selectedVisual, hierarchyElement, delta);
        }

        /// <summary>
        /// Moves the visual elments.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="visual">The visual.</param>
        /// <param name="hierarchyElement">The hierarchy element.</param>
        /// <param name="delta">The delta.</param>
        private static void MoveVisual(Panel canvas, DependencyObject visual, HierarchyElementBase hierarchyElement, Point delta)
        {
            var startX = (double)visual.GetValue(Canvas.LeftProperty);
            var startY = (double)visual.GetValue(Canvas.TopProperty);

            visual.SetValue(Canvas.LeftProperty, startX - delta.X);
            visual.SetValue(Canvas.TopProperty, startY - delta.Y);

            hierarchyElement.EntryPoint = ApplyDelta(hierarchyElement.EntryPoint, delta);
            hierarchyElement.ExitPoint = ApplyDelta(hierarchyElement.ExitPoint, delta);

            hierarchyElement.DrawConnections(canvas);

            foreach (var child in hierarchyElement.Children)
            {
                if (Keyboard.Modifiers != ModifierKeys.Control)
                {
                    MoveVisual(canvas, child.VisualElement, child, delta);
                }
                else
                {
                    child.DrawConnections(canvas);
                }
            }
        }

        /// <summary>
        /// Applies the delta.
        /// </summary>
        /// <param name="startingPoint">The starting point.</param>
        /// <param name="delta">The delta.</param>
        /// <returns>The adjusted point value.</returns>
        private static Point ApplyDelta(Point startingPoint, Point delta)
        {
            var output = new Point(startingPoint.X - delta.X, startingPoint.Y - delta.Y);

            return output;
        }
    }
}
