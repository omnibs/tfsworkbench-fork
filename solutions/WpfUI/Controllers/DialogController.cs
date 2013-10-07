// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DialogController.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the DialogController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.WpfUI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;

    using TfsWorkbench.UIElements;
    using TfsWorkbench.WpfUI.Controls;

    /// <summary>
    /// The dialog controller class.
    /// </summary>
    internal class DialogController : IDialogController
    {
        /// <summary>
        /// The dialog size map.
        /// </summary>
        private readonly IDictionary<Type, Size> dialogSizes = new Dictionary<Type, Size>();

        /// <summary>
        /// The handle mouse move delegate;
        /// </summary>
        private readonly RoutedEventHandler handleMouseMove;

        /// <summary>
        /// The handle mouse up delegate.
        /// </summary>
        private readonly RoutedEventHandler handleMouseUp;

        /// <summary>
        /// The handle mouse enter delegate.
        /// </summary>
        private readonly RoutedEventHandler handleMouseDown;

        /// <summary>
        /// The dialog canvas.
        /// </summary>
        private readonly MainAppWindow mainAppWindow;

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogController"/> class.
        /// </summary>
        /// <param name="mainAppWindow">The canvas.</param>
        public DialogController(MainAppWindow mainAppWindow)
        {
            if (mainAppWindow == null)
            {
                throw new ArgumentNullException("mainAppWindow");
            }

            this.mainAppWindow = mainAppWindow;
            this.handleMouseMove = this.OnMouseMove;
            this.handleMouseUp = this.OnMouseUp;
            this.handleMouseDown = this.OnMouseDown;
            this.mainAppWindow.AddHandler(UIElement.MouseDownEvent, this.handleMouseDown, true);
            this.mainAppWindow.AddHandler(UIElement.MouseMoveEvent, this.handleMouseMove);
            this.mainAppWindow.AddHandler(UIElement.MouseUpEvent, this.handleMouseUp, true);
        }

        /// <summary>
        /// Gets a value indicating whether this instance is displaying modal dialog.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is displaying modal dialog; otherwise, <c>false</c>.
        /// </value>
        public bool IsDisplayingModalDialog
        {
            get
            {
                return this.GetDialogWrappers().Any(w => !DialogWrapper.GetIsModeless(w.DialogContent));
            }
        }

        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <param name="dialogElement">The dialog element.</param>
        public void ShowDialog(FrameworkElement dialogElement)
        {
            if (dialogElement == null)
            {
                return;
            }

            const int SiblingOffset = 25;

            DialogWrapper wrapper;
            if (!this.TryGetExistingDialog(dialogElement, out wrapper))
            {
                wrapper = new DialogWrapper(dialogElement);

                wrapper.Resized += this.OnDialogResized;

                Size dialogSize;
                if (this.dialogSizes.TryGetValue(dialogElement.GetType(), out dialogSize))
                {
                    dialogElement.Width = dialogSize.Width;
                    dialogElement.Height = dialogSize.Height;
                }

                var existingDialogCount = this.GetDialogWrappers().Count();

                this.mainAppWindow.PART_DialogCanvas.Children.Add(wrapper);

                this.mainAppWindow.UpdateLayout();

                var siblingOffset = existingDialogCount * SiblingOffset;

                if (siblingOffset > this.mainAppWindow.ActualWidth || siblingOffset > this.mainAppWindow.ActualHeight)
                {
                    siblingOffset = 0;
                }

                var offSetLeft = (this.mainAppWindow.ActualWidth / 2) - (wrapper.ActualWidth / 2) +
                                 siblingOffset;
                var offSetTop = (this.mainAppWindow.ActualHeight / 2) - (wrapper.ActualHeight / 2) +
                                siblingOffset;

                wrapper.SetValue(Canvas.LeftProperty, offSetLeft);
                wrapper.SetValue(Canvas.TopProperty, offSetTop);
            }

            this.BringToTop(wrapper);

            wrapper.Focus();

            var isModal = !DialogWrapper.GetIsModeless(dialogElement);

            if (isModal)
            {
                CommandLibrary.DisableUserInputCommand.Execute(true, this.mainAppWindow);
            }
        }

        /// <summary>
        /// Closes all dialogs.
        /// </summary>
        public void CloseAllDialogs()
        {
            var hasModalDialog = this.IsDisplayingModalDialog;

            foreach (var dialogWrapper in this.GetDialogWrappers().ToArray())
            {
                this.mainAppWindow.PART_DialogCanvas.Children.Remove(dialogWrapper);
                dialogWrapper.Resized -= this.OnDialogResized;
            }

            if (hasModalDialog)
            {
                CommandLibrary.DisableUserInputCommand.Execute(false, this.mainAppWindow);
            }
        }

        /// <summary>
        /// Closes the dialog.
        /// </summary>
        /// <param name="dialogElement">The dialog element.</param>
        public void CloseDialog(FrameworkElement dialogElement)
        {
            var wrapper =
                this.GetDialogWrappers()
                    .FirstOrDefault(w => Equals(w.DialogContent, dialogElement));

            if (wrapper != null)
            {
                this.mainAppWindow.PART_DialogCanvas.Children.Remove(wrapper);
                wrapper.Resized -= this.OnDialogResized;

                var topMost = this.GetDialogWrappers().OrderByDescending(w => w.GetValue(Panel.ZIndexProperty)).FirstOrDefault();

                if (topMost != null)
                {
                    this.BringToTop(topMost);
                }
            }

            this.mainAppWindow.UpdateLayout();

            if (!this.IsDisplayingModalDialog)
            {
                CommandLibrary.DisableUserInputCommand.Execute(false, this.mainAppWindow);
            }
        }

        /// <summary>
        /// Called when [dialog resized].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private void OnDialogResized(object sender, EventArgs e)
        {
            var wrapper = sender as DialogWrapper;

            if (wrapper == null)
            {
                return;
            }

            var dialogContent = wrapper.DialogContent;
            if (dialogContent == null)
            {
                return;
            }

            var contentType = dialogContent.GetType();
            var size = new Size(dialogContent.ActualWidth, dialogContent.ActualHeight);

            if (this.dialogSizes.ContainsKey(contentType))
            {
                this.dialogSizes[contentType] = size;
            }
            else
            {
                this.dialogSizes.Add(contentType, size);
            }
        }

        /// <summary>
        /// Tries to get the existing dialog.
        /// </summary>
        /// <param name="dialogElement">The dialog element.</param>
        /// <param name="wrapper">The wrapper.</param>
        /// <returns><c>True</c> if an existing dialog is found; otherwise <c>false</c>.</returns>
        private bool TryGetExistingDialog(UIElement dialogElement, out DialogWrapper wrapper)
        {
            wrapper = null;

            if (dialogElement is SearchControl)
            {
                var exisingSearchControl =
                    this.GetDialogWrappers().FirstOrDefault(w => w.DialogContent is SearchControl);

                if (exisingSearchControl != null)
                {
                    wrapper = exisingSearchControl;
                }
            }

            return wrapper != null;
        }

        /// <summary>
        /// Gets the dialog wrappers.
        /// </summary>
        /// <returns>A list of all dialog wrappers.</returns>
        private IEnumerable<DialogWrapper> GetDialogWrappers()
        {
            return this.mainAppWindow.PART_DialogCanvas.Children.OfType<DialogWrapper>();
        }

        /// <summary>
        /// Called when [mouse down].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnMouseDown(object sender, RoutedEventArgs e)
        {
            var source = e.Source as DependencyObject;
            if (source == null)
            {
                return;
            }

            var clickedDialog = this.GetDialogWrappers().FirstOrDefault(source.IsInstanceOrChildOf);

            if (clickedDialog != null)
            {
                BringToTop(clickedDialog);
            }
        }

        /// <summary>
        /// Brings to top.
        /// </summary>
        /// <param name="wrapper">The wrapper.</param>
        private void BringToTop(DependencyObject wrapper)
        {
            var dialogWrappers = this.GetDialogWrappers();

            var maxZIndex = dialogWrappers.Max(w => (int)w.GetValue(Panel.ZIndexProperty));

            var currentZIndex = (int)wrapper.GetValue(Panel.ZIndexProperty);

            if (maxZIndex == 0 || currentZIndex != maxZIndex)
            {
                wrapper.SetValue(Panel.ZIndexProperty, maxZIndex + 1);
            }

            foreach (var dialogWrapper in dialogWrappers)
            {
                dialogWrapper.DialogContent.SetValue(DialogWrapper.IsTopMostProperty, dialogWrapper == wrapper);
            }
        }

        /// <summary>
        /// Called when [mouse move].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnMouseMove(object sender, RoutedEventArgs e)
        {
            foreach (var dialogWrapper in this.GetDialogWrappers())
            {
                dialogWrapper.OnMouseMove();
            }
        }

        /// <summary>
        /// Called when [mouse up].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void OnMouseUp(object sender, RoutedEventArgs e)
        {
            foreach (var dialogWrapper in this.GetDialogWrappers())
            {
                dialogWrapper.OnMouseUp();
            }
        }
    }
}
