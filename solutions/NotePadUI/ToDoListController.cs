using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using TfsWorkbench.NotePadUI.Models;
using TfsWorkbench.UIElements;

namespace TfsWorkbench.NotePadUI
{
    public class ToDoListController
    {
        private readonly ToDoList toDoList;
        private readonly ObservableCollection<ToDoItem> toDoItems = new ObservableCollection<ToDoItem>();

        public ToDoListController(ToDoList toDoList)
        {
            if (toDoList == null)
            {
                throw new ArgumentNullException("toDoList");
            }

            this.toDoList = toDoList;

            foreach (var toDoItem in toDoList.ToDoItems)
            {
                ToDoItems.Add(toDoItem);
            }

            ToDoItems.CollectionChanged += OnToDoItemsCollectionChanged;

            InitialiseCommands();
        }

        public ObservableCollection<ToDoItem> ToDoItems
        {
            get { return toDoItems; }
        }

        public ICommand DeleteCommand { get; set; }

        public ICommand AddCommand { get; set; }

        private void OnToDoItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (var item in e.NewItems.OfType<ToDoItem>())
                    {
                        toDoList.ToDoItems.Add(item);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var item in e.OldItems.OfType<ToDoItem>())
                    {
                        toDoList.ToDoItems.Remove(item);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    toDoList.ToDoItems.Clear();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void InitialiseCommands()
        {
            DeleteCommand = new RelayCommand(OnDeleteItem);
            AddCommand = new RelayCommand(OnAddItem);
        }

        private void OnAddItem(object obj)
        {
            ToDoItems.Add(new ToDoItem { Text = "Add your todo text here."} );
        }

        private void OnDeleteItem(object obj)
        {
            var itemToDelete = obj as ToDoItem;

            toDoItems.Remove(itemToDelete);
        }
    }
}