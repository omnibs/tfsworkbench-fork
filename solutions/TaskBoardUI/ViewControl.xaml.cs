// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewControl.xaml.cs" company="None">
//   None
// </copyright>
// <summary>
//   Interaction logic for ViewControl.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.TaskBoardUI
{
    using System;
    using System.Windows;

    using Core.Interfaces;

    using TfsWorkbench.TaskBoardUI.DataObjects;

    /// <summary>
    /// Interaction logic for ViewControl.xaml
    /// </summary>
    public partial class ViewControl
    {
        /// <summary>
        /// The view controller instance.
        /// </summary>
        private readonly ViewController controller;

        /// <summary>
        /// The project data property.
        /// </summary>
        private static readonly DependencyProperty projectDataProperty = DependencyProperty.Register(
            "ProjectData",
            typeof(IProjectData),
            typeof(ViewControl),
            new PropertyMetadata(null, OnProjectDataChanged));

        /// <summary>
        /// The View Property development.
        /// </summary>
        private static readonly DependencyProperty swimLaneViewProperty = DependencyProperty.Register(
            "SwimLaneView", 
            typeof(SwimLaneView), 
            typeof(ViewControl),
            new PropertyMetadata(null, OnSwimLaneViewPropertyChanged));

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewControl"/> class.
        /// </summary>
        public ViewControl()
        {
            this.InitializeComponent();
            this.controller = new ViewController(this);
        }

        /// <summary>
        /// Gets the view property.
        /// </summary>
        /// <value>The view property.</value>
        public static DependencyProperty SwimLaneViewProperty
        {
            get { return swimLaneViewProperty; }
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
        /// Gets or sets the project data.
        /// </summary>
        /// <value>The project data.</value>
        public IProjectData ProjectData
        {
            get { return (IProjectData)this.GetValue(ProjectDataProperty); }
            set { this.SetValue(ProjectDataProperty, value); }
        }

        /// <summary>
        /// Gets or sets the view.
        /// </summary>
        /// <value>The view object.</value>
        public SwimLaneView SwimLaneView
        {
            get { return (SwimLaneView)this.GetValue(SwimLaneViewProperty); }
            set { this.SetValue(SwimLaneViewProperty, value); }
        }

        /// <summary>
        /// Releases the resources.
        /// </summary>
        public void ReleaseResources()
        {
            this.controller.ReleaseResources();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.FrameworkElement.SizeChanged"/> event, using the specified information as part of the eventual event data.
        /// </summary>
        /// <param name="sizeInfo">Details of the old and new size involved in the change.</param>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            this.controller.ResizeColumnsToFit();
        }

        /// <summary>
        /// Called when [project data changed].
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        /// <param name="args">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnProjectDataChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs args)
        {
            var control = dependencyObject as ViewControl;
            if (control == null || control.ProjectData == null)
            {
                return;
            }

            control.controller.SetSortFields();
        }

        /// <summary>
        /// Called when [swim lane view property changed].
        /// </summary>
        /// <param name="d">The dedendency object.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnSwimLaneViewPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ViewControl;

            if (control == null || control.SwimLaneView == null)
            {
                return;
            }

            control.controller.WireUpSwimLaneView();
        }
    }
}