using System;
using TfsWorkbench.NotePadUI.Models;

namespace TfsWorkbench.NotePadUI.Helpers
{
    public static class PadItemFactory
    {
        private static readonly Random Rnd = new Random();

        public static TPadItem CreateInstance<TPadItem>(Guid projectGuid, Action<TPadItem> initialiser = null) where TPadItem : PadItemBase, new()
        {
            var output = new TPadItem
                {
                    LeftOffset = Rnd.Next(200),
                    TopOffset = Rnd.Next(200),
                    Width = 200,
                    Height = 175,
                    ProjectGuid = projectGuid.ToString(),
                    Angle = Rnd.Next(10) - 5
                };

            if (initialiser != null)
            {
                initialiser(output);
            }

            return output;
        }            
    }
}
