// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DispatcherHelper.cs" company="None">
//   None
// </copyright>
// <summary>
//   The dispatcher helper class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Tests.Helpers
{
    using System;
    using System.Threading;
    using System.Windows.Threading;

    /// <summary>
    /// The dispatcher helper class.
    /// </summary>
    public static class DispatcherHelper
    {
        /// <summary>
        /// The stop execution callback.
        /// </summary>
        private static readonly EventHandler stop;

        /// <summary>
        /// Initializes static members of the <see cref="DispatcherHelper"/> class.
        /// </summary>
        static DispatcherHelper()
        {
            stop = (s, e) =>
            {
                ((DispatcherTimer)s).Stop();
                Dispatcher.ExitAllFrames();
            };
        }

        /// <summary>
        /// Executes the on dispatcher thread.
        /// </summary>
        /// <param name="test">The test method.</param>
        /// <param name="secondsToWait">The seconds to wait.</param>
        public static void ExecuteOnDispatcherThread(Action test, int secondsToWait = 5)
        {
            Action dispatch = () =>
                {
                    Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Normal, test);
                    StartTimer(secondsToWait, Dispatcher.CurrentDispatcher);

                    Dispatcher.Run();
                };

            ExecuteOnNewThread(dispatch);
        }

        /// <summary>
        /// Executes the on dispatcher thread.
        /// </summary>
        /// <typeparam name="T">The test result type.</typeparam>
        /// <param name="test">The test to execute.</param>
        /// <param name="secondsToWait">The seconds to wait.</param>
        /// <returns>The test result.</returns>
        public static T ExecuteOnDispatcherThread<T>(Func<T> test, int secondsToWait = 5)
        {
            var result = default(T);

            Action execute = () => result = test();

            ExecuteOnDispatcherThread(execute, secondsToWait);

            return result;
        }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        /// <param name="secondsToWait">The seconds to wait.</param>
        /// <param name="dispatcher">The dispatcher.</param>
        private static void StartTimer(int secondsToWait, Dispatcher dispatcher)
        {
            var timeToWait = TimeSpan.FromSeconds(secondsToWait);
            var timer = new DispatcherTimer(timeToWait, DispatcherPriority.ApplicationIdle, stop, dispatcher);
            timer.Start();
        }

        /// <summary>
        /// Executes the specified action on a seperate thread.
        /// </summary>
        /// <param name="dispatcherAction">The dispatcher action.</param>
        private static void ExecuteOnNewThread(Action dispatcherAction)
        {
            var dispatchThread = new Thread(new ThreadStart(dispatcherAction));
            dispatchThread.SetApartmentState(ApartmentState.STA);
            dispatchThread.Start();
            dispatchThread.Join();
        }
    }
}