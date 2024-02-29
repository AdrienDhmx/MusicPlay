using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using MusicPlay.Database.Models;

namespace MusicPlayUI.MVVM.Views.PopupViews
{
    /// <summary>
    /// Interaction logic for AddToPlaylistPopup.xaml
    /// </summary>
    public partial class AddToPlaylistPopup : UserControl
    {
        public AddToPlaylistPopup()
        {
            InitializeComponent();
        }

        public ObservableCollection<Playlist> Playlists
        {
            get { return (ObservableCollection<Playlist>)GetValue(PlaylistsProperty); }
            set { SetValue(PlaylistsProperty, value); }
        }

        public Visibility ShowDropShadow
        {
            get { return (Visibility)GetValue(ShowDropShadowProperty); }
            set { SetValue(ShowDropShadowProperty, value); }
        }

        public ICommand AddToPlaylistCommand
        {
            get { return (ICommand)GetValue(AddToPlaylistCommandProperty); }
            set { SetValue(AddToPlaylistCommandProperty, value); }
        }

        public ICommand CreatePlaylistCommand
        {
            get { return (ICommand)GetValue(CreatePlaylistCommandProperty); }
            set { SetValue(CreatePlaylistCommandProperty, value); }
        }

        public static readonly DependencyProperty CreatePlaylistCommandProperty =
            DependencyProperty.Register("CreatePlaylistCommand", typeof(ICommand), typeof(AddToPlaylistPopup), new PropertyMetadata(null));

        public static readonly DependencyProperty AddToPlaylistCommandProperty =
            DependencyProperty.Register("AddToPlaylistCommand", typeof(ICommand), typeof(AddToPlaylistPopup), new PropertyMetadata(null));

        public static readonly DependencyProperty ShowDropShadowProperty =
            DependencyProperty.Register("ShowDropShadow", typeof(Visibility), typeof(AddToPlaylistPopup), new PropertyMetadata(Visibility.Visible));

        public static readonly DependencyProperty PlaylistsProperty =
            DependencyProperty.Register("Playlists", typeof(ObservableCollection<Playlist>), typeof(AddToPlaylistPopup), new PropertyMetadata(new ObservableCollection<Playlist>(), OnPlaylistsChanged));

        private static void OnPlaylistsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AddToPlaylistPopup popup = (AddToPlaylistPopup)d;

            if(popup != null && e.NewValue is ObservableCollection<Playlist> playlists)
            {
                popup.ShowDropShadow = playlists.Count > 4 ? Visibility.Visible : Visibility.Collapsed;
            }
        }
    }
}
