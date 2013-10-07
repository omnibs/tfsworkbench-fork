// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReportController.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ReportController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.ReportViewer
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Text;
    using System.Threading;
    using System.Web.Services.Protocols;
    using System.Windows;

    using TfsWorkbench.Core.DataObjects;
    using TfsWorkbench.Core.EventArgObjects;
    using TfsWorkbench.Core.Helpers;
    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.ReportViewer.Properties;
    using TfsWorkbench.UIElements;

    /// <summary>
    /// The report controller class.
    /// </summary>
    internal class ReportController : ProjectDataServiceConsumer, IReportController
    {
        /// <summary>
        /// The report root node.
        /// </summary>
        private IReportNode reportRootNode;

        /// <summary>
        /// The status text.
        /// </summary>
        private string status;

        /// <summary>
        /// The has loaded report list flag.
        /// </summary>
        private bool hasLoadedReportList;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportController"/> class.
        /// </summary>
        public ReportController()
        {
            this.ProjectDataService.ProjectDataChanged += this.OnProjectDataChanged;
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets a value indicating whether this instance has completed load.
        /// </summary>
        /// <value>
        /// <c>True</c> if this instance has completed load; otherwise, <c>false</c>.
        /// </value>
        public bool HasLoadedReportList
        {
            get
            {
                return this.hasLoadedReportList;
            }

            private set
            {
                if (this.hasLoadedReportList == value)
                {
                    return;
                }

                this.hasLoadedReportList = value;

                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("HasLoadedReportList"));
                }
            }
        }

        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <value>The status.</value>
        public string Status
        {
            get
            {
                return this.status;
            }

            private set
            {
                if (this.status == value)
                {
                    return;
                }

                this.status = value;

                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("Status"));
                }
            }
        }

        /// <summary>
        /// Shows the report viewer.
        /// </summary>
        /// <param name="element">The UI element.</param>
        public void ShowReportViewer(UIElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            if (!this.HasLoadedReportList || this.reportRootNode == null)
            {
                return;
            }

            var reportViewer = new ReportViewerControl(this) { ReportRoot = this.reportRootNode };
            CommandLibrary.ShowDialogCommand.Execute(reportViewer, element);
        }

        /// <summary>
        /// Shows the report.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="catalogItem">The catalog item.</param>
        public void ShowReport(UIElement element, CatalogItemBase catalogItem)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            if (catalogItem == null)
            {
                throw new ArgumentNullException("catalogItem");
            }

            try
            {
                var serviceEndPoint = this.GetReportServiceEndPoint();
                if (!catalogItem.IsReport || serviceEndPoint == null)
                {
                    return;
                }

                var reportDisplayPath =
                    catalogItem.Path.StartsWith(ReportProxyWrapperHelper.Get2005ProjectReportFolder(this.GetCurrentProjectData()))
                        ? ReportProxyWrapperHelper.ReportService2005.ReportDisplayPath
                        : ReportProxyWrapperHelper.ReportService2008.ReportDisplayPath;

                var serviceRoot = serviceEndPoint.AbsoluteUri.Substring(0, serviceEndPoint.AbsoluteUri.LastIndexOf("/"));

                CommandLibrary.SystemShellCommand.Execute(string.Concat(serviceRoot, reportDisplayPath, catalogItem.Path), element);
            }
            catch (ArgumentException aex)
            {
                CommandLibrary.ApplicationExceptionCommand.Execute(aex, element);
            }
        }

        /// <summary>
        /// Appends the inner exception.
        /// </summary>
        /// <param name="stringBuilder">The string builder.</param>
        /// <param name="exception">The exception.</param>
        private static void AppendInnerExceptions(StringBuilder stringBuilder, Exception exception)
        {
            var innerException = exception.InnerException;
            while (innerException != null)
            {
                stringBuilder.AppendLine(innerException.Message);
                innerException = innerException.InnerException;
            }
        }

        /// <summary>
        /// Tries to get then current project data.
        /// </summary>
        /// <returns>The current project data.</returns>
        private IProjectData GetCurrentProjectData()
        {
            var projectData = this.ProjectDataService.CurrentProjectData;

            if (projectData == null)
            {
                throw new ArgumentException(Resources.String01);
            }

            return projectData;
        }

        /// <summary>
        /// Tries to get the current data provider.
        /// </summary>
        /// <returns>The current data provider.</returns>
        private IDataProvider GetCurrentDataProvider()
        {
            var dataProvider = this.ProjectDataService.CurrentDataProvider;

            if (dataProvider == null)
            {
                throw new ArgumentException(Resources.String02);
            }

            return dataProvider;
        }

        /// <summary>
        /// Gets the report service end point.
        /// </summary>
        /// <returns>The report server endpoint for the current project data instance.</returns>
        /// <exception cref="ArgumentException" />
        private Uri GetReportServiceEndPoint()
        {
            return this.GetCurrentDataProvider().GetReportServiceEndPoint(this.GetCurrentProjectData());
        }

        /// <summary>
        /// Gets the project report folder.
        /// </summary>
        /// <returns>The project report fodler.</returns>
        private string GetProjectReportFolder()
        {
            var projectData = this.GetCurrentProjectData();

            return this.GetCurrentDataProvider().GetReportFolder(projectData)
                ?? ReportProxyWrapperHelper.Get2005ProjectReportFolder(projectData);
        }

        /// <summary>
        /// Loads the report list.
        /// </summary>
        private void LoadReportList()
        {
            var errorBuilder = new StringBuilder();

            try
            {
                var reportingServiceEndPoint = this.GetReportServiceEndPoint();

                if (reportingServiceEndPoint == null)
                {
                    throw new ArgumentException(Resources.String03);
                }

                var reportsFolder = this.GetProjectReportFolder();

                var serviceProxyWrapper = Equals(
                    reportsFolder, ReportProxyWrapperHelper.Get2005ProjectReportFolder(this.GetCurrentProjectData()))
                                              ? ReportProxyWrapperHelper.ReportService2005
                                              : ReportProxyWrapperHelper.ReportService2008;

                this.reportRootNode = serviceProxyWrapper.GetRootReportNode(this.GetCurrentProjectData(), reportingServiceEndPoint, reportsFolder);

                this.Status = Resources.String04;

                this.HasLoadedReportList = true;

                return;
            }
            catch (ArgumentException aex)
            {
                errorBuilder.AppendLine(Resources.String05);
                errorBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, Resources.String08, aex.GetType().Name));
                errorBuilder.AppendLine(string.Concat(Resources.String09, aex.Message));
                AppendInnerExceptions(errorBuilder, aex);
            }
            catch (InvalidOperationException ioex)
            {
                errorBuilder.AppendLine(Resources.String05);
                errorBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, Resources.String08, ioex.GetType().Name));
                errorBuilder.AppendLine(string.Concat(Resources.String09, ioex.Message));
                AppendInnerExceptions(errorBuilder, ioex);
            }
            catch (SoapException sex)
            {
                errorBuilder.AppendLine(Resources.String05);
                errorBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, Resources.String08, sex.GetType().Name));
                errorBuilder.AppendLine(string.Concat(Resources.String10, sex.Code.Namespace));
                errorBuilder.AppendLine(string.Concat(Resources.String11, sex.Code.Name));
                errorBuilder.AppendLine(string.Concat(Resources.String12, sex.Actor));
                errorBuilder.AppendLine(string.Concat(Resources.String09, sex.Message));
                AppendInnerExceptions(errorBuilder, sex);
            }
            catch (Exception ex)
            {
                // Unexpected exception type.
                if (!CommandLibrary.ApplicationExceptionCommand.CanExecute(ex, Application.Current.MainWindow))
                {
                    throw;
                }

                errorBuilder.AppendLine(Resources.String05);
                errorBuilder.AppendLine(string.Format(CultureInfo.InvariantCulture, Resources.String08, ex.GetType().Name));
                errorBuilder.AppendLine(string.Concat(Resources.String09, ex.Message));
                AppendInnerExceptions(errorBuilder, ex);

                CommandLibrary.ApplicationExceptionCommand.Execute(new ArgumentException(Resources.String05, ex), Application.Current.MainWindow);
            }

            this.Status = errorBuilder.ToString();
        }

        /// <summary>
        /// Called when [project data changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ProjectDataChangedEventArgs"/> instance containing the event data.</param>
        private void OnProjectDataChanged(object sender, ProjectDataChangedEventArgs e)
        {
            this.HasLoadedReportList = false;
            this.reportRootNode = null;

            if (e.NewValue == null) 
            {
                this.Status = Resources.String06;
                return;
            }

            this.Status = Resources.String07;
            ThreadPool.QueueUserWorkItem(delegate { this.LoadReportList(); });
        }
    }
}
