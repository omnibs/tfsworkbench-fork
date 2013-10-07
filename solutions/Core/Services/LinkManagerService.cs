// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinkManagerService.cs" company="None">
//   None
// </copyright>
// <summary>
//   The link manager service.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using TfsWorkbench.Core.EventArgObjects;
    using TfsWorkbench.Core.Helpers;
    using TfsWorkbench.Core.Interfaces;

    /// <summary>
    /// The link manager service.
    /// </summary>
    internal class LinkManagerService : ILinkManagerService
    {
        /// <summary>
        /// The item links collection.
        /// </summary>
        private readonly ICollection<ILinkItem> links = new Collection<ILinkItem>();

        /// <summary>
        /// The added links collection.
        /// </summary>
        private readonly ICollection<ILinkItem> addedLinks = new Collection<ILinkItem>();

        /// <summary>
        /// The deleted links collection.
        /// </summary>
        private readonly ICollection<ILinkItem> deletedLinks = new Collection<ILinkItem>();

        /// <summary>
        /// Occurs when [link added].
        /// </summary>
        public event EventHandler<LinkChangeEventArgs> LinkAdded;

        /// <summary>
        /// Occurs when [link removed].
        /// </summary>
        public event EventHandler<LinkChangeEventArgs> LinkRemoved;

        /// <summary>
        /// Gets the links.
        /// </summary>
        /// <value>The links.</value>
        public IEnumerable<ILinkItem> Links
        {
            get
            {
                return this.links;
            }
        }

        /// <summary>
        /// Gets the added links.
        /// </summary>
        /// <value>The added links.</value>
        public IEnumerable<ILinkItem> AddedLinks
        {
            get
            {
                return this.addedLinks;
            }
        }

        /// <summary>
        /// Gets the deleted links.
        /// </summary>
        /// <value>The deleted links.</value>
        public IEnumerable<ILinkItem> DeletedLinks
        {
            get
            {
                return this.deletedLinks;
            }
        }

        /// <summary>
        /// Adds the link and broadcasts the change.
        /// </summary>
        /// <param name="link">The link item.</param>
        public void AddLink(ILinkItem link)
        {
            this.AddLinkAndRaiseChangeEvent(link);
        }

        /// <summary>
        /// Adds the link without raising a link changed event.
        /// </summary>
        /// <param name="link">The item link.</param>
        public void AddLinkWithoutRasingChangeEvent(ILinkItem link)
        {
            this.AddLinkAndRaiseChangeEvent(link, false);
        }

        /// <summary>
        /// Marks the link as Removed and broadcasts the change.
        /// </summary>
        /// <param name="link">The item link.</param>
        public void RemoveLink(ILinkItem link)
        {
            if (!IsValidLink(link))
            {
                return;
            }

            ILinkItem existingLink;
            if (!this.links.TryGetExistingLinkItem(link, out existingLink))
            {
                return;
            }

            this.links.Remove(existingLink);

            if (this.addedLinks.Contains(existingLink))
            {
                this.addedLinks.Remove(existingLink);
            }
            else
            {
                this.deletedLinks.Add(existingLink);
            }

            this.OnLinkRemoved(existingLink);
        }

        /// <summary>
        /// Clears the dirty link.
        /// </summary>
        /// <param name="link">The link item.</param>
        public void ClearDirtyLink(ILinkItem link)
        {
            ILinkItem existingLink;
            if (this.addedLinks.TryGetExistingLinkItem(link, out existingLink))
            {
                this.addedLinks.Remove(existingLink);
            }

            if (this.deletedLinks.TryGetExistingLinkItem(link, out existingLink))
            {
                this.deletedLinks.Remove(existingLink);
            }
        }

        /// <summary>
        /// Removes the links.
        /// </summary>
        public void ClearAllLinks()
        {
            this.links.Clear();
            this.SetInitialLinkStatus();
        }

        /// <summary>
        /// Sets the initial link status.
        /// </summary>
        public void SetInitialLinkStatus()
        {
            this.addedLinks.Clear();
            this.deletedLinks.Clear();
        }

        /// <summary>
        /// Removes the links.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        public void ClearLinks(IWorkbenchItem workbenchItem)
        {
            foreach (var linkItem in this.links.GetLinksFor(workbenchItem).ToArray())
            {
                this.links.Remove(linkItem);
                this.OnLinkRemoved(linkItem);
            }

            foreach (var linkItem in this.addedLinks.GetLinksFor(workbenchItem).ToArray())
            {
                this.addedLinks.Remove(linkItem);
            }

            foreach (var linkItem in this.deletedLinks.GetLinksFor(workbenchItem).ToArray())
            {
                this.deletedLinks.Remove(linkItem);
            }
        }

        /// <summary>
        /// Syncs the links.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <param name="actualLinks">The actual links.</param>
        public void SyncLinks(IWorkbenchItem workbenchItem, IEnumerable<ILinkItem> actualLinks)
        {
            if (actualLinks == null)
            {
                throw new ArgumentNullException("actualLinks");
            }

            var existingLinks = this.links.GetLinksFor(workbenchItem).ToArray();

            foreach (var existingLink in existingLinks)
            {
                ILinkItem actualLink;
                if (!actualLinks.TryGetExistingLinkItem(existingLink, out actualLink))
                {
                    this.RemoveLink(existingLink);
                    this.ClearDirtyLink(existingLink);
                }
            }

            foreach (var actualLink in actualLinks)
            {
                ILinkItem existingLink;
                if (this.links.TryGetExistingLinkItem(actualLink, out existingLink))
                {
                    continue;
                }

                this.AddLink(actualLink);
                this.ClearDirtyLink(actualLink);
            }
        }

        /// <summary>
        /// Determines whether [the specified link] [is valid link].
        /// </summary>
        /// <param name="link">The item link.</param>
        /// <returns>
        /// <c>true</c> if [the specified link] [is valid link]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsValidLink(ILinkItem link)
        {
            return link != null && link.Parent != null && link.Child != null;
        }

        /// <summary>
        /// Adds the link and raise change event.
        /// </summary>
        /// <param name="link">The link to add.</param>
        /// <param name="broadcastChange">if set to <c>true</c> [broadcast change].</param>
        private void AddLinkAndRaiseChangeEvent(ILinkItem link, bool broadcastChange = true)
        {
            if (!IsValidLink(link))
            {
                return;
            }

            ILinkItem existingLink;
            if (this.links.TryGetExistingLinkItem(link, out existingLink))
            {
                return;
            }

            this.links.Add(link);

            if (this.deletedLinks.TryGetExistingLinkItem(link, out existingLink))
            {
                this.deletedLinks.Remove(existingLink);
            }
            else
            {
                this.addedLinks.Add(link);
            }

            if (broadcastChange)
            {
                this.OnLinkAdded(link);
            }
        }

        /// <summary>
        /// Called when [links removed].
        /// </summary>
        /// <param name="linkItem">The link item.</param>
        private void OnLinkRemoved(ILinkItem linkItem)
        {
            if (this.LinkRemoved != null)
            {
                this.LinkRemoved(null, new LinkChangeEventArgs(linkItem));
            }
        }

        /// <summary>
        /// Called when [links Added].
        /// </summary>
        /// <param name="linkItem">The link item.</param>
        private void OnLinkAdded(ILinkItem linkItem)
        {
            if (this.LinkAdded != null)
            {
                this.LinkAdded(null, new LinkChangeEventArgs(linkItem));
            }
        }
    }
}