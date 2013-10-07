// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkbenchItemChildCreator.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the ChildWorkbenchItemCreator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.WorkbenchItemGenerators
{
    using System;

    using TfsWorkbench.Core.Helpers;
    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.Core.Properties;
    using TfsWorkbench.Core.Services;

    /// <summary>
    /// The child workbench item creator class.
    /// </summary>
    internal class WorkbenchItemChildCreator : WorkbenchItemCreatorBase
    {
        /// <summary>
        /// The child creation parameters instance.
        /// </summary>
        private readonly IChildCreationParameters childCreationParameters;

        /// <summary>
        /// The link manager service instance.
        /// </summary>
        private readonly ILinkManagerService linkManagerService;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkbenchItemChildCreator"/> class.
        /// </summary>
        /// <param name="projectDataService">The project data service.</param>
        /// <param name="childCreationParameters">The child creation parameters.</param>
        public WorkbenchItemChildCreator(IProjectDataService projectDataService, IChildCreationParameters childCreationParameters)
            : this(projectDataService, childCreationParameters, ServiceManager.Instance.GetService<ILinkManagerService>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkbenchItemChildCreator"/> class.
        /// </summary>
        /// <param name="projectDataService">The project data service.</param>
        /// <param name="childCreationParameters">The child creation parameters.</param>
        /// <param name="linkManagerService">The link manager service.</param>
        public WorkbenchItemChildCreator(
            IProjectDataService projectDataService, 
            IChildCreationParameters childCreationParameters, 
            ILinkManagerService linkManagerService)
            : base(projectDataService.CurrentProjectData, projectDataService.CurrentDataProvider)
        {
            AssertParametersAreValid(linkManagerService, childCreationParameters);

            this.childCreationParameters = childCreationParameters;
            this.linkManagerService = linkManagerService;
        }

        /// <summary>
        /// Creates this instance.
        /// </summary>
        /// <returns>A new instance of the child workbench item.</returns>
        public override IWorkbenchItem Create()
        {
            var child = this.GenerateNewInstance(this.childCreationParameters.ChildTypeName);
            this.LinkParentAndChild(child);
            this.SetDefaultChildInheritanceValues(child);

            return child;
        }

        /// <summary>
        /// Asserts the parameters are valid.
        /// </summary>
        /// <param name="linkManagerService">The link manager service.</param>
        /// <param name="childCreationParameters">The child creation parameters.</param>
        private static void AssertParametersAreValid(ILinkManagerService linkManagerService, IChildCreationParameters childCreationParameters)
        {
            if (linkManagerService == null)
            {
                throw new ArgumentNullException("linkManagerService");
            }

            if (childCreationParameters == null)
            {
                throw new ArgumentNullException("childCreationParameters");
            }

            if (childCreationParameters.Parent == null)
            {
                throw new ArgumentException("No parent element specfied");
            }

            if (string.IsNullOrEmpty(childCreationParameters.ChildTypeName))
            {
                throw new Exception("No child type name specified");
            }
        }

        /// <summary>
        /// Sets the default child inheritance values.
        /// </summary>
        /// <param name="child">The child.</param>
        private void SetDefaultChildInheritanceValues(IWorkbenchItem child)
        {
            child[Settings.Default.TitleFieldName] = string.Concat("New ", this.childCreationParameters.ChildTypeName);
            child[Settings.Default.IterationPathFieldName] = this.childCreationParameters.Parent[Settings.Default.IterationPathFieldName];
            child[Settings.Default.AreaPathFieldName] = this.childCreationParameters.Parent[Settings.Default.AreaPathFieldName];
        }

        /// <summary>
        /// Links the parent and child.
        /// </summary>
        /// <param name="child">The child.</param>
        private void LinkParentAndChild(IWorkbenchItem child)
        {
            this.linkManagerService.AddLink(
                Factory.BuildLinkItem(
                    this.childCreationParameters.LinkTypeName, 
                    this.childCreationParameters.Parent, 
                    child));
        }
    }
}

