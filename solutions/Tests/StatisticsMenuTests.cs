// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatisticsMenuTests.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the StatisticsMenuTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Tests
{
    using NUnit.Framework;

    using SharpArch.Testing.NUnit;

    using TfsWorkbench.StatisticsViewer;

    /// <summary>
    /// The statistics menu test fixture class.
    /// </summary>
    [TestFixture]
    public class StatisticsMenuTests
    {
        /// <summary>
        /// Test: Menu_should_expose_the_show_statistics_command.
        /// </summary>
        [Test]
        public void Menu_should_expose_the_show_statistics_command()
        {
            // Arrange
            var menu = new StatisticsViewerMenuItem();
            
            // Assert
            var command = menu.Command;

            // Act
            command.ShouldEqual(LocalCommandLibrary.ShowStatisticsViewerCommand);
        }
    }
}
