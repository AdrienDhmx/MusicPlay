using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using DataBaseConnection.DataAccess;
using MessageControl;
using MusicPlayModels;
using MusicPlayModels.MusicModels;
using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Factories;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.MVVM.Models;

namespace MusicPlayUI.MVVM.ViewModels.PopupViewModels
{
    public class TagTargetPopupViewModel : ViewModel
    {
        internal readonly INavigationService _navigationService;
        internal readonly IModalService _modalService;

        private ObservableCollection<TagModel> _allTags;
        public ObservableCollection<TagModel> AllTags
        {
            get => _allTags;
            set
            {
                _allTags = value;
                OnPropertyChanged(nameof(AllTags));
            }
        }

        private bool _canRemoveFromGenre = false;
        public bool CanRemoveFromGenre
        {
            get => _canRemoveFromGenre;
            set
            {
                SetField(ref _canRemoveFromGenre, value);
            }
        }

        private TagModel _currentTagView;
        public TagModel CurrentTagView
        {
            get => _currentTagView;
            set
            {
                SetField(ref _currentTagView, value);
            }
        }

        private MessageCancelClosedModel<TagModel> _genreMessageCancelClosedModel { get; set; }
        private int RemovedDataTagIndex { get; set; }

        public ICommand RemoveTagCommand { get; }
        public TagTargetPopupViewModel(INavigationService navigationService, IModalService modalService) 
        {
            _navigationService = navigationService;
            _modalService = modalService;

            RemoveTagCommand = new RelayCommand<BaseModel>((data) =>
            {
                if (_navigationService.CurrentViewModel is GenreViewModel genreViewModel)
                {
                    _navigationService.ClosePopup();
                    switch (data)
                    {
                        case AlbumModel:
                            RemovedDataTagIndex = genreViewModel.RemoveAlbum(data.Id);

                            _genreMessageCancelClosedModel = new(CurrentTagView, () => RestoreGenre(data));
                            MessageHelper.PublishMessage(MessageFactory.AlbumRemovedFromGenre((data as AlbumModel).Name, CurrentTagView.Name, _genreMessageCancelClosedModel.Cancel, () => RemoveFromTagCloseCallBack(data)));
                            break;
                        case ArtistModel:
                            RemovedDataTagIndex = genreViewModel.RemoveArtist(data.Id);

                            _genreMessageCancelClosedModel = new(CurrentTagView, () => RestoreGenre(data));
                            MessageHelper.PublishMessage(MessageFactory.ArtistRemovedFromGenre((data as ArtistModel).Name, CurrentTagView.Name, _genreMessageCancelClosedModel.Cancel, () => RemoveFromTagCloseCallBack(data)));
                            break;
                        case PlaylistModel:
                            RemovedDataTagIndex = genreViewModel.RemovePlaylist(data.Id);

                            // TODO: changing the message factory function to fit playlists
                            _genreMessageCancelClosedModel = new(CurrentTagView, () => RestoreGenre(data));
                            MessageHelper.PublishMessage(MessageFactory.AlbumRemovedFromGenre((data as PlaylistModel).Name, CurrentTagView.Name, _genreMessageCancelClosedModel.Cancel, () => RemoveFromTagCloseCallBack(data)));
                            break;
                        case TrackModel:
                            RemovedDataTagIndex = genreViewModel.RemoveTrack(data.Id);

                            // TODO: changing the message factory function to fit tracks
                            _genreMessageCancelClosedModel = new(CurrentTagView, () => RestoreGenre(data));
                            MessageHelper.PublishMessage(MessageFactory.AlbumRemovedFromGenre((data as TrackModel).Title, CurrentTagView.Name, _genreMessageCancelClosedModel.Cancel, () => RemoveFromTagCloseCallBack(data)));
                            break;
                        default:
                            break;
                    }

                }

            });


            if (_navigationService.CurrentViewModel is GenreViewModel genreViewModel)
            {
                CurrentTagView = genreViewModel.Genre;
                CanRemoveFromGenre = _navigationService.PopupViewName switch
                {
                    ViewNameEnum.AlbumPopup => genreViewModel.Albums.Any(a => a.Id == _navigationService.PopupViewParameter.Id),
                    ViewNameEnum.ArtistPopup => genreViewModel.Artists.Any(a => a.Id == _navigationService.PopupViewParameter.Id),
                    ViewNameEnum.PlaylistPopup => genreViewModel.BindedPlaylists.Any(a => a.Id == _navigationService.PopupViewParameter.Id),
                    ViewNameEnum.TrackPopup => genreViewModel.Tracks.Any(a => a.Id == _navigationService.PopupViewParameter.Id),
                    _ => false,
                };
            }
        }

