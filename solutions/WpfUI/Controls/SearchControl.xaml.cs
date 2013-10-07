// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SearchControl.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   Interaction logic for SearchControl.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.WpfUI.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Threading;

    using Core.Helpers;
    using Core.Interfaces;
    using Core.Properties;

    using TfsWorkbench.Core.Services;

    using UIElements;

    /// <summary>
    /// Interaction logic for SearchControl.xaml
    /// </summary>
    public partial class SearchControl
    {
        /// <summary>
        /// The view in text.
        /// </summary>
        public const string ViewIn = "[View In]";

        /// <summary>
        /// The id column title.
        /// </summary>
        public const string IdColumnTitle = "Id";

        /// <summary>
        /// The caption column title.
        /// </summary>
        public const string CaptionColumnTitle = "Caption";

        /// <summary>
        /// The state column title.
        /// </summary>
        public const string StateColumnTitle = "State";

        /// <summary>
        /// The type column title.
        /// </summary>
        public const string TypeColumnTitle = "Type";

        /// <summary>
        /// The project data property.
        /// </summary>
        private static readonly DependencyProperty projectDataProperty = DependencyProperty.Register(
            "ProjectData",
            typeof(IProjectData),
            typeof(SearchControl));

        /// <summary>
        /// The result sorter.
        /// </summary>
        private readonly ResultsSorter resultSorter = new ResultsSorter();

        /// <summary>
        /// The is first render flag.
        /// </summary>
        private bool isFirstRender;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchControl"/> class.
        /// </summary>
        public SearchControl()
            : this(ServiceManager.Instance.GetService<IProjectDataService>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchControl"/> class.
        /// </summary>
        /// <param name="projectDataService">The project data service.</param>
        public SearchControl(IProjectDataService projectDataService)
        {
            if (projectDataService == null)
            {
                throw new ArgumentNullException("projectDataService");
            }

            InitializeComponent();

            this.MatchedItems = new SortableObservableCollection<IWorkbenchItem>();
            this.SearchProviders = new Dictionary<string, IHighlightProvider> { { ViewIn, null } };

            foreach (var searchProvider in projectDataService.HighlightProviders)
            {
                this.SearchProviders.Add(searchProvider.Title, searchProvider);
            }

            this.isFirstRender = true;
        }

        /// <summary>
        /// Gets the project data property.
        /// </summary>
        /// <value>The project data property.</value>
        public static DependencyProperty ProjectDataProperty
        {
            get { return projectDataProperty; }
        }

        /// <summary>
        /// Gets or sets the project data.
        /// </summary>
        /// <value>The project data.</value>
        public IProjectData ProjectData
        {
            get { return (IProjectData)this.GetValue(ProjectDataProperty); }
            set { this.SetValue(ProjectDataProperty, value); }
        }

        /// <summary>
        /// Gets the search modes.
        /// </summary>
        /// <value>The search modes.</value>
        public IDictionary<string, IHighlightProvider> SearchProviders { get; private set; }

        /// <summary>
        /// Gets the matched items.
        /// </summary>
        /// <value>The matched items.</value>
        public SortableObservableCollection<IWorkbenchItem> MatchedItems { get; private set; }

        /// <summary>
        /// When overridden in a derived class, participates in rendering operations that are directed by the layout system. The rendering instructions for this element are not used directly when this method is invoked, and are instead preserved for later asynchronous use by layout and drawing.
        /// </summary>
        /// <param name="drawingContext">The drawing instructions for a specific element. This context is provided to the layout system.</param>
        protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (!this.isFirstRender)
            {
                return;
            }

            this.isFirstRender = false;

            var callback = new EventHandler(
                (o, a) =>
                {
                    this.PART_SearchTerm.Focus();

                    ((DispatcherTimer)o).Stop();
                });

            var timer = new DispatcherTimer(
                TimeSpan.FromMilliseconds(500),
                DispatcherPriority.SystemIdle,
                callback,
                this.Dispatcher);

            timer.Start();
        }

        /// <summary>
        /// Handles the Click event of the CloseButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            CommandLibrary.CloseDialogCommand.Execute(this, Application.Current.MainWindow);

            this.ReleaseReferencedObjects();
        }

        /// <summary>
        /// Releases the referenced objects.
        /// </summary>
        private void ReleaseReferencedObjects()
        {
            this.ProjectData = null;
        }

        /// <summary>
        /// Handles the Click event of the SearchButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void SearchButtonClick(object sender, RoutedEventArgs e)
        {
            this.DoSearch();
        }

        /// <summary>
        /// Does the search.
        /// </summary>
        private void DoSearch()
        {
            if (this.ProjectData == null)
            {
                return;
            }

            var searchTerm = this.PART_SearchTerm.Text.ToLower();

            if (string.IsNullOrEmpty(searchTerm))
            {
                return;
            }

            Func<IWorkbenchItem, bool> isIdMatch;

            int parsedId;
            if (int.TryParse(searchTerm, out parsedId))
            {
                isIdMatch = w => parsedId.Equals(w.GetId()); 
            }
            else
            {
                isIdMatch = w => false;
            }

            Func<IWorkbenchItem, bool> isMatch = w =>
                {
                    var title = w.GetCaption();
                    var description = w.GetBody();

                    return isIdMatch(w) 
                            || (!string.IsNullOrEmpty(title) && title.ToLower().Contains(searchTerm)) 
                            || (!string.IsNullOrEmpty(description) && description.ToLower().Contains(searchTerm));
                };
            
            this.MatchedItems.Clear();

            var matchedItems = this.ProjectData.WorkbenchItems.Where(isMatch);

            foreach (var workbenchItem in matchedItems)
            {
                this.MatchedItems.Add(workbenchItem);
            }

            this.PART_Status.Text = string.Concat(matchedItems.Count(), " item(s) matched");

            var binding = this.PART_ResultsList.GetBindingExpression(ItemsControl.ItemsSourceProperty);
            if (binding != null)
            {
                binding.UpdateTarget();
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the Goto control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void GotoSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var source = sender as ComboBox;

            if (source == null)
            {
                return;
            }

            var selectedItem = source.SelectedItem as string;
            var item = source.DataContext as IWorkbenchItem;

            if (string.IsNullOrEmpty(selectedItem) || item == null)
            {
                return;
            }

            e.Handled = true;

            IHighlightProvider highlightProvider;
            if (!this.SearchProviders.TryGetValue(selectedItem, out highlightProvider) || highlightProvider == null)
            {
                return;
            }

            this.IsEnabled = false;

            CommandLibrary.ShowItemInCommand.Execute(new Tuple<IWorkbenchItem, IHighlightProvider>(item, highlightProvider), this);

            this.Dispatcher.BeginInvoke(
                DispatcherPriority.ApplicationIdle,
                (SendOrPostCallback)delegate { CommandLibrary.CloseDialogCommand.Execute(this, Application.Current.MainWindow); },
                null);

            this.ReleaseReferencedObjects();
        }

        /// <summary>
        /// Handles the KeyDown event of the SearchTerm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void SearchTermKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                this.DoSearch();
            }
        }

        /// <summary>
        /// Handles the Click event of the ResultList control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void ResultListClick(object sender, RoutedEventArgs e)
        {
            var headerClicked = e.OriginalSource as GridViewColumnHeader;

            if (headerClicked == null || headerClicked.Role == GridViewColumnHeaderRole.Padding)
            {
                return;
            }

            var columnTitle = headerClicked.Content as string;

            if (string.IsNullOrEmpty(columnTitle))
            {
                return;
            }

            switch (columnTitle)
            {
                case IdColumnTitle:
                    this.resultSorter.SetSortField(Settings.Default.IdFieldName);
                    break;

                case CaptionColumnTitle:
                    this.resultSorter.SetSortByCaption();
                    break;

                case StateColumnTitle:
                    this.resultSorter.SetSortField(Settings.Default.StateFieldName);
                    break;

                case TypeColumnTitle:
                    this.resultSorter.SetSortField(Settings.Default.TypeFieldName);
                    break;

                default:
                    this.resultSorter.SetSortField(Settings.Default.IdFieldName);
                    break;
            }

            this.MatchedItems.Sort(this.resultSorter);
        }

        /// <summary>
        /// The results sorter class.
        /// </summary>
        private class ResultsSorter : IComparer<IWorkbenchItem>
        {
            /// <summary>
            /// The sort field.
            /// </summary>
            private string sortField;

            /// <summary>
            /// The direction.
            /// </summary>
            private SortDirection direction;

            /// <summary>
            /// The is sort by caption flag.
            /// </summary>
            private bool isSortByCaption;

            /// <summary>
            /// Gets or sets a value indicating whether this instance is sort by cpation.
            /// </summary>
            /// <value>
            /// <c>true</c> if this instance is sort by cpation; otherwise, <c>false</c>.
            /// </value>
            public void SetSortByCaption()
            {
                if (this.isSortByCaption)
                {
                    this.InvertSortDirection();
                    return;
                }

                this.sortField = null;
                this.isSortByCaption = true;
            }

            /// <summary>
            /// Sets the sort field.
            /// </summary>
            /// <param name="fieldName">The sort field.</param>
            public void SetSortField(string fieldName)
            {
                if (Equals(this.sortField, fieldName))
                {
                    this.InvertSortDirection();
                }
                else
                {
                    this.sortField = fieldName;
                    this.direction = SortDirection.Ascending;
                }

                this.isSortByCaption = false;
            }

            /// <summary>
            /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
            /// </summary>
            /// <returns>
            /// A signed integer that indicates the relative values of <paramref name="x"/> and <paramref name="y"/>, as shown in the following table.Value Meaning Less than zero<paramref name="x"/> is less than <paramref name="y"/>.Zero<paramref name="x"/> equals <paramref name="y"/>.Greater than zero<paramref name="x"/> is greater than <paramref name="y"/>.
            /// </returns>
            /// <param name="x">The first object to compare.</param><param name="y">The second object to compare.</param>
            public int Compare(IWorkbenchItem x, IWorkbenchItem y)
            {
                if ((!this.isSortByCaption && string.IsNullOrEmpty(this.sortField)) || x == null || y == null)
                {
                    return 0;
                }

                var itemXField = this.isSortByCaption ? x.GetCaption() : x[this.sortField];
                var itemYField = this.isSortByCaption ? y.GetCaption() : y[this.sortField];

                var compareResult = this.direction == SortDirection.Ascending
                                        ? Comparer.Default.Compare(itemXField, itemYField)
                                        : Comparer.Default.Compare(itemYField, itemXField);

                // If the comparison values are equal then sort by caption
                if (compareResult.Equals(0))
                {
                    compareResult = this.direction == SortDirection.Ascending
                                        ? Comparer.Default.Compare(x.GetCaption(), y.GetCaption())
                                        : Comparer.Default.Compare(y.GetCaption(), x.GetCaption());
                }

                return compareResult;
            }

            /// <summary>
            /// Inverts the sort direction.
            /// </summary>
            private void InvertSortDirection()
            {
                this.direction = this.direction == SortDirection.Ascending
                                     ? SortDirection.Descending
                                     : SortDirection.Ascending;
            }
        }
    }
}
