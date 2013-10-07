// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkItemValueProvider.cs" company="None">
//   None
// </copyright>
// <summary>
//   The work item value provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TfsWorkbench.Core.Helpers;
using TfsWorkbench.Core.Interfaces;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using TfsWorkbench.Core.Services;

namespace TfsWorkbench.TFSDataProvider2010.Helpers
{
    /// <summary>
    /// The work item value provider.
    /// </summary>
    internal class WorkItemValueProvider : ProjectDataServiceConsumer, IValueProvider
    {
        /// <summary>
        /// The link manager service instance.
        /// </summary>
        private readonly ILinkManagerService linkManagerService;

        /// <summary>
        /// The allowed value cache.
        /// </summary>
        private readonly IDictionary<string, IEnumerable<object>> allowedValueCache = new Dictionary<string, IEnumerable<object>>();

        /// <summary>
        /// The field map cache.
        /// </summary>
        private readonly IDictionary<string, Field> fieldCache = new Dictionary<string, Field>();

        /// <summary>
        /// The has allowed value cahce.
        /// </summary>
        private readonly IDictionary<string, bool?> hasAllowedValueCache = new Dictionary<string, bool?>();

        /// <summary>
        /// The is limted to allowed value cahce.
        /// </summary>
        private readonly IDictionary<string, bool?> isLimitedToAllowedValueCache = new Dictionary<string, bool?>();

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkItemValueProvider"/> class.
        /// </summary>
        public WorkItemValueProvider()
            : this(ServiceManager.Instance.GetService<ILinkManagerService>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkItemValueProvider"/> class.
        /// </summary>
        /// <param name="linkManagerService">The link manager service.</param>
        public WorkItemValueProvider(ILinkManagerService linkManagerService)
        {
            this.linkManagerService = linkManagerService;
        }

        /// <summary>
        /// Gets or sets the work item.
        /// </summary>
        /// <value>The work item.</value>
        public WorkItem WorkItem { get; set; }

        /// <summary>
        /// Gets or sets the workbench item.
        /// </summary>
        /// <value>The workbench item.</value>
        public IWorkbenchItem WorkbenchItem { get; set; }

        /// <summary>
        /// Gets or sets the state changed action method.
        /// </summary>
        /// <value>The state changed.</value>
        public Action<string, string> StateChanged
        {
            get; set;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is dirty.
        /// </summary>
        /// <value><c>true</c> if this instance is dirty; otherwise, <c>false</c>.</value>
        public bool IsDirty
        {
            get
            {
                return this.WorkItem.IsDirty || this.WorkbenchItem.HasDirtyLinks();
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is new.
        /// </summary>
        /// <value><c>true</c> if this instance is new; otherwise, <c>false</c>.</value>
        public bool IsNew
        {
            get { return this.WorkItem.IsNew; }
        }

        /// <summary>
        /// Gets the validation errors.
        /// </summary>
        /// <value>The validation errors.</value>
        public IEnumerable<string> ValidationErrors
        {
            get
            {
                var validationErrors = this.WorkItem.Validate().Cast<Field>();

                return validationErrors.Select(
                    e =>
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "Field: '{0}' status: '{1}' value: '{2}'",
                        e.ReferenceName,
                        e.Status,
                        e.Value));
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is valid for saving.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is valid for saving; otherwise, <c>false</c>.
        /// </value>
        private bool IsValidForSaving
        {
            get
            {
                return this.ValidationErrors == null || !this.ValidationErrors.Any();
            }
        }

        /// <summary>
        /// Saves this instance or returns a list of validation errors.
        /// </summary>
        /// <returns>A list of validation errors if validation fails; otherwise null.</returns>
        public IEnumerable<string> Save()
        {
            IEnumerable<string> output = null;

            try
            {
                this.ApplyLinkChanges();

                if (this.IsValidForSaving)
                {
                    this.WorkItem.Save();

                    this.ClearCachedData();

                    this.CleanDirtyLinks();
                }
                else
                {
                    output = this.ValidationErrors;
                }
            }
            catch (Exception ex)
            {
                output = new[] { ex.Message };
            }

            return output;
        }

        /// <summary>
        /// Syncs to latest work item revision.
        /// </summary>
        public void SyncToLatest()
        {
            if (this.IsNew)
            {
                // Nothing to sync to.
                return;
            }

            this.ClearCachedData();

            var store = this.WorkItem.Store;
            store.RefreshCache();

            var oldState = this.WorkItem.State;
            this.WorkItem.SyncToLatest();
            var newState = this.WorkItem.State;

            if (this.StateChanged != null && !oldState.Equals(newState))
            {
                this.StateChanged(oldState, newState);
            }

            this.WorkbenchItem.OnPropertyChanged();

            this.SyncLinks();
        }

        /// <summary>
        /// Determines whether [the specified value name] [is limited to allowed values].
        /// </summary>
        /// <param name="valueName">Name of the value.</param>
        /// <returns>
        /// <c>true</c> if [the specified value name] [is limited to allowed values]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsLimitedToAllowedValues(string valueName)
        {
            bool? isLimitedToAllowedValue;
            if (!this.isLimitedToAllowedValueCache.TryGetValue(valueName, out isLimitedToAllowedValue))
            {
                Field field;
                isLimitedToAllowedValue = this.TryGetField(valueName, out field) ? field.IsLimitedToAllowedValues : false;
                this.isLimitedToAllowedValueCache.Add(valueName, isLimitedToAllowedValue);
            }

            return isLimitedToAllowedValue.HasValue ? isLimitedToAllowedValue.Value : false;
        }

        /// <summary>
        /// Determines whether [the specified value name] [has allowed values].
        /// </summary>
        /// <param name="valueName">Name of the value.</param>
        /// <returns>
        /// <c>true</c> if [the specified value name] [has allowed values]; otherwise, <c>false</c>.
        /// </returns>
        public bool HasAllowedValues(string valueName)
        {
            bool? hasAllowedValue;
            if (!this.hasAllowedValueCache.TryGetValue(valueName, out hasAllowedValue))
            {
                Field field;
                hasAllowedValue = this.TryGetField(valueName, out field) ? field.HasAllowedValuesList : false;
                this.hasAllowedValueCache.Add(valueName, hasAllowedValue);
            }

            return hasAllowedValue.HasValue ? hasAllowedValue.Value : false;
        }

        /// <summary>
        /// Get the value of the named item.
        /// </summary>
        /// <param name="valueName">The value name.</param>
        /// <returns>The specified value.</returns>
        public object GetValue(string valueName)
        {
            Field field;
            return this.TryGetField(valueName, out field) ? field.Value : null;
        }

        /// <summary>
        /// Sets the specified value.
        /// </summary>
        /// <param name="valueName">The value name.</param>
        /// <param name="newValue">The new value.</param>
        public void SetValue(string valueName, object newValue)
        {
            Field field;
            if (!this.TryGetField(valueName, out field))
            {
                return;
            }

            var previousValue = field.Value;

            this.EnsureWorkItemIsOpen();

            field.Value = newValue;

            if (!Equals(previousValue, newValue)
                && this.StateChanged != null
                && field.ReferenceName.Equals(Core.Properties.Settings.Default.StateFieldName))
            {
                this.StateChanged(previousValue.ToString(), newValue.ToString());
            }
        }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <param name="valueName">Name of the value.</param>
        /// <returns>The corresponding display name.</returns>
        public string GetDisplayName(string valueName)
        {
            Field field;
            return this.TryGetField(valueName, out field) ? field.Name : null;
        }

        /// <summary>
        /// Gets the allowed values.
        /// </summary>
        /// <param name="valueName">Name of the value.</param>
        /// <returns>
        /// A list of allowed values if specified; otherwise null.
        /// </returns>
        public IEnumerable<object> GetAllowedValues(string valueName)
        {
            IEnumerable<object> allowedValues;
            if (!this.allowedValueCache.TryGetValue(valueName, out allowedValues))
            {
                // If no cahced values found, call TFS.
                Field field;
                allowedValues = this.TryGetField(valueName, out field)
                                        ? field.AllowedValues.Cast<object>().ToArray()
                                        : new object[] { };

                var currentValueAsString = field.Value == null ? string.Empty : field.Value.ToString();

                // Ensure that the current value is in the list.
                if (!allowedValues.Contains(currentValueAsString))
                {
                    allowedValues = allowedValues.Union(new[] { currentValueAsString }).ToArray();
                }

                this.allowedValueCache.Add(valueName, allowedValues);
            }

            return allowedValues;
        }

        /// <summary>
        /// Determines whether the specified link A is match.
        /// </summary>
        /// <param name="linkA">The link A.</param>
        /// <param name="linkB">The link B.</param>
        /// <returns>
        /// <c>true</c> if the specified link A is match; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsMatch(RelatedLink linkA, RelatedLink linkB)
        {
            return Equals(linkA.RelatedWorkItemId, linkB.RelatedWorkItemId) && Equals(linkA.LinkTypeEnd, linkB.LinkTypeEnd);
        }

        /// <summary>
        /// Cleans the dirty links.
        /// </summary>
        private void CleanDirtyLinks()
        {
            foreach (var linkItem in this.GetDirtyLinks().ToArray())
            {
                this.linkManagerService.ClearDirtyLink(linkItem);
            }
        }

        /// <summary>
        /// Applies the link changes to the underlying workitem.
        /// </summary>
        private void ApplyLinkChanges()
        {
            this.ApplyDeletedLinks();

            this.ApplyAddedLinks();
        }

        /// <summary>
        /// Removes the deleted links from the underlying work item.
        /// </summary>
        private void ApplyDeletedLinks()
        {
            foreach (var linkItem in this.WorkbenchItem.GetDeletedLinks())
            {
                RelatedLink existingLink;
                if (this.TryGetExistingLink(linkItem, out existingLink))
                {
                    this.WorkItem.Links.Remove(existingLink);
                }
            }
        }

        /// <summary>
        /// Applies the added links to the undwerlying work item.
        /// </summary>
        private void ApplyAddedLinks()
        {
            foreach (var linkItem in this.WorkbenchItem.GetAddedLinks())
            {
                RelatedLink existingLink;
                if (this.TryGetExistingLink(linkItem, out existingLink))
                {
                    continue;
                }

                var workItemLink = this.Transform(linkItem);

                var isTargetNew = workItemLink.RelatedWorkItemId == 0;

                if (isTargetNew)
                {
                    continue;
                }

                this.WorkItem.Links.Add(workItemLink);
            }
        }

        /// <summary>
        /// Gets the dirty links.
        /// </summary>
        /// <returns>An enumerable of the altered links.</returns>
        private IEnumerable<ILinkItem> GetDirtyLinks()
        {
            return this.WorkbenchItem.GetDeletedLinks().Union(this.WorkbenchItem.GetAddedLinks());
        }

        /// <summary>
        /// Ensures the work item is open.
        /// </summary>
        private void EnsureWorkItemIsOpen()
        {
            if (!this.WorkItem.IsOpen)
            {
                this.WorkItem.Open();
            }
        }

        /// <summary>
        /// Clears the cached data.
        /// </summary>
        private void ClearCachedData()
        {
            this.allowedValueCache.Clear();
            this.hasAllowedValueCache.Clear();
            this.isLimitedToAllowedValueCache.Clear();
        }

        /// <summary>
        /// Syncs the links.
        /// </summary>
        private void SyncLinks()
        {
            // Reset the work item links.
            var projectData = this.ProjectDataService.CurrentProjectData;

            if (projectData == null)
            {
                return;
            }

            // Undo any deleted links
            foreach (var linkItem in this.WorkbenchItem.GetDeletedLinks().ToArray())
            {
                this.linkManagerService.AddLink(linkItem);
            }

            var actualLinks = new List<ILinkItem>();

            foreach (var relatedLink in this.WorkItem.Links.OfType<RelatedLink>())
            {
                var relatedWorkItemId = relatedLink.RelatedWorkItemId;

                var target =
                    projectData.WorkbenchItems.FirstOrDefault(
                        w => w.GetId().Equals(relatedWorkItemId));

                if (target == null)
                {
                    continue;
                }

                var linkTypeEnd = relatedLink.LinkTypeEnd;

                if (linkTypeEnd == null)
                {
                    actualLinks.Add(Factory.BuildLinkItem(null, this.WorkbenchItem, target));
                    actualLinks.Add(Factory.BuildLinkItem(null, target, this.WorkbenchItem));
                }
                else
                {
                    var linkName = linkTypeEnd.LinkType.ReferenceName;

                    // Non directional links should exist on both ends.
                    if (!linkTypeEnd.LinkType.IsDirectional)
                    {
                        actualLinks.Add(Factory.BuildLinkItem(linkName, this.WorkbenchItem, target));
                        actualLinks.Add(Factory.BuildLinkItem(linkName, target, this.WorkbenchItem));
                    }
                    else
                    {
                        var linkItem = linkTypeEnd.IsForwardLink
                                           ? Factory.BuildLinkItem(linkName, this.WorkbenchItem, target)
                                           : Factory.BuildLinkItem(linkName, target, this.WorkbenchItem);

                        actualLinks.Add(linkItem);
                    }
                }
            }

            this.WorkbenchItem.SyncLinks(actualLinks);
        }

        /// <summary>
        /// Tries the get field.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="field">The field.</param>
        /// <returns>
        /// <c>True</c> if the field is found; otherwise <c>false</c>.
        /// </returns>
        private bool TryGetField(string fieldName, out Field field)
        {
            field = null;

            if (!string.IsNullOrEmpty(fieldName) && !this.fieldCache.TryGetValue(fieldName, out field))
            {
                field = this.WorkItem.Fields.OfType<Field>().FirstOrDefault(f => f.ReferenceName.Equals(fieldName) || f.Name.Equals(fieldName));

                this.fieldCache.Add(fieldName, field);
            }

            return field != null;
        }

        /// <summary>
        /// Transforms the specified link item.
        /// </summary>
        /// <param name="linkItem">The link item.</param>
        /// <returns>A related link to match the specified link item.</returns>
        private RelatedLink Transform(ILinkItem linkItem)
        {
            var isReverseEnd = linkItem.Child.Equals(this.WorkbenchItem);
            var relatedItemId = isReverseEnd
                                    ? linkItem.Parent.GetId()
                                    : linkItem.Child.GetId();

            WorkItemLinkTypeEnd linkTypeEnd;

            return this.TryGetLinkTypeEnd(linkItem.LinkName, isReverseEnd, out linkTypeEnd)
                       ? new RelatedLink(linkTypeEnd, relatedItemId)
                       : new RelatedLink(relatedItemId);
        }

        /// <summary>
        /// Tries the get link type end.
        /// </summary>
        /// <param name="linkName">Name of the link.</param>
        /// <param name="isReverseEnd">if set to <c>true</c> [is reverse end].</param>
        /// <param name="linkTypeEnd">The link type end.</param>
        /// <returns><c>True</c> if the link type end is found; otherwise <c>false</c>.</returns>
        private bool TryGetLinkTypeEnd(string linkName, bool isReverseEnd, out WorkItemLinkTypeEnd linkTypeEnd)
        {
            linkTypeEnd = null;

            if (this.WorkItem != null && !string.IsNullOrEmpty(linkName))
            {
                var linkType =
                    this.WorkItem.Store.WorkItemLinkTypes.FirstOrDefault(lt => lt.ReferenceName.Equals(linkName));

                if (linkType != null)
                {
                    linkTypeEnd = isReverseEnd ? linkType.ReverseEnd : linkType.ForwardEnd;
                }
            }

            return linkTypeEnd != null;
        }

        /// <summary>
        /// Tries to get an existing link.
        /// </summary>
        /// <param name="linkItem">The link item.</param>
        /// <param name="existingLink">The existing link.</param>
        /// <returns><c>True</c> if an existing link is found; otherwise <c>false</c>.</returns>
        private bool TryGetExistingLink(ILinkItem linkItem, out RelatedLink existingLink)
        {
            var linkCandidate = this.Transform(linkItem);

            existingLink = this.WorkItem.Links.OfType<RelatedLink>().FirstOrDefault(el => IsMatch(el, linkCandidate));

            return existingLink != null;
        }
    }
}