using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System;
using MusicPlayUI.Core.Services;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Services.Interfaces;
using System.Collections.ObjectModel;
using Resources = MusicPlay.Language.Resources;
using System.Windows;
using MusicPlay.Database.Models;
using MusicPlay.Database.Models.DataBaseModels;
using MusicPlay.Database.Helpers;
using Root = LastFmNamespace.Models.Root;
using MusicFilesProcessor.Helpers;
using System.IO;
using MusicFilesProcessor;
using DynamicScrollViewer;

namespace MusicPlayUI.MVVM.ViewModels
{
    public class ArtistViewModel : ViewModel
    {
        private readonly ConnectivityHelper _connectivityHelper;

        public IQueueService QueueService { get; }
        private readonly ICommandsManager _commandsManager;
        private string _mainAlbumsHeader;
        public string MainAlbumsHeader
        {
            get { return _mainAlbumsHeader; }
            set
            {
                _mainAlbumsHeader = value;
                OnPropertyChanged(nameof(MainAlbumsHeader));
            }
        }

        private string _singlesAndEPHeader;
        public string SinglesAndEPHeader
        {
            get { return _singlesAndEPHeader; }
            set
            {
                _singlesAndEPHeader = value;
                OnPropertyChanged(nameof(SinglesAndEPHeader));
            }
        }

        private string _featuredInHeader;
        public string FeaturedInHeader
        {
            get { return _featuredInHeader; }
            set
            {
                _featuredInHeader = value;
                OnPropertyChanged(nameof(FeaturedInHeader));
            }
        }

        private string _composerOfHeader;
        public string ComposerOfHeader
        {
            get { return _composerOfHeader; }
            set
            {
                _composerOfHeader = value;
                OnPropertyChanged(nameof(ComposerOfHeader));
            }
        }

        private string _performedInHeader;
        public string PerformedInHeader
        {
            get { return _performedInHeader; }
            set
            {
                _performedInHeader = value;
                OnPropertyChanged(nameof(PerformedInHeader));
            }
        }

        private string _lyricistOfHeader;
        public string LyricistOfHeader
        {
            get { return _lyricistOfHeader; }
            set
            {
                _lyricistOfHeader = value;
                OnPropertyChanged(nameof(LyricistOfHeader));
            }
        }

        private Visibility _mainAlbumsVisibility = Visibility.Visible;
        public Visibility MainAlbumsVisibility
        {
            get { return _mainAlbumsVisibility; }
            set
            {
                _mainAlbumsVisibility = value;
                OnPropertyChanged(nameof(MainAlbumsVisibility));
            }
        }

        private Visibility _singlesAndEPVisibility = Visibility.Visible;
        public Visibility SinglesAndEPVisibility
        {
            get { return _singlesAndEPVisibility; }
            set
            {
                _singlesAndEPVisibility = value;
                OnPropertyChanged(nameof(SinglesAndEPVisibility));
            }
        }


        private Visibility _featuredInAlbumsVisbility = Visibility.Visible;
        public Visibility FeaturedInAlbumsVisbility
        {
            get { return _featuredInAlbumsVisbility; }
            set
            {
                _featuredInAlbumsVisbility = value;
                OnPropertyChanged(nameof(FeaturedInAlbumsVisbility));
            }
        }

        private Visibility _composedTracksVisibility = Visibility.Visible;
        public Visibility ComposedTracksVisibility
        {
            get { return _composedTracksVisibility; }
            set
            {
                _composedTracksVisibility = value;
                OnPropertyChanged(nameof(ComposedTracksVisibility));
            }
        }


        private Visibility _performedTracksVisibility = Visibility.Visible;
        public Visibility PerformedTracksVisibility
        {
            get { return _performedTracksVisibility; }
            set
            {
                _performedTracksVisibility = value;
                OnPropertyChanged(nameof(PerformedTracksVisibility));
            }
        }

        private Visibility _lyricistTracksVisibility = Visibility.Visible;
        public Visibility LyricistTracksVisiblity
        {
            get { return _lyricistTracksVisibility; }
            set
            {
                _lyricistTracksVisibility = value;
                OnPropertyChanged(nameof(LyricistTracksVisiblity));
            }
        }

