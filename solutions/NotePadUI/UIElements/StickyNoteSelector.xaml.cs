using System;
using System.Windows;
using TfsWorkbench.UIElements;

namespace TfsWorkbench.NotePadUI.UIElements
{
    /// <summary>
    /// Interaction logic for StickyNoteSelector.xaml
    /// </summary>
    public partial class StickyNoteSelector
    {
        private readonly RoutedEventHandler handleMouseDown;

        public StickyNoteSelector()
        {
            handleMouseDown = OnHandleMouseDown;

            InitializeComponent();

            if (Application.Current != null && Application.Current.MainWindow != null)
            {
                Application.Current.MainWindow.AddHandler(MouseDownEvent, handleMouseDown, true);
            }
        }

        private void OnHandleMouseDown(object sender, RoutedEventArgs e)
        {
            if (!PART_NoteSelectorPopup.IsOpen)
            {
                return;
            }

            var source = e.OriginalSource as DependencyObject;

            try
            {
                if (source.IsInstanceOrChildOf(PART_ToggleButton) || source.IsInstanceOrChildOf(PART_PopupContent))
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                if (CommandLibrary.ApplicationExceptionCommand.CanExecute(ex, this))
                {
                    CommandLibrary.ApplicationExceptionCommand.Execute(ex, this);
                }
                else
                {
                    throw;
                }
            }

            PART_NoteSelectorPopup.IsOpen = false;
        }

        private void PART_ToggleButton_OnClick(object sender, RoutedEventArgs e)
        {
            PART_NoteSelectorPopup.IsOpen = !PART_NoteSelectorPopup.IsOpen;
        }
    }
}
