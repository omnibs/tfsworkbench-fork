// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalCommandLibrary.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the LocalCommandLibrary type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.FilterService
{
    using System.Windows.Input;

    /// <summary>
    /// The local command library class.
    /// </summary>
    internal class LocalCommandLibrary
    {
        /// <summary>
        /// The show filter viewer command.
        /// </summary>
        private static readonly RoutedUICommand showFilterDialogCommand = 
            new RoutedUICommand("Show Filter Dialog", "showFitlerDialog", typeof(LocalCommandLibrary));

        /// <summary>
        /// The RemoveFilter Command.
        /// </summary>
        private static readonly RoutedUICommand removeFilterCommand =
            new RoutedUICommand("Remove Filter", "removeFilter", typeof(LocalCommandLibrary));

        /// <summary>
        /// The AddFilter Command.
        /// </summary>
        private static readonly RoutedUICommand addFilterCommand =
            new RoutedUICommand("AddFilter", "addFilter", typeof(LocalCommandLibrary));

        /// <summary>
        /// The ClearAllFilters Command.
        /// </summary>
        private static readonly RoutedUICommand clearAllFiltersCommand =
            new RoutedUICommand("ClearAllFilters", "clearAllFilters", typeof(LocalCommandLibrary));

        /// <summary>
        /// The ApplyFilters Command.
        /// </summary>
        private static readonly RoutedUICommand applyFiltersCommand =
            new RoutedUICommand("ApplyFilters", "applyFilters", typeof(LocalCommandLibrary));

        /// <summary>
        /// Gets the ApplyFilters command.
        /// </summary>
        /// <value>The ApplyFilters Command.</value>
        public static RoutedUICommand ApplyFiltersCommand
        {
            get { return applyFiltersCommand; }
        }

        /// <summary>
        /// Gets the ClearAllFilters command.
        /// </summary>
        /// <value>The ClearAllFilters Command.</value>
        public static RoutedUICommand ClearAllFiltersCommand
        {
            get { return clearAllFiltersCommand; }
        }

        /// <summary>
        /// The SelectFilter Command.
        /// </summary>
        private static readonly RoutedUICommand selectFilterCommand =
            new RoutedUICommand("SelectFilter", "selectFilter", typeof(LocalCommandLibrary));

        /// <summary>
        /// Gets the SelectFilter command.
        /// </summary>
        /// <value>The SelectFilter Command.</value>
        public static RoutedUICommand SelectFilterCommand
        {
            get { return selectFilterCommand; }
        }

        /// <summary>
        /// Gets the AddFilter command.
        /// </summary>
        /// <value>The AddFilter Command.</value>
        public static RoutedUICommand AddFilterCommand
        {
            get { return addFilterCommand; }
        }

        /// <summary>
        /// Gets the RemoveFilter command.
        /// </summary>
        /// <value>The RemoveFilter Command.</value>
        public static RoutedUICommand RemoveFilterCommand
        {
            get { return removeFilterCommand; }
        }

        /// <summary>
        /// Gets the show fitler viewer command.
        /// </summary>
        /// <value>The show fitler viewer command.</value>
        public static RoutedUICommand ShowFilterDialogCommand
        {
            get { return showFilterDialogCommand; }
        }
    }
}
