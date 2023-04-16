using DataBaseConnection.DataAccess;
using MusicPlayModels.MusicModels;
using MusicFilesProcessor;
using MusicFilesProcessor.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System;
using MusicPlayUI.Core.Services;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Services.Interfaces;
using System.Windows.Navigation;
using MusicPlayModels;
using System.Collections.ObjectModel;
using MusicPlayUI.Core.Helpers;
using MusicPlayUI.MVVM.Models;
using Resources = MusicPlay.Language.Resources;
using System.Windows;

namespace MusicPlayUI.MVVM.ViewModels
{
    public class ArtistViewModel : ViewModel
    {
        public IQueueService QueueService { get; }
        private readonly INavigationService _navigationService;

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

        private Visibility _composedTracksVisibility = Visibility.Collapsed;
        public Visibility ComposedTracksVisibility
        {
            get { return _composedTracksVisibility; }
            set
            {
                _composedTracksVisibility = value;
                OnPropertyChanged(nameof(ComposedTracksVisibility));
            }
        }


        private Visibility _performedTracksVisibility = Visibility.Collapsed;
        public Visibility PerformedTracksVisibility
        {
            get { return _performedTracksVisibility; }
            set
            {
                _performedTracksVisibility = value;
                OnPropertyChanged(nameof(PerformedTracksVisibility));
            }
        }

        private Visibility _lyricistTracksVisibility = Visibility.Collapsed;
        public Visibility LyricistTracksVisiblity
        {
            get { return _lyricistTracksVisibility; }
            set
            {
                _lyricistTracksVisibility = value;
                OnPropertyChanged(nameof(LyricistTracksVisiblity));
            }
        }

        private ArtistModel _artist;
        public ArtistModel Artist
        {
            get { return _artist; }
            set
            {
                _artist = value;
                OnPropertyChanged(nameof(Artist));
            }
        }

        private ObservableCollection<AlbumModel> _mainAlbums = new();
        public ObservableCollection<AlbumModel> MainAlbums
        {
            get { return _mainAlbums; }
            set
            {
                _mainAlbums = value;
                OnPropertyChanged(nameof(MainAlbums));
            }
        }

        private ObservableCollection<AlbumModel> _singlesAndEP = new();
        public ObservableCollection<AlbumModel> SinglesAndEP
        {
            get { return _singlesAndEP; }
            set
            {
                _singlesAndEP = value;
                OnPropertyChanged(nameof(SinglesAndEP));
            }
        }

        private List<UIOrderedTrackModel> _tracks;
        public List<UIOrderedTrackModel> Tracks
        {
            get { return _tracks; }
            set
            {
                _tracks = value;
                OnPropertyChanged(nameof(Tracks));
            }
        }

        private List<GenreModel> _genres;
        public List<GenreModel> Genres
        {
            get { return _genres; }
            set
            {
                _genres = value;
                OnPropertyChanged(nameof(Genres));
            }
        }

        private ObservableCollection<AlbumModel> _featuredInAlbum = new();
        public ObservableCollection<AlbumModel> FeaturedInAlbum
        {
            get { return _featuredInAlbum; }
            set
            {
                _featuredInAlbum = value;
                OnPropertyChanged(nameof(FeaturedInAlbum));
            }
        }

        private ObservableCollection<UIOrderedTrackModel> _performedInTracks = new();
        public ObservableCollection<UIOrderedTrackModel> PerformedInTracks
        {
            get { return _performedInTracks; }
            set
            {
                _performedInTracks = value;
                OnPropertyChanged(nameof(PerformedInTracks));
            }
        }

        private ObservableCollection<UIOrderedTrackModel> _composedTracks = new();
        public ObservableCollection<UIOrderedTrackModel> ComposedTracks
        {
            get { return _composedTracks; }
            set
            {
                _composedTracks = value;
                OnPropertyChanged(nameof(ComposedTracks));
            }
        }

