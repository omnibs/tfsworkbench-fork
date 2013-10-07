// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebRequestReaderFactoryFixture.cs" company="None">
//   Crispin Parker 2011
// </copyright>
// <summary>
//   Defines the WebRequestorFactoryFixture type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.VersionCheck.Tests
{
    using System;

    using NUnit.Framework;

    using SharpArch.Testing.NUnit;

    using TfsWorkbench.VersionCheck.Properties;
    using TfsWorkbench.VersionCheck.Services;

    /// <summary>
    /// The web requstor factory test fixture class.
    /// </summary>
    [TestFixture]
    public class WebRequestReaderFactoryFixture
    {
        /// <summary>
        /// Create, returns default reader, with expected end point.
        /// </summary>
        [Test]
        public void Create_ReturnsDefaultInstance_WithExpectedEndPoint()
        {
            // Act
            var reader = new WebRequestReaderFactory().Create();

            // Assert
            reader.ShouldNotBeNull();
            reader.ShouldBeOfType(typeof(WebRequestReader));
            reader.EndPointUri.ShouldEqual(new Uri(Settings.Default.CheckUrl));
        }
    }
}
