namespace TfsWorkbench.Tests.WpfUiProjectSelector
{
    using System;
    using System.Linq;

    using NUnit.Framework;

    using Rhino.Mocks;

    using SharpArch.Testing.NUnit;

    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.Core.Properties;
    using TfsWorkbench.WpfUI.ProjectSelector;

    [TestFixture]
    public class LoaderWithVolumeCheckFixture
    {
        private IProjectData projectData;

        private IProjectSelectorService service;

        private LoaderWithVolumeCheck loaderUnderTest;

        [SetUp]
        public void SetUp()
        {
            this.projectData = ProjectDataGenerationHelper.GenerateProjectData();
            this.service = MockRepository.GenerateMock<IProjectSelectorService>();
        }

        [TearDown]
        public void TearDown()
        {
            this.projectData = null;
            this.service = null;
            this.loaderUnderTest = null;
        }

        [Test]
        public void Constructor_WithNullParameters_ThrowsException()
        {
            // Act
            try
            {
                new LoaderWithVolumeCheck(null, null);
                Assert.Fail("Exception not thrown");
            }
            catch (ArgumentNullException)
            {
            }

            try
            {
                new LoaderWithVolumeCheck(this.projectData, null);
                Assert.Fail("Exception not thrown");
            }
            catch (ArgumentNullException)
            {
            }

            try
            {
                new LoaderWithVolumeCheck(null, this.service);
                Assert.Fail("Exception not thrown");
            }
            catch (ArgumentNullException)
            {
            }

            // Assert
            Assert.Pass();
        }

        [Test]
        public void Constructor_WithValidParameters_SetsIgnoreWarningsFlag()
        {
            // Arrange
            const bool ExpectedIgnoreWarningsValue = true;
            this.SetProjectDataHideVolumeWarningTo(ExpectedIgnoreWarningsValue);

            // Act
            this.InitialiseLoader();

            // Assert
            this.loaderUnderTest.IgnoreFutureVolumeWarnings.ShouldEqual(ExpectedIgnoreWarningsValue);
        }

        [Test]
        public void Start_WhenIgnoreVolumeWarningsIsTrue_BeginsLoad()
        {
            // Arrange
            this.service
                .Expect(s => s.BeginLoad(null, null))
                .IgnoreArguments()
                .Repeat.Once();

            this.SetProjectDataHideVolumeWarningTo(true);

            this.InitialiseLoader();

            // Act
            this.loaderUnderTest.Start();

            // Assert
            this.service.VerifyAllExpectations();
        }

        [Test]
        public void Start_WhenIgnoreVolumeWarningsIsFalse_BeginsVolumeCheck()
        {
            // Arrange
            this.service
                .Expect(s => s.BeginVolumeCheck(null, null))
                .IgnoreArguments()
                .Repeat.Once();

            this.SetProjectDataHideVolumeWarningTo(false);

            this.InitialiseLoader();

            // Act
            this.loaderUnderTest.Start();

            // Assert
            this.service.VerifyAllExpectations();
        }

        [Test]
        public void Start_WhenVolumeCheckExceedsWarningLevel_RaisesDecissionEvent()
        {
            // Arrange
            var exceedingVolume = Settings.Default.VolumeWarningLevel + 1;
            var hasRaised = false;

            this.SetServiceVolumeResult(exceedingVolume);

            this.InitialiseLoader();

            this.loaderUnderTest.VolumeWarning += (s, e) => hasRaised = true; ;

            // Act
            this.loaderUnderTest.Start();

            // Assert
            hasRaised.ShouldBeTrue();
        }

        [Test]
        public void Start_WhenVolumeCheckDoeNotExceedsWarningLevel_BeginsLoad()
        {
            // Arrange
            var lowVolume = Settings.Default.VolumeWarningLevel - 1;

            this.SetServiceVolumeResult(lowVolume);
            this.service
                .Expect(s => s.BeginLoad(null, null))
                .IgnoreArguments()
                .Repeat.Once();

            this.InitialiseLoader();

            this.loaderUnderTest.VolumeWarning += (s, e) => { };

            // Act
            this.loaderUnderTest.Start();

            // Assert
            this.service.VerifyAllExpectations();
        }

        [Test]
        public void Start_WhenUserAborts_RaisesAbortedEvent()
        {
            // Arrange
            const bool AbortOnVolumeWarning = false;
            this.SetUserVolumeWarningResponse(AbortOnVolumeWarning);

            var hasAborted = false;
            this.loaderUnderTest.Aborted += (s, e) => hasAborted = true;

            // Act
            this.loaderUnderTest.Start();

            // Assert
            hasAborted.ShouldBeTrue();
        }

        [Test]
        public void Start_WhenUserIgnoresVolumeWarning_BeginsLoad()
        {
            // Arrange
            const bool IgnoreVolumeWarning = true;
            this.SetUserVolumeWarningResponse(IgnoreVolumeWarning);

            this.service
                .Expect(s => s.BeginLoad(null, null))
                .IgnoreArguments()
                .Repeat.Once();

            // Act
            this.loaderUnderTest.Start();

            // Assert
            this.service.VerifyAllExpectations();
        }

