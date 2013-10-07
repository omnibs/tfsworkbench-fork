// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SorterBase.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the SorterBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.Helpers
{
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// The sort direction enumeration.
    /// </summary>
    public enum SortDirection
    {
        /// <summary>
        /// Sort ascending.
        /// </summary>
        Ascending = 0,

        /// <summary>
        /// Sort Descending
        /// </summary>
        Descending = 1
    }

    /// <summary>
    /// Initializes instance of SorterBase
    /// </summary>
    /// <typeparam name="T">The sortable child type</typeparam>
    public abstract class SorterBase<T> : IComparer<T>
    {
        /// <summary>
        /// Gets or sets the name of the field.
        /// </summary>
        /// <value>The name of the field.</value>
        [XmlAttribute(AttributeName = "field")]
        public string FieldName { get; set; }

        /// <summary>
        /// Gets or sets the direction.
        /// </summary>
        /// <value>The direction.</value>
        [XmlAttribute(AttributeName = "direction")]
        public SortDirection Direction { get; set; }

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
        public abstract int Compare(T x, T y);
    }
}