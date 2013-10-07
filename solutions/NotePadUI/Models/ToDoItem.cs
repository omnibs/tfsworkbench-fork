using System;
using System.ComponentModel;

namespace TfsWorkbench.NotePadUI.Models
{
    [Serializable]
    public class ToDoItem : INotifyPropertyChanged
    {
        private bool isDone;
        public event PropertyChangedEventHandler PropertyChanged;

        public string Text { get; set; }
        public bool IsDone
        {
            get { return isDone; }
            set
            {
                if (isDone == value)
                {
                    return;
                }

                isDone = value;

                OnPropertyChanged("IsDone");
            }
        }

        protected virtual void OnPropertyChanged(string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}