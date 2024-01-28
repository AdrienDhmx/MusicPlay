using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MessageControl;

using MusicPlay.Database.Models;
using MusicPlay.Database.Models.DataBaseModels;
using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Factories;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.MVVM.Models;

namespace MusicPlayUI.MVVM.ViewModels.PopupViewModels
{
    public class TagTargetPopupViewModel : PopupViewModel
    {
        internal readonly IModalService _modalService;

        private ObservableCollection<Tag> _allTags;
        public ObservableCollection<Tag> AllTags
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

        private Tag _currentTagView;
        public Tag CurrentTagView
        {
            get => _currentTagView;
            set
            {
                SetField(ref _currentTagView, value);
            }
        }

        private MessageCancelClosedModel<Tag> _genreMessageCancelClosedModel { get; set; }
        private int RemovedDataTagIndex { get; set; }

        public ICommand RemoveTagCommand { get; }
        public TagTargetPopupViewModel(IModalService modalService) 
        {
            _modalService = modalService;

            RemoveTagCommand = new RelayCommand<BaseModel>((data) =>
            {
                if (App.State.CurrentView.ViewModel is GenreViewModel genreViewModel)
                {
                    ClosePopup();
                    switch (data)
                    {
                        case Album:
                            RemovedDataTagIndex = genreViewModel.RemoveAlbum(data.Id);

                            _genreMessageCancelClosedModel = new(CurrentTagView, () => RestoreGenre(data));
                            MessageHelper.PublishMessage(MessageFactory.AlbumRemovedFromGenre((data as Album).Name, CurrentTagView.Name, _genreMessageCancelClosedModel.Cancel, () => RemoveFromTagCloseCallBack(data)));
                            break;
                        case Artist:
                            RemovedDataTagIndex = genreViewModel.RemoveArtist(data.Id);

                            _genreMessageCancelClosedModel = new(CurrentTagView, () => RestoreGenre(data));
                            MessageHelper.PublishMessage(MessageFactory.ArtistRemovedFromGenre((data as Artist).Name, CurrentTagView.Name, _genreMessageCancelClosedModel.Cancel, () => RemoveFromTagCloseCallBack(data)));
                            break;
                        case Playlist:
                            RemovedDataTagIndex = genreViewModel.RemovePlaylist(data.Id);

                            // TODO: changing the message factory function to fit playlists
                            _genreMessageCancelClosedModel = new(CurrentTagView, () => RestoreGenre(data));
                            MessageHelper.PublishMessage(MessageFactory.AlbumRemovedFromGenre((data as Playlist).Name, CurrentTagView.Name, _genreMessageCancelClosedModel.Cancel, () => RemoveFromTagCloseCallBack(data)));
                            break;
                        case Track:
                            RemovedDataTagIndex = genreViewModel.RemoveTrack(data.Id);

                            // TODO: changing the message factory function to fit tracks
                            _genreMessageCancelClosedModel = new(CurrentTagView, () => RestoreGenre(data));
                            MessageHelper.PublishMessage(MessageFactory.AlbumRemovedFromGenre((data as Track).Title, CurrentTagView.Name, _genreMessageCancelClosedModel.Cancel, () => RemoveFromTagCloseCallBack(data)));
                            break;
                        default:
                            break;
                    }

                }

            });


            if (App.State.CurrentView.ViewModel is GenreViewModel genreViewModel)
            {
                CurrentTagView = genreViewModel.Genre;

                int parameterId = App.State.CurrentPopup.State.Parameter.Id;

                CanRemoveFromGenre = App.State.CurrentPopup.ViewModel switch
                {
                    AlbumPopupViewModel albumPopup => genreViewModel.Albums.Any(a => a.Id == parameterId),
                    ArtistPopupViewModel artistPopup => genreViewModel.Artists.Any(a => a.Id == parameterId),
                    PlaylistPopupViewModel playlistPopup => genreViewModel.BindedPlaylists.Any(a => a.Id == parameterId),
                    TrackPopupViewModel trackPopup => genreViewModel.Tracks.Any(a => a.Track.Id == parameterId),
                    _ => false,
                };
            }
        }

