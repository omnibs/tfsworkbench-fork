// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoaderWithVolumeCheck.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ProjectLoader type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.WpfUI.ProjectSelector
{
    using System;
    using System.Globalization;

    using TfsWorkbench.Core.EventArgObjects;
    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.Core.Properties;
    using TfsWorkbench.UIElements;

    /// <summary>
    /// The project laded class.
    /// </summary>
    internal class LoaderWithVolumeCheck : ILoaderWithVolumeCheck
    {
        /// <summary>
        /// The project load criteria.
        /// </summary>
        private IProjectData projectData;

        /// <summary>
        /// The project selector service instance.
        /// </summary>
        private IProjectSelectorService service;

        /// <summary>
        /// The volume warning decision control.
        /// </summary>
        private DecisionControl decisionControl;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoaderWithVolumeCheck"/> class.
        /// </summary>
        /// <param name="projectData">The criteria.</param>
        /// <param name="service">The service.</param>
        public LoaderWithVolumeCheck(IProjectData projectData, IProjectSelectorService service)
        {
            if (projectData == null)
            {
                throw new ArgumentNullException("projectData");
            }

            if (service == null)
            {
                throw new ArgumentNullException("service");
            }

            this.projectData = projectData;
            this.service = service;
            this.IgnoreFutureVolumeWarnings = this.projectData.HideVolumeWarning;
        }

        /// <summary>
        /// Occurs when process complete.
        /// </summary>
        public event EventHandler Complete;

        /// <summary>
        /// Occurs on [volume warning].
        /// </summary>
        public event EventHandler<ContextEventArgs<DecisionControl>> VolumeWarning;

        /// <summary>
        /// Occurs when [aborted].
        /// </summary>
        public event EventHandler Aborted;

        /// <summary>
        /// Occurs when [confirm load data] is required.
        /// </summary>
        public event EventHandler<CancelEventArgs> ConfirmLoadData;

        /// <summary>
        /// Gets a value indicating whether [ignore volume warnings].
        /// </summary>
        /// <value>
        /// <c>true</c> if [ignore volume warnings]; otherwise, <c>false</c>.
        /// </value>
        public bool IgnoreFutureVolumeWarnings { get; private set; }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            if (this.IgnoreFutureVolumeWarnings)
            {
                this.BeginLoadData();
            }
            else
            {
                this.BeginVolumeCheck();
            }
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
        /// Begins the load data process.
        /// </summary>
        private void BeginLoadData()
        {
            if (this.OnConfirmLoadData())
            {
                this.service.BeginLoad(this.projectData, this.OnProjectLoaded);
            }
            else
            {
                this.OnAbort();
            }
        }

        /// <summary>
        /// Called when [project loaded].
        /// </summary>
        private void OnProjectLoaded()
        {
            if (this.Complete != null)
            {
                this.Complete(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Called when [confirm load date] is required.
        /// </summary>
        /// <returns><c>True</c> if not cancelled; otherwise <c>false</c>.</returns>
        private bool OnConfirmLoadData()
        {
            var cancelEventArges = new CancelEventArgs();

            if (this.ConfirmLoadData != null)
            {
                this.ConfirmLoadData(this, cancelEventArges);
            }

            return cancelEventArges.Cancel == false;
        }

        /// <summary>
        /// Begins the volume check.
        /// </summary>
        private void BeginVolumeCheck()
        {
            this.service.BeginVolumeCheck(this.projectData, this.OnVolumeCheckComplete);
        }

        /// <summary>
        /// Called when [volume check complete].
        /// </summary>
        /// <param name="volume">The volume.</param>
        private void OnVolumeCheckComplete(int volume)
        {
            var volumeExceedsWarningLevel = volume > Settings.Default.VolumeWarningLevel;

            if (volumeExceedsWarningLevel)
            {
                this.ShowVolumeWarning(volume);
            }
            else
            {
                this.BeginLoadData();
            }
        }

        /// <summary>
        /// Shows the volume warning.
        /// </summary>
        /// <param name="volume">The work item volume.</param>
        private void ShowVolumeWarning(int volume)
        {
            this.CreateDecisionControl(volume);

            this.AttachDecisionListener();

            this.OnVolumeWarning();
        }

        /// <summary>
        /// Creates the decision control.
        /// </summary>
        /// <param name="volume">The volume.</param>
        private void CreateDecisionControl(int volume)
        {
            this.decisionControl = new DecisionControl();

            var message = string.Format(
                CultureInfo.InvariantCulture,
                Properties.Resources.String040,
                volume);

            this.decisionControl.Message = message;
            this.decisionControl.Caption = Properties.Resources.String041;
            this.decisionControl.DoNotShowAgainText = Properties.Resources.String042;
        }

        /// <summary>
        /// Attaches the decision listener.
        /// </summary>
        private void AttachDecisionListener()
        {
            this.decisionControl.DecisionMade += this.OnDecisionMade;
        }

        /// <summary>
        /// Detaches the decision listener.
        /// </summary>
        private void DetachDecisionListener()
        {
            this.decisionControl.DecisionMade -= this.OnDecisionMade;
        }

        /// <summary>
        /// Called when [decision made].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnDecisionMade(object sender, EventArgs e)
        {
            this.ApplyIgnoreFutureVolumeWarningsDecision();

            this.DetachDecisionListener();

            this.HandleUserDecision();
        }

        /// <summary>
        /// Called when [volume warning].
        /// </summary>
        private void OnVolumeWarning()
        {
            if (this.VolumeWarning != null)
            {
                this.VolumeWarning(this, new ContextEventArgs<DecisionControl>(this.decisionControl));
            }
        }

        /// <summary>
        /// Handles the user decision.
        /// </summary>
        private void HandleUserDecision()
        {
            var ignoreVolumeWarningAndContinue = this.decisionControl.IsYes;

            if (ignoreVolumeWarningAndContinue)
            {
                this.BeginLoadData();
            }
            else
            {
                this.OnAbort();
            }
        }

        /// <summary>
        /// Applies the ignore future volume warnings decision.
        /// </summary>
        private void ApplyIgnoreFutureVolumeWarningsDecision()
        {
            this.IgnoreFutureVolumeWarnings = this.decisionControl.DoNotShowAgain;
        }

        /// <summary>
        /// Called when [abort].
        /// </summary>
        private void OnAbort()
        {
            if (this.Aborted != null)
            {
                this.Aborted(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="dispose"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool dispose)
        {
            if (dispose)
            {
                if (this.decisionControl != null)
                {
                    this.DetachDecisionListener();
                    this.decisionControl = null;
                }

                this.service = null;
                this.projectData = null;
            }
        }
    }
}
