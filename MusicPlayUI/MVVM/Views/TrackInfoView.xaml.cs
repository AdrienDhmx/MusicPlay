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

namespace MusicPlayUI.MVVM.Views
{
    /// <summary>
    /// Logique d'interaction pour TrackInfoView.xaml
    /// </summary>
    public partial class TrackInfoView : UserControl
    {
        public TrackInfoView()
        {
            InitializeComponent();
        }

        private void RootScroll_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            RootScroll.ScrollToVerticalOffset(RootScroll.VerticalOffset - e.Delta / 3);
        }
    }
}
