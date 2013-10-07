// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceManager.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ServiceManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Management.Instrumentation;

    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.Core.Properties;

    /// <summary>
    /// The service manager
    /// </summary>
    public class ServiceManager : IServiceManager
    {
        /// <summary>
        /// The service type map.
        /// </summary>
        private readonly IDictionary<Type, Type> serviceTypeMap = new Dictionary<Type, Type>();

        /// <summary>
        /// The service instance map.
        /// </summary>
        private readonly IDictionary<Type, object> serviceInstanceMap = new Dictionary<Type, object>();

        /// <summary>
        /// The service manager instance.
        /// </summary>
        private static IServiceManager instance;

        /// <summary>
        /// Gets or sets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static IServiceManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ServiceManager();
                    InitialiseDefaultServices(instance);
                }

                return instance;
            }

            set
            {
                instance = value;
            }
        }

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <returns>The service instance.</returns>
        public TInterface GetService<TInterface>()
        {
            object service;
            if (!this.serviceInstanceMap.TryGetValue(typeof(TInterface), out service))
            {
                service = this.BuildInstanceAndAddToMap<TInterface>();
            }

            return (TInterface)service;
        }

        /// <summary>
        /// Registers the service constructor.
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <typeparam name="TImplementation">The type of the implementation.</typeparam>
        public void RegisterConstructor<TInterface, TImplementation>() where TImplementation : new()
        {
            this.serviceTypeMap.Add(typeof(TInterface), typeof(TImplementation));
        }

        /// <summary>
        /// Initialises the default services.
        /// </summary>
        /// <param name="serviceManager">The service manager.</param>
        private static void InitialiseDefaultServices(IServiceManager serviceManager)
        {
            serviceManager.RegisterConstructor<ILinkManagerService, LinkManagerService>();
            serviceManager.RegisterConstructor<IProjectDataService, ProjectDataService>();
            serviceManager.RegisterConstructor<IFilterService, FilterService>();
            serviceManager.RegisterConstructor<IWorkbenchItemRepository, WorkbenchItemRepository>();
        }

        /// <summary>
        /// Builds the instance and adds it to the map.
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <returns>An instance of the service.</returns>
        private TInterface BuildInstanceAndAddToMap<TInterface>()
        {
            var service = Activator.CreateInstance(this.GetImplementaitonType<TInterface>());

            this.serviceInstanceMap.Add(typeof(TInterface), service);

            return (TInterface)service;
        }

        /// <summary>
        /// Gets the type of the implementaiton.
        /// </summary>
        /// <typeparam name="TInterface">The type of the interface.</typeparam>
        /// <returns>The implementaiton type for the specfied interface.</returns>
        private Type GetImplementaitonType<TInterface>()
        {
            Type implementaitonType;
            if (!this.serviceTypeMap.TryGetValue(typeof(TInterface), out implementaitonType))
            {
                throw new InstanceNotFoundException(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.String001, 
                        typeof(TInterface).Name));
            }

            return implementaitonType;
        }
    }
}
