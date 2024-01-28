using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MusicPlay.Database.Models;

namespace MusicPlayUI.MVVM.Views.ListViews
{
    /// <summary>
    /// Interaction logic for AlbumArtistslistView.xaml
    /// </summary>
    public partial class AlbumArtistslistView : UserControl
    {
        public AlbumArtistslistView()
        {
            InitializeComponent();
        }

        public List<TrackArtistsRole> Artists
        {
            get { return (List<TrackArtistsRole>)GetValue(ArtistsProperty); }
            set { SetValue(ArtistsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TrackArtistRole.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ArtistsProperty =
            DependencyProperty.Register("Artists", typeof(List<TrackArtistsRole>), typeof(AlbumArtistslistView), new PropertyMetadata(new List<TrackArtistsRole>()));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(AlbumArtistslistView), new PropertyMetadata(defaultValue: null));

    }
}
