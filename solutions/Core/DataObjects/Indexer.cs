// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Indexer.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the Indexer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.DataObjects
{
    using System;

    using Interfaces;

    /// <summary>
    /// Initializes instance of Indexer
    /// </summary>
    internal class Indexer : IIndexer
    {
        /// <summary>
        /// The resolution method fuinction.
        /// </summary>
        private Func<string, object> resolutionMethod;

        /// <summary>
        /// Initializes a new instance of the <see cref="Indexer"/> class.
        /// </summary>
        /// <param name="resolutionMethod">The resolution method.</param>
        public Indexer(Func<string, object> resolutionMethod)
        {
            this.resolutionMethod = resolutionMethod;
        }

        /// <summary>
        /// Gets the <see cref="System.Object"/> with the specified field name.
        /// </summary>
        /// <param name="fieldName">The field name.</param>
        /// <value>An object corresponding the specified field name.</value>
        public object this[string fieldName]
        {
            get
            {
                return this.resolutionMethod == null ? null : this.resolutionMethod(fieldName);
            }
        }

        /// <summary>
        /// Releases the resources.
        /// </summary>
        public void ReleaseResources()
        {
            this.resolutionMethod = null;
        }
    }
}
