// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatisticsViewerPluginTests.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the StatisticsViewerPluginTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Tests
{
    using System.Linq;

    using NUnit.Framework;

    using SharpArch.Testing.NUnit;

    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.StatisticsViewer;
    using TfsWorkbench.StatisticsViewer.Properties;

    /// <summary>
    /// The statistics viewer plugin test fixture class.
    /// </summary>
    [TestFixture]
    public class StatisticsViewerPluginTests
    {
        /// <summary>
        /// Test: Plugin_interface_should_be_available.
        /// </summary>
        [Test]
        public void Plugin_interface_should_be_available()
        {
            // Arrange
            var plugin = new StatisticsViewer.PluginInterface();

            // Act
            var result = typeof(IWorkbenchPlugin).IsAssignableFrom(plugin.GetType());

            // Assert
            result.ShouldBeTrue();
        }

        /// <summary>
        /// Test: Plugin_interface_should_provide_display_name_and_priority.
        /// </summary>
        [Test]
        public void Plugin_interface_should_provide_display_name_and_priority()
        {
            // Arrange
            var plugin = new StatisticsViewer.PluginInterface();

            // Act
            var resultA = plugin.DisplayName;
            var resultB = plugin.DisplayPriority;

            // Assert
            resultA.ShouldNotBeNull();
            resultA.ShouldEqual(Settings.Default.DisplayName);
            resultB.ShouldBeGreaterThan(0);
            resultB.ShouldEqual(Settings.Default.DisplayPriority);
        }

        /// <summary>
        /// Test: Plugin_interface_should_provide_menu_item.
        /// </summary>
        [Test]
        public void Plugin_interface_should_provide_menu_item()
        {
            // Arrange
            var plugin = new StatisticsViewer.PluginInterface();

            // Act
            var result = plugin.MenuItem;

            // Assert
            result.ShouldNotBeNull();
        }

        /// <summary>
        /// Test: Plugin_interface_should_provide_control_item.
        /// </summary>
        [Test]
        public void Plugin_interface_should_provide_control_item()
        {
            // Arrange
            var plugin = new StatisticsViewer.PluginInterface();

            // Act
            var result = plugin.ControlElement;

            // Assert
            result.ShouldNotBeNull();
        }

        /// <summary>
        /// Test: Plugin_interface_should_provide_command_bindings.
        /// </summary>
        [Test]
        public void Plugin_interface_should_provide_command_bindings()
        {
            // Arrange
            var plugin = new StatisticsViewer.PluginInterface();

            // Act
            var result = plugin.CommandBindings;

            // Assert
            result.ShouldNotBeNull();
        }

        /// <summary>
        /// Test: Plugin_interface_should_provide_show_statistics_command_binding.
        /// </summary>
        [Test]
        public void Plugin_interface_should_provide_show_statistics_command_binding()
        {
            // Arrange
            var plugin = new PluginInterface();

            // Act
            var result = plugin.CommandBindings;

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBeGreaterThan(0);

            result.Any(cb => Equals(cb.Command, LocalCommandLibrary.ShowStatisticsViewerCommand)).ShouldBeTrue();
        }
    }
}
