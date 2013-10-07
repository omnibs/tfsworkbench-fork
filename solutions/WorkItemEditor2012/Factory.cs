// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Factory.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the Factory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Windows.Controls;
using Microsoft.TeamFoundation.WorkItemTracking.Client;

namespace TfsWorkbench.WorkItemEditor2012
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
            try
            {
                var panel = new WorkItemEditPanel();

                panel.SetWowkItem(item);

                return panel;
            }
            catch (Exception ex)
            {
                return new TextBlock
                           {
                               Text = string.Format("An error occured while generating the TFS Work Item edit panel. {0} - {1}", ex.GetType().Name, ex.Message)
                           };
            }
        }
    }
}
