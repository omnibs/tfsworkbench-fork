// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalCommandLibrary.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the LocalCommandLibrary type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows.Input;

namespace TfsWorkbench.NotePadUI
{
    /// <summary>
    /// The local command library class.
    /// </summary>
    internal static class LocalCommandLibrary
    {
        public static readonly RoutedUICommand deletePadItemCommand =
            new RoutedUICommand("Delete", "deletePadItem", typeof(LocalCommandLibrary));

        public static readonly RoutedUICommand changeColourCommand =
            new RoutedUICommand("Change Colour", "changeColour", typeof(LocalCommandLibrary));

        public static readonly RoutedUICommand saveLayoutCommand =
            new RoutedUICommand("Save Layout", "saveLayout", typeof(LocalCommandLibrary));

        public static RoutedUICommand SaveLayoutCommand
        {
            get
            {
                return saveLayoutCommand;
            }
        }

        public static RoutedUICommand DeletePadItemCommand
        {
            get
            {
                return deletePadItemCommand;
            }
        }

        public static RoutedUICommand ChangeColourCommand
        {
            get
            {
                return changeColourCommand;
            }
        }
    }
}
