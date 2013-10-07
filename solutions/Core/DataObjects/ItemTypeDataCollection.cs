// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ItemTypeDataCollection.cs" company="None">
//   None
// </copyright>
// <summary>
//   The workbench item type data collection class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.DataObjects
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    /// The workbench item type data collection class.
    /// </summary>
    public class ItemTypeDataCollection : Collection<ItemTypeData>
    {
        /// <summary>
        /// Gets the type names.
        /// </summary>
        /// <value>The type names collection.</value>
        public IEnumerable<string> TypeNames
        {
            get
            {
                return this.Select(d => d.TypeName);
            }
        }

        /// <summary>
        /// Gets the <see cref="ItemTypeData"/> with the specified type name.
        /// </summary>
        /// <param name="typeName">The workbench item type name.</param>
        /// <value><c>Type data</c> if found; otherwise <c>null</c>.</value>
        public ItemTypeData this[string typeName]
        {
            get
            {
                return this.FirstOrDefault(witd => Equals(witd.TypeName, typeName));
            }
        }

        /// <summary>
        /// Tries the get the value.
        /// </summary>
        /// <param name="typeName">Name of the type.</param>
        /// <param name="itemTypeData">The item type data.</param>
        /// <returns><c>Trye</c> if the type name is matched; otherwise <c>false</c>.</returns>
        public bool TryGetValue(string typeName, out ItemTypeData itemTypeData)
        {
            itemTypeData = this[typeName];

            return itemTypeData != null;
        }
    }
}