        private Artist _artist;
        public Artist Artist
        {
            get { return _artist; }
            set
            {
                _artist = value;
                OnPropertyChanged(nameof(Artist));
            }
        }

        private string _biography;
        public string Biography
        {
            get => _biography;
            set
            {
                SetField(ref _biography, value);
            }
        }

        private ObservableCollection<Album> _mainAlbums = new();
        public ObservableCollection<Album> MainAlbums
        {
            get { return _mainAlbums; }
            set
            {
                _mainAlbums = value;
                OnPropertyChanged(nameof(MainAlbums));
            }
        }

        private ObservableCollection<Album> _singlesAndEP = new();
        public ObservableCollection<Album> SinglesAndEP
        {
            get { return _singlesAndEP; }
            set
            {
                _singlesAndEP = value;
                OnPropertyChanged(nameof(SinglesAndEP));
            }
        }

        private List<Track> _tracks;
        public List<Track> Tracks
        {
            get { return _tracks; }
            set
            {
                _tracks = value;
                OnPropertyChanged(nameof(Tracks));
            }
        }

        private List<Tag> _genres;
        public List<Tag> Genres
        {
            get { return _genres; }
            set
            {
                _genres = value;
                OnPropertyChanged(nameof(Genres));
            }
        }

        private ObservableCollection<Album> _featuredInAlbum = new();
        public ObservableCollection<Album> FeaturedInAlbum
        {
            get { return _featuredInAlbum; }
            set
            {
                _featuredInAlbum = value;
                OnPropertyChanged(nameof(FeaturedInAlbum));
            }
        }

        private ObservableCollection<OrderedTrack> _performedInTracks = new();
        public ObservableCollection<OrderedTrack> PerformedInTracks
        {
            get { return _performedInTracks; }
            set
            {
                _performedInTracks = value;
                OnPropertyChanged(nameof(PerformedInTracks));
            }
        }

        private ObservableCollection<OrderedTrack> _composedTracks = new();
        public ObservableCollection<OrderedTrack> ComposedTracks
        {
            get { return _composedTracks; }
            set
            {
                _composedTracks = value;
                OnPropertyChanged(nameof(ComposedTracks));
            }
        }

        private ObservableCollection<OrderedTrack> _lyricistOfTracks = new();
        public ObservableCollection<OrderedTrack> LyricistOfTracks
        {
            get { return _lyricistOfTracks; }
            set
            {
                _lyricistOfTracks = value;
                OnPropertyChanged(nameof(LyricistOfTracks));
            }
        }

