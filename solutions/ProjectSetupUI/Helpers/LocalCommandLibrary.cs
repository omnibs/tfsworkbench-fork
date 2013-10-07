// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalCommandLibrary.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the LocalCommandLibrary type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.ProjectSetupUI.Helpers
{
    using System.Windows.Input;

    /// <summary>
    /// Initializes instance of LocalCommandLibrary
    /// </summary>
    public static class LocalCommandLibrary
    {
        /// <summary>
        /// The show quick setup command instance.
        /// </summary>
        private static readonly RoutedUICommand showQuickSetupCommand =
            new RoutedUICommand("Quick Setup", "showQuickSetup", typeof(LocalCommandLibrary));

        /// <summary>
        /// The show advanced setup command instance.
        /// </summary>
        private static readonly RoutedUICommand showAdvancedSetupCommand =
            new RoutedUICommand("Advanced Setup", "showAdvancedSetup", typeof(LocalCommandLibrary));

        /// <summary>
        /// The add child node command.
        /// </summary>
        private static readonly RoutedUICommand addChildNodeCommand =
            new RoutedUICommand("Add Child", "addChildNode", typeof(LocalCommandLibrary));

        /// <summary>
        /// The add child node command.
        /// </summary>
        private static readonly RoutedUICommand deleteNodeCommand =
            new RoutedUICommand("Delete", "deleteNode", typeof(LocalCommandLibrary));

        /// <summary>
        /// The filter on node command.
        /// </summary>
        private static readonly RoutedUICommand filterOnNodeCommand =
            new RoutedUICommand("Show Descendents Only", "filterOnNode", typeof(LocalCommandLibrary));

        /// <summary>
        /// The rename node command.
        /// </summary>
        private static readonly RoutedUICommand renameNodeCommand =
            new RoutedUICommand("Rename", "renameNode", typeof(LocalCommandLibrary));

        /// <summary>
        /// The delete duration structure command.
        /// </summary>
        private static readonly RoutedUICommand deleteNamedItemCommand =
            new RoutedUICommand("Delete", "deleteDurationStructure", typeof(LocalCommandLibrary));

        /// <summary>
        /// The new Associated Item command.
        /// </summary>
        private static readonly RoutedUICommand newAssociatedItemCommand =
            new RoutedUICommand("New Associated Item", "newAssociatedItem", typeof(LocalCommandLibrary));

        /// <summary>
        /// Gets the show Advanced setup command.
        /// </summary>
        /// <value>The show Advanced setup command.</value>
        public static RoutedUICommand ShowAdvancedSetupCommand
        {
            get
            {
                return showAdvancedSetupCommand;
            }
        }

        /// <summary>
        /// Gets the show quick setup command.
        /// </summary>
        /// <value>The show quick setup command.</value>
        public static RoutedUICommand ShowQuickSetupCommand
        {
            get
            {
                return showQuickSetupCommand;
            }
        }

        /// <summary>
        /// Gets the add child node command.
        /// </summary>
        /// <value>The add child node command.</value>
        public static RoutedUICommand AddChildNodeCommand
        {
            get { return addChildNodeCommand; }
        }

        /// <summary>
        /// Gets the delete node command.
        /// </summary>
        /// <value>The delete node command.</value>
        public static RoutedUICommand DeleteNodeCommand
        {
            get { return deleteNodeCommand; }
        }

        /// <summary>
        /// Gets the filter on node command.
        /// </summary>
        /// <value>The filter on node command.</value>
        public static RoutedUICommand FilterOnNodeCommand
        {
            get { return filterOnNodeCommand; }
        }

        /// <summary>
        /// Gets the rename node command.
        /// </summary>
        /// <value>The rename node command.</value>
        public static RoutedUICommand RenameNodeCommand
        {
            get { return renameNodeCommand; }
        }

        /// <summary>
        /// Gets the delete name item command.
        /// </summary>
        /// <value>The delete name item command.</value>
        public static RoutedUICommand DeleteNamedItemCommand
        {
            get { return deleteNamedItemCommand; }
        }

        /// <summary>
        /// Gets the new associated item command.
        /// </summary>
        /// <value>The new associated item command.</value>
        public static RoutedUICommand NewAssociatedItemCommand
        {
            get { return newAssociatedItemCommand; }
        }
    }
}