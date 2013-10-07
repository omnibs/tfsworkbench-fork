// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CustomTabControl.cs" company="None">
//   None
// </copyright>
// <summary>
//   The Custom Tab Control class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.ItemListUI
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading;
    using System.Windows.Controls;
    using System.Windows.Threading;

    using TfsWorkbench.UIElements;

    /// <summary>
    /// The Custom Tab Control class.
    /// </summary>
    public class CustomTabControl : TabControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomTabControl"/> class.
        /// </summary>
        public CustomTabControl()
        {
            this.RenderedTabs = new List<TabItem>();

            var isVisibleChanged = DependencyPropertyDescriptor.FromProperty(IsVisibleProperty, typeof(CustomTabControl));

            isVisibleChanged.AddValueChanged(this, this.OnIsVisibleChanged);
        }

        /// <summary>
        /// Gets the rendered tabs.
        /// </summary>
        /// <value>The rendered tabs.</value>
        public IList<TabItem> RenderedTabs { get; private set; }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Controls.Primitives.Selector.SelectionChanged"/> routed event.
        /// </summary>
        /// <param name="e">Provides data for <see cref="T:System.Windows.Controls.SelectionChangedEventArgs"/>.</param>
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            this.BeginActionCallback(e, base.OnSelectionChanged);
        }

        /// <summary>
        /// Called when [is visible changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnIsVisibleChanged(object sender, EventArgs e)
        {
            Action<EventArgs> action = args => { };

            this.BeginActionCallback(e, action);
        }

        /// <summary>
        /// Begins the action callback.
        /// </summary>
        /// <typeparam name="T">The event args type.</typeparam>
        /// <param name="e">The event args instance.</param>
        /// <param name="method">The method.</param>
        private void BeginActionCallback<T>(T e, Action<T> method) where T : EventArgs
        {
            while (true)
            {
                if (!this.IsVisible)
                {
                    break;
                }

                var tabItem = this.SelectedItem as TabItem;

                if (tabItem == null || this.RenderedTabs.Contains(tabItem))
                {
                    break;
                }

                this.RenderedTabs.Add(tabItem);

                var content = tabItem.Content as ItemList;

                if (content == null || content.ControlItemGroups == null)
                {
                    break;
                }

                var itemCount = content.ControlItemGroups.Count();

                if (itemCount == 0)
                {
                    break;
                }

                CommandLibrary.ApplicationMessageCommand.Execute(string.Concat("Loading ", itemCount, " items into the list..."), this);
                CommandLibrary.DisableUserInputCommand.Execute(true, this);

                this.Dispatcher.BeginInvoke(DispatcherPriority.Background, (SendOrPostCallback)delegate { this.Callback(e, method, itemCount); }, null);

                return;
            }

            // If all conditions are not met; execute method.
            method(e);
        }

        /// <summary>
        /// Callback to the base selection changed method.
        /// </summary>
        /// <typeparam name="T">The event args type.</typeparam>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        /// <param name="method">The method to invoke.</param>
        /// <param name="itemCount">The item count.</param>
        private void Callback<T>(T e, Action<T> method, int itemCount) where T : EventArgs
        {
            method(e);

            CommandLibrary.ApplicationMessageCommand.Execute(string.Concat(itemCount, " items loaded into list."), this);
            CommandLibrary.DisableUserInputCommand.Execute(false, this);
        }
    }
}
