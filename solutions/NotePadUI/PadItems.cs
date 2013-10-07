using System;
using System.ComponentModel;
using System.Xml.Serialization;
using TfsWorkbench.Core.Interfaces;

namespace TfsWorkbench.NotePadUI
{
    public interface ISelectable
    {
        bool IsSelected { get; set; }
    }

    [XmlInclude(typeof(WorkbenchPadItem))]
    [XmlInclude(typeof(NotePadItem))]
    [XmlInclude(typeof(MarkerItem))]
    [Serializable]
    public class PadItemBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int ZIndex { get; set; }
        public double LeftOffset { get; set; }
        public double TopOffset { get; set; }
        public string ProjectGuid { get; set; }
        public double Angle { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public string Colour { get; set; }

        public virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    [Serializable]
    public class WorkbenchPadItem : PadItemBase, ISelectable
    {
        public int WorkbenchItemId { get; set; }

        public bool IsSelected { get; set; }

        [XmlIgnore]
        public IWorkbenchItem WorkbenchItem { get; set; }
    }

    [Serializable]
    public class NotePadItem : PadItemBase
    {
        public DateTime Created { get; set; }

        public String Text { get; set; }
    }

    [Serializable]
    public class MarkerItem : PadItemBase
    {
    }

}
