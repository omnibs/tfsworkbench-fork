// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReportViewerMenuTests.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ReportViewerMenuTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Tests
{
    using System;

    using NUnit.Framework;

    using Rhino.Mocks;

    using SharpArch.Testing.NUnit;

    using TfsWorkbench.ReportViewer;

    /// <summary>
    /// The report viewer button test fixture.
    /// </summary>
    [TestFixture]
    public class ReportViewerButtonTests
    {
        /// <summary>
        /// Test: Button_should_require_valid_controller_instance.
        /// </summary>
        [Test]
        public void Button_should_require_valid_controller_instance()
        {
            // Arrange
            var controller = MockRepository.GenerateMock<IReportController>();
            ReportViewerButton button;

            // Act
            try
            {
                button = new ReportViewerButton(null);
                Assert.Fail("Exception not thrown");
            }
            catch (ArgumentNullException)
            {
            }

            button = new ReportViewerButton(controller);

            // Assert
            button.ShouldNotBeNull();
        }
    }
}
