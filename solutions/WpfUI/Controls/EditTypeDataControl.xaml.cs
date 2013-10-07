// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EditTypeDataControl.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   Interaction logic for EditItemControl.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using TfsWorkbench.UIElements.ValueConverters;

namespace TfsWorkbench.WpfUI.Controls
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

    using Core.DataObjects;
    using Core.Interfaces;

    using UIElements;

    /// <summary>
    /// Interaction logic for EditItemControl.xaml
    /// </summary>
    public partial class EditTypeDataControl
    {
        /// <summary>
        /// The null selection text.
        /// </summary>
        private const string NullSelectionText = "[None]";

        /// <summary>
        /// The project data property.
        /// </summary>
        private static readonly DependencyProperty projectDataProperty = DependencyProperty.Register(
            "ProjectData", 
            typeof(IProjectData), 
            typeof(EditTypeDataControl),
            new PropertyMetadata(null, OnProjectDataChanged));

        /// <summary>
        /// The captionField dependency property.
        /// </summary>
        private static readonly DependencyProperty captionFieldProperty = DependencyProperty.Register(
            "CaptionField",
            typeof(string),
            typeof(EditTypeDataControl));

        /// <summary>
        /// The bodyField dependency property.
        /// </summary>
        private static readonly DependencyProperty bodyFieldProperty = DependencyProperty.Register(
            "BodyField",
            typeof(string),
            typeof(EditTypeDataControl));

        /// <summary>
        /// The numericField dependency property.
        /// </summary>
        private static readonly DependencyProperty numericFieldProperty = DependencyProperty.Register(
            "NumericField",
            typeof(string),
            typeof(EditTypeDataControl));

        /// <summary>
        /// The ownerField dependency property.
        /// </summary>
        private static readonly DependencyProperty ownerFieldProperty = DependencyProperty.Register(
            "OwnerField",
            typeof(string),
            typeof(EditTypeDataControl));

        /// <summary>
        /// The is initialising flag.
        /// </summary>
        private bool isInitialising;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditTypeDataControl"/> class.
        /// </summary>
        public EditTypeDataControl()
        {
            this.ItemTypeNames = new ObservableCollection<string>();
            this.AvailableDisplayFields = new ObservableCollection<string>();
            this.ColourOptions = new ObservableCollection<string>(BackgroundBrushConverter.ColourOptions);

            this.InitializeComponent();
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
        /// Gets the CaptionField property.
        /// </summary>
        /// <value>The name property.</value>
        public static DependencyProperty CaptionFieldProperty
        {
            get { return captionFieldProperty; }
        }

        /// <summary>
        /// Gets the NumericField property.
        /// </summary>
        /// <value>The name property.</value>
        public static DependencyProperty NumericFieldProperty
        {
            get { return numericFieldProperty; }
        }

        /// <summary>
        /// Gets the BodyField property.
        /// </summary>
        /// <value>The name property.</value>
        public static DependencyProperty BodyFieldProperty
        {
            get { return bodyFieldProperty; }
        }

        /// <summary>
        /// Gets the OwnerField property.
        /// </summary>
        /// <value>The name property.</value>
        public static DependencyProperty OwnerFieldProperty
        {
            get { return ownerFieldProperty; }
        }

        /// <summary>
        /// Gets or sets the control items.
        /// </summary>
        /// <value>The control items.</value>
        public IProjectData ProjectData
        {
            get { return (IProjectData)this.GetValue(ProjectDataProperty); }
            set { this.SetValue(ProjectDataProperty, value); }
        }

        /// <summary>
        /// Gets the type names.
        /// </summary>
        /// <value>The type names.</value>
        public ObservableCollection<string> ItemTypeNames { get; private set; }

        /// <summary>
        /// Gets the available fields.
        /// </summary>
        /// <value>The available fields.</value>
        public ObservableCollection<string> AvailableDisplayFields { get; private set; }

        public ObservableCollection<string> ColourOptions { get; private set; } 

        /// <summary>
        /// Gets or sets the instance CaptionField.
        /// </summary>
        /// <returns>The instance CaptionField.</returns>
        public string CaptionField
        {
            get { return (string)this.GetValue(CaptionFieldProperty); }
            set { this.SetValue(CaptionFieldProperty, value); }
        }

        /// <summary>
        /// Gets or sets the instance BodyField.
        /// </summary>
        /// <returns>The instance BodyField.</returns>
        public string BodyField
        {
            get { return (string)this.GetValue(BodyFieldProperty); }
            set { this.SetValue(BodyFieldProperty, value); }
        }

        /// <summary>
        /// Gets or sets the instance NumericField.
        /// </summary>
        /// <returns>The instance NumericField.</returns>
        public string NumericField
        {
            get { return (string)this.GetValue(NumericFieldProperty); }
            set { this.SetValue(NumericFieldProperty, value); }
        }

        /// <summary>
        /// Gets or sets the instance OwnerField.
        /// </summary>
        /// <returns>The instance OwnerField.</returns>
        public string OwnerField
        {
            get { return (string)this.GetValue(OwnerFieldProperty); }
            set { this.SetValue(OwnerFieldProperty, value); }
        }

        public string SelectedColour { get; private set; }

        /// <summary>
        /// Called when [project data changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnProjectDataChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var control = dependencyObject as EditTypeDataControl;
            if (control == null)
            {
                return;
            }

            control.ItemTypeNames.Clear();
            
            if (control.ProjectData == null)
            {
                return;
            }

            foreach (var typeName in control.ProjectData.ItemTypes.Select(t => t.TypeName).OrderBy(n => n))
            {
                control.ItemTypeNames.Add(typeName);
            }
        }

        /// <summary>
        /// Tries to get the ref name.
        /// </summary>
        /// <param name="selectedTypeData">The selected type data.</param>
        /// <param name="displayName">The display name.</param>
        /// <param name="refName">The ref name.</param>
        /// <returns>
        /// <c>True</c> if the ref name is found; otherwise <c>false</c>.
        /// </returns>
        private static bool TryGetRefName(ItemTypeData selectedTypeData, string displayName, out string refName)
        {
            var field = selectedTypeData.Fields.FirstOrDefault(f => Equals(f.DisplayName, displayName));

            refName = field == null ? null : field.ReferenceName;

            return !string.IsNullOrEmpty(refName);
        }

        /// <summary>
        /// Tries to get the display name.
        /// </summary>
        /// <param name="selectedTypeData">The selected type data.</param>
        /// <param name="refName">The ref name.</param>
        /// <param name="displayName">The display name.</param>
        /// <returns><c>True</c> if the display name is found; otherwise <c>false</c>.</returns>
        private static bool TryGetDisplayName(ItemTypeData selectedTypeData, string refName, out string displayName)
        {
            var field = selectedTypeData.Fields.FirstOrDefault(f => Equals(f.ReferenceName, refName));

            displayName = field == null ? null : field.DisplayName;

            return !string.IsNullOrEmpty(displayName);
        }

        /// <summary>
        /// Handles the Click event of the CloseButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            CommandLibrary.CloseDialogCommand.Execute(this, Application.Current.MainWindow);

            this.CommitDisplayFieldChanges();

            foreach (var workbenchItem in this.ProjectData.WorkbenchItems)
            {
                workbenchItem.OnPropertyChanged();
            }

            this.ReleaseReferencedObjects();
        }

        /// <summary>
        /// Commits the display field changes.
        /// </summary>
        private void CommitDisplayFieldChanges()
        {
            ItemTypeData selectedTypeData;
            if (!this.TryGetSelectedType(out selectedTypeData))
            {
                return;
            }

            string refName;
            selectedTypeData.CaptionField = TryGetRefName(selectedTypeData, this.CaptionField, out refName) ? refName : string.Empty;
            selectedTypeData.BodyField = TryGetRefName(selectedTypeData, this.BodyField, out refName) ? refName : string.Empty;
            selectedTypeData.NumericField = TryGetRefName(selectedTypeData, this.NumericField, out refName) ? refName : string.Empty;
            selectedTypeData.OwnerField = TryGetRefName(selectedTypeData, this.OwnerField, out refName) ? refName : string.Empty;
            selectedTypeData.DefaultColour = PART_ColourSelector.SelectedValue as string;
        }

        /// <summary>
        /// Releases the control collection.
        /// </summary>
        private void ReleaseReferencedObjects()
        {
            this.ProjectData = null;
            this.ItemTypeNames.Clear();
        }

        /// <summary>
        /// Handles the Click event of the AddItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void AddItemClick(object sender, RoutedEventArgs e)
        {
            ItemTypeData selectedTypeData;
            if (!this.TryGetSelectedType(out selectedTypeData))
            {
                return;
            }

            var selectedItems = this.PART_AvailableFields.SelectedItems.OfType<string>().ToArray();

            if (!selectedItems.Any())
            {
                return;
            }

            foreach (var selectedItem in selectedItems)
            {
                var localSelectedItem = selectedItem;

                this.PART_SelectedFields.Items.Add(localSelectedItem);
                this.PART_AvailableFields.Items.Remove(localSelectedItem);

                var fieldData = selectedTypeData.Fields.FirstOrDefault(f => f.DisplayName.Equals(localSelectedItem));

                if (fieldData == null)
                {
                    continue;
                }

                selectedTypeData.ContextFields.Add(fieldData.ReferenceName);
            }
        }

        /// <summary>
        /// Handles the Click event of the RemoveItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void RemoveItemClick(object sender, RoutedEventArgs e)
        {
            ItemTypeData selectedTypeData;
            if (!this.TryGetSelectedType(out selectedTypeData))
            {
                return;
            }

            var selectedItems = this.PART_SelectedFields.SelectedItems.OfType<string>().ToArray();

            if (!selectedItems.Any())
            {
                return;
            }

            foreach (var selectedItem in selectedItems)
            {
                var localSelectedItem = selectedItem;

                this.PART_SelectedFields.Items.Remove(localSelectedItem);
                this.PART_AvailableFields.Items.Add(localSelectedItem);

                var fieldData = selectedTypeData.Fields.FirstOrDefault(f => f.DisplayName.Equals(localSelectedItem));

                if (fieldData == null)
                {
                    continue;
                }

                selectedTypeData.ContextFields.Remove(fieldData.ReferenceName);
            }
        }

        /// <summary>
        /// Handles the SelectionChanged event of the ItemType control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void ItemTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.ProjectData == null)
            {
                return;
            }

            this.isInitialising = true;

            this.PART_AvailableFields.Items.Clear();
            this.PART_SelectedFields.Items.Clear();
            this.PART_AvailableDuplicationFields.Items.Clear();
            this.PART_SelectedDuplicationFields.Items.Clear();
            this.PART_SelectedFields.Items.Clear();
            this.AvailableDisplayFields.Clear();

            this.AvailableDisplayFields.Add(NullSelectionText);

            ItemTypeData selectedTypeData;
            if (!this.TryGetSelectedType(out selectedTypeData))
            {
                return;
            }

            string displayName;
            this.CaptionField = TryGetDisplayName(selectedTypeData, selectedTypeData.CaptionField, out displayName) ? displayName : NullSelectionText;
            this.BodyField = TryGetDisplayName(selectedTypeData, selectedTypeData.BodyField, out displayName) ? displayName : NullSelectionText;
            this.NumericField = TryGetDisplayName(selectedTypeData, selectedTypeData.NumericField, out displayName) ? displayName : NullSelectionText;
            this.OwnerField = TryGetDisplayName(selectedTypeData, selectedTypeData.OwnerField, out displayName) ? displayName : NullSelectionText;

            PART_ColourSelector.SelectedValue = selectedTypeData.DefaultColour;

            foreach (var field in selectedTypeData.Fields.OrderBy(f => f.DisplayName))
            {
                this.AvailableDisplayFields.Add(field.DisplayName);

                if (field.IsEditable && !selectedTypeData.DuplicationFields.Contains(field.ReferenceName))
                {
                    this.PART_AvailableDuplicationFields.Items.Add(field.DisplayName);
                }

                if (field.IsDisplayField && !selectedTypeData.ContextFields.Contains(field.ReferenceName))
                {
                    this.PART_AvailableFields.Items.Add(field.DisplayName);
                }
            }

            foreach (var contextField in
                selectedTypeData.ContextFields
                    .Select(name => selectedTypeData.Fields.FirstOrDefault(f => f.ReferenceName.Equals(name)))
                    .Where(field => field != null))
            {
                this.PART_SelectedFields.Items.Add(contextField.DisplayName);
            }

            foreach (var duplicationField in
                selectedTypeData.DuplicationFields
                    .Select(name => selectedTypeData.Fields.FirstOrDefault(f => f.ReferenceName.Equals(name)))
                    .Where(field => field != null))
            {
                this.PART_SelectedDuplicationFields.Items.Add(duplicationField.DisplayName);
            }

            var binding = this.PART_Body.GetBindingExpression(Selector.SelectedValueProperty);
            if (binding != null)
            {
                binding.UpdateTarget();
            }

            binding = this.PART_Caption.GetBindingExpression(Selector.SelectedValueProperty);
            if (binding != null)
            {
                binding.UpdateTarget();
            }

            binding = this.PART_Numeric.GetBindingExpression(Selector.SelectedValueProperty);
            if (binding != null)
            {
                binding.UpdateTarget();
            }

            binding = this.PART_Owner.GetBindingExpression(Selector.SelectedValueProperty);
            if (binding != null)
            {
                binding.UpdateTarget();
            }

            this.isInitialising = false;
        }

        /// <summary>
        /// Tries the type of the get selected.
        /// </summary>
        /// <param name="typeData">The type data.</param>
        /// <returns><c>True</c> if type data is matched; otherwise <c>false</c>.</returns>
        private bool TryGetSelectedType(out ItemTypeData typeData)
        {
            var selectedType = this.PART_SelectedItemType.SelectedItem as string;

            return this.ProjectData.ItemTypes.TryGetValue(selectedType, out typeData);
        }

        /// <summary>
        /// Handles the SelectionChanged event of the DisplayField control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.SelectionChangedEventArgs"/> instance containing the event data.</param>
        private void DisplayField_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.isInitialising)
            {
                return;
            }

            this.CommitDisplayFieldChanges();
        }

        private void AddDuplicationItemClick(object sender, RoutedEventArgs e)
        {
            ItemTypeData selectedTypeData;
            if (!this.TryGetSelectedType(out selectedTypeData))
            {
                return;
            }

            var selectedItems = this.PART_AvailableDuplicationFields.SelectedItems.OfType<string>().ToArray();

            if (!selectedItems.Any())
            {
                return;
            }

            foreach (var selectedItem in selectedItems)
            {
                var localSelectedItem = selectedItem;

                this.PART_SelectedDuplicationFields.Items.Add(localSelectedItem);
                this.PART_AvailableDuplicationFields.Items.Remove(localSelectedItem);

                var fieldData = selectedTypeData.Fields.FirstOrDefault(f => f.DisplayName.Equals(localSelectedItem));

                if (fieldData == null)
                {
                    continue;
                }

                selectedTypeData.DuplicationFields.Add(fieldData.ReferenceName);
            }
        }

        private void RemoveDuplicationItemClick(object sender, RoutedEventArgs e)
        {
            ItemTypeData selectedTypeData;
            if (!this.TryGetSelectedType(out selectedTypeData))
            {
                return;
            }

            var selectedItems = this.PART_SelectedDuplicationFields.SelectedItems.OfType<string>().ToArray();

            if (!selectedItems.Any())
            {
                return;
            }

            foreach (var selectedItem in selectedItems)
            {
                var localSelectedItem = selectedItem;

                this.PART_SelectedDuplicationFields.Items.Remove(localSelectedItem);
                this.PART_AvailableDuplicationFields.Items.Add(localSelectedItem);

                var fieldData = selectedTypeData.Fields.FirstOrDefault(f => f.DisplayName.Equals(localSelectedItem));

                if (fieldData == null)
                {
                    continue;
                }

                selectedTypeData.DuplicationFields.Remove(fieldData.ReferenceName);
            }
        }
    }
}