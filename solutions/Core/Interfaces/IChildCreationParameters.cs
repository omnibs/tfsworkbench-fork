// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IChildCreationParameters.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the IChildCreationParameters type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.Interfaces
{
    /// <summary>
    /// The child creation parameters interface.
    /// </summary>
    public interface IChildCreationParameters
    {
        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>The parent.</value>
        IWorkbenchItem Parent { get; }

        /// <summary>
        /// Gets the name of the child type.
        /// </summary>
        /// <value>The name of the child type.</value>
        string ChildTypeName { get; }

        /// <summary>
        /// Gets the name of the link type.
        /// </summary>
        /// <value>The name of the link type.</value>
        string LinkTypeName { get; }
    }
}