        [Test]
        public void Start_WhenIgnoreFurtureWarningsSet_AppliesUserResponse()
        {
            // Arrange
            const bool DoNotShowWarningAgain = true;
            this.SetUserVolumeWarningResponse(false, DoNotShowWarningAgain);

            // Act
            this.loaderUnderTest.Start();

            // Assert
            this.loaderUnderTest.IgnoreFutureVolumeWarnings.ShouldEqual(DoNotShowWarningAgain);
        }

        [Test]
        public void Start_WhenLoadingBegins_RaisesConfirmLoadDataEvent()
        {
            // Arrange
            this.service
                .Expect(s => s.BeginLoad(null, null))
                .IgnoreArguments()
                .Repeat.Once();

            this.SetProjectDataHideVolumeWarningTo(true);

            this.InitialiseLoader();

            var hasRaisedEvent = false;
            this.loaderUnderTest.ConfirmLoadData += (s, e) =>
            {
                hasRaisedEvent = true;
            };

            // Act
            this.loaderUnderTest.Start();

            // Assert
            this.service.VerifyAllExpectations();
            hasRaisedEvent.ShouldBeTrue();
        }

        [Test]
        public void Start_WhenConfirmLoadDataIsCancelled_DoesNotBeginLoading()
        {
            // Arrange
            this.service
                .Expect(s => s.BeginLoad(null, null))
                .IgnoreArguments()
                .Repeat.Never();

            this.SetProjectDataHideVolumeWarningTo(true);

            this.InitialiseLoader();

            this.loaderUnderTest.ConfirmLoadData += (s, e) => e.Cancel = true;

            // Act
            this.loaderUnderTest.Start();

            // Assert
            this.service.VerifyAllExpectations();
        }

        [Test]
        public void Start_WhenConfirmLoadDataIsCancelled_RasiesAbortEvent()
        {
            // Arrange
            this.SetProjectDataHideVolumeWarningTo(true);

            this.InitialiseLoader();

            this.loaderUnderTest.ConfirmLoadData += (s, e) => e.Cancel = true;

            var hasAborted = false;
            this.loaderUnderTest.Aborted += (s, e) => hasAborted = true;

            // Act
            this.loaderUnderTest.Start();

            // Assert
            hasAborted.ShouldBeTrue();
        }

        [Test]
        public void Start_WhenProjectLoaded_RaisesCompleteEvent()
        {
            // Arrange
            this.service
                .Expect(s => s.BeginLoad(null, null))
                .IgnoreArguments()
                .WhenCalled(mi => ((Action)mi.Arguments.ElementAt(1))())
                .Repeat.Any();

            this.SetProjectDataHideVolumeWarningTo(true);

            this.InitialiseLoader();

            var hasCompleted = false;
            this.loaderUnderTest.Complete += (s, e) =>
                {
                    hasCompleted = true;
                };

            // Act
            this.loaderUnderTest.Start();

            // Assert
            hasCompleted.ShouldBeTrue();
        }

        [Test]
        public void Dispose_WhenCalled_ExecutesWithoutException()
        {
            // Arrange
            var exceedingVolume = Settings.Default.VolumeWarningLevel + 1;
            this.SetServiceVolumeResult(exceedingVolume);
            this.InitialiseLoader();
            this.loaderUnderTest.Start();

            // Act
            this.loaderUnderTest.Dispose();

            // Assert
            Assert.Pass("No exception thrown.");
        }

        private void SetUserVolumeWarningResponse(bool userResponse, bool doNotShowAgain = false)
        {
            var exceedingVolume = Settings.Default.VolumeWarningLevel + 1;

            this.SetServiceVolumeResult(exceedingVolume);

            this.InitialiseLoader();

            this.loaderUnderTest.VolumeWarning += (s, e) =>
                {
                    e.Context.IsYes = userResponse;
                    e.Context.DoNotShowAgain = doNotShowAgain;
                    e.Context.OnDecisionMade();
                };
        }

        private void SetServiceVolumeResult(int volumeToReturn)
        {
            this.service
                .Expect(s => s.BeginVolumeCheck(null, null))
                .IgnoreArguments()
                .WhenCalled(mi => ((Action<int>)mi.Arguments.ElementAt(1))(volumeToReturn))
                .Repeat.Any();
        }

        private void InitialiseLoader()
        {
            this.loaderUnderTest = new LoaderWithVolumeCheck(this.projectData, this.service);
        }

        /// <summary>
        /// Sets the project data hide volume warning.
        /// </summary>
        /// <param name="expectwedHideWarningValue">if set to <c>true</c> [expectwed hide warning value].</param>
        private void SetProjectDataHideVolumeWarningTo(bool expectwedHideWarningValue)
        {
            this.projectData
                .Expect(pd => pd.HideVolumeWarning)
                .Return(expectwedHideWarningValue)
                .Repeat.Any();
        }
    }
}
