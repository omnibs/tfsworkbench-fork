using System.Windows;
using NUnit.Framework;
using TfsWorkbench.NotePadUI;
using TfsWorkbench.NotePadUI.Helpers;
using TfsWorkbench.NotePadUI.Models;

namespace TfsWorkbench.Tests
{
    [TestFixture]
    public class UIPadItemTemplateSelectorTests
    {
        [Test]
        public void When_constructing_then_no_exceptions_are_thrown()
        {
            // Arrange
            TestDelegate methodToTest = () => new UiPadItemTemplateSelector();

            // Act
            Assert.DoesNotThrow(methodToTest);

            // Assert
            Assert.Pass("Exception was not thrown");
        }

        [Test]
        public void When_selecting_template_with_null_then_null_is_returned()
        {
            // Arrange
            var tmpSelector = new UiPadItemTemplateSelector();
            PadItemBase itemCandidate = null;

            // Act
            var result = tmpSelector.SelectTemplate(itemCandidate, null);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void When_selecting_template_with_WorkbenchPadItem_then_expected_template_is_returned()
        {
            // Arrange
            var expectedTemplate = new DataTemplate();

            var tmpSelector = new UiPadItemTemplateSelector
                {
                    WorkbenchItemTemplate = expectedTemplate
                };

            var candidate = new WorkbenchPadItem();

            // Act
            var result = tmpSelector.SelectTemplate(candidate, null);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedTemplate, result);
        }

        [Test]
        public void When_selecting_template_with_StickyNotePadItem_then_expected_template_is_returned()
        {
            // Arrange
            var expectedTemplate = new DataTemplate();

            var tmpSelector = new UiPadItemTemplateSelector
            {
                StickyNoteTemplate = expectedTemplate
            };

            var candidate = new NotePadItem();

            // Act
            var result = tmpSelector.SelectTemplate(candidate, null);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(expectedTemplate, result);
        }
    }
}
