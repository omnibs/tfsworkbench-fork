using System;
using System.ComponentModel;
using System.Xml.Serialization;
using TfsWorkbench.UIElements;

namespace TfsWorkbench.NotePadUI.Models
{
    [XmlInclude(typeof(WorkbenchPadItem))]
    [XmlInclude(typeof(NotePadItem))]
    [XmlInclude(typeof(ToDoList))]
    [XmlInclude(typeof(ToDoItem))]
    [Serializable]
    public class PadItemBase : INotifyPropertyChanged, IColour
    {
        private int zIndex;
        public event PropertyChangedEventHandler PropertyChanged;

        public int Id { get; set; }
        public int ZIndex
        {
            get { return zIndex; }
            set
            {
                if (zIndex == value)
                {
                    return;
                }

                zIndex = value;

                OnPropertyChanged("ZIndex");
            }
        }

        public double LeftOffset { get; set; }
        public double TopOffset { get; set; }
        public string ProjectGuid { get; set; }
        public double Angle { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public string Colour { get; set; }
        public bool IsPinable { get; set; }
        public int? PinnedToId { get; set; }

        public virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}