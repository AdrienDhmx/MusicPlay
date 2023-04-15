using IconButton;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
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
    /// Logique d'interaction pour NowPlayingView.xaml
    /// </summary>
    public partial class NowPlayingView : UserControl
    {
        public NowPlayingView()
        {
            InitializeComponent();
            this.Unloaded += NowPlayingView_Unloaded;
        }

        private void NowPlayingView_Unloaded(object sender, RoutedEventArgs e)
        {
            spectrum.Dispose();
            Unloaded -= NowPlayingView_Unloaded;
        }
    }
}
