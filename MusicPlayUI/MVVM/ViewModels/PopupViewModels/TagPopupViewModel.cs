using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DataBaseConnection.DataAccess;
using MusicPlayModels;
using MusicPlayModels.MusicModels;
using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Helpers;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.MVVM.Models;

namespace MusicPlayUI.MVVM.ViewModels.PopupViewModels
{
    public class TagPopupViewModel : ViewModel
    {
        private readonly INavigationService _navigationService;
        private readonly IQueueService _queueService;
        private readonly IModalService _modalService;
        private readonly ICommandsManager _commandsManager;

        private UITagModel _tag;
        public UITagModel Tag
        {
            get => _tag;
            set
            {
                SetField(ref _tag, value);
            }
        }

        public ICommand PlayNextCommand { get; }
        public ICommand AddToQueueCommand { get; }
        public ICommand EditTagCommand { get; }
        public ICommand DeleteTagCommand { get; }
        public TagPopupViewModel(INavigationService navigationService, IQueueService queueService, IModalService modalService, ICommandsManager commandsManager)
        {
            _navigationService = navigationService;
            _queueService = queueService;
            _modalService = modalService;
            _commandsManager = commandsManager;

            PlayNextCommand = new RelayCommand(() => PlayNext());
            AddToQueueCommand = new RelayCommand(() => PlayNext(true));

            EditTagCommand = new RelayCommand(() =>
            {
                _modalService.OpenModal(ViewNameEnum.CreateTag, (bool canceled) => { }, Tag);
            });

            DeleteTagCommand = new RelayCommand(() =>
            {
                DataAccess.Connection.DeleteTag(Tag.Id);

                if(_navigationService.CurrentViewName == ViewNameEnum.SpecificGenre)
                {
                    _navigationService.NavigateBack();
                } 
                else if(_navigationService.CurrentViewName == ViewNameEnum.Genres)
                {
                    _navigationService.CurrentViewModel.Update();
                }
                _navigationService.ClosePopup();
            });

            Update();
        }

        private async void PlayNext(bool end = false)
        {
            List<TrackModel> tracks = await DataAccess.Connection.GetTracksFromAlbums(Tag.Albums.Select(a => a.Id));
            tracks.AddRange(await DataAccess.Connection.GetTracksFromArtists(Tag.Artists.Select(a => a.Id)));
            tracks.AddRange(await GetPlaylistsTracks());
            tracks.AddRange(Tag.Tracks);
            tracks = tracks.DistinctBy(t => t.Id).ToList();
            tracks = await tracks.GetAlbumTrackProperties();

            _queueService.AddTracks(tracks, end, album: false, name: Tag.Name);
            _navigationService.ClosePopup();
        }

        public async Task<List<TrackModel>> GetPlaylistsTracks()
        {
            List<TrackModel> tracks = new();

            foreach (PlaylistModel playlist in Tag.Playlists)
            {
                tracks.AddRange(await DataAccess.Connection.GetTracksFromPlaylist(playlist.Id));
            }
            return tracks;
        }

        public override void Update(BaseModel parameter = null)
        {
            if(parameter == null)
            {
                Tag = (UITagModel)_navigationService.PopupViewParameter;
            } 
            else
            {
                Tag = parameter as UITagModel;
            }
        }
    }
}
