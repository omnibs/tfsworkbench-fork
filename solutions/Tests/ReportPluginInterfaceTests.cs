// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReportServiceInterfaceTests.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ReportServiceTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Tests
{
    using NUnit.Framework;

    using SharpArch.Testing.NUnit;

    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.ReportViewer;

    /// <summary>
    /// The resport service test fixture class.
    /// </summary>
    [TestFixture]
    public class ReportPluginInterfaceTests
    {
        /// <summary>
        /// Test: Report_plugin_should_implement_workbench_plugin_interface.
        /// </summary>
        [Test]
        public void Report_plugin_should_implement_workbench_plugin_interface()
        {
            // Arrange

            // Act
            var result = typeof(IWorkbenchPlugin).IsAssignableFrom(typeof(PluginInterface));

            // Assert
            result.ShouldBeTrue();
        }

        /// <summary>
        /// Test: Report_plugin_interface_should_provide_a_menu_item.
        /// </summary>
        [Test]
        public void Report_plugin_interface_should_provide_a_menu_item()
        {
            // Arrange
            var service = new PluginInterface();

            // Act
            var result = service.MenuItem;

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType(typeof(ReportViewerMenuItem));
        }

        /// <summary>
        /// Test: Report_plugin_interface_should_provide_a_control_element.
        /// </summary>
        [Test]
        public void Report_plugin_interface_should_provide_a_control_element()
        {
            // Arrange
            var plugin = new PluginInterface();

            // Act
            var result = plugin.ControlElement;

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeOfType(typeof(ReportViewerButton));
        }

        /// <summary>
        /// Test: Report_plugin_interface_should_provide_a_positive_control_element_index.
        /// </summary>
        [Test]
        public void Report_plugin_interface_should_provide_a_positive_control_element_index()
        {
            // Arrange
            var plugin = new PluginInterface();

            // Act
            var result = plugin.DisplayPriority;

            // Assert
            result.ShouldBeGreaterThan(0);
        }

        [Test]
        public void Report_plugin_interface_should_provide_display_name()
        {
            // Arrange
            var plugin = new PluginInterface();

            // Act
            var result = plugin.DisplayName;

            // Assert
            result.ShouldNotBeNull();
            result.ShouldEqual(ReportViewer.Properties.Settings.Default.PluginName);
        }
    }
}
