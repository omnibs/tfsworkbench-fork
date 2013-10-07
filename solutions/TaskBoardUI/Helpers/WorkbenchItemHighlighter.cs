// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkbenchItemHighlighter.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ViewControlHighlighter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.TaskBoardUI.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Threading;

    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.TaskBoardUI.DataObjects;
    using TfsWorkbench.UIElements;

    /// <summary>
    /// The view control highlighter
    /// </summary>
    internal class WorkbenchItemHighlighter
    {
        /// <summary>
        /// The tab items collection.
        /// </summary>
        private readonly TabControl tabControl;

        /// <summary>
        /// The tab items containing the specified workbench item.
        /// </summary>
        private readonly ICollection<TabItem> containingTabItems;

        /// <summary>
        /// The dispatcher queue count.
        /// </summary>
        private int queueCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkbenchItemHighlighter"/> class.
        /// </summary>
        /// <param name="tabControl">
        /// The tab items.
        /// </param>
        private WorkbenchItemHighlighter(TabControl tabControl)
        {
            this.tabControl = tabControl;
            this.containingTabItems = new Collection<TabItem>();
            this.queueCount = 0;
        }

        /// <summary>
        /// Occurs when [highlight completed].
        /// </summary>
        public event EventHandler HighlightCompleted;

        /// <summary>
        /// Gets the containing view controls.
        /// </summary>
        /// <value>The containing view controls.</value>
        public IEnumerable<TabItem> ContainingTabItemControls
        {
            get
            {
                return this.containingTabItems;
            }
        }

        /// <summary>
        /// Creates the instance.
        /// </summary>
        /// <param name="tabControl">The view controls.</param>
        /// <returns>A new instance of the highlighter</returns>
        public static WorkbenchItemHighlighter CreateInstance(TabControl tabControl)
        {
            return new WorkbenchItemHighlighter(tabControl);
        }

        /// <summary>
        /// Highlights the items in views.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        public void HighlightItemsInViews(IWorkbenchItem workbenchItem)
        {
            this.containingTabItems.Clear();

            foreach (var tabItem in this.tabControl.Items.OfType<TabItem>())
            {
                this.HighlightItem(tabItem, workbenchItem);
            }
        }

        /// <summary>
        /// Enqueues the action.
        /// </summary>
        /// <param name="priority">The priority.</param>
        /// <param name="action">The action.</param>
        private void EnqueueAction(DispatcherPriority priority, Action action)
        {
            Action actionTacker = () =>
                {
                    action();
                    this.queueCount--;

                    this.OnProcessComplete();
                };

            this.queueCount++;

            this.tabControl.Dispatcher.BeginInvoke(priority, (SendOrPostCallback)delegate { actionTacker(); }, null);
        }

        /// <summary>
        /// Highlights the item.
        /// </summary>
        /// <param name="tabItem">The tab item.</param>
        /// <param name="workbenchItem">The workbench item.</param>
        private void HighlightItem(TabItem tabItem, IWorkbenchItem workbenchItem)
        {
            var viewControl = tabItem.Content as ViewControl;

            if (viewControl == null)
            {
                return;
            }

            var isOrphan = viewControl.SwimLaneView.Orphans.Contains(workbenchItem);
            var isRowChild = viewControl.SwimLaneView.SwimLaneRows.SelectMany(slr => slr.SwimLaneColumns).Any(c => c.Contains(workbenchItem));
            var isBucketItem = viewControl.SwimLaneView.BucketStates.Any(b => b.Contains(workbenchItem));
            var isParent = viewControl.SwimLaneView.SwimLaneRows.Any(slr => Equals(slr.Parent, workbenchItem));

            // If not found in this tab, exit.
            if (!isOrphan && !isBucketItem && !isRowChild && !isParent)
            {
                return;
            }

            if (!this.containingTabItems.Contains(tabItem))
            {
                this.containingTabItems.Add(tabItem);
            }

            // Expand the other items panel if required.
            if (!viewControl.PART_OtherItemsExpander.IsExpanded && (isOrphan || isBucketItem))
            {
                viewControl.PART_OtherItemsExpander.IsExpanded = true;

                this.EnqueueAction(DispatcherPriority.Background, () => this.HighlightItem(tabItem, workbenchItem));

                return;
            }

            // If this tab does not have focus, bring to front and callback
            if (this.tabControl.SelectedItem != tabItem)
            {
                this.EnqueueAction(
                    DispatcherPriority.Background,
                    () =>
                    {
                        this.tabControl.SelectedItem = tabItem;

                        this.EnqueueAction(
                            DispatcherPriority.Input,
                            () => this.HighlightItem(tabItem, workbenchItem));
                    });

                return;
            }

            Func<FrameworkElement, bool> isMatch = e => Equals(e.DataContext, workbenchItem);

            // If the item exists in the other items section:
            if (isOrphan || isBucketItem)
            {
                this.HighlightElement(viewControl.PART_OtherItemsExpander, isMatch);
            }

            // If the item is a row child:
            if (isRowChild)
            {
                var parentRows = viewControl.SwimLaneView.SwimLaneRows.Where(r => r.SwimLaneColumns.Any(c => c.Contains(workbenchItem)));

                foreach (var row in parentRows)
                {
                    this.HighlightRowElement(viewControl, row, isMatch);
                }
            }

            // Finally, check for parent instance
            if (isParent)
            {
                isMatch = e =>
                {
                    var context = e.DataContext as SwimLaneRow;

                    return context != null && Equals(context.Parent, workbenchItem);
                };

                var parentRow = viewControl.SwimLaneView.SwimLaneRows.Where(r => Equals(r.Parent, workbenchItem)).FirstOrDefault();

                if (parentRow != null)
                {
                    this.HighlightRowElement(viewControl, parentRow, isMatch);
                }
            }

            this.OnProcessComplete();
        }

        /// <summary>
        /// Called when [process complete].
        /// </summary>
        private void OnProcessComplete()
        {
            if (this.queueCount != 0)
            {
                return;
            }

            foreach (var tabItem in this.containingTabItems)
            {
                HighlightHelper.Highlight(tabItem);
            }

            var selectedItem = this.containingTabItems.FirstOrDefault();

            if (selectedItem != null && this.tabControl.SelectedItem != selectedItem)
            {
                this.tabControl.SelectedItem = selectedItem;
            }

            if (this.HighlightCompleted != null)
            {
                this.HighlightCompleted(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Highlights the row element.
        /// </summary>
        /// <param name="viewControl">The view control.</param>
        /// <param name="row">The swim lane row row.</param>
        /// <param name="isMatch">The is match.</param>
        private void HighlightRowElement(ViewControl viewControl, SwimLaneRow row, Func<FrameworkElement, bool> isMatch)
        {
            var rowElement =
                viewControl.PART_MainListView.ItemContainerGenerator.ContainerFromItem(row) as FrameworkElement;

            if (rowElement != null)
            {
                this.HighlightElement(rowElement, isMatch);
                return;
            }

            // If the row element has not been rendered:
            Action scrollIntoView = () => viewControl.PART_MainListView.ScrollIntoView(row);

            Action highlight = () =>
            {
                rowElement =
                    viewControl.PART_MainListView.ItemContainerGenerator.ContainerFromItem(row) as FrameworkElement;

                if (rowElement != null)
                {
                    this.HighlightElement(rowElement, isMatch);
                }
            };

            this.EnqueueAction(DispatcherPriority.Input, scrollIntoView);

            this.EnqueueAction(DispatcherPriority.ApplicationIdle, highlight);
        }

        /// <summary>
        /// Highlights the element.
        /// </summary>
        /// <param name="frameworkElement">The framework element.</param>
        /// <param name="isMatch">The is match.</param>
        private void HighlightElement(FrameworkElement frameworkElement, Func<FrameworkElement, bool> isMatch)
        {
            if (isMatch(frameworkElement))
            {
                frameworkElement.BringIntoView();

                this.EnqueueAction(DispatcherPriority.ApplicationIdle, () => HighlightHelper.Highlight(frameworkElement));
            }
            else
            {
                var child = frameworkElement.GetAllChildElementsOfType<ListViewItem>().FirstOrDefault(isMatch);
                if (child != null)
                {
                    this.HighlightElement(child, isMatch);
                }
            }
        }
    }
}
