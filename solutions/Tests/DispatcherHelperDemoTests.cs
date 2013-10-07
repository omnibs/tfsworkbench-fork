// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DispatcherHelperDemoTests.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the DispatcherHelperDemo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Tests
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows.Controls;
    using System.Windows.Threading;

    using NUnit.Framework;

    using Rhino.Mocks;

    using SharpArch.Testing.NUnit;

    using TfsWorkbench.Tests.Helpers;

    /// <summary>
    /// A demo repository interface.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Justification = "Reviewed. Suppression is OK here.")]
    public interface IRepository
    {
        /// <summary>
        /// Begins the load.
        /// </summary>
        void BeginLoad();
    }

    /// <summary>
    /// The dispatcher helper demo test fixture.
    /// </summary>
    [TestFixture]
    public class DispatcherHelperDemoTests
    {
        /// <summary>
        /// Test: Calling_a_dispatcher_helper_non_return_method.
        /// </summary>
        ///[Test]
        public void Calling_a_dispatcher_helper_non_return_method()
        {
            // Arrange
            var hasBeganLoading = false;

            Action test = () =>
                {
                    var testRepository = MockRepository.GenerateMock<IRepository>();

                    testRepository.Expect(cr => cr.BeginLoad()).WhenCalled(mi => hasBeganLoading = true);

                    var control = new MyUserControl();
                    control.BeginLoad(testRepository);
                };

            // Act
            DispatcherHelper.ExecuteOnDispatcherThread(test);

            // Assert
            hasBeganLoading.ShouldBeTrue();
        }

        /// <summary>
        /// Test: Calling_a_dispatcher_helper_method_with_return_value.
        /// </summary>
        ///[Test]
        public void Calling_a_dispatcher_helper_method_with_return_value()
        {
            // Arrange
            var hasCalledRepositoryLoad = false;

            Func<bool> test = () =>
                {
                    var testRepository = MockRepository.GenerateMock<IRepository>();

                    testRepository.Expect(cr => cr.BeginLoad()).WhenCalled(mi => hasCalledRepositoryLoad = true);

                    var control = new MyUserControl();
                    control.BeginLoad(testRepository);

                    return control.HasStartedLoad;
                };

            // Act
            var hasStartedLoading = DispatcherHelper.ExecuteOnDispatcherThread(test);

            // Assert
            hasStartedLoading.ShouldBeTrue();
            hasCalledRepositoryLoad.ShouldBeTrue();
        }
    }

    /// <summary>
    /// The test user control class.
    /// </summary>
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass",
        Justification = "Reviewed. Suppression is OK here.")]
    public class MyUserControl : UserControl
    {
        /// <summary>
        /// Gets a value indicating whether this instance has started load.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has started load; otherwise, <c>false</c>.
        /// </value>
        public bool HasStartedLoad { get; private set; }

        /// <summary>
        /// Begins the load.
        /// </summary>
        /// <param name="customerRepository">The customer repository.</param>
        public void BeginLoad(IRepository customerRepository)
        {
            EventHandler callback = (s, e) =>
                {
                    ((DispatcherTimer)s).Stop();
                    customerRepository.BeginLoad();
                };

            var dispatcherTimer = new DispatcherTimer(
                TimeSpan.FromSeconds(1), DispatcherPriority.Normal, callback, Dispatcher.CurrentDispatcher);

            dispatcherTimer.Start();

            this.HasStartedLoad = true;
        }
    }
}