        internal async Task AddToTag(Tag tag, BaseModel data)
        {
            switch (data)
            {
                case Album:
                    await Tag.Add(tag, data as Album);
                    MessageFactory.TrackAddedToPlaylist((data as Album).Name, tag.Name).Publish();
                    break;
                case Artist:
                    await Tag.Add(tag, data as Artist);
                    MessageFactory.TrackAddedToPlaylist((data as Artist).Name, tag.Name).Publish();
                    break;
                case Playlist:
                    await Tag.Add(tag, data as Playlist);
                    MessageFactory.TrackAddedToPlaylist((data as Playlist).Name, tag.Name).Publish();
                    break;
                case Track:
                    await Tag.Add(tag, data as Track);
                    MessageFactory.TrackAddedToPlaylist((data as Track).Title, tag.Name).Publish();
                    break;
                default:
                    break;
            }
            AllTags.Remove(tag);

            ViewModel currentPopupViewModel = App.State.CurrentPopup.ViewModel;
            if (currentPopupViewModel is GenreLibraryViewModel genreLibraryViewModel)
            {
                genreLibraryViewModel.Update();
            } 
            else if(currentPopupViewModel is GenreViewModel genreViewModel)
            {
                genreViewModel.Update();
            }
        }

        internal void CreateTag(BaseModel dataToAddToNewTag)
        {
            static async void CreateTag(string newName)
            {
                await Tag.Insert(new(newName));
            }
            CreateEditNameModel model = new("", "Tag", false, CreateTag, null);
            _modalService.OpenModal(ViewNameEnum.CreateTag, (bool canceled) => OnCreateTagClosed(canceled, dataToAddToNewTag), model);
        }

        private async void OnCreateTagClosed(bool isCanceled, BaseModel data)
        {
            if (!_modalService.IsModalOpen && !isCanceled)
            {
                await Task.Delay(500);
                var tags = Tag.GetAll();
                tags = [.. tags.ToList().OrderBy(t => t.Id)];
                Tag createdTag = tags.LastOrDefault();
                await AddToTag(createdTag, data);

                if (App.State.CurrentPopup.ViewModel is GenreLibraryViewModel genreLibraryViewModel)
                {
                    genreLibraryViewModel.Update();
                }

                MessageHelper.PublishMessage(MessageFactory.PlaylistCreatedWithAction(createdTag.Name, (bool valid) => NavigateToTag(createdTag), $"Go to {createdTag.Name}"));
            }
        }

        internal static void NavigateToTag(Tag tag)
        {
            App.State.NavigateTo<GenreViewModel>(tag);
        }

        private bool RestoreGenre(ObservableObject data)
        {
            if (App.State.CurrentView.ViewModel is GenreViewModel genreViewModel &&
                RemovedDataTagIndex != -1)
            {
                switch (data)
                {
                    case Album:
                        genreViewModel.Albums.Insert(RemovedDataTagIndex, data as Album);
                        break;
                    case Artist:
                        genreViewModel.Artists.Insert(RemovedDataTagIndex, data as Artist);
                        break;
                    case Playlist:
                        genreViewModel.BindedPlaylists.Insert(RemovedDataTagIndex, data as Playlist);
                        break;
                    case Track or OrderedTrack:
                        genreViewModel.Tracks.Insert(RemovedDataTagIndex, data as OrderedTrack);
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
                //switch (data)
                //{
                //    case Album:
                //        await DataAccess.Connection.RemoveAlbumTag(data.Id, _genreMessageCancelClosedModel.Data.Id);
                //        break;
                //    case Artist:
                //        await DataAccess.Connection.RemoveArtistTag(data.Id, _genreMessageCancelClosedModel.Data.Id);
                //        break;
                //    case PlaylistModel:
                //        await DataAccess.Connection.RemovePlaylistTag(data.Id, _genreMessageCancelClosedModel.Data.Id);
                //        break;
                //    case Track:
                //        await DataAccess.Connection.RemoveTrackTag(data.Id, _genreMessageCancelClosedModel.Data.Id);
                //        break;
                //    default:
                //        break;
                //}

                CanRemoveFromGenre = false;
                AllTags.Remove(CurrentTagView);
            }
        }

        internal void GetTags(IEnumerable<int> tagsIdsToRemove)
        {
            List<Tag> tags = Tag.GetAll();
            AllTags = new(tags.Where(t => !tagsIdsToRemove.Contains(t.Id)).OrderBy(t => t.Name));
        }
    }
}