        public ICommand PlayArtistCommand { get; }
        public ICommand PlayArtistShuffledCommand { get; }
        public ICommand PlayTrackCommand { get; }
        public ICommand PlayAlbumCommand { get; }
        public ICommand PlayAlbumsOnlyCommand { get; }
        public ICommand PlaySinglesAndEpCommand { get; }
        public ICommand PlayFeaturedInAlbumsCommand { get; }
        public ICommand PlayComposedTracksCommand { get; }
        public ICommand PlayPerformedTracksCommand { get; }
        public ICommand PlayLyricistOfTracksCommand { get; }
        public ICommand NavigateToAlbumCommand { get; }
        public ICommand NavigateToGenreCommand { get; }
        public ICommand NavigateToAlbumByIdCommand { get; }
        public ICommand NavigateToArtistByIdCommand { get; }
        public ICommand OpenAlbumPopupCommand { get; }
        public ICommand NavigateBackCommand { get; }
        public ICommand OpenTrackPopupCommand { get; }
        public ICommand OpenArtistPopupCommand { get; }
        public ICommand ShowHideMainAlbumsCommand { get; }
        public ICommand ShowHideSinglesAndEPCommand { get; }
        public ICommand ShowHideFeaturedAlbumsCommand { get; }
        public ICommand ShowHideComposedTracksCommand { get; }
        public ICommand ShowHidePerformedTracksCommand { get; }
        public ICommand ShowHideLyricistTracksCommand { get; }
        public ArtistViewModel(IQueueService queueService, ICommandsManager commandsManager)
        {
            this.QueueService = queueService;
            _commandsManager = commandsManager;

            // play
            PlayArtistCommand = _commandsManager.PlayNewQueueCommand;
            PlayArtistShuffledCommand = _commandsManager.PlayNewQueueShuffledCommand;
            PlayTrackCommand = new RelayCommand<Track>((track) => PlayArtist(false, track));
            PlayAlbumCommand = _commandsManager.PlayNewQueueCommand;
            PlayAlbumsOnlyCommand = new RelayCommand<string>(async (string shuffled) =>
            {
                await Task.Run(() =>
                {
                    List<Track> tracks = [];
                    foreach (Album album in MainAlbums)
                    {
                        tracks.AddRange(album.Tracks);
                    }

                    QueueService.SetNewQueue(tracks, Artist, Artist.Name, Artist.Cover, null, shuffled == "1", false, false);
                });
            });
            PlaySinglesAndEpCommand = new RelayCommand<string>(async (string shuffled) =>
            {
                await Task.Run(() =>
                {
                    List<Track> tracks = new();
                    foreach (Album album in SinglesAndEP)
                    {
                        tracks.AddRange(album.Tracks);
                    }

                    this.QueueService.SetNewQueue(tracks, Artist, Artist.Name, Artist.Cover, null, shuffled == "1", false, false);
                });
            });
            PlayFeaturedInAlbumsCommand = new RelayCommand<string>(async (string shuffled) =>
            {
                await Task.Run(() =>
                {
                    List<Track> tracks = new();
                    foreach (Album album in FeaturedInAlbum)
                    {
                        tracks.AddRange(album.Tracks);
                    }

                    this.QueueService.SetNewQueue(tracks, Artist, Artist.Name, Artist.Cover, null, shuffled == "1", false, false);
                });
            });
            PlayComposedTracksCommand = new RelayCommand<string>((string shuffled) =>
            {
                Task.Run(() => QueueService.SetNewQueue(ComposedTracks.ToList(), Artist, Artist.Name, Artist.Cover, null, shuffled == "1", shuffled == "1"));
            });
            PlayPerformedTracksCommand = new RelayCommand<string>((string shuffled) =>
            {
                Task.Run(() => QueueService.SetNewQueue(PerformedInTracks.ToList(), Artist, Artist.Name, Artist.Cover, null, shuffled == "1", shuffled == "1"));
            });
            PlayLyricistOfTracksCommand = new RelayCommand<string>((string shuffled) => { Task.Run(() => QueueService.SetNewQueue(LyricistOfTracks.ToList(), Artist, Artist.Name, Artist.Cover, null, shuffled == "1", shuffled == "1"));});

            // lists visibility
            ShowHideMainAlbumsCommand = new RelayCommand(() =>
            {
                MainAlbumsVisibility = MainAlbumsVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            });
            ShowHideSinglesAndEPCommand = new RelayCommand(() =>
            {
                SinglesAndEPVisibility = SinglesAndEPVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            });
            ShowHideFeaturedAlbumsCommand = new RelayCommand(() =>
            {
                FeaturedInAlbumsVisbility = FeaturedInAlbumsVisbility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            });
            ShowHideComposedTracksCommand = new RelayCommand(() =>
            {
                ComposedTracksVisibility = ComposedTracksVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            });
            ShowHidePerformedTracksCommand = new RelayCommand(() =>
            {
                PerformedTracksVisibility = PerformedTracksVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            });
            ShowHideLyricistTracksCommand = new RelayCommand(() =>
            {
                LyricistTracksVisiblity = LyricistTracksVisiblity == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            });

            // navigation
            NavigateToAlbumCommand = _commandsManager.NavigateToAlbumCommand;
            NavigateToGenreCommand = _commandsManager.NavigateToGenreCommand;
            NavigateBackCommand = _commandsManager.NavigateBackCommand;
            NavigateToAlbumByIdCommand = _commandsManager.NavigateToAlbumByIdCommand;
            NavigateToArtistByIdCommand = _commandsManager.NavigateToArtistByIdCommand;

            // open popup
            OpenAlbumPopupCommand = _commandsManager.OpenAlbumPopupCommand;
            OpenTrackPopupCommand = _commandsManager.OpenTrackPopupCommand;
            OpenArtistPopupCommand = _commandsManager.OpenArtistPopupCommand;
        }

        private void PlayArtist(bool shuffle = false, Track startingTrack = null)
        {
            QueueService.SetNewQueue(Tracks, Artist, Artist.Name, Artist.Cover, startingTrack, shuffle);
        }

