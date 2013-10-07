// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RowSorter.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ItemSorter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.Helpers
{
    using System.Collections;

    using TfsWorkbench.Core.Interfaces;

    /// <summary>
    /// Initializes instance of ItemSorter
    /// </summary>
    public class RowSorter : SorterBase<IParentContainer>
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
        public override int Compare(IParentContainer x, IParentContainer y)
        {
            if (x == null || y == null)
            {
                return 0;
            }

            var itemXField = x.Parent[this.FieldName];
            var itemYField = y.Parent[this.FieldName];

            var compareResult = this.Direction == SortDirection.Ascending
                  ? Comparer.Default.Compare(itemXField, itemYField)
                  : Comparer.Default.Compare(itemYField, itemXField);

            // If the comparison values are equal then sort by caption
            if (compareResult.Equals(0))
            {
                compareResult = this.Direction == SortDirection.Ascending
                                    ? Comparer.Default.Compare(x.Parent.GetCaption(), y.Parent.GetCaption())
                                    : Comparer.Default.Compare(y.Parent.GetCaption(), x.Parent.GetCaption());
            }

            return compareResult;
        }
    }
}