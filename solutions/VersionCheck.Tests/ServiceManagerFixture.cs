// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceManagerFixture.cs" company="None">
//   Crispin Parker 2011
// </copyright>
// <summary>
//   Defines the ServiceManagerFixture type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.VersionCheck.Tests
{
    using System.Management.Instrumentation;
    using System.Windows;

    using NUnit.Framework;

    using Rhino.Mocks;

    using SharpArch.Testing.NUnit;

    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.Core.Services;
    using TfsWorkbench.VersionCheck.Iterfaces;
    using TfsWorkbench.VersionCheck.Services;
    using TfsWorkbench.VersionCheck.ViewModels;

    /// <summary>
    /// The service manager test fixture class.
    /// </summary>
    [TestFixture]
    public class ServiceManagerFixture
    {
        /// <summary>
        /// The application.
        /// </summary>
        private Application application;

        /// <summary>
        /// The test service interface.
        /// </summary>
        private interface ITestService
        {
        }

        /// <summary>
        /// Sets up the tests fixture.
        /// </summary>
        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            // Sets up the Application.Current parameter.
            this.application = Application.Current ?? MockRepository.GenerateMock<Application>();
            this.application.MainWindow = MockRepository.GenerateMock<Window>();
        }

        /// <summary>
        /// Tests the fixture tear down.
        /// </summary>
        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            this.application.MainWindow = null;
            this.application = null;
        }

        /// <summary>
        /// Sets up the test environment.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            ServiceManager.Instance = null;
        }

        /// <summary>
        /// Tears down the test environment.
        /// </summary>
        public void TearDown()
        {
            ServiceManager.Instance = null;
        }

        /// <summary>
        /// Get instance, with null instance set, returns default service manager.
        /// </summary>
        [Test]
        public void GetInstance_WithNullInstanceSet_ReturnsDefaultServiceManager()
        {
            // Arrange
            ServiceManager.Instance = null;

            // Act
            var serviceManager = ServiceManager.Instance;

            // Assert
            serviceManager.ShouldNotBeNull();
            serviceManager.ShouldBeOfType(typeof(ServiceManager));
        }

        /// <summary>
        /// Get instance, with instance set, returns set instance.
        /// </summary>
        [Test]
        public void GetInstance_WithInstanceSet_ReturnsSetInstance()
        {
            // Arrange
            var serviceManager = MockRepository.GenerateMock<IServiceManager>();
            ServiceManager.Instance = serviceManager;

            // Act
            var instance = ServiceManager.Instance;

            // Assert
            instance.ShouldEqual(serviceManager);
        }

        /// <summary>
        /// Register constructor, registers the concrete type against the interface.
        /// </summary>
        [Test]
        public void RegisterConstructor_RegistersTheConcreteTypeAgainstTheInterface()
        {
            // Arrange
            ServiceManager.Instance.RegisterConstructor<ITestService, TestService>();

            // Act
            var returnedService = ServiceManager.Instance.GetService<ITestService>();

            // Assert
            returnedService.ShouldBeOfType(typeof(TestService));
        }

        /// <summary>
        /// Get service, when no matching service registered, throws exception.
        /// </summary>
        [Test]
        public void GetService_WhenNoMatchingServiceRegistered_ThrowsException()
        {
            // Act
            try
            {
                ServiceManager.Instance.GetService<ITestService>();
                Assert.Fail("Exception Not Thrown");
            }
            catch (InstanceNotFoundException)
            {
            }

            // Assert
            Assert.Pass();
        }

        [Test]
        public void ServiceRegistor_RegisterConstructors_InitialisesVersionCheckServices()
        {
            // Arrange
            var serviceManager = ServiceManager.Instance;
            ServiceRegistor.RegisterConstructors(serviceManager);

            // Act
            var applicationContextService = serviceManager.GetService<IApplicationContextService>();
            var versionCheckService = serviceManager.GetService<IVersionCheckService>();
            var webRequestReaderFactory = serviceManager.GetService<IWebRequestReaderFactory>();
            var mainViewModel = serviceManager.GetService<IMainViewModel>();

            // Assert
            applicationContextService.ShouldBeOfType(typeof(ApplicationContextService));
            versionCheckService.ShouldBeOfType(typeof(VersionCheckService));
            webRequestReaderFactory.ShouldBeOfType(typeof(WebRequestReaderFactory));
            mainViewModel.ShouldBeOfType(typeof(MainViewModel));
        }

        /// <summary>
        /// The test service class.
        /// </summary>
        private class TestService : ITestService
        {
        }
    }
}
