// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkbenchItem.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the WorkbenchItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.DataObjects
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    using EventArgObjects;

    using Helpers;

    using Interfaces;

    /// <summary>
    /// Initializes instance of WorkbenchItem
    /// </summary>
    internal class WorkbenchItem : IWorkbenchItem
    {
        /// <summary>
        /// The internal child collection
        /// </summary>
        private readonly ICollection<ILinkItem> links = new Collection<ILinkItem>();

        /// <summary>
        /// The display names indexer.
        /// </summary>
        private readonly IIndexer displayNames;

        /// <summary>
        /// The allowed values indexer.
        /// </summary>
        private readonly IIndexer allowedValues;

        /// <summary>
        /// The value provider instance.
        /// </summary>
        private IValueProvider valueProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkbenchItem"/> class.
        /// </summary>
        public WorkbenchItem()
        {
            this.displayNames = new Indexer(s => this.ValueProvider.GetDisplayName(s));
            this.allowedValues = new Indexer(s => this.ValueProvider.GetAllowedValues(s));
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs when the [state changed].
        /// </summary>
        public event EventHandler<ItemStateChangeEventArgs> StateChanged;

        /// <summary>
        /// Gets or sets the value provider.
        /// </summary>
        /// <value>The value provider.</value>
        public IValueProvider ValueProvider
        {
            get
            {
                return this.valueProvider;
            }

            set
            {
                if (this.valueProvider == value)
                {
                    return;
                }

                if (this.valueProvider != null)
                {
                    this.valueProvider.StateChanged -= this.OnStateChanged;
                }

                this.valueProvider = value;
                
                if (this.valueProvider != null)
                {
                    this.valueProvider.StateChanged += this.OnStateChanged;
                }
            }
        }

        /// <summary>
        /// Gets the child links collection.
        /// </summary>
        /// <value>The links.</value>
        public IEnumerable<ILinkItem> ChildLinks
        {
            get { return this.GetChildLinks(); }
        }

        /// <summary>
        /// Gets links where this item is the child.
        /// </summary>
        /// <value>The parenet links.</value>
        public IEnumerable<ILinkItem> ParentLinks
        {
            get { return this.GetParentLinks(); }
        }

        /// <summary>
        /// Gets the display names.
        /// </summary>
        /// <value>The display names.</value>
        public IIndexer DisplayNames
        {
            get { return this.displayNames; }
        }

        /// <summary>
        /// Gets the allowed values.
        /// </summary>
        /// <value>The allowed values.</value>
        public IIndexer AllowedValues
        {
            get { return this.allowedValues; }
        }

        /// <summary>
        /// Gets or sets the <see cref="System.Object"/> at the specified fieldName.
        /// </summary>
        /// <param name="fieldName">The field name</param>
        public object this[string fieldName]
        {
            get
            {
                return this.ValueProvider == null ? null : this.ValueProvider.GetValue(fieldName);
            }

            set
            {
                if (this.ValueProvider == null)
                {
                    return;
                }

                var oldValue = this.ValueProvider.GetValue(fieldName);

                if (Equals(oldValue, value))
                {
                    return;
                }

                this.ValueProvider.SetValue(fieldName, value);
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        public void OnPropertyChanged()
        {
            if (this.PropertyChanged == null)
            {
                return;
            }

            this.PropertyChanged(this, new PropertyChangedEventArgs(string.Empty));
        }

        /// <summary>
        /// Releases the resources.
        /// </summary>
        public void ReleaseResources()
        {
            this.links.Clear();
            this.displayNames.ReleaseResources();
            this.allowedValues.ReleaseResources();
            this.valueProvider.WorkbenchItem = null;
            this.ValueProvider = null;
        }

        /// <summary>
        /// Called when [state changed].
        /// </summary>
        /// <param name="stateFrom">The state from.</param>
        /// <param name="stateTo">The state to.</param>
        private void OnStateChanged(string stateFrom, string stateTo)
        {
            if (this.StateChanged == null)
            {
                return;
            }

            this.StateChanged(this, new ItemStateChangeEventArgs(this, stateFrom, stateTo));
        }
    }
}