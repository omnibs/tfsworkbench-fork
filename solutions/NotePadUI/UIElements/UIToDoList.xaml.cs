using System.Windows;
using TfsWorkbench.NotePadUI.Models;

namespace TfsWorkbench.NotePadUI.UIElements
{
    /// <summary>
    /// Interaction logic for UIToDoList.xaml
    /// </summary>
    public partial class UIToDoList
    {
        public static readonly DependencyProperty ToDoListProperty = DependencyProperty.Register(
            "ToDoList", 
            typeof (ToDoList), 
            typeof (UIToDoList), 
            new PropertyMetadata(default(ToDoList), OnToDoListChanged));

        private static void OnToDoListChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var instance = d as UIToDoList;

            if (instance == null || instance.ToDoList == null)
            {
                return;
            }

            instance.DataContext = new ToDoListController(instance.ToDoList);
        }

        public ToDoList ToDoList
        {
            get { return (ToDoList) GetValue(ToDoListProperty); }
            set { SetValue(ToDoListProperty, value); }
        }

        public UIToDoList()
        {
            InitializeComponent();
        }
    }
}
