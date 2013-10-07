// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NumericConverter.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the CaptionConverter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.UIElements.ValueConverters
{
    using System;

    using Core.DataObjects;

    /// <summary>
    /// The numeric converter class.
    /// </summary>
    public class NumericConverter : DisplayFieldConverterBase
    {
        /// <summary>
        /// The value getter method.
        /// </summary>
        private readonly Func<ItemTypeData, string> getDisplayFieldName = t => t.NumericField;

        /// <summary>
        /// Gets the value getter.
        /// </summary>
        /// <value>The value getter.</value>
        protected override Func<ItemTypeData, string> GetDisplayFieldName
        {
            get
            {
                return this.getDisplayFieldName;
            }
        }
    }
}
