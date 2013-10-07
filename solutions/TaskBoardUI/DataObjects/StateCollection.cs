// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateCollection.cs" company="None">
//   None
// </copyright>
// <summary>
//   Initializes instance of StateCollection
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.TaskBoardUI.DataObjects
{
    using System;
    using System.Collections.Specialized;
    using System.Linq;

    using Core.Helpers;
    using Core.Interfaces;

    using TfsWorkbench.Core.Properties;
    using TfsWorkbench.Core.Services;
    using TfsWorkbench.TaskBoardUI.Helpers;

    /// <summary>
    /// Initializes instance of StateCollection
    /// </summary>
    public sealed class StateCollection : SortableObservableCollection<IWorkbenchItem>
    {
        /// <summary>
        /// The link manager service instance.
        /// </summary>
        private readonly ILinkManagerService linkManagerService;

        /// <summary>
        /// Initializes a new instance of the <see cref="StateCollection"/> class.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="linkName">Name of the link.</param>
        public StateCollection(string state, string linkName)
            : this(state, linkName, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateCollection"/> class.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="linkName">Name of the link.</param>
        /// <param name="parent">The parent.</param>
        public StateCollection(string state, string linkName, IWorkbenchItem parent)
            : this(state, linkName, parent, ServiceManager.Instance.GetService<ILinkManagerService>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateCollection"/> class.
        /// </summary>
        /// <param name="state">The state.</param>
        /// <param name="linkName">Name of the link.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="linkManagerService">The link manager service.</param>
        public StateCollection(string state, string linkName, IWorkbenchItem parent, ILinkManagerService linkManagerService)
        {
            this.linkManagerService = linkManagerService;
            if (linkManagerService == null)
            {
                throw new ArgumentNullException("linkManagerService");
            }

            this.Parent = parent;
            this.State = state;
            this.LinkName = linkName;

            this.CollectionChanged += this.HandleParentLinks;
        }

        /// <summary>
        /// Gets the name of the link.
        /// </summary>
        /// <value>The name of the link.</value>
        public string LinkName { get; private set; }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>The parent.</value>
        public IWorkbenchItem Parent { get; set; }

        /// <summary>
        /// Gets the state.
        /// </summary>
        /// <value>The state.</value>
        public string State { get; private set; }

        /// <summary>
        /// Adds the specified child.
        /// </summary>
        /// <param name="child">The child.</param>
        public new void Add(IWorkbenchItem child)
        {
            var itemStatus = SwimLaneHelper.GetItemStatus(child);
            if (itemStatus != this.State)
            {
                if (WorkbenchItemHelper.CustomStates.Contains(this.State))
                {
                    var body = child.GetBody();
                    if (body.ToLowerInvariant().EndsWith("]"))
                    {
                        var index = body.LastIndexOf("[", System.StringComparison.Ordinal);
                        var substr = body.Substring(index);
                        body = body.Replace(substr, "[" + this.State + "]");
                        child[WorkbenchItemHelper.GetBodyFieldName(child.GetTypeName())] = body;
                    }
                    else
                    {
                        body += "[" + this.State + "]";
                        child[WorkbenchItemHelper.GetBodyFieldName(child.GetTypeName())] = body;
                    }
                }
                else
                {
                    child.SetState(this.State);
                }
            }

            if (!this.Contains(child))
            {
                base.Add(child);
            }
        }

        /// <summary>
        /// Releases the resources.
        /// </summary>
        public void ReleaseResources()
        {
            this.Parent = null;
            this.Clear();
            this.CollectionChanged -= this.HandleParentLinks;
        }

        /// <summary>
        /// Handles the parent links.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The instance containing the event data.</param>
        private void HandleParentLinks(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.Parent == null)
            {
                return;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:

                    var newLinks = e.NewItems.Cast<IWorkbenchItem>()
                        .Where(wbi => !this.Parent.ChildLinks.Any(l => l.Child.Equals(wbi) && Equals(l.LinkName, this.LinkName)))
                        .Select(wbi => Factory.BuildLinkItem(this.LinkName, this.Parent, wbi));

                    foreach (var link in newLinks)
                    {
                        this.linkManagerService.AddLink(link);
                    }

                    break;
                case NotifyCollectionChangedAction.Remove:

                    var removedChildren = e.OldItems.Cast<IWorkbenchItem>();

                    var oldLinks = this.Parent.ChildLinks.Where(l => Equals(l.LinkName, this.LinkName) && removedChildren.Contains(l.Child)).ToArray();

                    foreach (var link in oldLinks)
                    {
                        this.linkManagerService.RemoveLink(link);
                    }

                    break;
            }
        }
    }
}