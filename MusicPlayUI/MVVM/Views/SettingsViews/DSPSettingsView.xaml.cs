using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MusicPlayUI.MVVM.ViewModels;

namespace MusicPlayUI.MVVM.Views.SettingsViews
{
    /// <summary>
    /// Interaction logic for DSPSettingsView.xaml
    /// </summary>
    public partial class DSPSettingsView : UserControl
    {
        public DSPSettingsView()
        {
            InitializeComponent();
        }

        private void Grid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            UIElement element = Mouse.DirectlyOver as UIElement;

            Popup ancestor = MainViewModel.FindParent<Popup>(element);

            if (element is not null && ancestor is not null)
            {
                PresetPopupScroll.ScrollToVerticalOffset(PresetPopupScroll.VerticalOffset - e.Delta / 3d);
                e.Handled = true;
            }
        }
    }
}
