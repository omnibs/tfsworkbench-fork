// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemSorter.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ItemSorter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.Helpers
{
    using System;
    using System.Collections;

    using Interfaces;

    using Properties;

    /// <summary>
    /// Initializes instance of ItemSorter
    /// </summary>
    public class ItemSorter : SorterBase<IWorkbenchItem>
    {
        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>Value Condition Less than zero
        /// <paramref name="x"/> is less than <paramref name="y"/>.
        /// Zero
        /// <paramref name="x"/> equals <paramref name="y"/>.
        /// Greater than zero
        /// <paramref name="x"/> is greater than <paramref name="y"/>.
        /// </returns>
        public override int Compare(IWorkbenchItem x, IWorkbenchItem y)
        {
            if (x == null || y == null)
            {
                return 0;
            }

            var itemXField = x[this.FieldName];
            var itemYField = y[this.FieldName];

            var compareResult = this.Direction == SortDirection.Ascending
                              ? Comparer.Default.Compare(itemXField, itemYField) 
                              : Comparer.Default.Compare(itemYField, itemXField);

            // If the comparison values are equal then sort by caption
            if (compareResult.Equals(0))
            {
                compareResult = this.Direction == SortDirection.Ascending
                                    ? Comparer.Default.Compare(x.GetCaption(), y.GetCaption())
                                    : Comparer.Default.Compare(y.GetCaption(), x.GetCaption());
            }

            return compareResult;
        }
    }
}