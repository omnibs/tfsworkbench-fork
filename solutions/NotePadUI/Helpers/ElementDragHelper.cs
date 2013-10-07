// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ElementDragHelper.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ElementDragHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TfsWorkbench.NotePadUI.Models;
using TfsWorkbench.NotePadUI.UIElements;
using TfsWorkbench.UIElements;

namespace TfsWorkbench.NotePadUI.Helpers
{
    /// <summary>
    /// The element drag helper class.
    /// </summary>
    internal static class ElementDragHelper
    {
        /// <summary>
        /// The moveOffset point.
        /// </summary>
        private static Point moveOffset;

        /// <summary>
        /// The selected visual.
        /// </summary>
        private static UIPadItem selectedPadItem;

        private static DragBehaviour.DragOption dragAction;
        private static Canvas canvas;
        private static double initialAngle;
        private static DragBehaviour.AspectOption resizeAspect;
        private static readonly Collection<FrameworkElement> pinnedElements = new Collection<FrameworkElement>();

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

        public static void BringToTop(DependencyObject visual, Panel panel)
        {
            var itemToRaise = visual as UIPadItem;

            if (itemToRaise == null)
            {
                return;
            }

            var itemPairs = panel
                .Children
                .OfType<UIPadItem>()
                .Where(item => !Equals(item, visual))
                .Select(item => Tuple.Create(item, (PadItemBase)item.DataContext))
                .OrderBy(pair => pair.Item2.ZIndex)
                .Union(new[] { Tuple.Create(itemToRaise, (PadItemBase)itemToRaise.DataContext) })
                .ToArray();

            var index = 1;

            foreach (var pair in itemPairs)
            {
                var isPinned = pair.Item2.PinnedToId.HasValue;

                if (isPinned && !Equals(itemToRaise, pair.Item1))
                {
                    continue;
                }

                pair.Item2.ZIndex = index;

                var id = pair.Item2.Id;

                var pinnedItems = itemPairs.Where(ip => ip.Item2.PinnedToId == id);

                foreach (var pinnedItem in pinnedItems)
                {
                    pinnedItem.Item2.ZIndex = ++index;
                }

                index++;
            }
        }

        public static void ResizePanelToContent(this Panel panel)
        {
            panel.UpdateLayout();

            var frameworkElements = panel.Children.OfType<FrameworkElement>().ToArray();

            var hasChildren = frameworkElements.Any();

            var maxWidth = hasChildren ? frameworkElements.Max(c => VisualTreeHelper.GetOffset(c).X + c.ActualWidth) : 0;
            var maxHeight = hasChildren ? frameworkElements.Max(c => VisualTreeHelper.GetOffset(c).Y + c.ActualHeight) : 0;

            var dimensions = new Size(Math.Max(maxWidth, 0), Math.Max(maxHeight, 0));

            var scrollViewer = panel.GetParentOfType<ScrollViewer>();

            if (scrollViewer == null)
            {
                return;
            }

            const int scrollBarWidth = 24;

            var parentSize = new Size(scrollViewer.ActualWidth - scrollBarWidth, scrollViewer.ActualHeight - scrollBarWidth);

            panel.Width = Math.Max(parentSize.Width, dimensions.Width);
            panel.Height = Math.Max(parentSize.Height, dimensions.Height);
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
                selectedPadItem = null;
                return;
            }

            if (!TryGetDragElements(e.OriginalSource))
            {
                return;
            }

            var padItemPosition = VisualTreeHelper.GetOffset(selectedPadItem);

            var mouseDownPosition = GetCurrentMousePosition();

            moveOffset = new Point(mouseDownPosition.X - padItemPosition.X, mouseDownPosition.Y - padItemPosition.Y);

            initialAngle = GetAngle() - ((PadItemBase)selectedPadItem.DataContext).Angle;

            BringToTop(selectedPadItem, canvas);

            CapturePinnedElements();

            Mouse.OverrideCursor = CustomCursors.MoveHand;
        }

        private static void CapturePinnedElements()
        {
            pinnedElements.Clear();

            var selectedItemContext = selectedPadItem.DataContext as PadItemBase;

            if (selectedItemContext == null || selectedItemContext.IsPinable)
            {
                return;
            }

            var uiAndDataPairs = canvas.Children.OfType<UIPadItem>().Select(ui => Tuple.Create(ui, (PadItemBase) ui.DataContext));

            foreach (var uiAndDataPair in uiAndDataPairs)
            {
                if (uiAndDataPair.Item2.PinnedToId == selectedItemContext.Id)
                {
                    pinnedElements.Add(uiAndDataPair.Item1);
                }
            }
        }

        private static bool TryGetDragElements(object originalSource)
        {
            var originalVisual = originalSource as Visual;
            if (originalVisual == null)
            {
                return false;
            }

            dragAction = (DragBehaviour.DragOption)originalVisual.GetValue(DragBehaviour.DragActionProperty);

            if (dragAction == DragBehaviour.DragOption.None)
            {
                return false;
            }

            resizeAspect = (DragBehaviour.AspectOption)originalVisual.GetValue(DragBehaviour.ResizeAspectProperty);

            selectedPadItem = originalVisual.GetParentOfType<UIPadItem>();

            if (selectedPadItem == null)
            {
                return false;
            }

            canvas = selectedPadItem.GetParentOfType<Canvas>();
            return canvas != null;
        }

