// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWebRequestReaderFactory.cs" company="None">
//   Crispin Parker 2011
// </copyright>
// <summary>
//   The web request creator factory interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.VersionCheck.Iterfaces
{
    /// <summary>
    /// The web request creator factory interface.
    /// </summary>
    public interface IWebRequestReaderFactory
    {
        /// <summary>
        /// Creates a new instance of IWebRequestReader.
        /// </summary>
        /// <returns>A new instance of IWebRequestReader.</returns>
        IWebRequestReader Create();
    }
}