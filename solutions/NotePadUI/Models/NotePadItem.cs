using System;

namespace TfsWorkbench.NotePadUI.Models
{
    [Serializable]
    public class NotePadItem : PadItemBase
    {
        public DateTime Created { get; set; }

        public String Text { get; set; }
    }
}