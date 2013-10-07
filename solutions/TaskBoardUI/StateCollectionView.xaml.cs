// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateCollectionView.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   Interaction logic for StateCollectionView.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.TaskBoardUI
{
    using System.Windows;

    using TfsWorkbench.TaskBoardUI.DataObjects;

    /// <summary>
    /// Interaction logic for StateCollectionView.xaml
    /// </summary>
    public partial class StateCollectionView
    {
        /// <summary>
        /// The StateCollection Dependency Property
        /// </summary>
        private static readonly DependencyProperty stateCollectionProperty = DependencyProperty.Register(
            "StateCollection", 
            typeof(StateCollection), 
            typeof(StateCollectionView));

        /// <summary>
        /// Initializes a new instance of the <see cref="StateCollectionView"/> class.
        /// </summary>
        public StateCollectionView()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets the state collection property.
        /// </summary>
        /// <value>The state collection property.</value>
        public static DependencyProperty StateCollectionProperty
        {
            get { return stateCollectionProperty; }
        }

        /// <summary>
        /// Gets or sets the state collection.
        /// </summary>
        /// <value>The state collection.</value>
        public StateCollection StateCollection
        {
            get { return (StateCollection)this.GetValue(StateCollectionProperty); }
            set { this.SetValue(StateCollectionProperty, value); }
        }
    }
}