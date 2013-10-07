// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiSelectHelper.cs" company="None">
//   None
// </copyright>
// <summary>
//   The multi select helper class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.ItemListUI
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Input;

    using Core.Interfaces;

    /// <summary>
    /// The multi select helper class.
    /// </summary>
    internal class MultiSelectHelper
    {
        /// <summary>
        /// The item list.
        /// </summary>
        private readonly ItemList itemList;

        /// <summary>
        /// The selected control item colleciton.
        /// </summary>
        private readonly ICollection<MultiSelectControlItem> selectedControls = new Collection<MultiSelectControlItem>();

        /// <summary>
        /// The is updating flag.
        /// </summary>
        private bool isUpdating;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiSelectHelper"/> class.
        /// </summary>
        /// <param name="itemList">The item list.</param>
        public MultiSelectHelper(ItemList itemList)
        {
            this.itemList = itemList;
        }

        /// <summary>
        /// Handles the control click.
        /// </summary>
        /// <param name="controlItem">The control item.</param>
        /// <returns><c>True</c> if the event is handelled; otherwise <c>false</c>.</returns>
        public bool HandleControlClick(MultiSelectControlItem controlItem)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control || (Keyboard.Modifiers == ModifierKeys.Shift && !this.selectedControls.Any()))
            {
                if (controlItem.IsSelected)
                {
                    this.RemoveControlFromSelection(controlItem);
                }
                else
                {
                    this.AddControlToSelection(controlItem);
                }

                return true;
            }

            if (Keyboard.Modifiers == ModifierKeys.Shift)
            {
                Func<IControlItem, int> getRowIndex =
                    c =>
                    this.itemList.ControlItemGroups.IndexOf(
                        this.itemList.ControlItemGroups.First(cic => cic.WorkbenchItem.Equals(c.WorkbenchItem)));

                Func<IControlItem, int> getColumnIndex =
                    c =>
                    this.itemList.ControlItemGroups
                        .First(cic => cic.WorkbenchItem.Equals(c.WorkbenchItem))
                        .ControlItems
                        .ToList()
                        .IndexOf(c);

                var lowestSelectedIndex = this.selectedControls.Select(getRowIndex).OrderBy(i => i).First();
                var newControlIndex = getRowIndex(controlItem);
                var columnIndex = getColumnIndex(controlItem);

                this.ClearAllSelections();

                var firstIndex = lowestSelectedIndex < newControlIndex ? lowestSelectedIndex : newControlIndex;
                var lastIndex = lowestSelectedIndex > newControlIndex ? lowestSelectedIndex : newControlIndex;

                for (var i = firstIndex; i <= lastIndex; i++)
                {
                    var control = this.itemList.ControlItemGroups[i].MultiSelectControlItems.ElementAtOrDefault(columnIndex);

                    if (control == null)
                    {
                        continue;
                    }

                    this.AddControlToSelection(control);
                }

                return true;
            }

            if (!this.selectedControls.Contains(controlItem))
            {
                this.ClearAllSelections();
            }

            return false;
        }

        /// <summary>
        /// Clears all selections.
        /// </summary>
        public void ClearAllSelections()
        {
            foreach (var controlItem in this.selectedControls)
            {
                controlItem.IsSelected = false;
                controlItem.PropertyChanged -= this.MultiCastValue;
            }

            this.selectedControls.Clear();
        }

        /// <summary>
        /// Adds the control to selection.
        /// </summary>
        /// <param name="controlItem">The control item.</param>
        internal void AddControlToSelection(MultiSelectControlItem controlItem)
        {
            controlItem.IsSelected = true;
            if (!this.selectedControls.Contains(controlItem))
            {
                this.selectedControls.Add(controlItem);
                controlItem.PropertyChanged += this.MultiCastValue;
            }
        }

        /// <summary>
        /// Removes the control from selection.
        /// </summary>
        /// <param name="controlItem">The control item.</param>
        internal void RemoveControlFromSelection(MultiSelectControlItem controlItem)
        {
            controlItem.IsSelected = false;
            if (this.selectedControls.Contains(controlItem))
            {
                this.selectedControls.Remove(controlItem);
                controlItem.PropertyChanged -= this.MultiCastValue;
            }
        }

        /// <summary>
        /// Multis the cast value.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs"/> instance containing the event data.</param>
        private void MultiCastValue(object sender, PropertyChangedEventArgs e)
        {
            var controlItem = sender as MultiSelectControlItem;
            if (controlItem == null || !controlItem.IsValueUpdateSource || this.isUpdating || !e.PropertyName.Equals("Value"))
            {
                return;
            }

            this.isUpdating = true;

            var value = controlItem.Value;

            foreach (var item in this.selectedControls.Where(c => !c.Equals(controlItem)))
            {
                item.Value = value;
            }

            this.isUpdating = false;
        }
    }
}
