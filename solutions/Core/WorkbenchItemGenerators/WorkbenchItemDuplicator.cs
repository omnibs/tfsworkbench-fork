// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkbenchItemDuplicator.cs" company="None">
//   None
// </copyright>
// <summary>
//   The workbench item duplicator class.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.WorkbenchItemGenerators
{
    using System;
    using System.Linq;

    using TfsWorkbench.Core.Helpers;
    using TfsWorkbench.Core.Interfaces;

    /// <summary>
    /// The workbench item duplicator class.
    /// </summary>
    internal class WorkbenchItemDuplicator : WorkbenchItemCreatorBase
    {
        /// <summary>
        /// The source work item.
        /// </summary>
        private readonly IWorkbenchItem sourceItem;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkbenchItemDuplicator"/> class.
        /// </summary>
        /// <param name="projectDataService">The project data service.</param>
        /// <param name="sourceItem">The source item.</param>
        public WorkbenchItemDuplicator(IProjectDataService projectDataService, IWorkbenchItem sourceItem)
            : base(projectDataService.CurrentProjectData, projectDataService.CurrentDataProvider)
        {
            if (sourceItem == null)
            {
                throw new ArgumentNullException("sourceItem");
            }

            this.sourceItem = sourceItem;
        }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns>A new instance of the child workbench item.</returns>
        public override IWorkbenchItem Create()
        {
            var duplicate = this.GenerateNewInstance(this.sourceItem.GetTypeName());

            this.ApplyDuplicatedFieldValues(duplicate);

            return duplicate;
        }

        /// <summary>
        /// Applies the duplicated field values.
        /// </summary>
        /// <param name="duplicate">The duplicate.</param>
        private void ApplyDuplicatedFieldValues(IWorkbenchItem duplicate)
        {
            var itemTypeInfo = this.ProjectData.ItemTypes.First(it => it.TypeName == duplicate.GetTypeName());

            foreach (var duplicationField in itemTypeInfo.DuplicationFields)
            {
                duplicate[duplicationField] = this.sourceItem[duplicationField];
            }
        }
    }
}