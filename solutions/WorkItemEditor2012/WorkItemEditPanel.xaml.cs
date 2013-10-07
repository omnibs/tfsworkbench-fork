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
using Microsoft.TeamFoundation.WorkItemTracking.WpfControls;

namespace TfsWorkbench.WorkItemEditor2012
{
    /// <summary>
    /// Interaction logic for WorkItemEditPanel.xaml
    /// </summary>
    public partial class WorkItemEditPanel : IDisposable
    {
        private readonly WorkItemControl _workItemControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkItemEditPanel"/> class.
        /// </summary>
        public WorkItemEditPanel()
        {
            InitializeComponent();

            _workItemControl = new WorkItemControl();
            
            PART_Grid.Children.Add(_workItemControl);
        }

        /// <summary>
        /// Sets the wowk item.
        /// </summary>
        /// <param name="item">The work item.</param>
        public void SetWowkItem(WorkItem item)
        {
            _workItemControl.Item = item;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            Dispose(true);
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

            PART_Grid.Children.Clear();

            if (_workItemControl != null)
            {
                try
                {
                    _workItemControl.Dispose();
                }
                catch (Exception)
                {
                    // Disposal can throw errors if work item contains test steps, so ignore them.
                }
            }
        }
    }
}
