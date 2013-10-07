// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandViewModel.cs" company="None">
//   Crispin Parker 2011
// </copyright>
// <summary>
//   Defines the CommandViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.VersionCheck.ViewModels
{
    using System;
    using System.Windows.Input;

    /// <summary>
    /// The command view model class.
    /// </summary>
    public class CommandViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandViewModel"/> class.
        /// </summary>
        /// <param name="displayName">The display name.</param>
        /// <param name="command">The command.</param>
        public CommandViewModel(string displayName, ICommand command)
            : this(displayName)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            this.Command = command;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandViewModel"/> class.
        /// </summary>
        /// <param name="displayName">The display name.</param>
        public CommandViewModel(string displayName)
        {
            this.DisplayName = displayName;
        }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public string DisplayName { get; private set; }

        /// <summary>
        /// Gets the command.
        /// </summary>
        /// <value>The command.</value>
        public ICommand Command { get; private set; }
    }
}