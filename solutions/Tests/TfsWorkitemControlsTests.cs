using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using Microsoft.TeamFoundation.WorkItemTracking.WpfControls;
using NUnit.Framework;

namespace TfsWorkbench.Tests
{
    [TestFixture]
    public class TfsWorkitemControlsTests
    {
        [Test]
        public void When_creating_control_then_expected_control_type_is_returned()
        {
            // Arrange
            var controlFactory = new WpfControlFactoryXXX();

            // Act
            var fieldName = "Scrum.v3.BusinessValue";
            var controlType = "FieldControl";

            var result = controlFactory.CreateControl(fieldName, controlType);

            // Assert

        }
    }

    public class WpfControlFactoryXXX
    {
        public FrameworkElement CreateControl(string fieldName, string controlType)
        {
            var assembly = typeof (WorkItemControl).Assembly;

            var type = assembly.GetType("Microsoft.TeamFoundation.WorkItemTracking.WpfControls.WpfControlFactory");

            var instance = Activator.CreateInstance(type);

            var methodInfo = type.GetMethods(BindingFlags.Instance | BindingFlags.Public).First(mi => mi.Name == "CreateControl");

            var result = methodInfo.Invoke(instance, new object[] {fieldName, controlType});

            var resultType = result.GetType();

            var propInfo = resultType.GetProperty("WorkItemDatasource");

            propInfo.SetValue(result, null);

            return result as FrameworkElement;
        }
    }
}
