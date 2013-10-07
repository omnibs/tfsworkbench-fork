// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ControlItemCollection.cs" company="EMC Consulting">
//   EMC Consulting 2009
// </copyright>
// <summary>
//   Defines the ControlItemCollection type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Emcc.TeamSystem.TaskBoard.Core.DataObjects
{
    using System.Collections.ObjectModel;
    using System.Xml.Serialization;

    using Interfaces;

    /// <summary>
    /// Initializes instance of ControlItemCollection
    /// </summary>
    [System.Serializable]
    [System.Diagnostics.DebuggerStepThrough]
    [System.ComponentModel.DesignerCategory("code")]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.emcc.com/TaskBoard/ControlItems")]
    [XmlRoot(Namespace = "http://schemas.emcc.com/TaskBoard/ControlItems", IsNullable = false)]
    public class ControlItemCollection
    {
        /// <summary>
        /// The control item collection.
        /// </summary>
        private readonly Collection<ControlItem> controlItemCollection = new Collection<ControlItem>();

        /// <summary>
        /// The task board item field.
        /// </summary>
        private ITaskBoardItem taskBoardItem;

        /// <summary>
        /// Gets the control items.
        /// </summary>
        /// <value>The control items.</value>
        [XmlArrayItem("ControlItem", typeof(ControlItem), IsNullable = false)]
        public Collection<ControlItem> ControlItems
        {
            get
            {
                return this.controlItemCollection;
            }
        }

        /// <summary>
        /// Gets or sets the task board item.
        /// </summary>
        /// <value>The task board item.</value>
        [XmlIgnore]
        public ITaskBoardItem TaskBoardItem
        {
            get
            {
                return this.taskBoardItem;
            }

            set
            {
                this.taskBoardItem = value;

                foreach (var controlItem in this.ControlItems)
                {
                    controlItem.TaskBoardItem = value;
                }
            }
        }
    }
}