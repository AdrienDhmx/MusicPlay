using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MusicPlay.Database.Models;

namespace MusicPlayUI.MVVM.Views.ListViews
{
    /// <summary>
    /// Interaction logic for ArtistsRelationListView.xaml
    /// </summary>
    public partial class ArtistsRelationListView : UserControl
    {
        public ArtistsRelationListView()
        {
            InitializeComponent();
        }

        public ObservableCollection<TrackArtistsRole> Artists
        {
            get { return (ObservableCollection<TrackArtistsRole>)GetValue(ArtistsProperty); }
            set { SetValue(ArtistsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TrackArtistRole.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ArtistsProperty =
            DependencyProperty.Register("Artists", typeof(ObservableCollection<TrackArtistsRole>), typeof(ArtistsRelationListView), new PropertyMetadata(new ObservableCollection<TrackArtistsRole>()));


        public int AcceptAlbumArtist
        {
            get { return (int)GetValue(AcceptAlbumArtistProperty); }
            set { SetValue(AcceptAlbumArtistProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AcceptAlbumArtist.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AcceptAlbumArtistProperty =
            DependencyProperty.Register("AcceptAlbumArtist", typeof(int), typeof(ArtistsRelationListView), new PropertyMetadata(-1));



        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(ArtistsRelationListView), new PropertyMetadata(defaultValue: null));


    }
}
