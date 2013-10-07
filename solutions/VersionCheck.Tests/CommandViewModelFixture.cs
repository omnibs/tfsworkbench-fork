// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandViewModelFixture.cs" company="None">
//   Crispin Parker 2011
// </copyright>
// <summary>
//   Defines the CommandViewModelFixture type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.VersionCheck.Tests
{
    using System;
    using System.Windows.Input;

    using NUnit.Framework;

    using Rhino.Mocks;

    using SharpArch.Testing.NUnit;

    using TfsWorkbench.VersionCheck.ViewModels;

    /// <summary>
    /// The command view model test fixture class.
    /// </summary>
    [TestFixture]
    public class CommandViewModelFixture
    {
        /// <summary>
        /// Calling constructor, with null command, throws an exception.
        /// </summary>
        [Test]
        public void Constructor_WithNullCommand_ThrowsException()
        {
            // Arrange
            ICommand command = null;

            // Act
            try
            {
                new CommandViewModel(null, command);
                Assert.Fail("Exception not throw");
            }
            catch (ArgumentNullException)
            {
            }

            // Assert
            Assert.Pass();
        }

        /// <summary>
        /// Calling constructor, with valid parameters, sets initial values.
        /// </summary>
        [Test]
        public void Constructor_WithValidParameters_SetsInitialValues()
        {
            // Arrange
            const string DisplayName = "Display Message";
            var command = MockRepository.GenerateMock<ICommand>();

            // Act
            var viewModel = new CommandViewModel(DisplayName, command);

            // Assert
            viewModel.DisplayName.ShouldEqual(DisplayName);
            viewModel.Command.ShouldEqual(command);
        }
    }
}
