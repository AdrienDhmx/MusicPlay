using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MusicPlayUI.MVVM.Views.SettingsViews
{
    /// <summary>
    /// Logique d'interaction pour GeneralSettingView.xaml
    /// </summary>
    public partial class GeneralSettingView : UserControl
    {
        public GeneralSettingView()
        {
            InitializeComponent();
        }

        private void SettingScroll_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            SettingScroll.ScrollToVerticalOffset(SettingScroll.VerticalOffset - e.Delta / 3);
        }
    }
}
