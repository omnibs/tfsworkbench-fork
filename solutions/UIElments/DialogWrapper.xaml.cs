// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DialogWrapper.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   Interaction logic for DialogWrapper.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.UIElements
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for DialogWrapper.xaml
    /// </summary>
    public partial class DialogWrapper
    {
        /// <summary>
        /// The is resizable attached property.
        /// </summary>
        private static readonly DependencyProperty isResizeableProperty = DependencyProperty.RegisterAttached(
            "IsResizable",
            typeof(bool),
            typeof(DialogWrapper),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// The is non modal attached porperty.
        /// </summary>
        private static readonly DependencyProperty isModelessProperty = DependencyProperty.RegisterAttached(
            "IsModeless",
            typeof(bool),
            typeof(DialogWrapper),
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));

        /// <summary>
        /// The is dragging attached porperty.
        /// </summary>
        private static readonly DependencyProperty isDraggingProperty = DependencyProperty.RegisterAttached(
            "IsDragging",
            typeof(bool),
            typeof(DialogWrapper),
            new FrameworkPropertyMetadata(false));

        /// <summary>
        /// The is top most attached porperty.
        /// </summary>
        private static readonly DependencyProperty isTopMostProperty = DependencyProperty.RegisterAttached(
            "IsTopMost",
            typeof(bool),
            typeof(DialogWrapper),
            new FrameworkPropertyMetadata(false));

        /// <summary>
        /// The dialog content property.
        /// </summary>
        private static readonly DependencyProperty dialogContentProperty = DependencyProperty.Register(
            "DialogContent",
            typeof(FrameworkElement),
            typeof(DialogWrapper),
            new PropertyMetadata(null, OnDialogContextChanged));

        /// <summary>
        /// The mouse down offset.
        /// </summary>
        private Point offset;

        /// <summary>
        /// The mouse down indication flag.
        /// </summary>
        private bool isMouseDown;

        /// <summary>
        /// The is resize flag.
        /// </summary>
        private bool isResize;

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogWrapper"/> class.
        /// </summary>
        /// <param name="dialogElement">The dialog element.</param>
        public DialogWrapper(FrameworkElement dialogElement)
        {
            InitializeComponent();

            this.DialogContent = dialogElement;
        }

        /// <summary>
        /// Occurs when [resized].
        /// </summary>
        public event EventHandler Resized;

        /// <summary>
        /// Gets the is dragging property.
        /// </summary>
        /// <value>The is dragging property.</value>
        public static DependencyProperty IsTopMostProperty
        {
            get { return isTopMostProperty; }
        }

        /// <summary>
        /// Gets the is dragging property.
        /// </summary>
        /// <value>The is dragging property.</value>
        public static DependencyProperty IsDraggingProperty
        {
            get { return isDraggingProperty; }
        }

        /// <summary>
        /// Gets the is resizeable property.
        /// </summary>
        /// <value>The is resizeable property.</value>
        public static DependencyProperty IsResizeableProperty
        {
            get { return isResizeableProperty; }
        }

        /// <summary>
        /// Gets the is non modal property.
        /// </summary>
        /// <value>The is non modal property.</value>
        public static DependencyProperty IsModelessProperty
        {
            get { return isModelessProperty; }
        }

        /// <summary>
        /// Gets the dialog content property.
        /// </summary>
        /// <value>The dialog content property.</value>
        public static DependencyProperty DialogContentProperty
        {
            get { return dialogContentProperty; }
        }

        /// <summary>
        /// Gets or sets the content of the dialog.
        /// </summary>
        /// <value>The content of the dialog.</value>
        public FrameworkElement DialogContent
        {
            get { return (FrameworkElement)this.GetValue(DialogContentProperty); }
            set { this.SetValue(DialogContentProperty, value); }
        }

        /// <summary>
        /// Sets the is Top Most attached value.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetIsTopMost(UIElement element, bool value)
        {
            element.SetValue(IsTopMostProperty, value);
        }

        /// <summary>
        /// Gets the is Top Most attached value.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns><c>True</c> if the element is Top Most; otherwise <c>false</c>.</returns>
        public static bool GetIsTopMost(UIElement element)
        {
            return (bool)element.GetValue(IsTopMostProperty);
        }

        /// <summary>
        /// Sets the is dragging property.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetIsDragging(UIElement element, bool value)
        {
            element.SetValue(IsDraggingProperty, value);
        }

        /// <summary>
        /// Gets the is dragging.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns><c>True</c> if the element is dragging; otherwise <c>false</c>.</returns>
        public static bool GetIsDragging(UIElement element)
        {
            return (bool)element.GetValue(IsDraggingProperty);
        }

        /// <summary>
        /// Sets the is resizeable property.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetIsResizeable(UIElement element, bool value)
        {
            element.SetValue(IsResizeableProperty, value);
        }

        /// <summary>
        /// Gets the is resizeable property.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns><c>True</c> if the element is resizable; otherwise <c>false</c>.</returns>
        public static bool GetIsResizeable(UIElement element)
        {
            return (bool)element.GetValue(IsResizeableProperty);
        }

        /// <summary>
        /// Sets the is modeless.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="value">if set to <c>true</c> [value].</param>
        public static void SetIsModeless(UIElement element, bool value)
        {
            element.SetValue(IsModelessProperty, value);
        }

        /// <summary>
        /// Gets the is modeless.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns><c>True</c> if the emlement is modeless; otherwise <c>false</c>.</returns>
        public static bool GetIsModeless(UIElement element)
        {
            return (bool)element.GetValue(IsModelessProperty);
        }
        
        /// <summary>
        /// Called when [mouse move].
        /// </summary>
        public void OnMouseMove()
        {
            if (!this.isMouseDown)
            {
                return;
            }

            Point position;

            if (this.isResize)
            {
                const double MinDialogWidth = 50;
                const double MinDialogHeight = 40;

                position = Mouse.GetPosition(this);

                var widthCandidate = this.DialogContent.Width + position.X - this.offset.X;
                var heightCandidate = this.DialogContent.Height + position.Y - this.offset.Y;

                if (widthCandidate > MinDialogWidth)
                {
                    this.DialogContent.Width = widthCandidate;
                }

                if (heightCandidate > MinDialogHeight)
                {
                    this.DialogContent.Height = heightCandidate;
                }

                this.offset = position;

                if (this.Resized != null)
                {
                    this.Resized(this, EventArgs.Empty);
                }

                return;
            }

            var parent = this.GetParentOfType<Canvas>();

            if (parent == null)
            {
                return;
            }

            position = Mouse.GetPosition(parent);

            this.SetValue(Canvas.LeftProperty, position.X - this.offset.X);
            this.SetValue(Canvas.TopProperty, position.Y - this.offset.Y);
        }

        /// <summary>
        /// Called when [drag handle mouse up].
        /// </summary>
        public void OnMouseUp()
        {
            this.isMouseDown = false;
            this.isResize = false;
            Mouse.OverrideCursor = null;

            SetIsDragging(this.DialogContent, false);
        }

        /// <summary>
        /// Called when [dialog context changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnDialogContextChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var control = dependencyObject as DialogWrapper;
            if (control == null)
            {
                return;
            }

            control.PART_ContentPresenter.Content = control.DialogContent;
            control.PART_ResizeHandle.Visibility = 
                control.DialogContent == null || !GetIsResizeable(control.DialogContent)
                    ? Visibility.Hidden
                    : Visibility.Visible;
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
            this.isResize = false;
            Mouse.OverrideCursor = CustomCursors.MoveHand;

            SetIsDragging(this.DialogContent, true);
        }

        /// <summary>
        /// Called when [resize handle mouse down].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        private void OnResizeHandleMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }

            this.offset = Mouse.GetPosition(this);
            this.isMouseDown = true;
            this.isResize = true;
            Mouse.OverrideCursor = CustomCursors.MoveHand;

            SetIsDragging(this.DialogContent, true);
        }
    }
}
