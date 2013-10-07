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
    /// The report viewer menu test fixture.
    /// </summary>
    [TestFixture]
    public class ReportViewerMenuTests
    {
        /// <summary>
        /// Test: Menu_should_require_valid_controller_instance.
        /// </summary>
        [Test]
        public void Menu_should_require_valid_controller_instance()
        {
            // Arrange
            var controller = MockRepository.GenerateMock<IReportController>();
            ReportViewerMenuItem menu;

            // Act
            try
            {
                menu = new ReportViewerMenuItem(null);
                Assert.Fail("Exception not thrown");
            }
            catch (ArgumentNullException)
            {
            }

            menu = new ReportViewerMenuItem(controller);

            // Assert
            menu.ShouldNotBeNull();
        }
    }
}
