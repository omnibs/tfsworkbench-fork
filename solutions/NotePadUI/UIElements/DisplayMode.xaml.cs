using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using TfsWorkbench.Core.Interfaces;
using TfsWorkbench.NotePadUI.Helpers;
using TfsWorkbench.NotePadUI.Properties;

namespace TfsWorkbench.NotePadUI
{
    /// <summary>
    /// Interaction logic for DisplayMode.xaml
    /// </summary>
    [Export(typeof(IDisplayMode))]
    public partial class DisplayMode : IDisplayMode
    {
        private readonly MenuItem menuControl;
        private readonly DisplayModeController displayModeController;

        public DisplayMode()
        {
            InitializeComponent();

            displayModeController = new DisplayModeController(this);
            DataContext = displayModeController;
            menuControl = new MainMenu { DisplayMode = this };

            ElementDragHelper.RegisterScrollViewer(PART_LayoutScroller);

            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => PART_LayoutScroller.Focus()));
        }

        public bool IsHighlightProvider
        {
            get { return true; }
        }

        public string Title
        {
            get
            {
                return Settings.Default.Title;
            }
        }

        public string Description
        {
            get
            {
                return Settings.Default.Description;
            }
        }

        public bool IsActive
        {
            get
            {
                return Visibility == Visibility.Visible;
            }
        }

        public void Highlight(IWorkbenchItem workbenchItem)
        {
            displayModeController.Highlight(workbenchItem);
        }

        public int DisplayPriority
        {
            get
            {
                return Settings.Default.DisplayPriority;
            }
        }

        public MenuItem MenuControl
        {
            get
            {
                return menuControl;
            }
        }

        private void ScrollViewerPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers != ModifierKeys.Shift)
            {
                return;
            }

            if (e.Delta < 0)
            {
                PART_ScaleSlider.Value -= 0.05;
            }
            else
            {
                PART_ScaleSlider.Value += 0.05;
            }
        }
    }
}
