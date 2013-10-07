// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainViewModel.cs" company="None">
//   Crispin Parker 2011
// </copyright>
// <summary>
//   Defines the MenuOptionViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.VersionCheck.ViewModels
{
    using System;

    using TfsWorkbench.Core.Services;
    using TfsWorkbench.VersionCheck.Iterfaces;
    using TfsWorkbench.VersionCheck.Models;
    using TfsWorkbench.VersionCheck.Properties;
    using TfsWorkbench.VersionCheck.Services;

    /// <summary>
    /// The menu option view model class.
    /// </summary>
    internal class MainViewModel : ViewModelBase, IMainViewModel
    {
        /// <summary>
        /// The verison check service instance.
        /// </summary>
        private readonly IVersionCheckService versionCheckService;

        /// <summary>
        /// The application context instance.
        /// </summary>
        private readonly IApplicationContextService applicationContextService;

        /// <summary>
        /// Flag to indicate if version check is in progress.
        /// </summary>
        private bool isVersionCheckInProgress;

        /// <summary>
        /// The button tool tip.
        /// </summary>
        private string buttonToolTip;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        public MainViewModel()
            : this(ServiceManager.Instance.GetService<IApplicationContextService>(), ServiceManager.Instance.GetService<IVersionCheckService>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        /// <param name="applicationContextService">The application context.</param>
        /// <param name="versionCheckService">The version check service.</param>
        public MainViewModel(IApplicationContextService applicationContextService, IVersionCheckService versionCheckService)
        {
            if (applicationContextService == null)
            {
                throw new ArgumentNullException("applicationContextService");
            }

            if (versionCheckService == null)
            {
                throw new ArgumentNullException("versionCheckService");
            }

            this.CreateCommands();

            this.versionCheckService = versionCheckService;
            this.applicationContextService = applicationContextService;
            this.buttonToolTip = Resources.String007;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [check version on start up].
        /// </summary>
        /// <value>
        /// <c>true</c> if [check version on start up]; otherwise, <c>false</c>.
        /// </value>
        public bool CheckVersionOnStartUp
        {
            get
            {
                return Settings.Default.CheckOnStartUp;
            }

            set
            {
                if (Settings.Default.CheckOnStartUp == value)
                {
                    return;
                }

                Settings.Default.CheckOnStartUp = value;
                Settings.Default.Save();

                this.OnPropertyChanged("CheckVersionOnStartUp");
            }
        }

        /// <summary>
        /// Gets the header text.
        /// </summary>
        /// <value>The header text.</value>
        public string MenuHeaderText
        {
            get
            {
                return Resources.String012;
            }
        }

        /// <summary>
        /// Gets the toggle start up check header text.
        /// </summary>
        /// <value>The toggle start up check header text.</value>
        public string ToggleStartUpCheckHeaderText
        {
            get
            {
                return Resources.String013;
            }
        }

        /// <summary>
        /// Gets the button tool tip.
        /// </summary>
        /// <value>The button tool tip.</value>
        public string ButtonToolTip
        {
            get
            {
                return this.buttonToolTip;
            }

            private set
            {
                this.buttonToolTip = value;

                this.OnPropertyChanged("ButtonToolTip");
            }
        }

        /// <summary>
        /// Gets the button content text.
        /// </summary>
        /// <value>The button content text.</value>
        public string ButtonContentText
        {
            get
            {
                return Resources.String001;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [show button].
        /// </summary>
        /// <value><c>true</c> if [show button]; otherwise, <c>false</c>.</value>
        public bool ShowButton
        {
            get
            {
                return Settings.Default.ShowButton;
            }

            set
            {
                if (Settings.Default.ShowButton == value)
                {
                    return;
                }

                Settings.Default.ShowButton = value;
                Settings.Default.Save();

                this.OnPropertyChanged("ShowButton");
            }
        }

        /// <summary>
        /// Gets the show button header text.
        /// </summary>
        /// <value>The show button header text.</value>
        public string ShowButtonHeaderText
        {
            get
            {
                return Resources.String016;
            }
        }

        /// <summary>
        /// Gets the version check command.
        /// </summary>
        /// <value>The version check command.</value>
        public CommandViewModel VersionCheckCommand { get; private set; }

        /// <summary>
        /// Gets the go to download page command.
        /// </summary>
        /// <value>The go to download page command.</value>
        public CommandViewModel GoToDownloadPageCommand { get; private set; }

        /// <summary>
        /// Executes the version check.
        /// </summary>
        public void ExecuteVersionCheck()
        {
            this.ExecuteVersionCheck(null);
        }

        /// <summary>
        /// Creates the commands.
        /// </summary>
        private void CreateCommands()
        {
            this.VersionCheckCommand = new CommandViewModel(
                Resources.String007, 
                new RelayCommand(this.ExecuteVersionCheck, this.CanExecuteVersionCheck));

            this.GoToDownloadPageCommand = new CommandViewModel(
                Resources.String008, 
                new RelayCommand(this.ExecuteGoToDownloadPage));
        }

        /// <summary>
        /// Sets the in progress status.
        /// </summary>
        /// <param name="isInProgress">if set to <c>true</c> [is in progress].</param>
        private void SetInProgressStatus(bool isInProgress)
        {
            this.isVersionCheckInProgress = isInProgress;
            this.applicationContextService.SuggestCommandRequery();
        }

        /// <summary>
        /// Executes the go to download page command.
        /// </summary>
        /// <param name="obj">The parameter object.</param>
        private void ExecuteGoToDownloadPage(object obj)
        {
            this.applicationContextService.SendSystemShellCommand(Settings.Default.DownloadUrl);
        }

        /// <summary>
        /// Determines whether this instance [can execute version check].
        /// </summary>
        /// <param name="obj">The parameter object.</param>
        /// <returns>
        /// <c>true</c> if this instance [can execute version check]; otherwise, <c>false</c>.
        /// </returns>
        private bool CanExecuteVersionCheck(object obj)
        {
            return !this.isVersionCheckInProgress;
        }

        /// <summary>
        /// Executes the version check.
        /// </summary>
        /// <param name="obj">The parameter object.</param>
        private void ExecuteVersionCheck(object obj)
        {
            if (this.isVersionCheckInProgress)
            {
                return;
            }

            this.SetInProgressStatus(true);

            this.UpdateToolTipAndSendApplicationMessage(Resources.String009);

            this.versionCheckService.BeginAsyncGetVersionStatus(this.OnVersionCheckComplete);
        }

        /// <summary>
        /// Called when [version check complete].
        /// </summary>
        /// <param name="versionStatus">The version status.</param>
        /// <param name="error">The error.</param>
        private void OnVersionCheckComplete(VersionStatus versionStatus, Exception error)
        {
            if (Helpers.IsNotNull(error))
            {
                this.UpdateToolTipAndSendApplicationError(error);
            }
            else
            {
                this.UpdateToolTipAndSendApplicationMessage(versionStatus.DisplayMessage);

                if (versionStatus.Status == VersionStatusOption.OutDated)
                {
                    this.AskUserToOpenDownloadPage();
                    return;
                }
            }

            this.SetInProgressStatus(false);
        }

        /// <summary>
        /// Asks the user to open download page.
        /// </summary>
        private void AskUserToOpenDownloadPage()
        {
            Action<bool, bool> callBack = (openPage, doStartUpCheck) =>
                {
                    if (openPage)
                    {
                        this.ExecuteGoToDownloadPage(null);
                    }

                    this.CheckVersionOnStartUp = doStartUpCheck;

                    this.SetInProgressStatus(false);
                };

            this.applicationContextService.DoesUserWantToOpenDownloadPage(callBack);
        }

        /// <summary>
        /// Updates the tool tip and send application message.
        /// </summary>
        /// <param name="message">The message.</param>
        private void UpdateToolTipAndSendApplicationMessage(string message)
        {
            this.ButtonToolTip = message;
            this.applicationContextService.SendApplicationMessage(message);
        }

        /// <summary>
        /// Updates the tool tip and send application error.
        /// </summary>
        /// <param name="exception">The exception.</param>
        private void UpdateToolTipAndSendApplicationError(Exception exception)
        {
            this.ButtonToolTip = exception.Message;
            this.applicationContextService.SendApplciationError(exception);
        }
    }
}