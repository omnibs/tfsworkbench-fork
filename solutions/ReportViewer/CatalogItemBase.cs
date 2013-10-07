// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CatalogItemBase.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the CatalogItemBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.ReportViewer
{
    /// <summary>
    /// The catalog item base class.
    /// </summary>
    public abstract class CatalogItemBase
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The item name.</value>
        /// <remarks/>
        public abstract string Name { get; set; }

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>The item path.</value>
        /// <remarks/>
        public abstract string Path { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        /// <remarks/>
        public abstract string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="CatalogItemBase"/> is hidden.
        /// </summary>
        /// <value><c>true</c> if hidden; otherwise, <c>false</c>.</value>
        /// <remarks/>
        public abstract bool Hidden { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance is report.
        /// </summary>
        /// <value><c>true</c> if this instance is report; otherwise, <c>false</c>.</value>
        public abstract bool IsReport { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is folder.
        /// </summary>
        /// <value><c>true</c> if this instance is folder; otherwise, <c>false</c>.</value>
        public abstract bool IsFolder { get; }
    }
}

