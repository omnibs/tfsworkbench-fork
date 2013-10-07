// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetupDialogBase.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the SetupDialogBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.ProjectSetupUI
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Threading;

    using DataObjects;

    using UIElements;

    /// <summary>
    /// Initializes instance of SetupDialogBase
    /// </summary>
    /// <remarks>
    /// This class cannot be made abstract because WPF does not allow naming of abstract classes
    /// when used as immediate controls.
    /// </remarks>
    public class SetupDialogBase : UserControl
    {
        /// <summary>
        /// Occurs when [apply quick start].
        /// </summary>
        internal virtual event EventHandler<ProjectSetupEventArgs> ApplySetup;

        /// <summary>
        /// Gets or sets the project setup.
        /// </summary>
        /// <value>The project setup.</value>
        internal virtual ProjectSetup ProjectSetup { get; set; }

        /// <summary>
        /// Called when [project setup changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        protected static void OnProjectSetupChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var control = dependencyObject as SetupDialogBase;
            if (control == null)
            {
                return;
            }

            var oldSetup = e.OldValue as ProjectSetup;
            var newSetup = e.NewValue as ProjectSetup;

            if (oldSetup != null)
            {
                oldSetup.PropertyChanged -= control.OnSetupPropertyChanged;
            }

            if (newSetup != null)
            {
                newSetup.PropertyChanged += control.OnSetupPropertyChanged;
            }
        }

        /// <summary>
        /// Executes the setup.
        /// </summary>
        protected void ExecuteSetup()
        {
            if (!this.IsValid() || this.ApplySetup == null)
            {
                return;
            }

            var message = string.Concat(
                "Applying these setup parameters will delete any conflicting project data, including work items and iteration paths.",
                Environment.NewLine,
                Environment.NewLine,
                "Delete any existing conflicts?");

            if (MessageBox.Show(
                message,
                "Discard Conflicting Data",
                MessageBoxButton.OKCancel,
                MessageBoxImage.Warning) == MessageBoxResult.Cancel)
            {
                return;
            }

            this.IsEnabled = false;

            var callback = (SendOrPostCallback)delegate
            {
                this.ApplySetup(this, new ProjectSetupEventArgs(this.ProjectSetup));
                CommandLibrary.ApplicationMessageCommand.Execute("Project setup applied", this);
            };

            this.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, callback, null);

            CommandLibrary.ApplicationMessageCommand.Execute("Applying project setup...", this);
        }

        /// <summary>
        /// Determines whether this instance is valid.
        /// </summary>
        /// <returns>
        /// <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool IsValid()
        {
            return false;
        }

        /// <summary>
        /// Called when [setup property changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected virtual void OnSetupPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}