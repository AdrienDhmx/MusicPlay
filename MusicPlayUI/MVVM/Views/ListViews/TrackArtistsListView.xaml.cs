using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DynamicScrollViewer;
using MusicPlay.Database.Models;
using MusicPlayUI.Controls;

namespace MusicPlayUI.MVVM.Views.ListViews
{
    /// <summary>
    /// Interaction logic for TrackArtistsListView.xaml
    /// </summary>
    public partial class TrackArtistsListView : UserControl, IIsInViewport
    {
        public TrackArtistsListView()
        {
            InitializeComponent();
        }

        public Track Track
        {
            get { return (Track)GetValue(TrackProperty); }
            set { SetValue(TrackProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Track.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TrackProperty =
            DependencyProperty.Register("Track", typeof(Track), typeof(TrackArtistsListView), new PropertyMetadata(null));


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

        public bool IsInViewport
        {
            get { return (bool)GetValue(IsInViewportProperty); }
            set { SetValue(IsInViewportProperty, value); }
        }

        public static readonly DependencyProperty IsInViewportProperty =
            DependencyProperty.Register("IsInViewport", typeof(bool), typeof(TrackArtistsListView), new PropertyMetadata(false, OnIsInViewPortChange));

        private static void OnIsInViewPortChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TrackArtistsListView trackArtistsListView && e.NewValue is bool newvalue)
            {
                trackArtistsListView.IsInViewPortChange(newvalue);
            }
        }

        private void IsInViewPortChange(bool isInViewport)
        {
            if (isInViewport)
            {
                Artists = Track.TrackArtistRole; // only load the artists once in viewport
            }
        }
    }
}
