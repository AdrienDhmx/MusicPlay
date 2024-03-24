using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;
using MusicPlay.Database.Models;

using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Services.Interfaces;

namespace MusicPlayUI.MVVM.ViewModels.ModalViewModels
{
    public class EditAlbumModalViewModel : ModalViewModel
    {
        private readonly ICommandsManager _commandsManager;
        private Album _album;
        public Album Album
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

        private ObservableCollection<Tag> _genres;
        public ObservableCollection<Tag> Genre
        {
            get => _genres;
            set { SetField(ref _genres, value); }
        }

        public ICommand UpdateAlbumCover { get; }
        public EditAlbumModalViewModel(IModalService modalService, ICommandsManager commandsManager) : base(modalService)
        {
            _commandsManager = commandsManager;

            UpdateAlbumCover = _commandsManager.UpdateAlbumCover;
        }

        public override void Update(BaseModel parameter = null)
        {
            if(parameter != null && parameter is Album album)
            {
                Album = album;
            }
            else
            {
                 Album = _modalService.ModalParameter as Album;
            }

            Cover = Album.AlbumCover;


        }
    }
}
