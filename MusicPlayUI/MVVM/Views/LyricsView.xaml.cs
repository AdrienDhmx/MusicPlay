using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Logique d'interaction pour LyricsView.xaml
    /// </summary>
    public partial class LyricsView : UserControl
    {

        public LyricsView()
        {
            InitializeComponent();
        }

        private void LyricsScroll_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            LyricsScroll.ScrollToVerticalOffset(LyricsScroll.VerticalOffset - e.Delta / 3);
        }

        private void TimedLyricsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            double height = TimedLyricsListView.ActualHeight;
            int totalItem = TimedLyricsListView.Items.Count;
            double viewPort = LyricsScroll.ViewportHeight;
            double itemheight = height / totalItem;
            double halvedItemInViewPort = (viewPort / itemheight) * 0.3;
            double verticalOffset = itemheight * ((double)TimedLyricsListView.SelectedIndex - halvedItemInViewPort);
            LyricsScroll.ScrollToVerticalOffsetWithAnimation(verticalOffset);
        }
    }
}
