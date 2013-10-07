// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PluginInterfaceFixture.cs" company="None">
//   Crispin Parker 2011
// </copyright>
// <summary>
//   Defines the PluginInterfaceFixture type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.VersionCheck.Tests
{
    using System.Linq;

    using NUnit.Framework;

    using Properties;

    using Rhino.Mocks;

    using SharpArch.Testing.NUnit;

    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.Core.Services;
    using TfsWorkbench.VersionCheck.Iterfaces;
    using TfsWorkbench.VersionCheck.Services;
    using TfsWorkbench.VersionCheck.Views;

    /// <summary>
    /// The plug in interface test fixture.
    /// </summary>
    [TestFixture]
    public class PluginInterfaceFixture
    {
        /// <summary>
        /// The main view model.
        /// </summary>
        private IMainViewModel mainViewModel;

        /// <summary>
        /// Sets up the test environment.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            var serviceManager = MockRepository.GenerateMock<IServiceManager>();

            this.mainViewModel = MockRepository.GenerateMock<IMainViewModel>();

            serviceManager
                .Expect(sm => sm.GetService<IMainViewModel>())
                .Return(this.mainViewModel)
                .Repeat.Any();

            ServiceManager.Instance = serviceManager;
        }

        /// <summary>
        /// Tears down the test envrionment.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            ServiceManager.Instance = null;
        }

        /// <summary>
        /// Command bindings returns a non null empty array.
        /// </summary>
        [Test]
        public void CommandBindings_ReturnsNonNullEmptyArray()
        {
            // Arrange
            var pluginInterface = GetPluginInterface();

            // Act
            var commands = pluginInterface.CommandBindings;

            // Assert
            commands.ShouldNotBeNull();
            commands.Any().ShouldBeFalse();
        }

        /// <summary>
        /// Control element, returns version control button.
        /// </summary>
        [Test]
        public void ControlElement_ReturnsVersionControlButton()
        {
            // Arrange
            var pluginInterface = GetPluginInterface();

            // Act
            var controlElement = pluginInterface.ControlElement;

            // Assert
            controlElement.ShouldNotBeNull();
            controlElement.ShouldBeOfType(typeof(VersionCheckButton));
        }

        /// <summary>
        /// Display name, returns expected text.
        /// </summary>
        [Test]
        public void DisplayName_ReturnsExpectedText()
        {
            // Arrange
            var pluginInterface = GetPluginInterface();

            // Act
            var displayName = pluginInterface.DisplayName;

            // Assert
            displayName.ShouldNotBeNull();
            displayName.ShouldEqual(Resources.String001);
        }

        /// <summary>
        /// Display priority, returns expected number.
        /// </summary>
        [Test]
        public void DisplayPriority_ReturnsExpectedNumber()
        {
            // Arrange
            var pluginInterface = GetPluginInterface();

            // Act
            var displayPriority = pluginInterface.DisplayPriority;

            // Assert
            displayPriority.ShouldEqual(Settings.Default.DisplayPriority);
        }

        /// <summary>
        /// Menu item, on first call, intilises the menu item.
        /// </summary>
        [Test]
        public void MenuItem_OnFirstCall_InitialisesMenuItem()
        {
            // Arrange
            var pluginInterface = GetPluginInterface();

            // Act
            var menuItemA = pluginInterface.MenuItem;
            var menuItemB = pluginInterface.MenuItem;

            // Assert
            menuItemA.ShouldNotBeNull();
            menuItemA.ShouldEqual(menuItemB);
        }

        /// <summary>
        /// Menu item, is of expected type.
        /// </summary>
        [Test]
        public void MenuItem_IsExpectedType()
        {
            // Arrange
            var pluginInterface = GetPluginInterface();

            // Act
            var menuItem = pluginInterface.MenuItem;

            // Assert
            menuItem.ShouldBeOfType(typeof(VersionCheckMenuItem));
        }

        /// <summary>
        /// Menu item, on first call and check on start up is true, performs version check.
        /// </summary>
        [Test]
        public void MenuItem_OnFirstCallAndCheckOnStartUpIsTrue_PerformsVersionCheck()
        {
            // Arrange
            var pluginInterface = GetPluginInterface();

            this.mainViewModel
                .Expect(mvm => mvm.CheckVersionOnStartUp)
                .Return(true)
                .Repeat.Once();

            this.mainViewModel
                .Expect(mvm => mvm.ExecuteVersionCheck())
                .Repeat.Once();

            // Act
            Assert.IsNotNull(pluginInterface.MenuItem);

            // Assert
            this.mainViewModel.VerifyAllExpectations();
        }

        /// <summary>
        /// Menu item, on first call and check on start up is false, does not perform version check.
        /// </summary>
        [Test]
        public void MenuItem_OnFirstCallAndCheckOnStartUpIsFalse_DoesNotPerformVersionCheck()
        {
            // Arrange
            var pluginInterface = GetPluginInterface();

            this.mainViewModel
                .Expect(mvm => mvm.CheckVersionOnStartUp)
                .Return(false)
                .Repeat.Once();

            this.mainViewModel
                .Expect(mvm => mvm.ExecuteVersionCheck())
                .Repeat.Never();

            // Act
            Assert.IsNotNull(pluginInterface.MenuItem);

            // Assert
            this.mainViewModel.VerifyAllExpectations();
        }

        /// <summary>
        /// Gets the plugin interface.
        /// </summary>
        /// <returns>A new instance of the plugin interface type.</returns>
        private static PluginInterface GetPluginInterface()
        {
            return new PluginInterface();
        }
    }
}
