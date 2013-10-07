// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalCommandLibrary.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the LocalCommandLibrary type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.HierarchyUI
{
    using System.Windows.Input;

    /// <summary>
    /// The local command library class.
    /// </summary>
    internal static class LocalCommandLibrary
    {
        /// <summary>
        /// The filter to item command.
        /// </summary>
        private static readonly RoutedUICommand filterToItemCommand =
            new RoutedUICommand("Filter", "filterToItem", typeof(LocalCommandLibrary));

        /// <summary>
        /// The filter to view command.
        /// </summary>
        private static readonly RoutedUICommand filterToViewCommand =
            new RoutedUICommand("Filter To View", "filterToView", typeof(LocalCommandLibrary));

        /// <summary>
        /// The edit view command.
        /// </summary>
        private static readonly RoutedUICommand editViewCommand =
            new RoutedUICommand("Edit View", "editView", typeof(LocalCommandLibrary));

        /// <summary>
        /// The add child item command.
        /// </summary>
        private static readonly RoutedUICommand addChildItemCommand =
            new RoutedUICommand("Add Child Item", "addChildItem", typeof(LocalCommandLibrary));

        /// <summary>
        /// The add child item command.
        /// </summary>
        private static readonly RoutedUICommand linkItemCommand =
            new RoutedUICommand("Link Item", "linkItem", typeof(LocalCommandLibrary));

        /// <summary>
        /// The unlink command.
        /// </summary>
        private static readonly RoutedUICommand unlinkItemCommand =
            new RoutedUICommand("Unlink Item", "unlinkItem", typeof(LocalCommandLibrary));

        /// <summary>
        /// Gets the filter to item command.
        /// </summary>
        /// <value>The filter to item command.</value>
        public static RoutedUICommand FilterToItemCommand
        {
            get
            {
                return filterToItemCommand;
            }
        }

        /// <summary>
        /// Gets the filter to view command.
        /// </summary>
        /// <value>The filter to view command.</value>
        public static RoutedUICommand FilterToViewCommand
        {
            get
            {
                return filterToViewCommand;
            }
        }
        
        /// <summary>
        /// Gets the edit view command.
        /// </summary>
        /// <value>The edit view command.</value>
        public static RoutedUICommand EditViewCommand
        {
            get
            {
                return editViewCommand;
            }
        }

        /// <summary>
        /// Gets the Add Child Item command.
        /// </summary>
        /// <value>The add child item command.</value>
        public static RoutedUICommand AddChildItemCommand
        {
            get
            {
                return addChildItemCommand;
            }
        }

        /// <summary>
        /// Gets the link item command.
        /// </summary>
        /// <value>The link item command.</value>
        public static RoutedUICommand LinkItemCommand
        {
            get
            {
                return linkItemCommand;
            }
        }

        /// <summary>
        /// Gets the unlink command.
        /// </summary>
        /// <value>The unlink command.</value>
        public static RoutedUICommand UnlinkItemCommand
        {
            get
            {
                return unlinkItemCommand;
            }
        }
    }
}
