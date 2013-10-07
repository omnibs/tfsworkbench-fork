// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewMapEventArgs.cs" company="None">
//   None
// </copyright>
// <summary>
//   The view map updated event args class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.EventArgObjects
{
    using TfsWorkbench.Core.DataObjects;

    /// <summary>
    /// The view map updated event args class.
    /// </summary>
    public class ViewMapEventArgs : ContextEventArgs<ViewMap>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewMapEventArgs"/> class.
        /// </summary>
        /// <param name="viewMap">The view map.</param>
        public ViewMapEventArgs(ViewMap viewMap)
            : base(viewMap)
        {
        }
    }
}