// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FilterInfo.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the FilterInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.FilterService
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    using TfsWorkbench.FilterService.Converters;
    using TfsWorkbench.FilterService.Properties;

    /// <summary>
    /// The filter info class.
    /// </summary>
    public class FilterInfo : INotifyPropertyChanged
    {
        /// <summary>
        /// The filter button display text.
        /// </summary>
        private string filterStatus;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterInfo"/> class.
        /// </summary>
        public FilterInfo()
        {
            this.InitaliseValues();
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the filter status text.
        /// </summary>
        /// <value>The filter status text.</value>
        public string FilterStatus
        {
            get
            {
                return this.filterStatus;
            }

            set
            {
                if (this.filterStatus == value)
                {
                    return;
                }

                this.filterStatus = value;

                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs("FilterStatus"));
                }
            }
        }

        /// <summary>
        /// Gets the filter operators.
        /// </summary>
        /// <value>The filter operators.</value>
        public ObservableCollection<string> FilterOperators { get; private set; }

        /// <summary>
        /// Gets the filter actions.
        /// </summary>
        /// <value>The filter actions.</value>
        public ObservableCollection<string> FilterActions { get; private set; }

        /// <summary>
        /// Gets the instance FieldNames.
        /// </summary>
        /// <returns>The instance FieldNames.</returns>
        public ObservableCollection<string> FieldNames { get; private set; }

        /// <summary>
        /// Gets the instance ItemTypes.
        /// </summary>
        /// <returns>The instance ItemTypes.</returns>
        public ObservableCollection<string> ItemTypes { get; private set; }

        /// <summary>
        /// Gets the instance ItemTypes.
        /// </summary>
        /// <returns>The instance ItemTypes.</returns>
        public ObservableCollection<string> StateOptions { get; private set; }

        /// <summary>
        /// Initalises the values.
        /// </summary>
        private void InitaliseValues()
        {
            this.filterStatus = Settings.Default.DisplayName;
            this.FilterOperators = new ObservableCollection<string>();
            this.FilterActions = new ObservableCollection<string>();
            this.FieldNames = new ObservableCollection<string>();
            this.ItemTypes = new ObservableCollection<string>();
            this.StateOptions = new ObservableCollection<string>();

            foreach (var operatorsAsString in OperatorToStringConverter.OperatorsAsStrings)
            {
                this.FilterOperators.Add(operatorsAsString);
            }

            this.FilterActions.Add(Resources.String014);
            this.FilterActions.Add(Resources.String015);
        }
    }
}
