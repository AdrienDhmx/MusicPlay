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
    /// Interaction logic for TrackArtistsListView.xaml
    /// </summary>
    public partial class TrackArtistsListView : UserControl
    {
        public TrackArtistsListView()
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
            DependencyProperty.Register("Artists", typeof(ObservableCollection<TrackArtistsRole>), typeof(TrackArtistsListView), new PropertyMetadata(new ObservableCollection<TrackArtistsRole>()));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Command.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(TrackArtistsListView), new PropertyMetadata(defaultValue:null));
    }
}
