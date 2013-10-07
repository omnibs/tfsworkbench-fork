// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CaptionConverter.cs" company="None">
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
    /// The caption converter class.
    /// </summary>
    public class CaptionConverter : DisplayFieldConverterBase
    {
        /// <summary>
        /// The value getter method.
        /// </summary>
        private readonly Func<ItemTypeData, string> getDisplayFieldName = t => t.CaptionField;

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
