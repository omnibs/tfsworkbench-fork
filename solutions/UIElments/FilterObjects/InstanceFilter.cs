// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstanceFilter.cs" company="None">
//   None
// </copyright>
// <summary>
//   The workbench item instance filter class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Globalization;
using TfsWorkbench.Core.Helpers;
using TfsWorkbench.Core.Interfaces;

namespace TfsWorkbench.UIElements.FilterObjects
{
    /// <summary>
    /// The workbench item instance filter class.
    /// </summary>
    public class InstanceFilter : FilterItemBase
    {
        /// <summary>
        /// The context workbench item;
        /// </summary>
        private IWorkbenchItem context;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceFilter"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public InstanceFilter(IWorkbenchItem context) : this(null, context)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceFilter"/> class.
        /// </summary>
        /// <param name="parentFilter">The parent filter.</param>
        /// <param name="context">The context.</param>
        public InstanceFilter(IFilterItem parentFilter, IWorkbenchItem context) : base(parentFilter)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            this.context = context;
            this.context.PropertyChanged += this.OnContextPropertyChanged;

            Func<IWorkbenchItem, bool> predicate = w => Equals(w, this.context);

            this.FilterPredicate = predicate;
        }

        /// <summary>
        /// Gets the display text.
        /// </summary>
        /// <value>The display text.</value>
        public override string DisplayText
        {
            get
            {
                return string.Format(
                    CultureInfo.InvariantCulture,
                    "({0}) {1} - ({2})",
                    this.context.GetId(),
                    this.context.GetCaption(),
                    this.context.GetState());
            }
        }

        /// <summary>
        /// Releases the resources.
        /// </summary>
        public void ReleaseResources()
        {
            this.context.PropertyChanged -= this.OnContextPropertyChanged;
            this.context = null;
        }

        /// <summary>
        /// Called when [context property changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnContextPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.OnPropertyChanged("DisplayText");
        }
    }
}