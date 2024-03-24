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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MusicPlayUI.MVVM.Views.Windows;

namespace MusicPlayUI.MVVM.Views
{
    /// <summary>
    /// Logique d'interaction pour HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();
            Unloaded += HomeView_Unloaded;
        }

        private void HomeView_Unloaded(object sender, RoutedEventArgs e)
        {
            Chart.Dispose();
            Unloaded -= HomeView_Unloaded;
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Chart.Redraw();
        }
    }
}
