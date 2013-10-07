// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NodeLayoutHelper.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the NodeLayoutHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.ProjectSetupUI.NodeVisualisation
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Shapes;

    using Core.Interfaces;

    using DataObjects;

    using UIElements;

    /// <summary>
    /// Initializes instance of NodeLayoutHelper
    /// </summary>
    internal static class NodeLayoutHelper
    {
        /// <summary>
        /// The node element height.
        /// </summary>
        public const double ElementHeight = 75d;

        /// <summary>
        /// The node element width.
        /// </summary>
        public const double ElementWidth = 125d;

        /// <summary>
        /// The node horizontal spacing.
        /// </summary>
        public const double HorizontalSpacing = ElementWidth / 8;

        /// <summary>
        /// The node vertical spacing.
        /// </summary>
        public const double VerticalSpacing = ElementHeight * 1.25;

        /// <summary>
        /// The selected visual
        /// </summary>
        private static ContentControl selectedVisual;

        /// <summary>
        /// The canvas registration flag.
        /// </summary>
        private static bool isCanvasRegistered;

        /// <summary>
        /// The mouse offset.
        /// </summary>
        private static Point offset;

        /// <summary>
        /// The path node template.
        /// </summary>
        private static DataTemplate pathNodeTemplate;

        /// <summary>
        /// The line style.
        /// </summary>
        private static Style lineStyle;

        /// <summary>
        /// The ellipse style.
        /// </summary>
        private static Style ellipseStyle;

        /// <summary>
        /// Renders the project node visuals.
        /// </summary>
        /// <param name="layoutCanvas">The layout canvas.</param>
        /// <param name="pathFilter">The path filter.</param>
        /// <param name="projectNode">The project node.</param>
        public static void RenderProjectNodeVisuals(Canvas layoutCanvas, string pathFilter, ProjectNodeVisual projectNode)
        {
            if (!isCanvasRegistered)
            {
                RegisterCanvas(layoutCanvas);
            }
            
            layoutCanvas.Children.Clear();
            RenderFilteredVisuals(layoutCanvas, pathFilter, projectNode);
            MoveElementsToTopLeft(layoutCanvas);

            layoutCanvas.UpdateLayout();

            ResizeCanvasToContent(layoutCanvas);
        }

        /// <summary>
        /// Renders the visual element.
        /// </summary>
        /// <param name="layoutCanvas">The layout canvas.</param>
        /// <param name="pathFilter">The path filter.</param>
        /// <param name="projectNode">The project node.</param>
        public static void RenderFilteredVisuals(Panel layoutCanvas, string pathFilter, ProjectNodeVisual projectNode)
        {
            if (string.IsNullOrEmpty(pathFilter) || projectNode.Path.StartsWith(pathFilter))
            {
                RenderVisualIfMissing(layoutCanvas, projectNode);
                RenderConnectionVisuals(layoutCanvas, projectNode);
            }

            foreach (var child in projectNode.Children.OfType<ProjectNodeVisual>())
            {
                RenderFilteredVisuals(layoutCanvas, pathFilter, child);
            }
        }

        /// <summary>
        /// Sizes the canvas to it's content.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        public static void ResizeCanvasToContent(Panel canvas)
        {
            var frameworkElements = canvas.Children.OfType<FrameworkElement>();

            var hasChildren = frameworkElements.Any();

            var maxWidth = hasChildren ? frameworkElements.Max(c => VisualTreeHelper.GetOffset(c).X + c.ActualWidth) : 0;
            var maxHeight = hasChildren ? frameworkElements.Max(c => VisualTreeHelper.GetOffset(c).Y + c.ActualHeight) : 0;

            canvas.Width = maxWidth < 0 ? 0 : maxWidth;
            canvas.Height = maxHeight < 0 ? 0 : maxHeight;
        }

        /// <summary>
        /// Clears all the visual elements of the specified node.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="node">The project node.</param>
        public static void ClearVisuals(Canvas canvas, ProjectNodeVisual node)
        {
            Action<UIElement> removeIfPresent = e =>
                {
                    if (e == null)
                    {
                        return;
                    }

                    if (canvas.Children.Contains(e))
                    {
                        canvas.Children.Remove(e);
                    }
                };

            removeIfPresent(node.InPointVisual);
            removeIfPresent(node.OutPointVisual);
            removeIfPresent(node.ParentLine);
            removeIfPresent(node.Visual);

            node.InPointVisual = null;
            node.OutPointVisual = null;
            node.ParentLine = null;
            node.Visual = null;

            foreach (var child in node.Children.OfType<ProjectNodeVisual>())
            {
                ClearVisuals(canvas, child);
            }
        }

        /// <summary>
        /// Registers the canvas.
        /// </summary>
        /// <param name="layoutCanvas">The layout canvas.</param>
        private static void RegisterCanvas(FrameworkElement layoutCanvas)
        {
            isCanvasRegistered = true;
            var canvasVisualParent = layoutCanvas.GetParentOfType<ScrollViewer>();
            if (canvasVisualParent == null)
            {
                throw new ArgumentException("Canvas scroll view parent cannot be found.");
            }

            pathNodeTemplate = (DataTemplate)layoutCanvas.FindResource("PathNodeTemplate");
            lineStyle = (Style)layoutCanvas.FindResource("NodeLinkLine");
            ellipseStyle = (Style)layoutCanvas.FindResource("NodePointEllipse");

            canvasVisualParent.PreviewMouseMove += OnPreviewMouseMove;
            canvasVisualParent.PreviewMouseLeftButtonUp += OnPreviewLeftMouseUp;
        }

        /// <summary>
        /// Moves the elements to top.
        /// </summary>
        /// <param name="layoutCanvas">The layout canvas.</param>
        private static void MoveElementsToTopLeft(Panel layoutCanvas)
        {
            var frameworkElements = layoutCanvas.Children.OfType<FrameworkElement>();
            if (!frameworkElements.Any())
            {
                return;
            }

            var minTop = frameworkElements.OfType<ContentControl>().Min(c => VisualTreeHelper.GetOffset(c).Y);
            var minLeft = frameworkElements.OfType<ContentControl>().Min(c => VisualTreeHelper.GetOffset(c).X);

            // Move the rendered elements up to the top left of the canvas
            foreach (var element in frameworkElements)
            {
                var elementAsLine = element as Line;

                if (elementAsLine != null)
                {
                    elementAsLine.Y1 -= minTop;
                    elementAsLine.Y2 -= minTop;
                    elementAsLine.X1 -= minLeft;
                    elementAsLine.X2 -= minLeft;
                    continue;
                }

                element.SetValue(Canvas.TopProperty, (double)element.GetValue(Canvas.TopProperty) - minTop);
                element.SetValue(Canvas.LeftProperty, (double)element.GetValue(Canvas.LeftProperty) - minLeft);
            }
        }

        /// <summary>
        /// Renders the connection visuals.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="visualNode">The visual node.</param>
        private static void RenderConnectionVisuals(Panel canvas, ProjectNodeVisual visualNode)
        {
            var hasChildren = visualNode.Children.Count() != 0;
            var hasVisibleParent = visualNode.Parent != null && canvas.Children.Contains(visualNode.Parent.Visual);

            if (hasChildren)
            {
                visualNode.OutPointVisual = visualNode.OutPointVisual ?? CreateNodeEllipse();

                visualNode.OutPointVisual.SetValue(Canvas.TopProperty, visualNode.OutPoint.Y);
                visualNode.OutPointVisual.SetValue(Canvas.LeftProperty, visualNode.OutPoint.X);

                if (!canvas.Children.Contains(visualNode.OutPointVisual))
                {
                    canvas.Children.Add(visualNode.OutPointVisual);
                }
            }

            if (hasVisibleParent)
            {
                visualNode.ParentLine = visualNode.ParentLine ?? CreateParentLine();
                visualNode.InPointVisual = visualNode.InPointVisual ?? CreateNodeEllipse();

                visualNode.InPointVisual.SetValue(Canvas.TopProperty, visualNode.InPoint.Y);
                visualNode.InPointVisual.SetValue(Canvas.LeftProperty, visualNode.InPoint.X);

                visualNode.ParentLine.X1 = visualNode.Parent.OutPoint.X;
                visualNode.ParentLine.X2 = visualNode.InPoint.X;
                visualNode.ParentLine.Y1 = visualNode.Parent.OutPoint.Y;
                visualNode.ParentLine.Y2 = visualNode.InPoint.Y;

                if (!canvas.Children.Contains(visualNode.InPointVisual))
                {
                    canvas.Children.Add(visualNode.InPointVisual);
                }

                if (!canvas.Children.Contains(visualNode.ParentLine))
                {
                    canvas.Children.Add(visualNode.ParentLine);
                }
            }
        }

        /// <summary>
        /// Gets the parent line.
        /// </summary>
        /// <returns>A new line instance.</returns>
        private static Line CreateParentLine()
        {
            return new Line { Style = lineStyle };
        }

        /// <summary>
        /// Gets the node ellipse.
        /// </summary>
        /// <returns>An instance of an ellipse.</returns>
        private static Ellipse CreateNodeEllipse()
        {
            var ellipse = new Ellipse
                {
                    Style = ellipseStyle
                };

            return ellipse;
        }

        /// <summary>
        /// Creates the Visual.
        /// </summary>
        /// <param name="visualNode">The visual node.</param>
        /// <returns>A Visual element</returns>
        private static FrameworkElement CreateVisual(ProjectNodeVisual visualNode)
        {
            return new ContentControl
                {
                    Content = visualNode,
                    ContentTemplate = pathNodeTemplate
                };
        }

        /// <summary>
        /// Renders the visual if missing.
        /// </summary>
        /// <param name="layoutCanvas">The layout canvas.</param>
        /// <param name="visualNode">The visual node.</param>
        private static void RenderVisualIfMissing(Panel layoutCanvas, ProjectNodeVisual visualNode)
        {
            if (visualNode.Visual != null && layoutCanvas.Children.Contains(visualNode.Visual))
            {
                return;
            }

            var requiredWidth = GetRequiredWidth(visualNode);

            var parentPoint = visualNode.Parent == null ? new Vector() : visualNode.Parent.OutPoint;

            var preceedingSiblings = visualNode.Parent != null 
                                         ? visualNode.Parent.Children.TakeWhile(c => !Equals(c, visualNode)) 
                                         : null;

            var requiredSiblingWidth = preceedingSiblings == null
                                           ? 0d
                                           : preceedingSiblings.Sum(c => GetRequiredWidth(c));

            var pointX = parentPoint.X - (GetRequiredWidth(visualNode.Parent) / 2) + requiredSiblingWidth + (requiredWidth / 2) - (ElementWidth / 2);

            var insertionPoint = new Point(pointX, parentPoint.Y + VerticalSpacing);

            visualNode.Visual = visualNode.Visual ?? CreateVisual(visualNode);
            visualNode.Visual.SetValue(Canvas.LeftProperty, insertionPoint.X);
            visualNode.Visual.SetValue(Canvas.TopProperty, insertionPoint.Y);
            visualNode.Visual.PreviewMouseLeftButtonDown += OnPreviewLeftMouseDown;

            layoutCanvas.Children.Add(visualNode.Visual);

            layoutCanvas.UpdateLayout();
        }

        /// <summary>
        /// Get the required width.
        /// </summary>
        /// <param name="visualNode">The visual node.</param>
        /// <returns>
        /// The maximum width required by the specified node and its descendents.
        /// </returns>
        private static double GetRequiredWidth(IProjectNode visualNode)
        {
            if (visualNode == null)
            {
                return 0;
            }

            var requiredWidth = visualNode.Children.Sum(child => GetRequiredWidth(child));

            if (requiredWidth == 0)
            {
                requiredWidth = ElementWidth + HorizontalSpacing;
            }

            return requiredWidth;
        }

        /// <summary>
        /// Called when [preview left mouse up].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private static void OnPreviewLeftMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (selectedVisual != null)
            {
                var canvas = selectedVisual.GetParentOfType<Canvas>();
                if (canvas != null)
                {
                    MoveElementsToTopLeft(canvas);
                    canvas.UpdateLayout();
                    ResizeCanvasToContent(canvas);
                }
            }

            selectedVisual = null;

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Called when [preview left mouse down].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private static void OnPreviewLeftMouseDown(object sender, MouseButtonEventArgs e)
        {
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

            var visual = selectedVisual.Content as ProjectNodeVisual;

            if (visual == null)
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

            MoveVisual(canvas, visual, delta);
        }

        /// <summary>
        /// Moves the visual.
        /// </summary>
        /// <param name="canvas">The canvas.</param>
        /// <param name="projectNode">The project node.</param>
        /// <param name="delta">The delta.</param>
        private static void MoveVisual(Panel canvas, ProjectNodeVisual projectNode, Point delta)
        {
            var startX = (double)projectNode.Visual.GetValue(Canvas.LeftProperty);
            var startY = (double)projectNode.Visual.GetValue(Canvas.TopProperty);

            projectNode.Visual.SetValue(Canvas.LeftProperty, startX - delta.X);
            projectNode.Visual.SetValue(Canvas.TopProperty, startY - delta.Y);

            RenderConnectionVisuals(canvas, projectNode);

            foreach (var child in projectNode.Children.OfType<ProjectNodeVisual>())
            {
                if (Keyboard.Modifiers == ModifierKeys.Control)
                {
                    MoveVisual(canvas, child, delta);
                }
                else
                {
                    RenderConnectionVisuals(canvas, child);
                }
            }
        }
    }
}