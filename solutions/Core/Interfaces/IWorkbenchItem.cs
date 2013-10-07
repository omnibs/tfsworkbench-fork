// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWorkbenchItem.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the IWorkbenchItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    using EventArgObjects;

    /// <summary>
    /// The item base interface.
    /// </summary>
    public interface IWorkbenchItem : INotifyPropertyChanged 
    {
        /// <summary>
        /// Occurs when the [state changed].
        /// </summary>
        event EventHandler<ItemStateChangeEventArgs> StateChanged;

        /// <summary>
        /// Gets the value provider.
        /// </summary>
        /// <value>The value provider.</value>
        IValueProvider ValueProvider { get; }

        /// <summary>
        /// Gets the children collection.
        /// </summary>
        /// <value>The children.</value>
        IEnumerable<ILinkItem> ChildLinks { get; }

        /// <summary>
        /// Gets links where this item is the child.
        /// </summary>
        /// <value>The parent links.</value>
        IEnumerable<ILinkItem> ParentLinks { get; }

        /// <summary>
        /// Gets the display names.
        /// </summary>
        /// <value>The display names.</value>
        IIndexer DisplayNames { get; }

        /// <summary>
        /// Gets the allowed values.
        /// </summary>
        /// <value>The allowed values.</value>
        IIndexer AllowedValues { get; }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> at the specified fieldName.
        /// </summary>
        /// <param name="fieldName">The field name</param>
        object this[string fieldName]
        {
            get; set;
        }

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        void OnPropertyChanged();

        /// <summary>
        /// Releases the resources.
        /// </summary>
        void ReleaseResources();
    }
}