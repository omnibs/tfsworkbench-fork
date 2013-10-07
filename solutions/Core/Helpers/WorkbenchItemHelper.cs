// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkbenchItemHelper.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the WorkbenchItemHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.Helpers
{
    using System;

    using DataObjects;

    using Interfaces;

    using Properties;

    using TfsWorkbench.Core.Services;

    /// <summary>
    /// Initializes instance of WorkbenchItemHelper
    /// </summary>
    public static class WorkbenchItemHelper
    {
        /// <summary>
        /// The internal getId method.
        /// </summary>
        private static readonly Func<IWorkbenchItem, int> getId = tbi => (int)tbi[Settings.Default.IdFieldName];

        /// <summary>
        /// The internal getState method.
        /// </summary>
        private static readonly Func<IWorkbenchItem, string> getState = tbi => (string)tbi[Settings.Default.StateFieldName];

        /// <summary>
        /// The internal getType method.
        /// </summary>
        private static readonly Func<IWorkbenchItem, string> getType = tbi => (string)tbi[Settings.Default.TypeFieldName];

        /// <summary>
        /// The internal getId method.
        /// </summary>
        private static readonly Action<IWorkbenchItem, int> setId = (tbi, value) => tbi[Settings.Default.IdFieldName] = value;

        /// <summary>
        /// The internal getState method.
        /// </summary>
        private static readonly Action<IWorkbenchItem, string> setState = (tbi, value) => tbi[Settings.Default.StateFieldName] = value;

        /// <summary>
        /// The internal getType method.
        /// </summary>
        private static readonly Action<IWorkbenchItem, string> setType = (tbi, value) => tbi[Settings.Default.TypeFieldName] = value;

        /// <summary>
        /// Gets the id.
        /// </summary>
        /// <param name="workbenchItem">The task board item.</param>
        /// <returns>The task board item id.</returns>
        public static int GetId(this IWorkbenchItem workbenchItem)
        {
            return getId(workbenchItem);
        }

        /// <summary>
        /// Gets the Type.
        /// </summary>
        /// <param name="workbenchItem">The task board item.</param>
        /// <returns>The task board item Type.</returns>
        public static string GetTypeName(this IWorkbenchItem workbenchItem)
        {
            return getType(workbenchItem);
        }

        /// <summary>
        /// Gets the State.
        /// </summary>
        /// <param name="workbenchItem">The task board item.</param>
        /// <returns>The task board item State.</returns>
        public static string GetState(this IWorkbenchItem workbenchItem)
        {
            return getState(workbenchItem);
        }

        /// <summary>
        /// Gets the revision.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <returns>The work benchitem revision number.</returns>
        public static int GetRevision(this IWorkbenchItem workbenchItem)
        {
            return (int)workbenchItem[Settings.Default.RevisionFieldName];
        }

        /// <summary>
        /// Gets the caption.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <returns>The workbench item caption.</returns>
        public static string GetCaption(this IWorkbenchItem workbenchItem)
        {
            var caption = workbenchItem[GetCaptionFieldName(workbenchItem.GetTypeName())];
            return caption == null ? null : caption.ToString();
        }

        /// <summary>
        /// Gets the body text.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <returns>The workbench item body text.</returns>
        public static string GetBody(this IWorkbenchItem workbenchItem)
        {
            return workbenchItem[GetBodyFieldName(workbenchItem.GetTypeName())] as string;
        }

        /// <summary>
        /// Gets the metric.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <returns>The metric value of the specfied workbench item.</returns>
        public static object GetMetric(this IWorkbenchItem workbenchItem)
        {
            return workbenchItem[GetMetricFieldName(workbenchItem.GetTypeName())];
        }

        /// <summary>
        /// Gets the body text.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <returns>The workbench item body text.</returns>
        public static string GetOwner(this IWorkbenchItem workbenchItem)
        {
            return workbenchItem[GetOwnerFieldName(workbenchItem.GetTypeName())] as string;
        }

        /// <summary>
        /// Gets the name of the caption field.
        /// </summary>
        /// <param name="workbenchItemTypeName">Name of the workbench item type.</param>
        /// <returns>The work item caption field name.</returns>
        public static string GetCaptionFieldName(string workbenchItemTypeName)
        {
            ItemTypeData typeData;
            if (!TryGetItemTypeData(workbenchItemTypeName, out typeData))
            {
                return null;
            }

            return string.IsNullOrEmpty(typeData.CaptionField)
                       ? Settings.Default.TitleFieldName
                       : typeData.CaptionField;
        }

        /// <summary>
        /// Gets the name of the body field.
        /// </summary>
        /// <param name="workbenchItemTypeName">Name of the workbench item type.</param>
        /// <returns>The selected body field name.</returns>
        public static string GetBodyFieldName(string workbenchItemTypeName)
        {
            ItemTypeData typeData;
            return TryGetItemTypeData(workbenchItemTypeName, out typeData) ? typeData.BodyField : null;
        }

        /// <summary>
        /// Gets the name of the metric field.
        /// </summary>
        /// <param name="workbenchItemTypeName">Name of the workbench item type.</param>
        /// <returns>The name of the selected metric field.</returns>
        public static string GetMetricFieldName(string workbenchItemTypeName)
        {
            ItemTypeData typeData;
            return TryGetItemTypeData(workbenchItemTypeName, out typeData) ? typeData.NumericField : null;
        }
        
        /// <summary>
        /// Gets the name of the owner field.
        /// </summary>
        /// <param name="workbenchItemTypeName">Name of the workbench item type.</param>
        /// <returns>The work item owner field name.</returns>
        public static string GetOwnerFieldName(string workbenchItemTypeName)
        {
            ItemTypeData typeData;
            return TryGetItemTypeData(workbenchItemTypeName, out typeData) ? typeData.OwnerField : null;
        }
        
        /// <summary>
        /// Sets the id.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <param name="id">The id value.</param>
        public static void SetId(this IWorkbenchItem workbenchItem, int id)
        {
            setId(workbenchItem, id);
        }

        /// <summary>
        /// Sets the Type.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <param name="type">The Type value.</param>
        public static void SetType(this IWorkbenchItem workbenchItem, string type)
        {
            setType(workbenchItem, type);
        }

        /// <summary>
        /// Sets the State.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <param name="state">The state value.</param>
        public static void SetState(this IWorkbenchItem workbenchItem, string state)
        {
            setState(workbenchItem, state);
        }

        /// <summary>
        /// Determines whether the specified workbench item is excluded.
        /// </summary>
        /// <param name="workbenchItem">The workbench item.</param>
        /// <returns>
        /// <c>true</c> if the specified workbench item is excluded; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsExcluded(this IWorkbenchItem workbenchItem)
        {
            return ServiceManager.Instance.GetService<IFilterService>().IsExcluded(workbenchItem);
        }

        /// <summary>
        /// Tries to get the item type data.
        /// </summary>
        /// <param name="itemTypeName">Name of the item type.</param>
        /// <param name="itemTypeData">The item type data.</param>
        /// <returns><c>True</c> if the type data is found; otherwise <c>false</c>.</returns>
        private static bool TryGetItemTypeData(string itemTypeName, out ItemTypeData itemTypeData)
        {
            var projectData = ServiceManager.Instance.GetService<IProjectDataService>().CurrentProjectData;

            if (projectData == null)
            {
                itemTypeData = null;
            }
            else
            {
                projectData.ItemTypes.TryGetValue(itemTypeName, out itemTypeData);
            }

            return itemTypeData != null;
        }
    }
}
