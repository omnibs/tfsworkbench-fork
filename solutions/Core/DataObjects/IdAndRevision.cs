// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IdAndRevision.cs" company="None">
//   None
// </copyright>
// <summary>
//   The Id and Revision structure.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.DataObjects
{
    /// <summary>
    /// The Id and Revision structure.
    /// </summary>
    public struct IdAndRevision
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        /// <value>The item id.</value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the revision.
        /// </summary>
        /// <value>The revision.</value>
        public int Revision { get; set; }
    }
}