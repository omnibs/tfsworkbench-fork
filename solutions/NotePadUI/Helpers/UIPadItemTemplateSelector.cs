using System.Windows;
using System.Windows.Controls;
using TfsWorkbench.NotePadUI.Models;

namespace TfsWorkbench.NotePadUI.Helpers
{
    public class UiPadItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate WorkbenchItemTemplate { get; set; }
        public DataTemplate StickyNoteTemplate { get; set; }
        public DataTemplate ToDoTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var wbiPadItem = item as WorkbenchPadItem;

            if (wbiPadItem != null)
            {
                return WorkbenchItemTemplate;
            }

            var stickyNoteItem = item as NotePadItem;
            if (stickyNoteItem != null)
            {
                return StickyNoteTemplate;
            }

            var toList = item as ToDoList;
            if (toList != null)
            {
                return ToDoTemplate;
            }

            return null;
        }
    }
}