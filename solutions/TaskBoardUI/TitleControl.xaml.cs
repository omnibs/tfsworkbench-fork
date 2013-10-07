// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TitleControl.xaml.cs" company="EMC Consulting">
//   EMC Consulting 2009
// </copyright>
// <summary>
//   Interaction logic for TitleControl.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Emcc.TeamSystem.TaskBoard.TaskBoardUI
{
    using System.ComponentModel.Composition;
    using System.Windows;
    using System.Windows.Controls;

    using Core.Interfaces;

    /// <summary>
    /// Interaction logic for TitleControl.xaml
    /// </summary>
    [Export(typeof(ITaskBoardDockItem))]
    public partial class TitleControl : UserControl, ITaskBoardDockItem
    {
        public static DependencyProperty ProjectDataProperty = DependencyProperty.Register(
            "ProjectData",
            typeof(IProjectData),
            typeof(TitleControl),
            new PropertyMetadata(null, OnProjectDataChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="TitleControl"/> class.
        /// </summary>
        public TitleControl()
        {
            this.InitializeComponent();
            this.DataContext = this;
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
        /// Gets the display priority.
        /// </summary>
        /// <value>The display priority.</value>
        public int DisplayPriority
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// Called when [project data changed].
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnProjectDataChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as TitleControl;
            if (control == null)
            {
                return;
            }

            control.OnPropertyChanged(new DependencyPropertyChangedEventArgs(ProjectDataProperty, null, control.ProjectData));
        }
    }
}