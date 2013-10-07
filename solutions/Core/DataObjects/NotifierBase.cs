// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotifierBase.cs" company="None">
//   None
// </copyright>
// <summary>
//   Initializes instance of NotifierBase
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.DataObjects
{
    using System.ComponentModel;

    /// <summary>
    /// Initializes instance of NotifierBase
    /// </summary>
    public abstract class NotifierBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        protected void OnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (this.PropertyChanged == null)
            {
                return;
            }

            this.PropertyChanged(sender, args);
        }

        /// <summary>
        /// Updates the with notification.
        /// </summary>
        /// <typeparam name="T">The field type.</typeparam>
        /// <param name="propertyTitle">The property title.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="targetField">The target field.</param>
        protected void UpdateWithNotification<T>(string propertyTitle, T newValue, ref T targetField)
        {
            if (Equals(targetField, newValue))
            {
                return;
            }

            targetField = newValue;

            this.OnPropertyChanged(this, new PropertyChangedEventArgs(propertyTitle));
        }
    }
}
