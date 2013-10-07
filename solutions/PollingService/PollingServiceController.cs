// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PollingServiceController.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ChangePollerHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.PollingService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Threading;

    using TfsWorkbench.Core.EventArgObjects;
    using TfsWorkbench.Core.Helpers;
    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.Core.Services;
    using TfsWorkbench.PollingService.Properties;
    using TfsWorkbench.UIElements;

    /// <summary>
    /// The change poller helper class.
    /// </summary>
    internal class PollingServiceController : IPollingServiceController
    {
        /// <summary>
        /// The project data service instance.
        /// </summary>
        private readonly IProjectDataService projectDataService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PollingServiceController"/> class.
        /// </summary>
        public PollingServiceController()
            : this(new ChangePoller(), ServiceManager.Instance.GetService<IProjectDataService>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PollingServiceController"/> class.
        /// </summary>
        /// <param name="changePoller">The change poller.</param>
        /// <param name="projectDataService">The project data service.</param>
        public PollingServiceController(ChangePoller changePoller, IProjectDataService projectDataService)
        {
            this.projectDataService = projectDataService;
            if (projectDataService == null)
            {
                throw new ArgumentNullException("projectDataService");
            }

            this.ChangePoller = changePoller;
            this.ChangePoller.ChangesFound += this.OnChangesFound;
            this.projectDataService.ProjectDataChanged += this.OnProjectDataChanged;
        }

        /// <summary>
        /// Gets the change poller.
        /// </summary>
        /// <value>The change poller.</value>
        public ChangePoller ChangePoller { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance can show dialog.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance can show dialog; otherwise, <c>false</c>.
        /// </value>
        public bool CanShowDialog { get; private set; }

        /// <summary>
        /// Attaches the project data.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        public void AttachProjectData(IProjectData projectData)
        {
            this.ChangePoller.ProjectData = projectData;
        }

        /// <summary>
        /// Dettaches the project data.
        /// </summary>
        public void DetachProjectData()
        {
            this.StopPolling();
            this.ChangePoller.ProjectData = null;
        }

        /// <summary>
        /// Starts the polling.
        /// </summary>
        public void StartPolling()
        {
            this.ChangePoller.Start();
        }

        /// <summary>
        /// Stops the polling.
        /// </summary>
        public void StopPolling()
        {
            this.ChangePoller.Stop();
        }

        /// <summary>
        /// Shows the change poller control.
        /// </summary>
        /// <param name="sourceElement">The calling ui element.</param>
        public void ShowChangePollerControl(UIElement sourceElement)
        {
            var pollingControl = new PollingServiceDialog { ChangePoller = this.ChangePoller };

            CommandLibrary.ShowDialogCommand.Execute(pollingControl, sourceElement);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="isDisposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected void Dispose(bool isDisposing)
        {
            if (!isDisposing)
            {
                return;
            }

            this.ChangePoller.ChangesFound -= this.OnChangesFound;
        }

        /// <summary>
        /// Called when [changes found].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="IdRevisionListEventArgs"/> instance containing the event data.</param>
        private void OnChangesFound(object sender, IdRevisionListEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow;

            if (e.Context == null || mainWindow == null)
            {
                return;
            }

            var dispatcher = mainWindow.Dispatcher;

            if (dispatcher.Thread != Thread.CurrentThread)
            {
                dispatcher.Invoke(
                    DispatcherPriority.Normal, 
                    new Action(() => this.OnChangesFound(sender, e)));

                return;
            }

            var revisedWorkbenchItems = this.projectDataService.CurrentProjectData.WorkbenchItems
                .Where(w => e.Context.Any(ir => Equals(ir.Id, w.GetId()) && !Equals(ir.Revision, w.GetRevision())));

            var updatedItems = new List<int>();
            var conflicts = new List<IWorkbenchItem>();

            foreach (var workbenchItem in revisedWorkbenchItems)
            {
                if (workbenchItem.ValueProvider.IsDirty)
                {
                    conflicts.Add(workbenchItem);
                }
                else
                {
                    workbenchItem.ValueProvider.SyncToLatest();
                    updatedItems.Add(workbenchItem.GetId());
                }
            }

            Action reportChanges = () =>
            {
                if (!updatedItems.Any())
                {
                    return;
                }

                var updateMessage = string.Empty;

                foreach (var id in updatedItems.OrderBy(i => i))
                {
                    if (!string.IsNullOrEmpty(updateMessage))
                    {
                        updateMessage = string.Concat(updateMessage, Resources.String007);
                    }

                    updateMessage = string.Concat(updateMessage, id);
                }

                updateMessage = string.Concat(Resources.String003, updateMessage);

                CommandLibrary.ApplicationMessageCommand.Execute(updateMessage, mainWindow);
            };

            if (!conflicts.Any() || Settings.Default.IgnorePollingConflicts)
            {
                reportChanges();
                return;
            }

            var decisionControl = new DecisionControl
            {
                DoNotShowAgainText = Resources.String005,
                Caption = Resources.String006,
                Message = Resources.String004
            };

            decisionControl.DecisionMade +=
                (s, e2) =>
                {
                    Settings.Default.IgnorePollingConflicts = decisionControl.DoNotShowAgain;
                    if (decisionControl.IsYes)
                    {
                        foreach (var conflict in conflicts)
                        {
                            conflict.ValueProvider.SyncToLatest();
                            updatedItems.Add(conflict.GetId());
                        }
                    }

                    reportChanges();
                };

            CommandLibrary.ShowDialogCommand.Execute(decisionControl, mainWindow);
        }

        /// <summary>
        /// Called when [project data changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ProjectDataChangedEventArgs"/> instance containing the event data.</param>
        private void OnProjectDataChanged(object sender, ProjectDataChangedEventArgs e)
        {
            var projectData = e.NewValue;

            this.CanShowDialog = projectData != null;

            if (projectData == null)
            {
                this.DetachProjectData();
            }
            else
            {
                if (this.ChangePoller.DataProvider == null)
                {
                    this.ChangePoller.DataProvider = this.projectDataService.CurrentDataProvider;
                }

                this.AttachProjectData(projectData);
                if (Settings.Default.ChangePollingEnabled)
                {
                    this.StartPolling();
                }
            }
        }
    }
}
