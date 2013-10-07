// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BodyConverter.cs" company="None">
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
    /// The body converter class.
    /// </summary>
    public class BodyConverter : DisplayFieldConverterBase
    {
        /// <summary>
        /// The value getter method.
        /// </summary>
        private readonly Func<ItemTypeData, string> getDisplayFieldName = t => t.BodyField;

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
