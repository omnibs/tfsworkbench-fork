// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RelayCommandFixture.cs" company="None">
//   Crispin Parker 2011
// </copyright>
// <summary>
//   The relay command test fixture.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.VersionCheck.Tests
{
    using System;

    using NUnit.Framework;

    using SharpArch.Testing.NUnit;

    /// <summary>
    /// The relay command test fixture.
    /// </summary>
    [TestFixture]
    public class RelayCommandFixture
    {
        /// <summary>
        /// Calling the constructor, with a single null parameter, throws an exception.
        /// </summary>
        [Test]
        public void Constructor_WithSingleNullParameter_ThrowsException()
        {
            // Arrange
            Action<object> nullAction = null;

            // Act
            try
            {
                new RelayCommand(nullAction);
                Assert.Fail("Exception not thrown");
            }
            catch (ArgumentNullException)
            {
            }

            // Assert
            Assert.Pass();
        }

        /// <summary>
        /// Calling the constructor, with a single non null parameter, sets the execution action.
        /// </summary>
        [Test]
        public void Constructor_WithSingleNonNullParameter_SetsExecutionAction()
        {
            // Arrange
            var hasCalled = false;
            Action<object> action = o => hasCalled = true;

            // Act
            new RelayCommand(action).Execute(null);

            // Assert
            hasCalled.ShouldEqual(true);
        }

        /// <summary>
        /// Calling constructor, with a single non null parameter, can always execute.
        /// </summary>
        [Test]
        public void Constructor_WithSingleNonNullParameter_CanAlwaysExecute()
        {
            // Arrange

            // Act
            var canExecute = new RelayCommand(o => { }).CanExecute(null);

            // Assert
            canExecute.ShouldEqual(true);
        }

        /// <summary>
        /// Calling constructor, with non null second parameter, calls the specified predicate.
        /// </summary>
        [Test]
        public void Constructor_WithNonNullSecondParameter_CallsPredicateOnCanExecute()
        {
            // Arrange
            var hasCalled = false;

            Predicate<object> predicate = o =>
                {
                    hasCalled = true;
                    return true;
                };

            new RelayCommand(o => { }, predicate).CanExecute(null);

            // Assert
            hasCalled.ShouldEqual(true);
        }

        /// <summary>
        /// Can execute changed, when add or removed, attaches to requery suggest.
        /// </summary>
        [Test]
        public void CanExecuteChanged_WhenAddRemove_AttachesToRequerySuggest()
        {
            // Arrange
            var relayCommand = new RelayCommand(o => { });
            EventHandler canExecuteChanged = (s, e) => { };

            // Act
            relayCommand.CanExecuteChanged += canExecuteChanged;

            relayCommand.CanExecuteChanged -= canExecuteChanged;

            // Assert
            Assert.Pass();
        }
    }
}
