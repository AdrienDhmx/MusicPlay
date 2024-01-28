using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;


using MusicPlay.Database.Enums;
using MusicPlay.Database.Models;

using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Factories;
using MusicPlayUI.Core.Helpers;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.MVVM.Models;

namespace MusicPlayUI.MVVM.ViewModels.PopupViewModels
{
    public class TagPopupViewModel : PopupViewModel
    {
        private readonly IQueueService _queueService;
        private readonly IModalService _modalService;
        private readonly ICommandsManager _commandsManager;

        private UITagModel _tag;
        public UITagModel CurrentTag
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
        public TagPopupViewModel(IQueueService queueService, IModalService modalService, ICommandsManager commandsManager)
        {
            _queueService = queueService;
            _modalService = modalService;
            _commandsManager = commandsManager;

            PlayNextCommand = new RelayCommand(() => PlayNext());
            AddToQueueCommand = new RelayCommand(() => PlayNext(true));

            EditTagCommand = new RelayCommand(() =>
            {
                async void updateTag(string newName)
                {
                    await Tag.Update(CurrentTag, newName);
                }
                CreateEditNameModel model = new CreateEditNameModel(CurrentTag.Name, "Tag", true, null, updateTag);
                _modalService.OpenModal(ViewNameEnum.CreateTag, (bool canceled) => { }, model);
            });

            DeleteTagCommand = new RelayCommand(() =>
            {
                _modalService.OpenModal(ViewNameEnum.ConfirmAction, DeleteTag, ConfirmActionModelFactory.CreateConfirmDeleteModel(CurrentTag.Name, ModelTypeEnum.Tag));
            });
        }

        private async void PlayNext(bool end = false)
        {
            //List<Track> tracks = await DataAccess.Connection.GetTracksFromAlbums(CurrentTag.AlbumTags.Select(a => a.Id));
            //tracks.AddRange(await DataAccess.Connection.GetTracksFromArtists(CurrentTag.TrackArtistRole.Select(a => a.Id)));
            //tracks.AddRange(await GetPlaylistsTracks());
            //tracks.AddRange(CurrentTag.TrackTags);
            //tracks = tracks.DistinctBy(t => t.Id).ToList();
            //tracks = await tracks.GetAlbumTrackProperties();

            //_queueService.AddTracks(tracks, end, album: false, name: CurrentTag.Name);
            //_navigationService.ClosePopup();
        }

        private List<Track> GetPlaylistsTracks()
        {
            List<Track> tracks = new();

            //foreach (Playlist playlist in CurrentTag.PlaylistTags)
            //{
            //    tracks.AddRange(playlist.Tracks);
            //}
            return tracks;
        }

        private async void DeleteTag(bool canceled)
        {
            if (canceled) return;

            await Tag.Delete(CurrentTag);

            if (App.State.CurrentView.ViewModel is GenreViewModel)
            {
                App.State.NavigateBack();
            }
            else
            {
                App.State.UpdateCurrentViewIfIs([typeof(GenreLibraryViewModel)]);
            }
            ClosePopup();
        }

        public override void Init()
        {
            Update();
        }

        public override void Update(BaseModel parameter = null)
        {
            if(parameter == null)
            {
                CurrentTag = (UITagModel)App.State.CurrentPopup.State.Parameter;
            } 
            else
            {
                CurrentTag = parameter as UITagModel;
            }
        }
    }
}
