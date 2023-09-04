using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;
using MusicPlayModels;
using MusicPlayModels.MusicModels;
using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Services.Interfaces;

namespace MusicPlayUI.MVVM.ViewModels.ModalViewModels
{
    public class EditAlbumModalViewModel : ModalViewModel
    {
        private readonly ICommandsManager _commandsManager;
        private AlbumModel _album;
        public AlbumModel Album
        {
            get { return _album; }
            set
            {
                SetField(ref _album, value);
            }
        }

        private string _cover;
        public string Cover
        {
            get { return _cover; }
            set { SetField(ref _cover, value); }
        }

        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                SetField(ref _title, value);
            }
        }

        private int _year;
        public int Year
        {
            get => _year;
            set { SetField(ref _year, value);}
        }

        private ObservableCollection<TagModel> _genres;
        public ObservableCollection<TagModel> Genre
        {
            get => _genres;
            set { SetField(ref _genres, value); }
        }

        public ICommand UpdateAlbumCover { get; }
        public EditAlbumModalViewModel(INavigationService navigationService, IModalService modalService, ICommandsManager commandsManager) : base(modalService,navigationService)
        {
            _commandsManager = commandsManager;

            UpdateAlbumCover = _commandsManager.UpdateAlbumCover;
        }

        public override void Update(BaseModel parameter = null)
        {
            if(parameter != null && parameter is AlbumModel album)
            {
                Album = album;
            }
            else
            {
                 Album = _modalService.ModalParameter as AlbumModel;
            }

            Cover = Album.AlbumCover;


        }
    }
}
