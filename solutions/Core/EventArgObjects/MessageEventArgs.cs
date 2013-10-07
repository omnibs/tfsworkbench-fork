// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageEventArgs.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the LayoutSaveErrorEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.EventArgObjects
{
    /// <summary>
    /// Initialises and instance of TfsWorkbench.Core.EventArgObjects.MessageEventArgs
    /// </summary>
    public class MessageEventArgs : ContextEventArgs<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageEventArgs"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public MessageEventArgs(string message)
            : base(message)
        {
        }
    }
}