        private ObservableCollection<UIOrderedTrackModel> _lyricistOfTracks = new();
        public ObservableCollection<UIOrderedTrackModel> LyricistOfTracks
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
        public ArtistViewModel(INavigationService navigationService, IQueueService queueService)
        {
            _navigationService = navigationService;
            this.QueueService = queueService;

            // play
            PlayArtistCommand = new RelayCommand(() => PlayArtist());
            PlayArtistShuffledCommand = new RelayCommand(() => PlayArtist(true));
            PlayTrackCommand = new RelayCommand<UIOrderedTrackModel>((track) => PlayArtist(false, track));
            PlayAlbumCommand = new RelayCommand<AlbumModel>(async (album) =>
            {
                if (album is not null)
                {
                    List<TrackModel> tracks = await DataAccess.Connection.GetTracksFromAlbum(album.Id);
                    this.QueueService.SetNewQueue(tracks, album.Name, ModelTypeEnum.Album, album.AlbumCover, null, false, false, true);
                }
            });
            PlayAlbumsOnlyCommand = new RelayCommand<string>(async (string shuffled) =>
            {
                List<TrackModel> tracks = new();
                foreach (AlbumModel album in MainAlbums)
                {
                    tracks.AddRange(await DataAccess.Connection.GetTracksFromAlbum(album.Id));
                }

                this.QueueService.SetNewQueue(tracks, Artist.Name, ModelTypeEnum.Artist, Artist.Cover, null, shuffled == "1", false, false);
            });
            PlaySinglesAndEpCommand = new RelayCommand<string>(async (string shuffled) =>
            {
                List<TrackModel> tracks = new();
                foreach (AlbumModel album in SinglesAndEP)
                {
                    tracks.AddRange(await DataAccess.Connection.GetTracksFromAlbum(album.Id));
                }

                this.QueueService.SetNewQueue(tracks, Artist.Name, ModelTypeEnum.Artist, Artist.Cover, null, shuffled == "1", false, false);
            });
            PlayFeaturedInAlbumsCommand = new RelayCommand<string>(async (string shuffled) =>
            {
                List<TrackModel> tracks = new();
                foreach (AlbumModel album in FeaturedInAlbum)
                {
                    tracks.AddRange(await DataAccess.Connection.GetTracksFromAlbum(album.Id));
                }

                this.QueueService.SetNewQueue(tracks, Artist.Name, ModelTypeEnum.Artist, Artist.Cover, null, shuffled == "1", false, false);
            });
            PlayComposedTracksCommand = new RelayCommand<string>((string shuffled) =>
            {
                this.QueueService.SetNewQueue(ComposedTracks.ToList(), Artist.Name, ModelTypeEnum.Artist, Artist.Cover, null, shuffled == "1", false, false);
            });
            PlayPerformedTracksCommand = new RelayCommand<string>((string shuffled) =>
            {
                this.QueueService.SetNewQueue(PerformedInTracks.ToList(), Artist.Name, ModelTypeEnum.Artist, Artist.Cover, null, shuffled == "1", false, false);
            });
            PlayLyricistOfTracksCommand = new RelayCommand<string>((string shuffled) => { this.QueueService.SetNewQueue(LyricistOfTracks.ToList(), Artist.Name, ModelTypeEnum.Artist, Artist.Cover, null, shuffled == "1", false, false); });

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
            NavigateToAlbumCommand = new RelayCommand<AlbumModel>((album) => _navigationService.NavigateTo(ViewNameEnum.SpecificAlbum, album));
            NavigateBackCommand = new RelayCommand(_navigationService.NavigateBack);
            NavigateToAlbumByIdCommand = new RelayCommand<int>(async (id) =>_navigationService.NavigateTo(ViewNameEnum.SpecificAlbum, await DataAccess.Connection.GetAlbum(id)));
            NavigateToArtistByIdCommand = new RelayCommand<int>(async (int id) =>_navigationService.NavigateTo(ViewNameEnum.SpecificArtist, await DataAccess.Connection.GetArtist(id)));

            // open popup
            OpenAlbumPopupCommand = new RelayCommand<AlbumModel>((album) => _navigationService.OpenPopup(ViewNameEnum.AlbumPopup, album));
            OpenTrackPopupCommand = new RelayCommand<UIOrderedTrackModel>((track) => _navigationService.OpenPopup(ViewNameEnum.TrackPopup, track));
            OpenArtistPopupCommand = new RelayCommand(() => _navigationService.OpenPopup(ViewNameEnum.ArtistPopup, Artist));

            // load
            Update();
        }

