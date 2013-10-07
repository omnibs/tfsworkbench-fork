// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LinkManagerService.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the LinkChangeHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Interfaces;

    using TfsWorkbench.Core.Services;

    /// <summary>
    /// The link change helper service.
    /// </summary>
    public static class LinkManagerExtensions
    {
        /// <summary>
        /// Gets the link manager service.
        /// </summary>
        /// <value>The link manager service.</value>
        private static ILinkManagerService LinkManagerService
        {
            get
            {
                return ServiceManager.Instance.GetService<ILinkManagerService>();
            }
        }

        /// <summary>
        /// Gets all child links.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <returns>A list of all links for the specified item.</returns>
        public static IEnumerable<ILinkItem> GetChildLinks(this IWorkbenchItem workbenchItem)
        {
            AssertLinkManagerIsNotNull(LinkManagerService);
            return LinkManagerService.Links.Where(l => l.Parent.Equals(workbenchItem) && !l.Child.IsExcluded());
        }

        /// <summary>
        /// Gets the parent links.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <returns>A list of all links where the specified item is a child.</returns>
        public static IEnumerable<ILinkItem> GetParentLinks(this IWorkbenchItem workbenchItem)
        {
            AssertLinkManagerIsNotNull(LinkManagerService);
            return LinkManagerService.Links.Where(l => l.Child.Equals(workbenchItem));
        }

        /// <summary>
        /// Gets the added links.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <returns>A list of all added links for the specified item.</returns>
        public static IEnumerable<ILinkItem> GetAddedLinks(this IWorkbenchItem workbenchItem)
        {
            AssertLinkManagerIsNotNull(LinkManagerService);
            return LinkManagerService.AddedLinks.GetLinksFor(workbenchItem);
        }

        /// <summary>
        /// Gets the deleted links.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <returns>A list of all deleted links for the specified item.</returns>
        public static IEnumerable<ILinkItem> GetDeletedLinks(this IWorkbenchItem workbenchItem)
        {
            AssertLinkManagerIsNotNull(LinkManagerService);
            return LinkManagerService.DeletedLinks.GetLinksFor(workbenchItem);
        }

        /// <summary>
        /// Determines whether [has dirty links] [the specified workbench item].
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <returns>
        /// <c>true</c> if [has dirty links] [the specified workbench item]; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasDirtyLinks(this IWorkbenchItem workbenchItem)
        {
            return GetAddedLinks(workbenchItem).Any() || GetDeletedLinks(workbenchItem).Any();
        }

        /// <summary>
        /// Removes the links.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        public static void ClearLinks(this IWorkbenchItem workbenchItem)
        {
            AssertLinkManagerIsNotNull(LinkManagerService);
            LinkManagerService.ClearLinks(workbenchItem);
        }

        /// <summary>
        /// Syncs the links.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <param name="actualLinks">The actual links.</param>
        public static void SyncLinks(this IWorkbenchItem workbenchItem, IEnumerable<ILinkItem> actualLinks)
        {
            AssertLinkManagerIsNotNull(LinkManagerService);
            LinkManagerService.SyncLinks(workbenchItem, actualLinks);
        }

        /// <summary>
        /// Tries the get existing link item.
        /// </summary>
        /// <param name="linkItems">The link items.</param>
        /// <param name="link">The link item.</param>
        /// <param name="existingLink">The existing link.</param>
        /// <returns><c>True</c> if the link exists; otherwise <c>false</c>.</returns>
        internal static bool TryGetExistingLinkItem(this IEnumerable<ILinkItem> linkItems, ILinkItem link, out ILinkItem existingLink)
        {
            Func<ILinkItem, bool> isMatch =
                linkItem =>
                Equals(linkItem.Child, link.Child)
                && Equals(linkItem.Parent, link.Parent)
                && Equals(linkItem.LinkName, link.LinkName);

            existingLink = linkItems.FirstOrDefault(isMatch);

            return existingLink != null;
        }

        /// <summary>
        /// Gets the links for.
        /// </summary>
        /// <param name="linkItems">The link items.</param>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <returns>An enumerable list of matching links.</returns>
        internal static IEnumerable<ILinkItem> GetLinksFor(this IEnumerable<ILinkItem> linkItems, IWorkbenchItem workbenchItem)
        {
            var linksFor = linkItems.Where(linkItem => linkItem.Child.Equals(workbenchItem) || linkItem.Parent.Equals(workbenchItem));

            return linksFor;
        }

        /// <summary>
        /// Asserts the link manager is not null.
        /// </summary>
        /// <param name="linkManagerService">The link manager service.</param>
        /// <exception cref="ArgumentNullException" />
        private static void AssertLinkManagerIsNotNull(ILinkManagerService linkManagerService)
        {
            if (linkManagerService == null)
            {
                throw new ArgumentNullException("linkManagerService");
            }
        }
    }
}
