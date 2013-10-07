// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IReportController.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the IReportController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.ReportViewer
{
    using System.ComponentModel;
    using System.Windows;

    /// <summary>
    /// The report controller interface.
    /// </summary>
    public interface IReportController : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets a value indicating whether this instance has completed load.
        /// </summary>
        /// <value>
        /// <c>True</c> if this instance has completed load; otherwise, <c>false</c>.
        /// </value>
        bool HasLoadedReportList { get; }

        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <value>The status.</value>
        string Status { get; }

        /// <summary>
        /// Shows the report viewer.
        /// </summary>
        /// <param name="element">The UI element.</param>
        void ShowReportViewer(UIElement element);

        /// <summary>
        /// Shows the report.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="catalogItem">The catalog item.</param>
        void ShowReport(UIElement element, CatalogItemBase catalogItem);
    }
}