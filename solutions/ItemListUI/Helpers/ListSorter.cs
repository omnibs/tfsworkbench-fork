// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListSorter.cs" company="None">
//   None
// </copyright>
// <summary>
//   Initializes instance of ListSorter
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.ItemListUI
{
    using System.Collections;
    using System.Collections.Generic;

    using Core.Helpers;
    using Core.Interfaces;

    /// <summary>
    /// Initializes instance of ListSorter
    /// </summary>
    public class ListSorter : IComparer<IControlItemGroup>
    {
        /// <summary>
        /// Gets or sets the name of the field.
        /// </summary>
        /// <value>The name of the field.</value>
        public string FieldName { get; set; }

        /// <summary>
        /// Gets or sets the direction.
        /// </summary>
        /// <value>The direction.</value>
        public SortDirection Direction { get; set; }

        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns> Value Condition Less than zero
        /// <paramref name="x"/> is less than <paramref name="y"/>.
        /// Zero
        /// <paramref name="x"/> equals <paramref name="y"/>.
        /// Greater than zero
        /// <paramref name="x"/> is greater than <paramref name="y"/>.
        /// </returns>
        public int Compare(IControlItemGroup x, IControlItemGroup y)
        {
            var compareResult = 0;

            if (x.WorkbenchItem != null && y.WorkbenchItem != null)
            {
                // Sort by field value.
                var itemXField = x.WorkbenchItem[this.FieldName];
                var itemYField = y.WorkbenchItem[this.FieldName];

                compareResult = this.Direction == SortDirection.Ascending
                                  ? Comparer.Default.Compare(itemXField, itemYField)
                                  : Comparer.Default.Compare(itemYField, itemXField);

                // Finally, if the comparison values are equal then sort by caption
                if (compareResult.Equals(0))
                {
                    compareResult = this.Direction == SortDirection.Ascending
                        ? Comparer.Default.Compare(x.WorkbenchItem.GetCaption(), y.WorkbenchItem.GetCaption())
                        : Comparer.Default.Compare(y.WorkbenchItem.GetCaption(), x.WorkbenchItem.GetCaption());
                }
            }

            return compareResult;
        }
    }
}
