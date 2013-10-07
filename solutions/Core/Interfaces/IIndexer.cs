// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IIndexer.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the IIndexer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.Interfaces
{
    /// <summary>
    /// The indexer interface.
    /// </summary>
    public interface IIndexer
    {
        /// <summary>
        /// Gets the <see cref="System.Object"/> with the specified field name.
        /// </summary>
        /// <param name="fieldName">The field name.</param>
        /// <value>An object corresponding the specified field name.</value>
        object this[string fieldName]
        {
            get;
        }

        /// <summary>
        /// Releases the resources.
        /// </summary>
        void ReleaseResources();
    }
}