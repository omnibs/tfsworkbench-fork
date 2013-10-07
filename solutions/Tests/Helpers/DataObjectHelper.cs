// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataObjectHelper.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the DataObjectHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Tests.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Rhino.Mocks;

    using TfsWorkbench.Core.DataObjects;
    using TfsWorkbench.Core.Helpers;
    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.Core.Properties;
    using TfsWorkbench.Core.Services;

    /// <summary>
    /// The dat object helper class.
    /// </summary>
    public static class DataObjectHelper
    {
        /// <summary>
        /// The "State 1".
        /// </summary>
        public const string State1 = "State 1";

        /// <summary>
        /// The "State 2".
        /// </summary>
        public const string State2 = "State 2";

        /// <summary>
        /// The excluded sate.
        /// </summary>
        public const string ExcludedState = "ExcludedState";

        /// <summary>
        /// The bucket state.
        /// </summary>
        public const string BucketState = "BucketState";

        /// <summary>
        /// The parent type.
        /// </summary>
        public const string ParentType = "Parent Type";

        /// <summary>
        /// The child type.
        /// </summary>
        public const string ChildType = "Child Type";

        /// <summary>
        /// The link type.
        /// </summary>
        public const string LinkType1 = "Link.Name.1";

        /// <summary>
        /// The field values collection.
        /// </summary>
        private static readonly Collection<Tuple<IWorkbenchItem, IDictionary<string, object>>> fieldValues =
            new Collection<Tuple<IWorkbenchItem, IDictionary<string, object>>>();

        /// <summary>
        /// Generates the project data.
        /// </summary>
        /// <param name="parents">The parents.</param>
        /// <param name="children">The children.</param>
        /// <param name="orphans">The orphans.</param>
        /// <param name="bucketChildren">The bucket children.</param>
        /// <returns>A demo project data instance.</returns>
        public static IProjectData GenerateProjectData(int parents = 0, int children = 0, int orphans = 0, int bucketChildren = 0)
        {
            var view = CreateViewMap();

            view.SwimLaneStates.Add(State1);
            view.SwimLaneStates.Add(State2);
            view.BucketStates.Add(BucketState);

            var projectData = CreateProjectData()
                .AddItemTypeData(new ItemTypeData { TypeName = ParentType })
                .AddItemTypeData(new ItemTypeData { TypeName = ChildType })
                .AddViewMap(view)
                .AddLinkType(LinkType1);

            for (int i = 0; i < parents; i++)
            {
                var parentItem = CreateWorkbenchItem()
                    .SetFieldValue(Settings.Default.TypeFieldName, ParentType)
                    .SetFieldValue(Settings.Default.StateFieldName, State1);

                projectData = projectData.AddWorkItem(parentItem);

                for (int j = 0; j < children; j++)
                {
                    var childItem = CreateWorkbenchItem()
                        .SetFieldValue(Settings.Default.TypeFieldName, ChildType)
                        .SetFieldValue(Settings.Default.StateFieldName, State1);

                    projectData = projectData.AddWorkItem(childItem);

                    parentItem = parentItem.LinkChild(childItem, LinkType1);
                }

                for (int j = 0; j < bucketChildren; j++)
                {
                    var childItem = CreateWorkbenchItem()
                        .SetFieldValue(Settings.Default.TypeFieldName, ChildType)
                        .SetFieldValue(Settings.Default.StateFieldName, BucketState);

                    projectData = projectData.AddWorkItem(childItem);

                    parentItem = parentItem.LinkChild(childItem, LinkType1);
                }
            }

            for (int i = 0; i < orphans; i++)
            {
                var orphan = CreateWorkbenchItem()
                    .SetFieldValue(Settings.Default.TypeFieldName, ChildType)
                    .SetFieldValue(Settings.Default.StateFieldName, State1);

                projectData = projectData.AddWorkItem(orphan);
            }

            return projectData;
        }

        /// <summary>
        /// Creates the view map.
        /// </summary>
        /// <param name="parentTypes">The parent types.</param>
        /// <param name="childType">Type of the child.</param>
        /// <param name="linkName">Name of the link.</param>
        /// <param name="displayOrder">The display order.</param>
        /// <returns>A new instance of a view map.</returns>
        public static ViewMap CreateViewMap(IEnumerable<string> parentTypes = null, string childType = null, string linkName = null, int? displayOrder = null)
        {
            var viewMap = new ViewMap
                {
                    ChildType = childType ?? ChildType, 
                    LinkName = linkName ?? LinkType1,
                    DisplayOrder = displayOrder ?? 0
                };

            if (parentTypes == null)
            {
                viewMap.ParentTypes.Add(ParentType);
            }
            else
            {
                foreach (var parentType in parentTypes)
                {
                    viewMap.ParentTypes.Add(parentType);
                }
            }

            return viewMap;
        }

        /// <summary>
        /// Creates the project data.
        /// </summary>
        /// <returns>A mock instance of the project data class.</returns>
        public static IProjectData CreateProjectData()
        {
            var projectData = MockRepository.GenerateMock<IProjectData>();
            
            var viewCollection = new ObservableCollection<ViewMap>();
            projectData.Expect(pd => pd.ViewMaps).Return(viewCollection).Repeat.Any();

            var itemTypeDataCollection = new ItemTypeDataCollection();
            projectData.Expect(pd => pd.ItemTypes).Return(itemTypeDataCollection).Repeat.Any();

            var workbenchIteCollection = MockRepository.GenerateMock<IWorkbenchItemRepository>();
            var itemCollection = new List<IWorkbenchItem>();
            workbenchIteCollection.Expect(r => r.GetEnumerator())
                .WhenCalled(mi => mi.ReturnValue = itemCollection.GetEnumerator())
                .Return(null)
                .Repeat.Any();
            workbenchIteCollection.Expect(r => r.Add(null))
                .IgnoreArguments()
                .WhenCalled(wi => itemCollection.Add(wi.Arguments.First() as IWorkbenchItem))
                .Repeat.Any();
            workbenchIteCollection.Expect(r => r.UnfilteredList)
                .Return(itemCollection)
                .Repeat.Any();

            projectData.Expect(pd => pd.WorkbenchItems).Return(workbenchIteCollection).Repeat.Any();

            var linkTypes = new Collection<string>();
            projectData.Expect(pd => pd.LinkTypes).Return(linkTypes).Repeat.Any();

            return projectData;
        }

        /// <summary>
        /// Appends the view map.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <param name="viewMap">The view map.</param>
        /// <returns>The project data include the view map.</returns>
        public static IProjectData AddViewMap(this IProjectData projectData, ViewMap viewMap)
        {
            var collection = projectData.ViewMaps;

            collection.Add(viewMap);

            return projectData;
        }

        /// <summary>
        /// Adds the type of the item.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <param name="itemTypeData">The item type data.</param>
        /// <returns>The project data including the specified item type.</returns>
        public static IProjectData AddItemTypeData(this IProjectData projectData, ItemTypeData itemTypeData)
        {
            projectData.ItemTypes.Add(itemTypeData);

            return projectData;
        }

        /// <summary>
        /// Adds the type of the link.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <param name="linkType">Type of the link.</param>
        /// <returns>The project data including teh specified link type.</returns>
        public static IProjectData AddLinkType(this IProjectData projectData, string linkType)
        {
            projectData.LinkTypes.Add(linkType);

            return projectData;
        }
        
        /// <summary>
        /// Adds the work item.
        /// </summary>
        /// <param name="projectData">The project data.</param>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <returns>The project data including reference to the specified work item.</returns>
        public static IProjectData AddWorkItem(this IProjectData projectData, IWorkbenchItem workbenchItem)
        {
            var collection = projectData.WorkbenchItems;

            collection.Add(workbenchItem);

            return projectData;
        }
      
        /// <summary>
        /// Creates the workbench item.
        /// </summary>
        /// <returns>A mocked workbench item.</returns>
        public static IWorkbenchItem CreateWorkbenchItem()
        {
            var workbenchItem = MockRepository.GenerateMock<IWorkbenchItem>();

            workbenchItem.Expect(w => w.ChildLinks).Return(workbenchItem.GetChildLinks()).Repeat.Any();

            workbenchItem.Expect(w => w.ParentLinks).Return(workbenchItem.GetParentLinks()).Repeat.Any();

            var valueProvider = MockRepository.GenerateMock<IValueProvider>();

            workbenchItem.Expect(w => w.ValueProvider).Return(valueProvider).Repeat.Any();

            return workbenchItem;
        }

        /// <summary>
        /// Appends the field value.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="value">The value.</param>
        /// <returns>The workbench item including the specified field.</returns>
        public static IWorkbenchItem SetFieldValue(this IWorkbenchItem workbenchItem, string fieldName, object value)
        {
            bool isNew;
            SetFieldValue(workbenchItem, fieldName, value, out isNew);

            if (isNew)
            {
                workbenchItem.Expect(w => w[fieldName]).WhenCalled(mi => mi.ReturnValue = GetFieldValue(workbenchItem, fieldName)).Return(null).Repeat.Any();
            }

            return workbenchItem;
        }

        /// <summary>
        /// Links the child.
        /// </summary>
        /// <param name="parentItem">The workbench item.</param>
        /// <param name="childItem">The child item.</param>
        /// <param name="linkName">Name of the link.</param>
        /// <returns>The workbench item include the child link.</returns>
        public static IWorkbenchItem LinkChild(this IWorkbenchItem parentItem, IWorkbenchItem childItem, string linkName)
        {
            var linkManagerService = ServiceManager.Instance.GetService<ILinkManagerService>();
            linkManagerService.AddLink(Factory.BuildLinkItem(linkName, parentItem, childItem));

            return parentItem;
        }

        /// <summary>
        /// Sets the field value.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="value">The value.</param>
        /// <param name="isNew">If set to <c>true</c> [is new].</param>
        private static void SetFieldValue(IWorkbenchItem workbenchItem, string fieldName, object value, out bool isNew)
        {
            var fieldMap = fieldValues.FirstOrDefault(fv => fv.Item1.Equals(workbenchItem));

            if (fieldMap == null)
            {
                fieldMap = new Tuple<IWorkbenchItem, IDictionary<string, object>>(
                    workbenchItem, new Dictionary<string, object>());

                fieldValues.Add(fieldMap);
            }

            isNew = !fieldMap.Item2.ContainsKey(fieldName);

            if (isNew)
            {
                fieldMap.Item2.Add(fieldName, value);
            }
            else
            {
                fieldMap.Item2[fieldName] = value;
            }
        }

        /// <summary>
        /// Gets the field value.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>The field value.</returns>
        private static object GetFieldValue(IWorkbenchItem workbenchItem, string fieldName)
        {
            var fieldMap = fieldValues.FirstOrDefault(fv => fv.Item1.Equals(workbenchItem));

            if (fieldMap == null || !fieldMap.Item2.ContainsKey(fieldName))
            {
                return null;
            }

            return fieldMap.Item2[fieldName];
        }
    }
}