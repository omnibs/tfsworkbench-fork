using System;
using System.Xml.Serialization;
using TfsWorkbench.Core.Interfaces;

namespace TfsWorkbench.NotePadUI.Models
{
    [Serializable]
    public class WorkbenchPadItem : PadItemBase, ISelectable
    {
        public int WorkbenchItemId { get; set; }

        public bool IsSelected { get; set; }

        [XmlIgnore]
        public IWorkbenchItem WorkbenchItem { get; set; }
    }
}