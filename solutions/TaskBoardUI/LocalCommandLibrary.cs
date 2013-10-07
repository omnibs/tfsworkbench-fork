// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalCommandLibrary.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the LocalCommandLibrary type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.TaskBoardUI
{
    using System.Windows.Input;

    /// <summary>
    /// The local command library class.
    /// </summary>
    internal class LocalCommandLibrary
    {
        /// <summary>
        /// The newWindow item command.
        /// </summary>
        private static readonly RoutedUICommand newWindowCommand =
            new RoutedUICommand("NewWindow", "newWindow", typeof(LocalCommandLibrary));

        /// <summary>
        /// The Reset command.
        /// </summary>
        private static readonly RoutedUICommand resetCommand =
            new RoutedUICommand("Reset", "reset", typeof(LocalCommandLibrary));

        /// <summary>
        /// Gets the Reset command.
        /// </summary>
        /// <value>The Reset command.</value>
        public static RoutedUICommand ResetCommand
        {
            get
            {
                return resetCommand;
            }
        }

        /// <summary>
        /// Gets the newWindow command.
        /// </summary>
        /// <value>The newWindow command.</value>
        public static RoutedUICommand NewWindowCommand
        {
            get
            {
                return newWindowCommand;
            }
        }
    }
}
