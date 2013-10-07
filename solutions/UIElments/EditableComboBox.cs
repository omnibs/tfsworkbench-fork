// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EditableComboBox.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the EditableComboBox type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.UIElements
{
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// The editable combo box class.
    /// </summary>
    public class EditableComboBox : ComboBox
    {
        /// <summary>            
        /// Initializes a new instance of the <see cref="EditableComboBox"/> class.            
        /// </summary>            
        public EditableComboBox()
        {            
            this.IsTextSearchEnabled = false;
            this.IsEditable = true;
        }

        /// <summary>        
        /// Gets a reference to the internal editable textbox.        
        /// </summary>        
        /// <value>A reference to the internal editable textbox.</value>        
        /// <remarks>        
        /// We need this to get access to the real MaxLength.        
        /// </remarks>        
        protected TextBox EditableTextBox
        {
            get
            {
                return this.GetTemplateChild("PART_EditableTextBox") as TextBox;
            }
        }

        /// <summary>            
        /// Updates the Text property binding when the user presses the Enter key.          
        /// </summary>        
        /// <remarks>        
        /// KeyDown is not raised for Arrows, Tab and Enter keys.        
        /// They are swallowed by the DropDown if it is open.        
        /// So use the KeyUp instead.           
        /// </remarks>        
        /// <param name="e">A Key EventArgs.</param>        
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (e.Key == Key.Return || e.Key == Key.Enter)
            {
                this.UpdateDataSource();
            }
        }

        /// <summary>            
        /// Updates the Text property binding when the selection changes.         
        /// </summary>        
        /// <param name="e">A SelectionChanged EventArgs.</param>        
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            base.OnSelectionChanged(e);
            this.UpdateDataSource();
        }

        /// <summary>            
        /// Updates the data source.        
        /// </summary>            
        private void UpdateDataSource()
        {
            var expression = GetBindingExpression(TextProperty);
            if (expression != null)
            {
                expression.UpdateSource();
            }
        }
    }
}