        public override void OnScrollEvent(OnScrollEvent e)
        {
            AppBar.AnimateElevation(e.VerticalOffset);
            base.OnScrollEvent(e);
        }

        public override void UpdateAppBarStyle()
        {
            AppBar.SetStyle(AppTheme.Palette.PrimaryContainer, 0, 0, false);
            AppBar.SetForeground(AppTheme.Palette.OnPrimaryContainer);
        }

        public override void Init()
        {
            base.Init();
            Update();
        }

        public override async void Update(BaseModel baseModel = null)
        {
            if(baseModel is null || baseModel is not MusicPlay.Database.Models.Artist)
            {
                Artist = (Artist)App.State.CurrentView.State.Parameter;
            }
            else
            {
                Artist = (Artist)baseModel;
            }
            AppBar.SetData(Artist.Name, "");

            Genres = [..Artist.ArtistTags.Select(at => at.Tag)];
            Biography = Artist.Biography;
            List<Track> tracks = new();

            {
                // Get all the albums the artist made
                List<Album> albums = Artist.Albums;

                List<Album> mainAlbums = new();
                List<Album> singlesAndEP = new();

                foreach (Album album in albums.OrderByDescending(a => a.ReleaseDate))
                {
                    if (album.IsMainAlbum())
                    {
                        mainAlbums.Add(album);
                    }
                    else
                    {
                        singlesAndEP.Add(album);
                    }
                }

                MainAlbums = new(mainAlbums);
                SinglesAndEP = new(singlesAndEP);
            }

            MainAlbumsHeader = $"{(MainAlbums.Count == 1 ? Resources.Album : Resources.Albums_View)}: {MainAlbums.Count}";
            SinglesAndEPHeader = $"Singles & EP: {SinglesAndEP.Count}";
            FeaturedInHeader = $"Featured in {FeaturedInAlbum.Count} {(FeaturedInAlbum.Count == 1 ? Resources.Album : Resources.Albums_View)}";

            await Task.Run(() =>
            {
                {
                    IOrderedEnumerable<Track> tempTracks = Artist.Tracks.OrderBy(t => t.AlbumId);
                    tracks.AddRange(tempTracks);
                    Tracks = tracks.DistinctBy(t => t.Id).ToList();
                    List<OrderedTrack> composedTracks = new();
                    List<OrderedTrack> performedTracks = new();
                    List<OrderedTrack> lyricistTracks = new();
                    foreach (Track track in Tracks)
                    {
                        bool isComposer = track.TrackArtistRole.Any(tar => tar.ArtistRole.Role.Name == "Composer");
                        bool isPerformer = track.TrackArtistRole.Any(tar => tar.ArtistRole.Role.Name == "Performer");
                        bool isLyricist = track.TrackArtistRole.Any(tar => tar.ArtistRole.Role.Name == "Lyricist");

                        if (isComposer)
                        {
                            composedTracks.Add(new OrderedTrack(track, composedTracks.Count + 1));
                        }
                        else if (isPerformer)
                        {
                            performedTracks.Add(new OrderedTrack(track, performedTracks.Count + 1));
                        }
                        else if (isLyricist)
                        {
                            lyricistTracks.Add(new OrderedTrack(track, lyricistTracks.Count + 1));
                        }
                    }

                    ComposedTracks = new(composedTracks);
                    PerformedInTracks = new(performedTracks);
                    LyricistOfTracks = new(lyricistTracks);
                }

                ComposerOfHeader = $"Composer of {ComposedTracks.Count} {(ComposedTracks.Count == 1 ? Resources.Track : Resources.Tracks)}";
                PerformedInHeader = $"Performed in {PerformedInTracks.Count} {(PerformedInTracks.Count == 1 ? Resources.Track : Resources.Tracks)}";
                LyricistOfHeader = $"Lyricist of {LyricistOfTracks.Count} {(LyricistOfTracks.Count == 1 ? Resources.Track : Resources.Tracks)}";
            });

            // Fetch and update if needed with data from external sources (last.fm...)
            if(Biography.IsNullOrWhiteSpace())
            {
                _ = Task.Run(async () => {
                    Root data = await Artist.GetExternalData();
                    Biography = data.Artist.Bio.Content;
                    OnPropertyChanged(nameof(Artist));
                });
            }
        }
    }
}
