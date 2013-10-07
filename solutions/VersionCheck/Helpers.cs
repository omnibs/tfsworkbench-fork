// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Helpers.cs" company="None">
//   Crispin Parker 2011
// </copyright>
// <summary>
//   Defines the Helpers type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.VersionCheck
{
    using System.IO;
    using System.Net;
    using System.Reflection;

    using TfsWorkbench.Core.Interfaces;

    /// <summary>
    /// The helpers class.
    /// </summary>
    internal static class Helpers
    {
        /// <summary>
        /// Determines whether [the specified object to test] [is not null].
        /// </summary>
        /// <param name="objectToTest">The object to test.</param>
        /// <returns>
        /// <c>true</c> if [the specified object to test] [is not null]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNotNull(object objectToTest)
        {
            return objectToTest != null;
        }

        /// <summary>
        /// Reads the first line of the specifeid web response.
        /// </summary>
        /// <param name="webResponse">The web response.</param>
        /// <returns><c>Null</c> if the web response is null; otherwise first line of response.</returns>
        public static string ReadFirstLine(this WebResponse webResponse)
        {
            string firstLineOfResponse = null;
            if (IsNotNull(webResponse))
            {
                using (var responseStream = webResponse.GetResponseStream())
                {
                    firstLineOfResponse = responseStream.ReadFirstLine();
                }

                webResponse.Close();
            }

            return firstLineOfResponse;
        }

        /// <summary>
        /// Gets the core version.
        /// </summary>
        /// <returns>The core assembly version.</returns>
        public static string GetLocalCoreVersion()
        {
            return Assembly.GetAssembly(typeof(IProjectData)).GetName().Version.ToString();
        }

        /// <summary>
        /// Reads the first line of the specified stream.
        /// </summary>
        /// <param name="stream">The response stream.</param>
        /// <returns><c>Null</c> if the stream is null; otherwise first line of the stream.</returns>
        private static string ReadFirstLine(this Stream stream)
        {
            string firstLineOfStream = null;
            if (IsNotNull(stream))
            {
                using (var sr = new StreamReader(stream))
                {
                    firstLineOfStream = sr.ReadLine();
                }
            }

            return firstLineOfStream;
        }
    }
}
