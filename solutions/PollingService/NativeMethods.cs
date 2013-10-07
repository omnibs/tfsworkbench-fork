// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NativeMethods.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ServerTime type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.PollingService
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Runtime.InteropServices;

    /// <summary>
    /// The server time class.
    /// </summary>
    internal class NativeMethods
    {
        /// <summary>
        /// The success resturn code.
        /// </summary>
        private const int NERR_Success = 0;

        /// <summary>
        /// Gets the server time.
        /// </summary>
        /// <param name="serverUri">The server URI.</param>
        /// <returns>The remote server time.</returns>
        /// <exception cref="ArgumentException" />
        public static DateTime GetServerTime(Uri serverUri)
        {
            try
            {
                return GetRemoteTime(serverUri.Host);
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Unable to get the remote server time.", ex);
            }
        }

        [DllImport("netapi32.dll", SetLastError = true, EntryPoint = "NetRemoteTOD", CharSet = CharSet.Unicode,
            ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        internal static extern int GetRemoteTime(string uncServerName, ref IntPtr bufPtr);

        [DllImport("netapi32.dll", SetLastError = true, EntryPoint = "NetApiBufferFree")]
        internal static extern int NetApiBufferFree(IntPtr bufPtr);

        /// <summary>
        /// Gets the remote time.
        /// </summary>
        /// <param name="hostName">Name of the host.</param>
        /// <returns>The server time.</returns>
        private static DateTime GetRemoteTime(string hostName)
        {
            var remoteTimePtr = IntPtr.Zero;

            var result = GetRemoteTime(hostName, ref remoteTimePtr);
            if (result != 0)
            {
                throw new Win32Exception(result);
            }

            var remoteTimeInfo = (TimeOfDayInfo)Marshal.PtrToStructure(remoteTimePtr, typeof(TimeOfDayInfo));

            var netApiResult = NetApiBufferFree(remoteTimePtr);

            if (netApiResult != NERR_Success)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Attempt to 'Get Remote Time' returned {0}", netApiResult));
            }

            var remoteTime = new DateTime(
                remoteTimeInfo.Year,
                remoteTimeInfo.Month,
                remoteTimeInfo.Day,
                remoteTimeInfo.Hours,
                remoteTimeInfo.Minutes,
                remoteTimeInfo.Seconds,
                remoteTimeInfo.Hundredth * 10,
                DateTimeKind.Unspecified);

            if (TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now))
            {
                remoteTime = remoteTime.AddHours(1);
            }

            return remoteTime;
        }

        /// <summary>
        /// The time of dat structure.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct TimeOfDayInfo
        {
            /// <summary>
            /// Elapsed time element.
            /// </summary>
            public int Elapsed;

            /// <summary>
            /// Milliseconds time element.
            /// </summary>
            public int Milliseconds;

            /// <summary>
            /// Hours time element.
            /// </summary>
            public int Hours;

            /// <summary>
            /// Minutes time element.
            /// </summary>
            public int Minutes;

            /// <summary>
            /// Seconds time element.
            /// </summary>
            public int Seconds;

            /// <summary>
            /// Hundredth time element.
            /// </summary>
            public int Hundredth;

            /// <summary>
            /// Timezone time element.
            /// </summary>
            public int Timezone;

            /// <summary>
            /// Interval time element.
            /// </summary>
            public int Interval;

            /// <summary>
            /// Day time element.
            /// </summary>
            public int Day;

            /// <summary>
            /// Month time element.
            /// </summary>
            public int Month;

            /// <summary>
            /// Year time element.
            /// </summary>
            public int Year;

            /// <summary>
            /// Weekday time element.
            /// </summary>
            public int Weekday;
        }
    }
}
