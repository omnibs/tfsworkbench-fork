// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFilterServiceController.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the IFilterServiceController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.FilterService
{
    using System.Windows.Input;

    /// <summary>
    /// The filter service controller interface.
    /// </summary>
    public interface IFilterServiceController
    {
        /// <summary>
        /// Gets the filters.
        /// </summary>
        /// <value>The filters.</value>
        WorkbenchFilterCollection Filters { get; }

        /// <summary>
        /// Gets the filter info.
        /// </summary>
        /// <value>The filter info.</value>
        FilterInfo FilterInfo { get; }

        /// <summary>
        /// Shows the filter dialog.
        /// </summary>
        /// <param name="executedRoutedEventArgs">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        void ShowFilterDialog(ExecutedRoutedEventArgs executedRoutedEventArgs);

        /// <summary>
        /// Determines whether this instance [can show filter dialog].
        /// </summary>
        /// <param name="canExecuteRoutedEventArgs">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        void CanShowFilterDialog(CanExecuteRoutedEventArgs canExecuteRoutedEventArgs);

        /// <summary>
        /// Executes the apply filters command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        void ExecuteApplyFiltersCommand(object sender, ExecutedRoutedEventArgs e);

        /// <summary>
        /// Determines whether this instance [can clear all filters] the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.CanExecuteRoutedEventArgs"/> instance containing the event data.</param>
        void CanClearAllFilters(object sender, CanExecuteRoutedEventArgs e);

        /// <summary>
        /// Executes the clear all filters command.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        void ExecuteClearAllFiltersCommand(object sender, ExecutedRoutedEventArgs e);

        /// <summary>
        /// Executes the clear and reapply.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.ExecutedRoutedEventArgs"/> instance containing the event data.</param>
        void ExecuteClearAllAndReApply(object sender, ExecutedRoutedEventArgs e);

        /// <summary>
        /// Closes the filter dialog.
        /// </summary>
        void CloseFilterDialog();
    }
}