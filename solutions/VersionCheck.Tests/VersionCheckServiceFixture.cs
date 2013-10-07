// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VersionCheckServiceFixture.cs" company="None">
//   Crispin Parker 2011
// </copyright>
// <summary>
//   Defines the VersionCheckServiceFixture type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.VersionCheck.Tests
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Threading;

    using NUnit.Framework;

    using Rhino.Mocks;

    using SharpArch.Testing.NUnit;

    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.Core.Services;
    using TfsWorkbench.VersionCheck.Iterfaces;
    using TfsWorkbench.VersionCheck.Models;
    using TfsWorkbench.VersionCheck.Services;

    /// <summary>
    /// The version check service test fixture class.
    /// </summary>
    [TestFixture]
    public class VersionCheckServiceFixture
    {
        /// <summary>
        /// The version check service.
        /// </summary>
        private VersionCheckService versionCheckService;

        /// <summary>
        /// The web request reader factory.
        /// </summary>
        private IWebRequestReaderFactory webRequestReaderFactory;

        /// <summary>
        /// Sets up the test methods.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            this.webRequestReaderFactory = MockRepository.GenerateMock<IWebRequestReaderFactory>();
            this.versionCheckService = new VersionCheckService(this.webRequestReaderFactory);
        }

        /// <summary>
        /// Tears down the test context.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            this.versionCheckService = null;
        }

        /// <summary>
        /// Constructor, without parameter, calls service manager.
        /// </summary>
        [Test]
        public void Constructor_WithoutParameter_CallsServiceManager()
        {
            // Arrange
            var serviceManager = MockRepository.GenerateMock<IServiceManager>();
            serviceManager
                .Expect(sm => sm.GetService<IWebRequestReaderFactory>())
                .Return(this.webRequestReaderFactory)
                .Repeat.Once();

            // Act
            ServiceManager.Instance = serviceManager;
            new VersionCheckService();
            ServiceManager.Instance = null;

            // Assert
            serviceManager.VerifyAllExpectations();
        }

        /// <summary>
        /// Constructor, with null parameter, throws an exception.
        /// </summary>
        [Test]
        public void Constructor_WithNullParameter_ThrowsException()
        {
            // Act
            try
            {
                new VersionCheckService(null);
                Assert.Fail("Exception Not Thrown");
            }
            catch (ArgumentNullException)
            {
            }

            // Assert
            Assert.Pass();
        }

        /// <summary>
        /// Get version status, when the version is up to date, returns up to date status.
        /// </summary>
        [Test]
        public void GetVersionStatus_WhenVersionIsUpToDate_ReturnsUpToDateStatus()
        {
            // Arrange
            var actualVersion = Assembly.GetAssembly(typeof(IProjectData)).GetName().Version.ToString();
            this.SetUpWebRequestReader(actualVersion);

            // Act
            var result = this.GetVersionStatus();

            // Assert
            result.ShouldNotBeNull();
            result.Status.ShouldEqual(VersionStatusOption.UpToDate);

            Debug.WriteLine(actualVersion);
        }

        /// <summary>
        /// Get version status, when version is out of date, returns out dated status.
        /// </summary>
        [Test]
        public void GetVersionStatus_WhenVersionIsOutOfDate_ReturnsOutDatedStatus()
        {
            // Arrange
            const string WrongVersion = "99.99.99.99";
            this.SetUpWebRequestReader(WrongVersion);

            // Act
            var result = this.GetVersionStatus();

            // Assert
            result.ShouldNotBeNull();
            result.Status.ShouldEqual(VersionStatusOption.OutDated);
        }

        /// <summary>
        /// Get version status, when version check fails, returns failed version check with exception message.
        /// </summary>
        [Test]
        public void GetVersionStatus_WhenVersionCheckFails_ReturnsFailedVersionCheckWithExceptionMessage()
        {
            // Arrange
            const string ErrorMessage = "Web Exception";
            this.SetUpWebRequestReader(ErrorMessage, false);

            // Act
            var result = this.GetVersionStatus();

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType(typeof(FailedToCheckVersionStatus));
            result.DisplayMessage.ShouldEndWith(ErrorMessage);
        }

        /// <summary>
        /// Begin async version check, when request complete, executes the specified call back action.
        /// </summary>
        [Test]
        public void BeginAsyncVersionCheck_WhenRequestComplete_ExecutesTheSpecifiedCallBackAction()
        {
            // Arrange
            this.SetUpWebRequestReader("A Response");
            var hasCalledBack = false;
            var resetEvent = new AutoResetEvent(false);

            Action<VersionStatus, Exception> callBack = (vs, ex) =>
                {
                    hasCalledBack = true;
                    resetEvent.Set();
                };

            // Act
            this.versionCheckService.BeginAsyncGetVersionStatus(callBack);
            resetEvent.WaitOne(1000);

            // Assert
            hasCalledBack.ShouldBeTrue();
        }

        /// <summary>
        /// Gets the version status.
        /// </summary>
        /// <returns>The version status.</returns>
        private VersionStatus GetVersionStatus()
        {
            VersionStatus statusToReturn = null;
            var resetEvent = new AutoResetEvent(false);

            Action<VersionStatus, Exception> callBack = (vs, ex) =>
            {
                if (ex != null)
                {
                    throw ex;
                }

                statusToReturn = vs;

                resetEvent.Set();
            };

            this.versionCheckService.BeginAsyncGetVersionStatus(callBack);
            resetEvent.WaitOne(1000);

            return statusToReturn;
        }

        /// <summary>
        /// Sets up the web request reader.
        /// </summary>
        /// <param name="requiredResponse">The required response.</param>
        /// <param name="wasSuccessful">if set to <c>true</c> [was successful].</param>
        private void SetUpWebRequestReader(string requiredResponse, bool wasSuccessful = true)
        {
            var webRequestReader = MockRepository.GenerateMock<IWebRequestReader>();

            string response;
            webRequestReader
                .Expect(wrr => wrr.TryReadFirstLine(out response))
                .IgnoreArguments()
                .OutRef(requiredResponse)
                .Return(wasSuccessful)
                .Repeat.Any();

            this.webRequestReaderFactory
                .Expect(wrrf => wrrf.Create())
                .Return(webRequestReader)
                .Repeat.Any();
        }
    }
}
