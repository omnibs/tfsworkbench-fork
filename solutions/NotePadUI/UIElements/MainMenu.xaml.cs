using TfsWorkbench.Core.Interfaces;

namespace TfsWorkbench.NotePadUI
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        public IDisplayMode DisplayMode { get; set; }
    }
}
