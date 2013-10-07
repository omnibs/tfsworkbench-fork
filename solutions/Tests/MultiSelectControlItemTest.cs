// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiSelectControlItemTest.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the MultiSelectControlItemTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Tests
{
    using System;
    using System.ComponentModel;
    using System.Windows.Input;

    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.ItemListUI;

    using NUnit.Framework;

    using Rhino.Mocks;

    using SharpArch.Testing.NUnit;

    /// <summary>
    /// The multi select control item test fixture.
    /// </summary>
    [TestFixture]
    public class MultiSelectControlItemTest
    {
        [Test]
        public void Control_should_indicate_when_it_is_update_source_item()
        {
            // Arrange
            var controlItem = MockRepository.GenerateMock<IControlItem>();
            var multiSelectControl = new MultiSelectControlItem(controlItem);
            var results = new System.Collections.Generic.List<bool>();
            Action raisePropertyChanged = () => controlItem.Raise(w => w.PropertyChanged += null, this, new PropertyChangedEventArgs("Value"));

            multiSelectControl.PropertyChanged += (s, e) => results.Add(multiSelectControl.IsValueUpdateSource);
            controlItem.Expect(ci => ci.Value = null).IgnoreArguments().WhenCalled(mi => raisePropertyChanged()).Repeat.Any();

            // Act
            controlItem.Value = "Value test 1";
            multiSelectControl.Value = "A value";

            // Assert
            results.Count.ShouldEqual(2);
            results[0].ShouldBeFalse();
            results[1].ShouldBeTrue();
        }

        [Test]
        public void Multicast_update_should_only_occur_when_multi_select_control_is_update_source()
        {
            // Arrange
            const string TestValue1 = "Test 1";

            var controlItemA = MockRepository.GenerateMock<IControlItem>();
            var controlItemB = MockRepository.GenerateMock<IControlItem>();
            var controlItemC = MockRepository.GenerateMock<IControlItem>();
            var multiSelectControlA = new MultiSelectControlItem(controlItemA);
            var multiSelectControlB = new MultiSelectControlItem(controlItemB);
            var multiSelectControlC = new MultiSelectControlItem(controlItemC);

            var multiSelectHelper = new MultiSelectHelper(new ItemList());

            multiSelectHelper.AddControlToSelection(multiSelectControlA);
            multiSelectHelper.AddControlToSelection(multiSelectControlB);
            multiSelectHelper.AddControlToSelection(multiSelectControlC);

            Action<IControlItem> raisePropertyChanged = c => c.Raise(w => w.PropertyChanged += null, this, new PropertyChangedEventArgs("Value"));

            controlItemA.Expect(ci => ci.Value = TestValue1).WhenCalled(mi => raisePropertyChanged(controlItemA)).Repeat.Twice();

            var updateCountB = 0;
            var updateCountC = 0;
            controlItemB.Expect(ci => ci.Value = null).IgnoreArguments().WhenCalled(mi => updateCountB++).Repeat.Any();
            controlItemC.Expect(ci => ci.Value = null).IgnoreArguments().WhenCalled(mi => updateCountC++).Repeat.Any();

            // Act
            controlItemA.Value = TestValue1;
            multiSelectControlA.Value = TestValue1;

            // Asert
            controlItemA.VerifyAllExpectations();
            updateCountB.ShouldEqual(1);
            updateCountC.ShouldEqual(1);
        }
    }
}
