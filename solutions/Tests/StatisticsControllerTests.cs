// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StatisticsControllerTests.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the StatisticsControllerTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Tests
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using NUnit.Framework;

    using Rhino.Mocks;

    using SharpArch.Testing.NUnit;

    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.StatisticsViewer;
    using TfsWorkbench.Tests.Helpers;
    using TfsWorkbench.UIElements;

    /// <summary>
    /// The statistics controller test fixzture class.
    /// </summary>
    [TestFixture]
    public class StatisticsControllerTests
    {
        private IProjectData projectData;

        private UIElement button;

        private IStatisticsController controllerUnderTest;

        /// <summary>
        /// Sets up the test environment.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            this.GenerateProjectDataService();
            this.controllerUnderTest = StatisticsController.Instance;
            this.button = new Button();
        }

        /// <summary>
        /// Tears down the test environment.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            this.projectData = null;
            if (this.button != null)
            {
                this.button.CommandBindings.Clear();
                this.button = null;
            }
            this.controllerUnderTest = null;

            StatisticsController.Instance = null;
            ServiceManagerHelper.ClearDummyManager();
        }

        /// <summary>
        /// Test: Controller_should_provide_default_singleton_instance.
        /// </summary>
        [Test]
        public void Controller_should_provide_default_singleton_instance()
        {
            // Assert
            this.controllerUnderTest.ShouldNotBeNull();
            this.controllerUnderTest.ShouldBeOfType(typeof(StatisticsController));
        }

        /// <summary>
        /// Test: Controller_should_provide_show_statistics_command_handler.
        /// </summary>
        [Test]
        public void Controller_should_provide_show_statistics_command_handlers()
        {
            // Act
            new CommandBinding(
                LocalCommandLibrary.ShowStatisticsViewerCommand,
                this.controllerUnderTest.OnShowStatistics,
                this.controllerUnderTest.OnCanExecute);

            // Assert
            Assert.Pass();
        }

        /// <summary>
        /// Test: Controller_should_show_statistics_dialog.
        /// </summary>
        [Test]
        public void Controller_should_show_statistics_dialog()
        {
            // Arrange
            this.projectData = DataObjectHelper.GenerateProjectData();

            var hasRaised = false;
            UIElement dialogInstance = null;

            ExecutedRoutedEventHandler onShowDialog = (s, e) =>
                {
                    hasRaised = true;
                    dialogInstance = e.Parameter as UIElement;
                };

            this.button.CommandBindings.Add(
                new CommandBinding(CommandLibrary.ShowDialogCommand, onShowDialog, (s, e) => e.CanExecute = true));

            // Act
            this.controllerUnderTest.OnShowStatistics(this.button, null);

            // Assert
            hasRaised.ShouldBeTrue();
            dialogInstance.ShouldNotBeNull();
        }

        [Test]
        public void Controller_should_indicate_validity()
        {
            // Arrange
            this.GenerateProjectDataService();

            this.button.CommandBindings.Add(
                new CommandBinding(LocalCommandLibrary.ShowStatisticsViewerCommand, (s, e) => { }, this.controllerUnderTest.OnCanExecute));

            // Act
            this.projectData = null;
            var canExecuteA = LocalCommandLibrary.ShowStatisticsViewerCommand.CanExecute(null, this.button);

            this.projectData = DataObjectHelper.CreateProjectData();
            var canExecuteB = LocalCommandLibrary.ShowStatisticsViewerCommand.CanExecute(null, this.button);

            // Assert
            canExecuteA.ShouldBeFalse();
            canExecuteB.ShouldBeTrue();
        }

        [Test]
        public void Controller_should_provide_list_of_statistics_groups()
        {
            // Act
            if (this.controllerUnderTest == null)
            {
                throw new NullReferenceException("The controller is null");
            }

            var statisticsGroups = this.controllerUnderTest.StatisticPages;

            // Assert
            statisticsGroups.ShouldNotBeNull();
        }

        /// <summary>
        /// Generates the project data service.
        /// </summary>
        private void GenerateProjectDataService()
        {
            var projectDataService = MockRepository.GenerateMock<IProjectDataService>();

            projectDataService
                .Expect(pds => pds.CurrentProjectData)
                .WhenCalled(mi => mi.ReturnValue = this.projectData)
                .Return(null)
                .Repeat.Any();

            ServiceManagerHelper.MockServiceManager(projectDataService);
        }
    }
}
