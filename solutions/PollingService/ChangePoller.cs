// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangePoller.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ChangePoller type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.PollingService
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Threading;

    using Core.DataObjects;
    using Core.Interfaces;
    using Core.Services;
    using Properties;
    using UIElements;

    /// <summary>
    /// The change poller class.
    /// </summary>
    public class ChangePoller : INotifyPropertyChanged
    {
        /// <summary>
        /// The next poll time proerty change arguments.
        /// </summary>
        private readonly PropertyChangedEventArgs nextPollChangedEventArgs = new PropertyChangedEventArgs("NextPollIn");

        /// <summary>
        /// The service dispatcher isntance.
        /// </summary>
        private Dispatcher serviceDispatcher;

        /// <summary>
        /// The last poll time.
        /// </summary>
        private DateTime lastPollTime;

        /// <summary>
        /// The is running flag.
        /// </summary>
        private bool isRunning;

        /// <summary>
        /// Occurs when [changes found].
        /// </summary>
        public event EventHandler<IdRevisionListEventArgs> ChangesFound;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the project data.
        /// </summary>
        /// <value>The project data.</value>
        public IProjectData ProjectData { get; set; }

        /// <summary>
        /// Gets or sets the data provider.
        /// </summary>
        /// <value>The data provider.</value>
        public IDataProvider DataProvider { get; set; }

        /// <summary>
        /// Gets or sets the duration of the pause.
        /// </summary>
        /// <value>The duration of the pause.</value>
        public TimeSpan Interval
        {
            get
            {
                return Settings.Default.ChangePollingInterval;
            }

            set
            {
                if (Settings.Default.ChangePollingInterval == value)
                {
                    return;
                }

                Settings.Default.ChangePollingInterval = value;

                OnPropertyChanged(new PropertyChangedEventArgs("Interval"));
            }
        }

        /// <summary>
        /// Gets the last poll time.
        /// </summary>
        /// <value>The last poll time.</value>
        public DateTime LastPollTime
        {
            get
            {
                return lastPollTime;
            }

            private set
            {
                if (lastPollTime == value)
                {
                    return;
                }

                lastPollTime = value;

                OnPropertyChanged(new PropertyChangedEventArgs("LastPollTime"));
            }
        }

        /// <summary>
        /// Gets the next poll time.
        /// </summary>
        /// <value>The next poll time.</value>
        public TimeSpan? NextPollIn
        {
            get
            {
                return isRunning ? (TimeSpan?)LastPollTime.Add(Interval).Subtract(DateTime.Now) : null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is running.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is running; otherwise, <c>false</c>.
        /// </value>
        public bool IsRunning
        {
            get
            {
                return isRunning;
            }

            private set
            {
                if (isRunning == value)
                {
                    return;
                }

                isRunning = value;

                OnPropertyChanged(new PropertyChangedEventArgs("IsRunning"));
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
        public bool IsValid
        {
            get
            {
                return DataProvider != null && ProjectData != null;
            }
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            if (!IsValid || IsRunning)
            {
                return;
            }

            IsRunning = true;

            if (LastPollTime == DateTime.MinValue)
            {
                LastPollTime = DateTime.Now;
            }

            serviceDispatcher = Dispatcher.CurrentDispatcher;

            ThreadPool.QueueUserWorkItem(delegate { Tick(); });
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            IsRunning = false;
        }

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="args">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        public void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            if (PropertyChanged == null)
            {
                return;
            }

            PropertyChanged(this, args);

            if (!Equals(args.PropertyName, nextPollChangedEventArgs.PropertyName))
            {
                PropertyChanged(this, nextPollChangedEventArgs);
            }
        }

        /// <summary>
        /// The background tick method.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exception passed to dispatcher thread.")]
        private void Tick()
        {
            while (IsRunning && NextPollIn.HasValue)
            {
                var now = DateTime.Now;

                if (NextPollIn.Value.TotalMilliseconds < 1)
                {
                    IdAndRevision[] results = null;
                    Exception error = null;

                    // Poll for changes
                    try
                    {
                        results = PollForChanges();
                        LastPollTime = now;
                    }
                    catch (Exception ex)
                    {
                        error = ex;
                    }

                    serviceDispatcher.BeginInvoke(
                        DispatcherPriority.Normal, 
                        new Action(() => TickComplete(results, error)));
                }

                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// The background tick complete method.
        /// </summary>
        /// <param name="results">The results.</param>
        /// <param name="error">The error.</param>
        private void TickComplete(IdAndRevision[] results, Exception error)
        {
            if (!IsRunning)
            {
                return;
            }

            if (error != null)
            {
                CommandLibrary.ApplicationExceptionCommand.Execute(new ArgumentException(Resources.String001, error), Application.Current.MainWindow);
                Stop();
                return;
            }

            if (results != null && results.Any() && ChangesFound != null)
            {
                ChangesFound(this, new IdRevisionListEventArgs(results));
            }
        }

        /// <summary>
        /// Polls for changes.
        /// </summary>
        /// <returns>A list of changed work item ids.</returns>
        private IdAndRevision[] PollForChanges()
        {
            if (!IsValid)
            {
                return null;
            }

            var filter = ServiceManager.Instance.GetService<IProjectDataService>()
                .GeneratePathFilter(ProjectData.ProjectIterationPath, ProjectData.ProjectAreaPath);

            return DataProvider.GetRevisedItemIds(filter, ProjectData, LastPollTime.AddHours(-1)).ToArray();
        }
    }
}
