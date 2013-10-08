// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DragListView.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the DragListView type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.UIElements.DragHelpers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Shapes;

    using Core.Interfaces;

    using TfsWorkbench.Core.Helpers;

    /// <summary>
    /// Initialises and instance of TfsWorkbench.WpfUI.DragListView
    /// </summary>
    public class DragListView : ListView, IDragTarget<IWorkbenchItem>
    {
        /// <summary>
        /// The state property.
        /// </summary>
        private static readonly DependencyProperty stateProperty = DependencyProperty.Register(
            "State", 
            typeof(string), 
            typeof(DragListView));

        /// <summary>
        /// The is drop valid property.
        /// </summary>
        private static readonly DependencyProperty isDropValidProperty = DependencyProperty.Register(
            "IsDropValid",
            typeof(bool),
            typeof(DragListView));

        /// <summary>
        /// The is drop valid property.
        /// </summary>
        private static readonly DependencyProperty isDragOverProperty = DependencyProperty.Register(
            "IsDragOver",
            typeof(bool),
            typeof(DragListView));

        /// <summary>
        /// Gets the is drop valid property.
        /// </summary>
        /// <value>The is drop valid property.</value>
        public static DependencyProperty IsDropValidProperty
        {
            get { return isDropValidProperty; }
        }

        /// <summary>
        /// Gets the state property.
        /// </summary>
        /// <value>The state property.</value>
        public static DependencyProperty StateProperty
        {
            get { return stateProperty; }
        }

        /// <summary>
        /// Gets the is drag over property.
        /// </summary>
        /// <value>The is drag over property.</value>
        public static DependencyProperty IsDragOverProperty
        {
            get { return isDragOverProperty; }
        }

        /// <summary>
        /// Gets or sets the state.
        /// </summary>
        /// <value>The state.</value>
        public string State
        {
            get { return (string)this.GetValue(StateProperty); }
            set { this.SetValue(StateProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is drop valid.
        /// </summary>
        /// <value>
        /// <c>true</c> if this is a drop valid instance; otherwise, <c>false</c>.
        /// </value>
        public bool IsDropValid
        {
            get { return (bool)this.GetValue(IsDropValidProperty); }
            set { this.SetValue(IsDropValidProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is drag over.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is drag over; otherwise, <c>false</c>.
        /// </value>
        public bool IsDragOver
        {
            get { return (bool)this.GetValue(IsDragOverProperty); }
            set { this.SetValue(IsDragOverProperty, value); }
        }

        /// <summary>
        /// Gets the elements to drag.
        /// </summary>
        /// <value>The elements to drag.</value>
        public IEnumerable<IWorkbenchItem> ElementsToDrag
        {
            get
            {
                return this.SelectedItems.OfType<IWorkbenchItem>();
            }
        }

        /// <summary>
        /// Determines whether this instance [is valid drop location] for [the specified drop items].
        /// </summary>
        /// <param name="dropItems">The drop items.</param>
        /// <returns>
        /// <c>True</c> if this instance [is valid drop location] for [the specified drop items]; otherwise, <c>false</c>.
        /// </returns>
        public bool TestDropLocation(IEnumerable<IWorkbenchItem> dropItems)
        {
            var isValidDropLocation = true;

            if (!string.IsNullOrEmpty(this.State))
            {
                foreach (var item in dropItems)
                {
                    var allowedValues = item.AllowedValues[Core.Properties.Settings.Default.StateFieldName];

                    var allowedValuesArray = allowedValues as IEnumerable<object>;

                    if (allowedValuesArray == null || allowedValuesArray.Contains(this.State) ||  WorkbenchItemHelper.CustomStates.Any(c => c.Name == this.State))
                    {
                        continue;
                    }

                    isValidDropLocation = false;
                    break;
                }
            }

            this.IsDropValid = isValidDropLocation;

            return isValidDropLocation;
        }

        /// <summary>
        /// Creates the adorner element.
        /// </summary>
        /// <param name="elements">The dragged elements.</param>
        /// <returns>A uielement based on the dragged items.</returns>
        public UIElement CreateAdornerElement(IEnumerable<IWorkbenchItem> elements)
        {
            var canvas = new Canvas { Opacity = 0.75 };
            var offset = 0d;
            double? width = null, height = null;
            var isFirstLoop = true;

            foreach (var child in elements)
            {
                var container = this.ItemContainerGenerator.ContainerFromItem(child) as FrameworkElement;

                if (container == null)
                {
                    continue;
                }

                var content = container.GetFirstChildElementOfType<ContentPresenter>();

                if (content == null)
                {
                    continue;
                }

                if (isFirstLoop)
                {
                    isFirstLoop = false;
                    width = container.ActualWidth;
                    height = container.ActualHeight;
                }

                var rectangle = new Rectangle
                    {
                        Width = container.ActualWidth,
                        Height = container.ActualHeight,
                        Fill = new VisualBrush(content)
                    };

                rectangle.SetValue(Canvas.LeftProperty, offset);
                rectangle.SetValue(Canvas.TopProperty, offset);

                canvas.Children.Add(rectangle);

                offset += 8;
            }

            if (width.HasValue)
            {
                var parentGrid = this.GetParentOfType<Grid>();

                var colCount = parentGrid.ColumnDefinitions.Count();
                var rowCount = parentGrid.RowDefinitions.Count();

                if (colCount != 0)
                {
                    canvas.SetValue(Grid.ColumnSpanProperty, colCount);
                }

                if (rowCount != 0)
                {
                    canvas.SetValue(Grid.RowSpanProperty, rowCount);
                }

                canvas.Width = width.Value + offset;
                canvas.Height = height.Value + offset;
                parentGrid.Children.Add(canvas);
                canvas.UpdateLayout();
                parentGrid.Children.Remove(canvas);
            }

            return canvas;
        }

        /// <summary>
        /// Called on drag enter event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.DragEventArgs"/> that contains the event data.</param>
        protected override void OnDragEnter(DragEventArgs e)
        {
            this.IsDragOver = true;
            base.OnDragEnter(e);
        }

        /// <summary>
        /// Called on drag leave event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.DragEventArgs"/> that contains the event data.</param>
        protected override void OnDragLeave(DragEventArgs e)
        {
            this.IsDragOver = false;
            base.OnDragLeave(e);
        }

        /// <summary>
        /// Called on drop event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.DragEventArgs"/> that contains the event data.</param>
        protected override void OnDrop(DragEventArgs e)
        {
            this.IsDragOver = false;
            base.OnDrop(e);
        }
    }
}