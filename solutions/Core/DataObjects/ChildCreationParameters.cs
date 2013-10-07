// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChildCreationParameters.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ChildCreationParameters type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.DataObjects
{
    using Interfaces;

    /// <summary>
    /// Initializes instance of ChildCreationParameters
    /// </summary>
    public class ChildCreationParameters : IChildCreationParameters
    {
        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public IWorkbenchItem Parent { get; set; }

        /// <summary>
        /// Gets or sets the name of the child type.
        /// </summary>
        /// <value>The name of the child type.</value>
        public string ChildTypeName { get; set; }

        /// <summary>
        /// Gets or sets the name of the link type.
        /// </summary>
        /// <value>The name of the link type.</value>
        public string LinkTypeName { get; set; }
    }
}
