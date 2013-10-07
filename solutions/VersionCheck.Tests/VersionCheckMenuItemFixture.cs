// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VersionCheckMenuItemFixture.cs" company="None">
//   Crispin Parker 2011
// </copyright>
// <summary>
//   Defines the MenuOptionFixture type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.VersionCheck.Tests
{
    using NUnit.Framework;

    using SharpArch.Testing.NUnit;

    using TfsWorkbench.VersionCheck.Views;

    /// <summary>
    /// The menu option test fixture.
    /// </summary>
    [TestFixture]
    public class VersionCheckMenuItemFixture
    {
        /// <summary>
        /// Calling constructor, returns a new instance.
        /// </summary>
        [Test]
        public void Construtor_ReturnsNewInstance()
        {
            // Act
            var menuOption = new VersionCheckMenuItem();

            // Assert
            menuOption.ShouldNotBeNull();
        }
    }
}
