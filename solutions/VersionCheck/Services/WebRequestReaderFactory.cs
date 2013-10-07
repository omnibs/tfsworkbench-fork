// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebRequestReaderFactory.cs" company="None">
//   Crispin Parker 2011
// </copyright>
// <summary>
//   Defines the WebRequestReaderFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.VersionCheck.Services
{
    using TfsWorkbench.VersionCheck.Iterfaces;
    using TfsWorkbench.VersionCheck.Properties;

    /// <summary>
    /// The web request reader factory class.
    /// </summary>
    internal class WebRequestReaderFactory : IWebRequestReaderFactory
    {
        /// <summary>
        /// Creates a new instance of IWebRequestReader.
        /// </summary>
        /// <returns>A new instance of IWebRequestReader.</returns>
        public IWebRequestReader Create()
        {
            return new WebRequestReader(Settings.Default.CheckUrl);
        }
    }
}
