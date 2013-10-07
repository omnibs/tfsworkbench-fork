// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EditNodeControl.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   Interaction logic for EditNodeControl.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.ProjectSetupUI.NodeVisualisation
{
    using System;
    using System.Windows;
    using System.Windows.Threading;

    using Core.Interfaces;

    using DataObjects;

    using Helpers;

    using UIElements;

    /// <summary>
    /// Interaction logic for EditNodeControl.xaml
    /// </summary>
    public partial class EditNodeControl
    {
        /// <summary>
        /// The parent node property.
        /// </summary>
        private static readonly DependencyProperty parentNodeProperty = DependencyProperty.Register(
            "ParentNode",
            typeof(ProjectNodeVisual),
            typeof(EditNodeControl));

        /// <summary>
        /// The child node property.
        /// </summary>
        private static readonly DependencyProperty childNodeProperty = DependencyProperty.Register(
            "ChildNode",
            typeof(ProjectNodeVisual),
            typeof(EditNodeControl));

        /// <summary>
        /// Initializes a new instance of the <see cref="EditNodeControl"/> class.
        /// </summary>
        public EditNodeControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the parent node property.
        /// </summary>
        /// <value>The parent node property.</value>
        public static DependencyProperty ParentNodeProperty
        {
            get { return parentNodeProperty; }
        }

        /// <summary>
        /// Gets the child node property.
        /// </summary>
        /// <value>The child node property.</value>
        public static DependencyProperty ChildNodeProperty
        {
            get { return childNodeProperty; }
        }

        /// <summary>
        /// Gets or sets the parent node.
        /// </summary>
        /// <value>The parent node.</value>
        public IProjectNode ParentNode
        {
            get { return (IProjectNode)this.GetValue(ParentNodeProperty); }
            set { this.SetValue(ParentNodeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the child node.
        /// </summary>
        /// <value>The child node.</value>
        public IProjectNode ChildNode
        {
            get { return (IProjectNode)this.GetValue(ChildNodeProperty); }
            set { this.SetValue(ChildNodeProperty, value); }
        }

        /// <summary>
        /// Gets or sets the cancel callback.
        /// </summary>
        /// <value>The cancel callback.</value>
        public Action CancelCallback
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the callback.
        /// </summary>
        /// <value>The callback.</value>
        public Action SaveCallback
        {
            get;
            set;
        }

        /// <summary>
        /// Handles the TextChanged event of the TextBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.TextChangedEventArgs"/> instance containing the event data.</param>
        private void TextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            this.SaveButton.IsEnabled = ValidationHelper.IsValidName(this.ChildNode.Name)
                                        && ValidationHelper.HasUniqueNames(this.ParentNode.Children);
        }

        /// <summary>
        /// Handles the Click event of the Close control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            this.QueueCallback(
                () =>
                    {
                        if (this.SaveCallback != null)
                        {
                            this.SaveCallback();
                        }

                        CommandLibrary.CloseDialogCommand.Execute(this, this);
                    });

            this.IsEnabled = false;
        }

        /// <summary>
        /// Handles the Click event of the Cancel control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.QueueCallback(
                () =>
                {
                    if (this.CancelCallback != null)
                    {
                        this.CancelCallback();
                    }

                    CommandLibrary.CloseDialogCommand.Execute(this, this);
                });

            this.IsEnabled = false;
        }

        /// <summary>
        /// Queues the callback.
        /// </summary>
        /// <param name="callback">The callback.</param>
        private void QueueCallback(Action callback)
        {
            var dispatcherCallback = (System.Threading.SendOrPostCallback)delegate { callback(); };

            this.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, dispatcherCallback, null);
        }
    }
}