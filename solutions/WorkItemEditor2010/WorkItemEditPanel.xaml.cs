// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkItemEditPanel.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   Interaction logic for WorkItemEditPanel.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Controls;

namespace TfsWorkbench.WorkItemEditor2010
{
    /// <summary>
    /// Interaction logic for WorkItemEditPanel.xaml
    /// </summary>
    public partial class WorkItemEditPanel : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WorkItemEditPanel"/> class.
        /// </summary>
        public WorkItemEditPanel()
        {
            InitializeComponent();

            var workItemFormControl = new WorkItemFormControl();

            this.PART_Host.Child = workItemFormControl;
        }

        /// <summary>
        /// Sets the wowk item.
        /// </summary>
        /// <param name="item">The work item.</param>
        public void SetWowkItem(WorkItem item)
        {
            ((WorkItemFormControl)this.PART_Host.Child).Item = item;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            var winFormHost = this.PART_Host;

            this.PART_Grid.Children.Clear();

            var workItemformcontrol = winFormHost.Child as WorkItemFormControl;
            if (workItemformcontrol != null)
            {
                workItemformcontrol.Dispose();
                winFormHost.Child = null;
            }

            winFormHost.Dispose();
        }
    }
}