        private void PlayArtist(bool shuffle = false, TrackModel startingTrack = null)
        {
            QueueService.SetNewQueue(Tracks, Artist.Name, ModelTypeEnum.Artist, Artist.Cover, startingTrack, shuffle);
        }

        public async override void Update(BaseModel baseModel = null)
        {
            if(baseModel is null || baseModel is not ArtistModel)
            {
                Artist = (ArtistModel)_navigationService.CurrentViewParameter;
            }
            else
            {
                Artist = (ArtistModel)baseModel;
            }

            Genres = await DataAccess.Connection.GetArtistGenre(Artist.Id);
            Artist.Genres = Genres;

            Tracks = (await (await ArtistServices.GetArtistTracks(Artist.Id))
                            .GetAlbumTrackProperties())
                            .ToUIOrderedTrackModel(QueueService.AlbumCoverOnly, QueueService.AutoCover);
            Artist.Duration = Tracks.GetTotalLength(out int _);

            {
                // Get all the albums the artist made
                List<AlbumModel> albums = await DataAccess.Connection.GetAlbumsFromArtist(Artist.Id);

                List<AlbumModel> mainAlbums = new();
                List<AlbumModel> singlesAndEP = new();
                List<AlbumModel> featuredInAlbum = new();
                foreach (AlbumModel album in albums.OrderByDescending(a => a.Year))
                {
                    bool isAlbumArtist = album.IsAlbumArtist(Artist.Id);
                    bool isFeatured = album.IsFeatured(Artist.Id, true);
                    bool isAlbum = album.IsAlbum();

                    if (isAlbumArtist)
                    {
                        if (isAlbum)
                        {
                            mainAlbums.Add(album);
                        }
                        else
                        {
                            singlesAndEP.Add(album);
                        }
                    }
                    else if (isFeatured)
                    {
                        featuredInAlbum.Add(album);
                    }
                }

                MainAlbums = new(mainAlbums);
                SinglesAndEP = new(singlesAndEP);
                FeaturedInAlbum = new(featuredInAlbum);
            }

            MainAlbumsHeader = $"{(MainAlbums.Count == 1 ? Resources.Album : Resources.Albums_View)}: {MainAlbums.Count}";
            SinglesAndEPHeader = $"Singles & EP: {SinglesAndEP.Count}";
            FeaturedInHeader = $"Featured in {FeaturedInAlbum.Count} {(FeaturedInAlbum.Count == 1 ? Resources.Album : Resources.Albums_View)}";

            {                    
                List<UIOrderedTrackModel> composedTracks = new();
                List<UIOrderedTrackModel> performedTracks = new();
                List<UIOrderedTrackModel> lyricistTracks = new();
                foreach (UIOrderedTrackModel track in Tracks.OrderBy(t => t.AlbumId))
                {
                    bool isComposer = track.IsComposer(Artist.Id, true);
                    bool isPerformer = track.IsPerformer(Artist.Id, true);
                    bool isLyricist = track.IsLyricist(Artist.Id, true);

                    if (isComposer)
                    {
                        composedTracks.Add(track);
                    }
                    else if(isPerformer)
                    {
                        performedTracks.Add(track);
                    }
                    else if(isLyricist)
                    {
                        lyricistTracks.Add(track);
                    }
                }

                ComposedTracks = new(composedTracks);
                PerformedInTracks = new(performedTracks);
                LyricistOfTracks = new(lyricistTracks);
            }


            ComposerOfHeader = $"Composer of {ComposedTracks.Count} {(ComposedTracks.Count == 1 ? Resources.Track : Resources.Tracks)}";
            PerformedInHeader = $"Performed in {PerformedInTracks.Count} {(PerformedInTracks.Count == 1 ? Resources.Track : Resources.Tracks)}";
            LyricistOfHeader = $"Lyricist of {LyricistOfTracks.Count} {(LyricistOfTracks.Count == 1 ? Resources.Track : Resources.Tracks)}";
        }
    }
}
