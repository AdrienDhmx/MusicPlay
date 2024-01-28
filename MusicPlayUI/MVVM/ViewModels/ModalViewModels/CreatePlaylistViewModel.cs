using MessageControl;
using Microsoft.Win32;
using MusicPlay.Database.Models;
using MusicPlay.Language;
using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Factories;
using MusicPlayUI.Core.Services;
using MusicPlayUI.Core.Services.Interfaces;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MusicPlayUI.MVVM.ViewModels.ModalViewModels
{
    public class CreatePlaylistViewModel : ModalViewModel
    {
        private MusicPlay.Database.Models.Playlist PlaylistModel { get; set; }

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

        public ICommand CreatePlaylistCommand { get; }
        public ICommand SelectCoverCommand { get; }
        public CreatePlaylistViewModel(IModalService modalService) : base(modalService)
        {            
            CreatePlaylistCommand = new RelayCommand(CreatePlaylist);
            SelectCoverCommand = new RelayCommand(async () => await SelectCover());

            if(_modalService.ModalParameter is MusicPlay.Database.Models.Playlist playlist)
            {
                IsCreate = false; // updating instead
                PlaylistModel = playlist;

                PlaylistName = playlist.Name;
                PlaylistDescription = playlist.Description;
                PlaylistCover = playlist.Cover;
            }
            else
            {
                PlaylistModel = new();
            }
        }

        private async Task SelectCover()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Images (*.BMP;*.JPG;*.GIF,*.PNG,*.TIFF)|*.BMP;*.JPG;*.GIF;*.PNG;*.TIFF";
            openFileDialog.Multiselect = false;
            openFileDialog.Title = Resources.Select_an_Image;

            PlaylistCover = await CoverService.ChangeCover(PlaylistModel);
        }

        private async void CreatePlaylist()
        {
            (IsNameValid, IsCoverValid) = PlaylistsFactory.IsPlaylistDataValid(PlaylistName, PlaylistCover);

            if (IsNameValid && IsCoverValid)
            {
                if (IsCreate)
                {
                    Playlist playlist = await PlaylistsFactory.CreatePlaylist(PlaylistName, PlaylistDescription.Trim(), PlaylistCover);
                    MessageHelper.PublishMessage(PlaylistName.PlaylistCreatedWithAction((bool confirm) =>
                    {
                        if(confirm)
                        {
                            App.State.NavigateTo<PlaylistViewModel>(playlist);
                        }
                    }, "Go to playlist"));
                }
                else
                {
                    await Playlist.Update(PlaylistModel, PlaylistName, PlaylistDescription.Trim(), PlaylistCover);
                }
                CloseModal();
            }
        }
    }
}
