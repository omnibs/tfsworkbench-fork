// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWebRequestReader.cs" company="None">
//   Crispin Parker 2011
// </copyright>
// <summary>
//   Defines the IWebRequestReader type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.VersionCheck.Iterfaces
{
    using System;
    using System.Net;

    /// <summary>
    /// The web request reader interface.
    /// </summary>
    public interface IWebRequestReader
    {
        /// <summary>
        /// Gets the end point URI.
        /// </summary>
        /// <value>The end point URI.</value>
        Uri EndPointUri { get; }

        /// <summary>
        /// Gets the web request creator.
        /// </summary>
        /// <value>The web request creator.</value>
        IWebRequestCreate WebRequestCreator { get; }

        /// <summary>
        /// Tries the read the first line.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <returns><c>True</c> if the first line is read; otherwise <c>false</c>.</returns>
        bool TryReadFirstLine(out string response);
    }
}