        internal void AddToTag(TagModel tag, BaseModel data)
        {
            switch (data)
            {
                case AlbumModel:
                    DataAccess.Connection.InsertAlbumTag(data.Id, tag.Id);
                    MessageHelper.PublishMessage(MessageFactory.TrackAddedToPlaylist((data as AlbumModel).Name, tag.Name));
                    break;
                case ArtistModel:
                    DataAccess.Connection.InsertArtistTag(data.Id, tag.Id);
                    MessageHelper.PublishMessage(MessageFactory.TrackAddedToPlaylist((data as ArtistModel).Name, tag.Name));
                    break;
                case PlaylistModel:
                    DataAccess.Connection.InsertPlaylistTag(data.Id, tag.Id);
                    MessageHelper.PublishMessage(MessageFactory.TrackAddedToPlaylist((data as PlaylistModel).Name, tag.Name));
                    break;
                case TrackModel:
                    DataAccess.Connection.InsertTrackTag(data.Id, tag.Id);
                    MessageHelper.PublishMessage(MessageFactory.TrackAddedToPlaylist((data as TrackModel).Title, tag.Name));
                    break;
                default:
                    break;
            }
            AllTags.Remove(tag);

            if (_navigationService.CurrentViewName == ViewNameEnum.SpecificGenre || _navigationService.CurrentViewName == ViewNameEnum.Genres)
            {
                _navigationService.CurrentViewModel.Update();
            }
        }

        internal void CreateTag(BaseModel dataToAddToNewTag)
        {
            static void CreateTag(string newName)
            {
                TagModel tag = new();
                tag.Name = newName;
                DataAccess.Connection.InsertTag(tag);
            }
            CreateEditNameModel model = new CreateEditNameModel("", "Tag", false, CreateTag, null);
            _modalService.OpenModal(ViewNameEnum.CreateTag, (bool canceled) => OnCreateTagClosed(canceled, dataToAddToNewTag), model);
        }

        private async void OnCreateTagClosed(bool isCanceled, BaseModel data)
        {
            if (!_modalService.IsModalOpen && !isCanceled)
            {
                await Task.Delay(500);
                var tags = await DataAccess.Connection.GetAllTags();
                tags = tags.ToList().OrderBy(t => t.Id).ToList();
                TagModel createdTag = tags.LastOrDefault();
                AddToTag(createdTag, data);

                if (_navigationService.CurrentViewName == ViewNameEnum.Genres)
                {
                    _navigationService.CurrentViewModel.Update();
                }

                MessageHelper.PublishMessage(MessageFactory.PlaylistCreatedWithAction(createdTag.Name, (bool valid) => NavigateToTag(createdTag), $"Go to {createdTag.Name}"));
            }
        }

        internal void NavigateToTag(TagModel tag)
        {
            _navigationService.NavigateTo(ViewNameEnum.SpecificGenre, tag);
        }

        private bool RestoreGenre(BaseModel data)
        {
            if (_navigationService.CurrentViewModel is GenreViewModel genreViewModel &&
                RemovedDataTagIndex != -1)
            {
                switch (data)
                {
                    case AlbumModel:
                        genreViewModel.Albums.Insert(RemovedDataTagIndex, data as AlbumModel);
                        break;
                    case ArtistModel:
                        genreViewModel.Artists.Insert(RemovedDataTagIndex, data as ArtistModel);
                        break;
                    case PlaylistModel:
                        genreViewModel.BindedPlaylists.Insert(RemovedDataTagIndex, data as PlaylistModel);
                        break;
                    case TrackModel:
                        genreViewModel.Tracks.Insert(RemovedDataTagIndex, data as UIOrderedTrackModel);
                        break;
                    default:
                        break;
                }
            }
            return true;
        }

        private async void RemoveFromTagCloseCallBack(BaseModel data)
        {
            if (!_genreMessageCancelClosedModel.IsCanceled)
            {
                // real deletion
                switch (data)
                {
                    case AlbumModel:
                        await DataAccess.Connection.RemoveAlbumTag(data.Id, _genreMessageCancelClosedModel.Data.Id);
                        break;
                    case ArtistModel:
                        await DataAccess.Connection.RemoveArtistTag(data.Id, _genreMessageCancelClosedModel.Data.Id);
                        break;
                    case PlaylistModel:
                        await DataAccess.Connection.RemovePlaylistTag(data.Id, _genreMessageCancelClosedModel.Data.Id);
                        break;
                    case TrackModel:
                        await DataAccess.Connection.RemoveTrackTag(data.Id, _genreMessageCancelClosedModel.Data.Id);
                        break;
                    default:
                        break;
                }

                CanRemoveFromGenre = false;
                AllTags.Remove(CurrentTagView);
            }
        }

        internal async Task GetTags(IEnumerable<int> tagsIdsToRemove)
        {
            List<TagModel> tags = await DataAccess.Connection.GetAllTags();
            AllTags = new(tags.Where(t => !tagsIdsToRemove.Contains(t.Id)).OrderBy(t => t.Name));
        }
    }
}
