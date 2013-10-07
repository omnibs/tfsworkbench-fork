// --------------------------------------------------------------------------------------------------------------------
// <copyright file="INamedItem.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the INamedItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.ProjectSetupUI.DataObjects
{
    /// <summary>
    /// The named item interface.
    /// </summary>
    public interface INamedItem
    {
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The release name.</value>
        string Name { get; }
    }
}