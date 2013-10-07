// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListSorter.cs" company="EMC Consulting">
//   EMC Consulting 2010
// </copyright>
// <summary>
//   Initializes instance of ListSorter
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Emcc.ScrumMastersWorkbench.ItemListUI
{
    using System.Collections;
    using System.Collections.Generic;

    using Core.Helpers;
    using Core.Interfaces;
    using Core.Properties;

    /// <summary>
    /// Initializes instance of ListSorter
    /// </summary>
    public class ListSorter : IComparer<IControlItemCollection>
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
        public int Compare(IControlItemCollection x, IControlItemCollection y)
        {
            if (string.IsNullOrEmpty(this.FieldName) || x.WorkbenchItem == null || y.WorkbenchItem == null)
            {
                return 0;
            }

            var itemXField = x.WorkbenchItem[this.FieldName];
            var itemYField = y.WorkbenchItem[this.FieldName];

            var compareResult = this.Direction == SortDirection.Ascending
                              ? Comparer.Default.Compare(itemXField, itemYField)
                              : Comparer.Default.Compare(itemYField, itemXField);

            // If the comparison values are equal then sort by ascending title
            if (compareResult.Equals(0))
            {
                var titleFieldName = Settings.Default.TitleFieldName;

                itemXField = x.WorkbenchItem[titleFieldName];
                itemYField = y.WorkbenchItem[titleFieldName];

                compareResult = Comparer.Default.Compare(itemXField, itemYField);
            }

            return compareResult;
        }
    }
}
