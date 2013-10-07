// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DragTargetHelper.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the DragTargetHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.TaskBoardUI.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

    using TfsWorkbench.Core.Interfaces;
    using TfsWorkbench.UIElements;
    using TfsWorkbench.UIElements.DragHelpers;

    /// <summary>
    /// The drag target helper.
    /// </summary>
    internal class DragTargetHelper
    {
        /// <summary>
        /// The element drag controller instance.
        /// </summary>
        private readonly IElementDragController<IWorkbenchItem> elementDragController;

        /// <summary>
        /// The registered drag target collection.
        /// </summary>
        private readonly IDictionary<FrameworkElement, IEnumerable<IDragTarget<IWorkbenchItem>>> registeredDragTargetCollections =
            new Dictionary<FrameworkElement, IEnumerable<IDragTarget<IWorkbenchItem>>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="DragTargetHelper"/> class.
        /// </summary>
        /// <param name="elementDragController">The element drag controller.</param>
        public DragTargetHelper(IElementDragController<IWorkbenchItem> elementDragController)
        {
            if (elementDragController == null)
            {
                throw new ArgumentNullException("elementDragController");
            }

            this.elementDragController = elementDragController;
        }

        /// <summary>
        /// Synchronises the drag elements.
        /// </summary>
        /// <param name="itemsControl">The items control.</param>
        public void SynchroniseDragElements(ItemsControl itemsControl)
        {
            if (itemsControl == null)
            {
                throw new ArgumentNullException("itemsControl");
            }

            var generator = itemsControl.ItemContainerGenerator;

            if (generator == null || generator.Status != GeneratorStatus.ContainersGenerated)
            {
                return;
            }

            foreach (var collection in
                itemsControl.Items.OfType<object>()
                .Select(generator.ContainerFromItem).OfType<FrameworkElement>())
            {
                this.RegisterCollectionIfMissing(collection);
            }
        }

        /// <summary>
        /// Unregisters drag target collection.
        /// </summary>
        /// <param name="dragTargetCollection">The drag target collection.</param>
        public void UnregisterCollection(FrameworkElement dragTargetCollection)
        {
            IEnumerable<IDragTarget<IWorkbenchItem>> dragTargets;
            if (!this.registeredDragTargetCollections.TryGetValue(dragTargetCollection, out dragTargets))
            {
                return;
            }

            foreach (var dragTarget in dragTargets)
            {
                this.elementDragController.ReleaseDragTarget(dragTarget);
            }

            this.registeredDragTargetCollections.Remove(dragTargetCollection);
        }

        /// <summary>
        /// Unregisters all collections.
        /// </summary>
        public void UnregisterAllCollections()
        {
            foreach (var registeredDragTargetCollection in this.registeredDragTargetCollections.Keys.ToArray())
            {
                this.UnregisterCollection(registeredDragTargetCollection);
            }
        }

        /// <summary>
        /// Registers the drag target collection.
        /// </summary>
        /// <param name="dragTargetCollection">The drag target collection.</param>
        private void RegisterCollectionIfMissing(FrameworkElement dragTargetCollection)
        {
            if (dragTargetCollection == null || this.registeredDragTargetCollections.ContainsKey(dragTargetCollection))
            {
                return;
            }

            IEnumerable<DragListView> dragTargets = dragTargetCollection.GetAllChildElementsOfType<DragListView>().ToArray();

            if (!dragTargets.Any())
            {
                return;
            }

            foreach (var dragTarget in dragTargets)
            {
                this.elementDragController.RegisterDragTarget(dragTarget);
            }

            this.registeredDragTargetCollections.Add(dragTargetCollection, dragTargets);
        }
    }
}
