// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DecisionControl.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   Interaction logic for DecisionControl.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.UIElements
{
    using System;
    using System.Windows;

    /// <summary>
    /// Interaction logic for DecisionControl.xaml
    /// </summary>
    public partial class DecisionControl 
    {
        /// <summary>
        /// The caption propperty.
        /// </summary>
        private static readonly DependencyProperty captionProperty = DependencyProperty.Register(
            "Caption", typeof(string), typeof(DecisionControl));

        /// <summary>
        /// The message property.
        /// </summary>
        private static readonly DependencyProperty messageProperty = DependencyProperty.Register(
            "Message", typeof(string), typeof(DecisionControl));

        /// <summary>
        /// The Do Not Show Again Property.
        /// </summary>
        private static readonly DependencyProperty doNotShowAgainProperty = DependencyProperty.Register(
            "DoNotShowAgain", typeof(bool), typeof(DecisionControl));

        /// <summary>
        /// The do Not Show Again Text Property.
        /// </summary>
        private static readonly DependencyProperty doNotShowAgainTextProperty = DependencyProperty.Register(
            "DoNotShowAgainText", 
            typeof(string), 
            typeof(DecisionControl),
            new PropertyMetadata("Don't ask me again"));

        public static readonly DependencyProperty hideDoNotShowAgainProperty = DependencyProperty.Register(
            "HideDoNotShowAgain", 
            typeof (bool), 
            typeof (DecisionControl), 
            new PropertyMetadata(false));

        /// <summary>
        /// Initializes a new instance of the <see cref="DecisionControl"/> class.
        /// </summary>
        public DecisionControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Occurs when [decision made].
        /// </summary>
        public event EventHandler DecisionMade;

        public bool HideDoNotShowAgain
        {
            get { return (bool) GetValue(hideDoNotShowAgainProperty); }
            set { SetValue(hideDoNotShowAgainProperty, value); }
        }

        /// <summary>
        /// Gets the caption property.
        /// </summary>
        /// <value>The caption property.</value>
        public static DependencyProperty CaptionProperty
        {
            get { return captionProperty; }
        }

        /// <summary>
        /// Gets the message property.
        /// </summary>
        /// <value>The message property.</value>
        public static DependencyProperty MessageProperty
        {
            get { return messageProperty; }
        }

        /// <summary>
        /// Gets the do not show again.
        /// </summary>
        /// <value>The do not show again.</value>
        public static DependencyProperty DoNotShowAgainProperty
        {
            get { return doNotShowAgainProperty; }
        }

        /// <summary>
        /// Gets the do not show again text property.
        /// </summary>
        /// <value>The do not show again text property.</value>
        public static DependencyProperty DoNotShowAgainTextProperty
        {
            get { return doNotShowAgainTextProperty; }
        }

        /// <summary>
        /// Gets or sets the caption.
        /// </summary>
        /// <value>The caption.</value>
        public string Caption
        {
            get { return (string)this.GetValue(CaptionProperty); }
            set { this.SetValue(CaptionProperty, value); }
        }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        /// <value>The message.</value>
        public string Message
        {
            get { return (string)this.GetValue(MessageProperty); }
            set { this.SetValue(MessageProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [do not show again].
        /// </summary>
        /// <value><c>true</c> if [do not show again]; otherwise, <c>false</c>.</value>
        public bool DoNotShowAgain
        {
            get { return (bool)this.GetValue(DoNotShowAgainProperty); }
            set { this.SetValue(DoNotShowAgainProperty, value); }
        }

        /// <summary>
        /// Gets or sets the do not show again text.
        /// </summary>
        /// <value>The do not show again text.</value>
        public string DoNotShowAgainText
        {
            get { return (string)this.GetValue(DoNotShowAgainTextProperty); }
            set { this.SetValue(DoNotShowAgainTextProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is yes.
        /// </summary>
        /// <value><c>true</c> if this instance is yes; otherwise, <c>false</c>.</value>
        public bool IsYes { get; set; }

        /// <summary>
        /// Called when [decision made].
        /// </summary>
        public void OnDecisionMade()
        {
            if (this.DecisionMade != null)
            {
                this.DecisionMade(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Handles the OnClick event of the YesButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void YesButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.IsYes = true;

            if (this.DecisionMade != null)
            {
                this.DecisionMade(this, EventArgs.Empty);
            }

            CommandLibrary.CloseDialogCommand.Execute(this, this);
        }

        /// <summary>
        /// Handles the OnClick event of the NoButton control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        private void NoButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.IsYes = false;

            if (this.DecisionMade != null)
            {
                this.DecisionMade(this, EventArgs.Empty);
            }

            CommandLibrary.CloseDialogCommand.Execute(this, this);
        }
    }
}