        /// <summary>
        /// Called when [preview mouse up].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private static void OnPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (selectedPadItem != null)
            {
                canvas.ResizePanelToContent();

                var selectedItemContext = (PadItemBase)selectedPadItem.DataContext;

                if (selectedItemContext.IsPinable)
                {
                    var bounds = new Rect(selectedItemContext.LeftOffset, selectedItemContext.TopOffset, selectedItemContext.Width, selectedItemContext.Height);

                    var hitElements = new Collection<PadItemBase>();

                    foreach (var element in canvas.Children.OfType<FrameworkElement>().Where(element => !Equals(element, selectedPadItem)))
                    {
                        var elementContext = element.DataContext as PadItemBase;

                        if (elementContext == null || elementContext.IsPinable)
                        {
                            continue;
                        }

                        var elementBounds = new Rect(elementContext.LeftOffset, elementContext.TopOffset, elementContext.Width, elementContext.Height);

                        var isHit = bounds.IntersectsWith(elementBounds);

                        if (isHit)
                        {
                            hitElements.Add(elementContext);
                        }
                    }

                    var topMostItem = hitElements.OrderByDescending(pi => pi.ZIndex).FirstOrDefault();

                    selectedItemContext.PinnedToId = topMostItem == null ? null : (int?)topMostItem.Id;
                }

                LocalCommandLibrary.SaveLayoutCommand.Execute(canvas, canvas);

                selectedPadItem = null;
            }

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Called when [preview mouse move].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        private static void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (selectedPadItem == null)
            {
                return;
            }                        

            switch (dragAction)
            {
                case DragBehaviour.DragOption.None:
                    break;
                case DragBehaviour.DragOption.Rotate:
                    RotateVisual();
                    break;
                case DragBehaviour.DragOption.Resize:
                    ResizeVisual();
                    break;
                case DragBehaviour.DragOption.Move:
                    MoveVisual();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Moves the visual elments.
        /// </summary>
        private static void MoveVisual()
        {
            var position = GetCurrentMousePosition();

            var offsetPosition = new Point(position.X - moveOffset.X, position.Y - moveOffset.Y);
            var selectedItemOffset = VisualTreeHelper.GetOffset(selectedPadItem);

            var delta = new Point(selectedItemOffset.X - offsetPosition.X, selectedItemOffset.Y - offsetPosition.Y);

            MoveVisual(selectedPadItem, delta);

            foreach (var pinnedElement in pinnedElements)
            {
                MoveVisual(pinnedElement, delta);
            }
        }

        private static void MoveVisual(FrameworkElement elementToMove, Point delta)
        {
            var startX = (double)elementToMove.GetValue(Canvas.LeftProperty);
            var startY = (double)elementToMove.GetValue(Canvas.TopProperty);

            elementToMove.SetValue(Canvas.LeftProperty, startX - delta.X);
            elementToMove.SetValue(Canvas.TopProperty, startY - delta.Y);
        }

        private static void ResizeVisual()
        {
            var padItem = selectedPadItem.DataContext as PadItemBase;

            if (padItem == null)
            {
                return;
            }

            var mousePosition = GetCurrentMousePosition();

            var uiPadItemPosition = VisualTreeHelper.GetOffset(selectedPadItem);

            var size = new Size(
                Math.Max(mousePosition.X - uiPadItemPosition.X, 0), 
                Math.Max(mousePosition.Y - uiPadItemPosition.Y, 0));

            if (resizeAspect == DragBehaviour.AspectOption.Both || resizeAspect == DragBehaviour.AspectOption.Horizontal)
            {
                padItem.Width = size.Width;
                padItem.OnPropertyChanged("Width");                
            }

            if (resizeAspect == DragBehaviour.AspectOption.Both || resizeAspect == DragBehaviour.AspectOption.Vertical)
            {
                padItem.Height = size.Height;
                padItem.OnPropertyChanged("Height");
            }
        }

        private static void RotateVisual()
        {
            var padItem = selectedPadItem.DataContext as PadItemBase;

            if (padItem == null)
            {
                return;
            }

            var angle = GetAngle();

            padItem.Angle = angle - initialAngle;

            padItem.OnPropertyChanged("Angle");
        }

        private static double GetAngle()
        {
            var delta = GetRotationDelta();

            var radians = Math.Atan(delta.Y / delta.X);

            var angle = radians*180/Math.PI;

            if (delta.X < 0)
            {
                angle += 180;
            }

            return angle;
        }

        private static Point GetRotationDelta()
        {
            // where is the mouse now
            var position = GetCurrentMousePosition();

            // where is the centre of my control
            var parentOffset = VisualTreeHelper.GetOffset(selectedPadItem);

            var parentCentre = new Point(selectedPadItem.ActualWidth/2 + parentOffset.X,
                                         selectedPadItem.ActualHeight/2 + parentOffset.Y);

            var delta = new Point(position.X - parentCentre.X, position.Y - parentCentre.Y);

            return delta;
        }

        private static Point GetCurrentMousePosition()
        {
            return Mouse.GetPosition(canvas);
        }
    }
}
