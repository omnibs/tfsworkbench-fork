// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VersionStatusFixture.cs" company="None">
//   Crispin Parker 2011
// </copyright>
// <summary>
//   Defines the VersionStatusFixture type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.VersionCheck.Tests
{
    using NUnit.Framework;

    using SharpArch.Testing.NUnit;

    using TfsWorkbench.VersionCheck.Models;
    using TfsWorkbench.VersionCheck.Properties;

    /// <summary>
    /// The version status test fixture.
    /// </summary>
    [TestFixture]
    public class VersionStatusFixture
    {
        /// <summary>
        /// Constructing, Failed to check version status, returns expected status details.
        /// </summary>
        [Test]
        public void Constructing_FailedToCheckVersionStatus_ReturnsExpectedStatusDetails()
        {
            // Arrange
            const string ErrorText = "Test";
            var versionStatus = new FailedToCheckVersionStatus(ErrorText);

            // Act
            var status = versionStatus.Status;
            var message = versionStatus.DisplayMessage;

            // Assert
            status.ShouldEqual(VersionStatusOption.Unknown);
            message.ShouldEqual(string.Concat(Resources.String006, ErrorText));
        }

        /// <summary>
        /// Constructing, Out of date version status, returns expected status details.
        /// </summary>
        [Test]
        public void Constructing_OutOfDateVersionStatus_ReturnsExpectedStatusDetails()
        {
            // Arrange
            var versionStatus = new OutOfDateVersionStatus();

            // Act
            var status = versionStatus.Status;
            var message = versionStatus.DisplayMessage;

            // Assert
            status.ShouldEqual(VersionStatusOption.OutDated);
            message.ShouldEqual(Resources.String005);
        }

        /// <summary>
        /// Constructing, Unknown version status, returns expected status details.
        /// </summary>
        [Test]
        public void Constructing_UnknownVersionStatus_ReturnsExpectedStatusDetails()
        {
            // Arrange
            var versionStatus = new UnknownVersionStatus();

            // Act
            var status = versionStatus.Status;
            var message = versionStatus.DisplayMessage;

            // Assert
            status.ShouldEqual(VersionStatusOption.Unknown);
            message.ShouldEqual(Resources.String003);
        }

        /// <summary>
        /// Constructing, upto date version status, returns expected status details.
        /// </summary>
        [Test]
        public void Constructing_UptoDateVersionStatus_ReturnsExpectedStatusDetails()
        {
            // Arrange
            var versionStatus = new UptoDateVersionStatus();

            // Act
            var status = versionStatus.Status;
            var message = versionStatus.DisplayMessage;

            // Assert
            status.ShouldEqual(VersionStatusOption.UpToDate);
            message.ShouldEqual(Resources.String004);
        }
    }
}
