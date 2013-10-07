// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StructureExtensions.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the StructureExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.ProjectSetupUI.Helpers
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Initializes instance of StructureExtensions
    /// </summary>
    internal static class StructureExtensions
    {
        /// <summary>
        /// Clones the specified items.
        /// </summary>
        /// <typeparam name="T">The input item type.</typeparam>
        /// <param name="items">The items.</param>
        /// <returns>A cloned collection of the input items.</returns>
        public static IEnumerable<T> Clone<T>(this IEnumerable<T> items) where T : ICloneable
        {
            foreach (var item in items)
            {
                yield return (T)item.Clone();
            }
        }
    }
}
