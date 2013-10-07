// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RelayCommand.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the RelayCommand type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.UIElements
{
    using System;
    using System.Windows.Input;

    /// <summary>
    /// The relay command class.
    /// </summary>
    public class RelayCommand : ICommand
    {
        /// <summary>
        /// The command execution method.
        /// </summary>
        private readonly Action<object> onExecute;

        /// <summary>
        /// The command can execute predicate.
        /// </summary>
        private readonly Predicate<object> canExecute;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="onExecute">The on execute.</param>
        public RelayCommand(Action<object> onExecute)
            : this(onExecute, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="onExecute">The on execute.</param>
        /// <param name="canExecute">The can execute.</param>
        public RelayCommand(Action<object> onExecute, Predicate<object> canExecute)
        {
            if (onExecute == null)
            {
                throw new ArgumentNullException("onExecute");
            }

            this.onExecute = onExecute;
            this.canExecute = canExecute;
        }

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        /// <summary>
        /// Executes the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        public void Execute(object context)
        {
            this.onExecute(context);
        }

        /// <summary>
        /// Defines the method that determines whether the command can execute in its current state.
        /// </summary>
        /// <returns>
        /// true if this command can be executed; otherwise, false.
        /// </returns>
        /// <param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
        public bool CanExecute(object parameter)
        {
            return this.canExecute == null ? true : this.canExecute(parameter);
        }
    }
}