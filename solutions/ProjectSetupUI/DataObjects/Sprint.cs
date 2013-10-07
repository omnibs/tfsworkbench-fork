// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Sprint.cs" company="None">
//   None
// </copyright>
// <summary>
//   Initializes instance of Sprint
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.ProjectSetupUI.DataObjects
{
    /// <summary>
    /// Initializes instance of Sprint
    /// </summary>
    internal class Sprint : DurationStructureBase
    {
        /// <summary>
        /// The teams collection.
        /// </summary>
        private readonly TeamCollection teams = new TeamCollection();

        /// <summary>
        /// Gets the teams.
        /// </summary>
        /// <value>The teams.</value>
        public TeamCollection Teams
        {
            get
            {
                return this.teams;
            }
        }
    }
}