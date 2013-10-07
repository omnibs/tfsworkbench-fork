// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Factory.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the Factory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace TfsWorkbench.WorkItemEditor2010
{
    /// <summary>
    /// The factory class.
    /// </summary>
    public static class Factory
    {
        /// <summary>
        /// Builds the work item edit panel.
        /// </summary>
        /// <param name="item">The work item.</param>
        /// <returns>An instance of the work item panel control.</returns>
        public static object BuildWorkItemEditPanel(WorkItem item)
        {
            var panel = new WorkItemEditPanel();
            panel.SetWowkItem(item);

            return panel;
        }
    }
}
