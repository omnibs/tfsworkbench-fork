// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IServiceManager.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the IServiceManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.Interfaces
{
    /// <summary>
    /// The service manager interface.
    /// </summary>
    public interface IServiceManager
    {
        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <typeparam name="TInterface">The required service interface</typeparam>
        /// <returns>An instance of the service that implements the specfied interface.</returns>
        TInterface GetService<TInterface>();

        /// <summary>
        /// Registers the service.
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        void RegisterConstructor<TInterface, TImplementation>() where TImplementation : new();
    }
}