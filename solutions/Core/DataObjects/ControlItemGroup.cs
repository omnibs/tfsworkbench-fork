// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ControlItemGroup.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ControlItemGroup type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.DataObjects
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Xml.Serialization;

    using TfsWorkbench.Core.Interfaces;

    /// <summary>
    /// Initializes instance of ControlItemGroup
    /// </summary>
    [Serializable]
    [DesignerCategory(@"code")]
    [XmlType(AnonymousType = true, Namespace = "http://schemas.workbench/ControlItems")]
    [XmlRoot(Namespace = "http://schemas.workbench/ControlItems", IsNullable = false, ElementName = "ControlItemGroup")]
    public class ControlItemGroup : IControlItemGroup
    {
        /// <summary>
        /// The control item collection.
        /// </summary>
        private readonly Collection<ControlItem> controlItemCollection = new Collection<ControlItem>();

        /// <summary>
        /// The task board item field.
        /// </summary>
        private IWorkbenchItem workbenchItem;

        /// <summary>
        /// Gets the control items concrete.
        /// </summary>
        /// <value>The control items concrete.</value>
        [XmlArray(ElementName = "ControlItems")]
        [XmlArrayItem("ControlItem", typeof(ControlItem), IsNullable = false)]
        public Collection<ControlItem> ControlItemsConcrete
        {
            get
            {
                return this.controlItemCollection;
            }
        }

        /// <summary>
        /// Gets the control items.
        /// </summary>
        /// <value>The control items.</value>
        [XmlIgnore]
        public ICollection<IControlItem> ControlItems
        {
            get
            {
                return this.controlItemCollection.Cast<IControlItem>().ToList();
            }
        }

        /// <summary>
        /// Gets or sets the task board item.
        /// </summary>
        /// <value>The task board item.</value>
        [XmlIgnore]
        public IWorkbenchItem WorkbenchItem
        {
            get
            {
                return this.workbenchItem;
            }

            set
            {
                if (this.workbenchItem == value)
                {
                    return;
                }

                if (this.workbenchItem != null)
                {
                    this.workbenchItem.PropertyChanged -= this.OnWorkbenchItemPropertyChanged;
                }

                this.workbenchItem = value;

                foreach (var controlItem in this.ControlItems)
                {
                    controlItem.WorkbenchItem = value;
                }

                if (this.workbenchItem != null)
                {
                    this.workbenchItem.PropertyChanged += this.OnWorkbenchItemPropertyChanged;
                }
            }
        }

        /// <summary>
        /// Gets the <see cref="TfsWorkbench.Core.Interfaces.IControlItem"/> at the specified index.
        /// </summary>
        /// <param name="index">The item index.</param>
        /// <value>A control item.</value>
        public IControlItem this[int index]
        {
            get { return this.ControlItems.ElementAt(index); }
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns>A clone of the current instance.</returns>
        public ControlItemGroup Clone()
        {
            var output = new ControlItemGroup();

            foreach (var controlItem in this.ControlItems.OfType<ControlItem>())
            {
                output.ControlItemsConcrete.Add(controlItem.Clone());
            }

            return output;
        }

        /// <summary>
        /// Called when [workbench item property changed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void OnWorkbenchItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            foreach (var controlItem in this.ControlItemsConcrete)
            {
                controlItem.OnPropertyChanged();
            }
        }
    }
}