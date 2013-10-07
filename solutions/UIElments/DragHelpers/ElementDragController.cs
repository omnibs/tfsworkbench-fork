// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ElementDragController.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the DragController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.UIElements.DragHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;

    /// <summary>
    /// Defines the DragController type.
    /// </summary>
    /// <typeparam name="TElement">The type of the element.</typeparam>
    public class ElementDragController<TElement> : IElementDragController<TElement>
    {
        /// <summary>
        /// The drag targets.
        /// </summary>
        private readonly ICollection<IDragTarget<TElement>> dragTargets = new Collection<IDragTarget<TElement>>();

        /// <summary>
        /// The parent UI element.
        /// </summary>
        private UIElement parentElement;

        /// <summary>
        /// The adonder.
        /// </summary>
        private ElementDragAdorner adorner;

        /// <summary>
        /// The mouse is down flag.
        /// </summary>
        private bool isMouseDown;

        /// <summary>
        /// The is successful drop flag.
        /// </summary>
        private bool isSuccess;

        /// <summary>
        /// The source of the dragged elements.
        /// </summary>
        private IDragTarget<TElement> dragSource;

        /// <summary>
        /// The initial mouse down position.
        /// </summary>
        private Point mouseDownPosition;

        /// <summary>
        /// The is dragging flag.
        /// </summary>
        private bool isDragging;

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementDragController&lt;TElement&gt;"/> class.
        /// </summary>
        /// <param name="dragParent">The drag parent.</param>
        public ElementDragController(UIElement dragParent)
        {
            this.parentElement = dragParent;

            this.parentElement.AllowDrop = true;
            this.parentElement.DragOver += this.OnPanelDragOver;
            this.parentElement.PreviewGiveFeedback += OnPanelGiveFeedback;
        }

        /// <summary>
        /// Gets or sets the item drag failed.
        /// </summary>
        /// <value>The item drag failed.</value>
        public event EventHandler<DragDataEventArgs<TElement>> ItemDragFailed;

        /// <summary>
        /// Gets or sets the items dragged.
        /// </summary>
        /// <value>The items dragged.</value>
        public event EventHandler<DragDataEventArgs<TElement>> ItemsDragged;

        /// <summary>
        /// Gets or sets the items dropped.
        /// </summary>
        /// <value>The items dropped.</value>
        public event EventHandler<DragDataEventArgs<TElement>> ItemsDropped;

        /// <summary>
        /// Gets the drag targets.
        /// </summary>
        /// <value>The drag targets.</value>
        public IEnumerable<IDragTarget<TElement>> DragTargets
        {
            get { return this.dragTargets; }
        }

        /// <summary>
        /// Registers the drag target.
        /// </summary>
        /// <param name="dragTarget">The drag target.</param>
        public void RegisterDragTarget(IDragTarget<TElement> dragTarget)
        {
            lock (this.dragTargets)
            {
                if (this.dragTargets.Contains(dragTarget))
                {
                    return;
                }

                this.dragTargets.Add(dragTarget);
            }

            dragTarget.AllowDrop = true;
            dragTarget.PreviewMouseLeftButtonDown += this.OnTargetLeftButtonDown;
            dragTarget.PreviewMouseLeftButtonUp += this.OnTargetLeftButtonUp;
            dragTarget.PreviewMouseMove += this.OnTargetMove;
            dragTarget.Drop += this.OnTargetDrop;
            dragTarget.DragOver += OnDragOver;
        }

        /// <summary>
        /// Releases the drag target.
        /// </summary>
        /// <param name="dragTarget">The drag target.</param>
        public void ReleaseDragTarget(IDragTarget<TElement> dragTarget)
        {
            lock (this.dragTargets)
            {
                if (this.dragTargets.Contains(dragTarget))
                {
                    this.dragTargets.Remove(dragTarget);
                }
            }

            dragTarget.AllowDrop = false;
            dragTarget.PreviewMouseLeftButtonDown -= this.OnTargetLeftButtonDown;
            dragTarget.PreviewMouseLeftButtonUp -= this.OnTargetLeftButtonUp;
            dragTarget.PreviewMouseMove -= this.OnTargetMove;
            dragTarget.Drop -= this.OnTargetDrop;
            dragTarget.DragOver -= OnDragOver;
        }

        /// <summary>
        /// Unhooks the drag events.
        /// </summary>
        public void ReleaseResources()
        {
            this.parentElement.AllowDrop = false;
            this.parentElement.DragOver -= this.OnPanelDragOver;
            this.parentElement.PreviewGiveFeedback -= OnPanelGiveFeedback;

            foreach (var dragTarget in this.dragTargets.ToArray())
            {
                this.ReleaseDragTarget(dragTarget);
            }

            this.parentElement = null;
            this.adorner = null;
        }

        /// <summary>
        /// Gets the bound item from point.
        /// </summary>
        /// <param name="dragTarget">The drag target.</param>
        /// <param name="point">The point.</param>
        /// <returns>Bound item from point.</returns>
        private static object GetBoundItemFromPoint(IDragTarget<TElement> dragTarget, Point point)
        {
            if (dragTarget.ItemContainerGenerator == null)
            {
                return null;
            }

            var element = dragTarget.InputHitTest(point) as UIElement;
            while (element != null)
            {
                if (element == dragTarget)
                {
                    return null;
                }

                var item = dragTarget.ItemContainerGenerator.ItemFromContainer(element);
                var hasItem = !ReferenceEquals(item, DependencyProperty.UnsetValue);

                if (hasItem)
                {
                    return item;
                }

                element = VisualTreeHelper.GetParent(element) as UIElement;
            }

            return null;
        }

        /// <summary>
        /// Determines whether [the specified target] [has dragged elements].
        /// </summary>
        /// <param name="target">The target.</param>
        /// <returns>
        /// <c>true</c> if [the specified target] [has dragged elements]; otherwise, <c>false</c>.
        /// </returns>
        private static bool HasElementsToDrag(IDragTarget<TElement> target)
        {
            return target.ElementsToDrag != null && target.ElementsToDrag.Count() != 0;
        }

        /// <summary>
        /// Called when [drag over].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private static void OnDragOver(object sender, DragEventArgs e)
        {
            var dragTarget = sender as IDragTarget<TElement>;

            if (dragTarget == null)
            {
                return;
            }

            Mouse.OverrideCursor =
                dragTarget.TestDropLocation(e.Data.GetData(DragDataNames.DataFormat) as IEnumerable<TElement>)
                    ? CustomCursors.MoveHand
                    : CustomCursors.HandNo;
        }

        /// <summary>
        /// Called when [panel give feedback].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private static void OnPanelGiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            e.UseDefaultCursors = false;
            e.Handled = true;
        }

        /// <summary>
        /// Does the drag drop.
        /// </summary>
        /// <param name="source">The source.</param>
        private void DoDragDrop(IDragTarget<TElement> source)
        {
            if (source == null || !HasElementsToDrag(source))
            {
                this.isMouseDown = false;
                return;
            }

            this.dragSource = source;

            var dragElementsMap = source.ElementsToDrag.ToDictionary(e => source.Items.IndexOf(e), e => e);

            this.isSuccess = false;

            this.adorner = new ElementDragAdorner(
                this.parentElement, source.CreateAdornerElement(dragElementsMap.Values), true, 1);

            var layer = AdornerLayer.GetAdornerLayer(this.parentElement);

            layer.Add(this.adorner);

            if (this.ItemsDragged != null)
            {
                this.ItemsDragged(this, new DragDataEventArgs<TElement>(source, null, dragElementsMap.Values));
            }

            try
            {
                DragDrop.DoDragDrop(
                    (DependencyObject)source,
                    new DataObject(DragDataNames.DataFormat, dragElementsMap.Values),
                    DragDropEffects.Move);
            }
            catch (Exception ex)
            {
                ex = new Exception("An error occured during the drag drop procedure.", ex);

                if (!CommandLibrary.ApplicationExceptionCommand.CanExecute(ex, Application.Current.MainWindow))
                {
                    throw;
                }

                CommandLibrary.ApplicationExceptionCommand.Execute(ex, Application.Current.MainWindow);
            }

            if (!this.isSuccess && this.ItemDragFailed != null)
            {
                this.ItemDragFailed(this, new DragDataEventArgs<TElement>(source, null, dragElementsMap.Values));
            }

            this.isDragging = false;
            this.isMouseDown = false;
            this.dragSource = null;

            // The adorner can be null if the DoDragDrop call fails.
            if (this.adorner != null)
            {
                layer.Remove(this.adorner);
            }

            this.adorner = null;

            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Called when [panel drag over].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnPanelDragOver(object sender, DragEventArgs e)
        {
            if (this.adorner == null)
            {
                return;
            }

            var position = e.GetPosition(this.parentElement);

            this.adorner.LeftOffset = position.X;
            this.adorner.TopOffset = position.Y;
        }

        /// <summary>
        /// Called when [target drop].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnTargetDrop(object sender, DragEventArgs e)
        {
            var target = sender as IDragTarget<TElement>;

            if (target == null || this.ItemsDropped == null)
            {
                return;
            }

            var payload = e.Data.GetData(DragDataNames.DataFormat) as IEnumerable<TElement>;

            this.isSuccess = target.TestDropLocation(payload);

            if (this.isSuccess)
            {
                this.ItemsDropped(this, new DragDataEventArgs<TElement>(this.dragSource, target, payload));
            }
        }

        /// <summary>
        /// Called when [target left button down].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnTargetLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var source = sender as IDragTarget<TElement>;

            if (source == null)
            {
                return;
            }

            this.mouseDownPosition = e.GetPosition((IInputElement)source);

            var clickedObject = GetBoundItemFromPoint(source, this.mouseDownPosition);

            if (clickedObject == null)
            {
                return;
            }

            this.isMouseDown = true;
            Mouse.OverrideCursor = CustomCursors.MoveHand;
        }

        /// <summary>
        /// Called when [target left button up].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnTargetLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.isMouseDown = false;
            this.isDragging = false;
            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Called when [target move].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnTargetMove(object sender, MouseEventArgs e)
        {
            const double Tolerance = 2;

            if (!this.isMouseDown || this.isDragging)
            {
                return;
            }

            var source = sender as IDragTarget<TElement>;

            if (source == null)
            {
                return;
            }

            var newPosition = e.GetPosition((IInputElement)source);

            var movement = new Point((newPosition.X - this.mouseDownPosition.X), (newPosition.Y - this.mouseDownPosition.Y));

            if ((movement.X > (Tolerance * -1) && movement.X < Tolerance) && (movement.Y > (Tolerance * -1) && movement.Y < Tolerance))
            {
                return;
            }

            this.isDragging = true;

            this.DoDragDrop(sender as IDragTarget<TElement>);
        }
    }
}