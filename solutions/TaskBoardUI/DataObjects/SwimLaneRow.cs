// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SwimLaneRow.cs" company="None">
//   None
// </copyright>
// <summary>
//   Initializes instance of SwimLaneRow
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.TaskBoardUI.DataObjects
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;

    using Core.DataObjects;
    using Core.Interfaces;

    /// <summary>
    /// Initializes instance of SwimLaneRow
    /// </summary>
    public class SwimLaneRow : NotifierBase, IParentContainer
    {
        /// <summary>
        /// The internal swim lane columns collection.
        /// </summary>
        private readonly ObservableCollection<StateCollection> swimLaneColumns = new ObservableCollection<StateCollection>();

        /// <summary>
        /// The row height.
        /// </summary>
        private double rowHeight;

        /// <summary>
        /// Initializes a new instance of the <see cref="SwimLaneRow"/> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="swimLaneStates">The swim lane states.</param>
        /// <param name="linkName">Name of the link.</param>
        /// <param name="childTypeName">Name of the child type.</param>
        /// <param name="initialRowHeight">Initial height of the row.</param>
        public SwimLaneRow(
            IWorkbenchItem parent,
            IEnumerable<string> swimLaneStates,
            string linkName,
            string childTypeName,
            double initialRowHeight)
        {
            if (swimLaneStates == null)
            {
                throw new ArgumentNullException("swimLaneStates");
            }

            this.Parent = parent;
            this.rowHeight = initialRowHeight;

            foreach (var stateCollection in swimLaneStates.Select(state => new StateCollection(state, linkName, parent)))
            {
                this.SwimLaneColumns.Add(stateCollection);
            }

            this.ChildCreationArguments = new ChildCreationParameters
                {
                    Parent = parent, 
                    ChildTypeName = childTypeName, 
                    LinkTypeName = linkName
                };
        }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public IWorkbenchItem Parent { get; private set; }

        /// <summary>
        /// Gets the child states.
        /// </summary>
        /// <value>The child states.</value>
        public ObservableCollection<StateCollection> SwimLaneColumns
        {
            get
            {
                return this.swimLaneColumns;
            }
        }

        /// <summary>
        /// Gets the child creation arguments.
        /// </summary>
        /// <value>The child creation arguments.</value>
        public IChildCreationParameters ChildCreationArguments { get; private set; }

        /// <summary>
        /// Gets or sets the height of the row.
        /// </summary>
        /// <value>The height of the row.</value>
        public double RowHeight 
        {
            get { return this.rowHeight; }
            set { this.UpdateWithNotification("RowHeight", value, ref this.rowHeight); }
        }

        /// <summary>
        /// Gets the IStateCollection with the specified state.
        /// </summary>
        /// <param name="state">The state of the collection.</param>
        /// <value>The matching state collection.</value>
        public StateCollection this[string state]
        {
            get
            {
                return this.SwimLaneColumns.FirstOrDefault(c => c.State.Equals(state));
            }
        }

        /// <summary>
        /// Releases the resources.
        /// </summary>
        public void ReleaseResources()
        {
            if (this.SwimLaneColumns != null)
            {
                foreach (var column in this.swimLaneColumns.ToArray())
                {
                    column.ReleaseResources();
                }

                this.swimLaneColumns.Clear();
            }

            this.ChildCreationArguments = null;

            this.Parent = null;

            this.OnPropertyChanged(this, new PropertyChangedEventArgs(null));
        }
    }
}