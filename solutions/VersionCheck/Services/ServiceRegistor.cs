// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceRegistor.cs" company="Scrum for Team System">
//   None
// </copyright>
// <summary>
//   Defines the ServiceRegistor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.VersionCheck.Services
{
    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.VersionCheck.Iterfaces;
    using TfsWorkbench.VersionCheck.ViewModels;

    /// <summary>
    /// The service registor class.
    /// </summary>
    internal static class ServiceRegistor
    {
        /// <summary>
        /// Registers the service constructors.
        /// </summary>
        /// <param name="serviceManager">The service manager.</param>
        public static void RegisterConstructors(IServiceManager serviceManager)
        {
            serviceManager.RegisterConstructor<IApplicationContextService, ApplicationContextService>();
            serviceManager.RegisterConstructor<IWebRequestReaderFactory, WebRequestReaderFactory>();
            serviceManager.RegisterConstructor<IVersionCheckService, VersionCheckService>();
            serviceManager.RegisterConstructor<IMainViewModel, MainViewModel>();
        }
    }
}
