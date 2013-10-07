// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeFilter.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the FilterItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using TfsWorkbench.Core.Helpers;
using TfsWorkbench.Core.Interfaces;

namespace TfsWorkbench.UIElements.FilterObjects
{
    /// <summary>
    /// The filter item class.
    /// </summary>
    public class TypeFilter : FilterItemBase
    {
        /// <summary>
        /// The filter type name.
        /// </summary>
        private readonly string typeName;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeFilter"/> class.
        /// </summary>
        /// <param name="typeName">The context.</param>
        public TypeFilter(string typeName) : this(null, typeName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeFilter"/> class.
        /// </summary>
        /// <param name="parentFilter">The parent filter.</param>
        /// <param name="typeName">The context.</param>
        public TypeFilter(IFilterItem parentFilter, string typeName) : base(parentFilter)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                throw new ArgumentNullException("typeName");
            }

            this.typeName = typeName;

            Func<IWorkbenchItem, bool> predicate = w => Equals(w.GetTypeName(), this.typeName);
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
                return this.typeName;
            }
        }
    }
}
