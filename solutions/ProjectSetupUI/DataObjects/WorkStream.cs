// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkStream.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the WorkStream type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.ProjectSetupUI.DataObjects
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    /// <summary>
    /// Initializes instance of WorkStream
    /// </summary>
    internal class WorkStream : NamedStructureBase
    {
        /// <summary>
        /// The sprints collection.
        /// </summary>
        private readonly ObservableCollection<Sprint> sprints = new ObservableCollection<Sprint>();

        /// <summary>
        /// The cadance field.
        /// </summary>
        private int? cadance;

        /// <summary>
        /// Gets or sets the cadance.
        /// </summary>
        /// <value>The cadance.</value>
        public int? Cadance
        {
            get
            {
                return this.cadance;
            }

            set
            {
                this.UpdateWithNotification("Cadance", value, ref this.cadance);
            }
        }

        /// <summary>
        /// Gets the sprints.
        /// </summary>
        /// <value>The sprints.</value>
        public ObservableCollection<Sprint> Sprints
        {
            get
            {
                return this.sprints;
            }
        }
    }
}
