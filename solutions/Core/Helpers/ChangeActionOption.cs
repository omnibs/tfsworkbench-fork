// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeActionOption.cs" company="None">
//   None
// </copyright>
// <summary>
//   The collection change options
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.Helpers
{
    /// <summary>
    /// The collection change options
    /// </summary>
    public enum ChangeActionOption
    {
        /// <summary>
        /// Items added to colleciton.
        /// </summary>
        Add,

        /// <summary>
        /// Item removed from the collection.
        /// </summary>
        Remove,

        /// <summary>
        /// Collection cleared.
        /// </summary>
        Clear,

        /// <summary>
        /// Collection refreshed
        /// </summary>
        Refresh
    }
}