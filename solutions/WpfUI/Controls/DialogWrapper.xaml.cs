// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DialogWrapper.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   Interaction logic for DialogWrapper.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.WpfUI.Controls
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using TfsWorkbench.UIElements;

    /// <summary>
    /// Interaction logic for DialogWrapper.xaml
    /// </summary>
    public partial class DialogWrapper
    {
        /// <summary>
        /// The mouse down offset.
        /// </summary>
        private Point offset;

        /// <summary>
        /// The mouse down indication flag.
        /// </summary>
        private bool isMouseDown;

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogWrapper"/> class.
        /// </summary>
        /// <param name="dialogElement">The dialog element.</param>
        public DialogWrapper(UIElement dialogElement)
        {
            InitializeComponent();

            this.PART_ContentPresenter.Content = dialogElement;
            this.WrappedElement = dialogElement;
        }

        /// <summary>
        /// Gets the wrapped element.
        /// </summary>
        /// <value>The wrapped element.</value>
        public UIElement WrappedElement { get; private set; }

        /// <summary>
        /// Called when [mouse move].
        /// </summary>
        internal void OnMouseMove()
        {
            if (!this.isMouseDown)
            {
                return;
            }

            var parent = VisualTreeHelperExtension.GetParentOfType<Canvas>(this);

            if (parent == null)
            {
                return;
            }

            var position = Mouse.GetPosition(parent);

            this.SetValue(Canvas.LeftProperty, position.X - this.offset.X);
            this.SetValue(Canvas.TopProperty, position.Y - this.offset.Y);
        }

        /// <summary>
        /// Called when [drag handle mouse up].
        /// </summary>
        internal void OnMouseUp()
        {
            this.isMouseDown = false;
            Mouse.OverrideCursor = null;
        }

        /// <summary>
        /// Called when [drag handle mouse down].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void OnDragHandleMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            this.offset = Mouse.GetPosition(this);
            this.isMouseDown = true;
            Mouse.OverrideCursor = CustomCursors.MoveHand;
        }
    }
}
