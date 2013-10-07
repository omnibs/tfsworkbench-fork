using System;
using System.Collections.ObjectModel;

namespace TfsWorkbench.NotePadUI.Models
{
    [Serializable]
    public class ToDoList : PadItemBase
    {
        private readonly Collection<ToDoItem> toDoItems = new Collection<ToDoItem>();

        public Collection<ToDoItem> ToDoItems
        {
            get { return toDoItems; }
        }
    }
}