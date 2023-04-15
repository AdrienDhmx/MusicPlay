using DataBaseConnection.DataAccess;
using MessageControl;
using Microsoft.Win32;
using MusicPlay.Language;
using MusicPlayModels.MusicModels;
using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Factories;
using MusicPlayUI.Core.Services;
using MusicPlayUI.Core.Services.Interfaces;
using System.IO;
using System.Windows.Input;
using Windows.Media.Playlists;

namespace MusicPlayUI.MVVM.ViewModels.ModalViewModels
{
    public class CreatePlaylistViewModel : ViewModel
    {
        private readonly IModalService _modalService;
        private readonly INavigationService _navigationService;

        private PlaylistModel Playlist { get; set; }

        private string _playlistName = "";
        public string PlaylistName
        {
            get { return _playlistName; }
            set
            {
                _playlistName = value;
                if (!IsNameValid && !string.IsNullOrWhiteSpace(value))
                    IsNameValid = true;
                OnPropertyChanged(nameof(PlaylistName));
            }
        }

        private string _playlistDescription = "";
        public string PlaylistDescription
        {
            get { return _playlistDescription; }
            set
            {
                SetField(ref _playlistDescription, value);
            }
        }

        private string _playlistCover = "";
        public string PlaylistCover
        {
            get { return _playlistCover; }
            set
            {
                _playlistCover = value;
                if (!IsCoverValid && File.Exists(_playlistCover))
                    IsCoverValid = true;
                OnPropertyChanged(nameof(PlaylistCover));
            }
        }

        private bool _isNameValid = true;
        public bool IsNameValid
        {
            get { return _isNameValid; }
            set
            {
                _isNameValid = value;
                OnPropertyChanged(nameof(IsNameValid));
            }
        }

        private bool _iCoverValid = true;
        public bool IsCoverValid
        {
            get { return _iCoverValid; }
            set
            {
                _iCoverValid = value;
                OnPropertyChanged(nameof(IsCoverValid));
            }
        }


        private bool _isCreate = true;
        public bool IsCreate
        {
            get { return _isCreate; }
            set
            {
                _isCreate = value;
                OnPropertyChanged(nameof(IsCreate));
            }
        }

        public ICommand CloseModalCommand { get; }
        public ICommand CreatePlaylistCommand { get; }
        public ICommand SelectCoverCommand { get; }
        public CreatePlaylistViewModel(IModalService modalService, INavigationService navigationService)
        {
            _modalService = modalService;
            
            _navigationService = navigationService;

            CloseModalCommand = new RelayCommand(() => CloseModal(true));
            CreatePlaylistCommand = new RelayCommand(CreatePlaylist);
            SelectCoverCommand = new RelayCommand(SelectCover);

            if (_navigationService.IsPopupOpen)
            {
                _navigationService.ClosePopup();
            }

            if(_modalService.ModalParameter is PlaylistModel playlist)
            {
                IsCreate = false; // updating instead
                Playlist = playlist;

                PlaylistName = playlist.Name;
                PlaylistDescription = playlist.Description;
                PlaylistCover = playlist.Cover;
            }
            else
            {
                Playlist = new();
            }
        }

        private void CloseModal(bool canceled = false)
        {   
            _modalService.CloseModal(canceled);
        }

        private void SelectCover()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Images (*.BMP;*.JPG;*.GIF,*.PNG,*.TIFF)|*.BMP;*.JPG;*.GIF;*.PNG;*.TIFF";
            openFileDialog.Multiselect = false;
            openFileDialog.Title = Resources.Select_an_Image;

            PlaylistCover =  CoverService.ChangeCover(Playlist);
        }

        private async void CreatePlaylist()
        {
            (IsNameValid, IsCoverValid) = PlaylistsFactory.IsPlaylistDataValid(PlaylistName, PlaylistCover);

            if (IsNameValid && IsCoverValid)
            {
                if (IsCreate)
                {
                    int id = await PlaylistsFactory.CreatePlaylist(PlaylistName, PlaylistDescription.Trim(), PlaylistCover);
                    MessageHelper.PublishMessage(PlaylistName.PlaylistCreatedWithAction(async (bool confirm) =>
                    {
                        if(confirm)
                        {
                            _navigationService.NavigateTo(Core.Enums.ViewNameEnum.SpecificPlaylist, await DataAccess.Connection.GetPlaylist(id));
                        }
                    }, "Go to playlist"));
                }
                else
                {
                    Playlist.Name = PlaylistName;
                    Playlist.Description = PlaylistDescription.Trim();
                    Playlist.Cover = PlaylistCover;

                    await DataAccess.Connection.UpdatePlaylist(Playlist);
                }
                CloseModal();
            }
        }
    }
}
