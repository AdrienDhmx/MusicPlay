using MusicPlayModels.MusicModels;
using DataBaseConnection.DataAccess;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MusicFilesProcessor;
using System.Windows.Input;
using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Factories;
using MusicFilesProcessor.Helpers;
using MusicPlayUI.Core.Services;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.Core.Helpers;
using MusicPlayModels;
using System.Linq;
using System.Collections.ObjectModel;
using MusicPlayUI.MVVM.Models;

namespace MusicPlayUI.MVVM.ViewModels
{
    public class AlbumViewModel : ViewModel
    {
        private readonly INavigationService _navigationService;
        public IQueueService QueueService { get; }

        private AlbumModel _album;
        public AlbumModel Album
        {
            get => _album;
            set
            {
                _album = value;
                OnPropertyChanged(nameof(Album));
            }
        }

        private ObservableCollection<UIOrderedTrackModel> _albumTracks;
        public ObservableCollection<UIOrderedTrackModel> AlbumTracks
        {
            get { return _albumTracks; }
            set
            {
                _albumTracks = value;
                OnPropertyChanged(nameof(AlbumTracks));
            }
        }

        private List<GenreModel> _chips;
        public List<GenreModel> Chips
        {
            get => _chips;
            set
            {
                SetField(ref _chips, value);
            }
        }

        private ArtistDataRelation _mainArtist;
        public ArtistDataRelation MainArtist
        {
            get { return _mainArtist; }
            set
            {
                _mainArtist = value;
                OnPropertyChanged(nameof(MainArtist));
            }
        }

        private ObservableCollection<ArtistModel> _artists;
        public ObservableCollection<ArtistModel> Artists
        {
            get { return _artists; }
            set
            {
                _artists = value;
                OnPropertyChanged(nameof(Artists));
            }
        }

        public ICommand PlayAlbumCommand { get; }
        public ICommand PlayShuffledAlbumCommand { get; }
        public ICommand PlayTrackCommand { get; }
        public ICommand NavigateToArtistByIdCommand { get; }
        public ICommand NavigateToArtistCommand { get; }
        public ICommand NavigateBackCommand { get; }
        public ICommand OpenTrackPopupCommand { get; }
        public ICommand OpenAlbumPopupCommand { get; }
        public ICommand OpenArtistPopupCommand { get; }
        public ICommand PlayArtistCommand { get; }
        public ICommand NavigateToAlbumByIdCommand { get; }
        public AlbumViewModel(INavigationService navigationService, IQueueService queueService)
        {
            _navigationService = navigationService;
            this.QueueService = queueService;

            // play
            PlayAlbumCommand = new RelayCommand(() =>
                this.QueueService.SetNewQueue(AlbumTracks.ToList(), Album.Name, ModelTypeEnum.Album, Album.AlbumCover, AlbumTracks[0], false, false));
            PlayShuffledAlbumCommand = new RelayCommand(() =>
                this.QueueService.SetNewQueue(AlbumTracks.ToList(), Album.Name, ModelTypeEnum.Album, Album.AlbumCover, null, true, false));
            PlayTrackCommand = new RelayCommand<UIOrderedTrackModel>((track) =>
                this.QueueService.SetNewQueue(AlbumTracks.ToList(), Album.Name, ModelTypeEnum.Album, Album.AlbumCover, track, false, false));
            PlayArtistCommand = new RelayCommand<ArtistModel>(async (artist) =>
                this.QueueService.SetNewQueue(await DataAccess.Connection.GetTracksFromArtist(artist.Id), artist.Name, ModelTypeEnum.Artist, artist.Cover, null, false, false));

            // navigate
            NavigateToArtistCommand = new RelayCommand<ArtistModel>((artist) => _navigationService.NavigateTo(ViewNameEnum.SpecificArtist, artist));
            NavigateToArtistByIdCommand = new RelayCommand<int>(async (id) =>
                _navigationService.NavigateTo(ViewNameEnum.SpecificArtist, await DataAccess.Connection.GetArtist(id)));
            NavigateToAlbumByIdCommand = new RelayCommand<int>(async (int id) =>
            {
                _navigationService.NavigateTo(ViewNameEnum.SpecificAlbum, await DataAccess.Connection.GetAlbum(id));
            });
            NavigateBackCommand = new RelayCommand(_navigationService.NavigateBack);


            // open popup
            OpenTrackPopupCommand = new RelayCommand<UIOrderedTrackModel>((track) => _navigationService.OpenPopup(ViewNameEnum.TrackPopup, track));
            OpenAlbumPopupCommand = new RelayCommand(() => _navigationService.OpenPopup(ViewNameEnum.AlbumPopup, Album));
            OpenArtistPopupCommand = new RelayCommand<ArtistModel>((artist) => _navigationService.OpenPopup(ViewNameEnum.ArtistPopup, artist));

            // load
            Update();
        }

        private async void LoadArtists()
        {
            List<ArtistDataRelation> artistsRelations = Album.Artists.Order(false);
            List<ArtistModel> artistModels = new List<ArtistModel>();
            foreach (ArtistDataRelation a in artistsRelations)
            {
                artistModels.Add(await DataAccess.Connection.GetArtist(a.ArtistId));
            }

            Artists = new(artistModels);
        }

        public override async void Update(BaseModel baseModel = null)
        {
            if (baseModel is null || baseModel is not AlbumModel)
            {
                Album = (AlbumModel)_navigationService.CurrentViewParameter;
            }
            else
            {
                Album = (AlbumModel)baseModel;
            }

            if(Album.Artists is null)
            {
                Album = await DataAccess.Connection.GetAlbum(Album.Id);
            }
            MainArtist = Album.GetAlbumArtist();

            Chips = await DataAccess.Connection.GetAlbumGenre(Album.Id);

            List<TrackModel> tracks = new(await DataAccess.Connection.GetTracksFromAlbum(Album.Id));
            tracks = tracks.OrderTracks();
            await tracks.GetAlbumTrackProperties();
            AlbumTracks = new(tracks.ToUIOrderedTrackModel(QueueService.AlbumCoverOnly, QueueService.AutoCover));

            LoadArtists();
            Album.Duration = AlbumTracks.GetTotalLength(out int _);
            OnPropertyChanged(nameof(Album));
        }
    }
}
