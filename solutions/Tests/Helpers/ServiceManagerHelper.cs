// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceManagerHelper.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ServiceManagerHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Tests.Helpers
{
    using Rhino.Mocks;

    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.Core.Services;

    /// <summary>
    /// The service manaer helper class.
    /// </summary>
    internal static class ServiceManagerHelper
    {
        /// <summary>
        /// Applies the dummy manager including.
        /// </summary>
        public static void MockServiceManager()
        {
            var serviceManager = MockRepository.GenerateStub<IServiceManager>();

            ServiceManager.Instance = serviceManager;
        }

        /// <summary>
        /// Applies the dummy manager including.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <param name="serviceInstance">The service instance.</param>
        public static void MockServiceManager<T>(T serviceInstance)
        {
            MockServiceManager();
            RegisterServiceInstance(serviceInstance);
        }

        /// <summary>
        /// Registers the service instance.
        /// </summary>
        /// <typeparam name="T">The service type.</typeparam>
        /// <param name="serviceInstance">The service instance.</param>
        public static void RegisterServiceInstance<T>(T serviceInstance)
        {
            ServiceManager.Instance
                .Expect(sm => sm.GetService<T>())
                .Return(serviceInstance)
                .Repeat.Any();
        }

        /// <summary>
        /// Clears the dummy manager.
        /// </summary>
        public static void ClearDummyManager()
        {
            ServiceManager.Instance = null;
        }
    }
}
