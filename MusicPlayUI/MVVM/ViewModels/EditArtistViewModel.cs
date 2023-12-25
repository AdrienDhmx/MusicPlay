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
using MusicPlayUI.Core.Services;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.MVVM.ViewModels.WindowViewModels;

namespace MusicPlayUI.MVVM.ViewModels
{
    public class EditArtistViewModel : WindowViewModel
    {
        private readonly ICommandsManager _commandsManager;

        private ArtistModel _artist;
        public ArtistModel Artist
        {
            get => _artist;
            set { 
                SetField(ref _artist, value);
            }
        }

        private string _artistName;
        public string ArtistName
        {
            get => _artistName;
            set
            {
                SetField(ref _artistName, value);
                if (Error && !_artistName.IsNullOrWhiteSpace())
                {
                    Error = false;
                }
            }
        }

        private bool _error = false;
        public bool Error
        {
            get => _error;
            set => SetField(ref _error, value);
        }

        public ICommand NavigateToTagCommand { get; }
        public ICommand ChangeCover { get; }
        public ICommand EditCommand { get; }
        public EditArtistViewModel(IWindowService windowService, ICommandsManager commandsManager) : base(windowService, ViewNameEnum.ArtistProperties)
        {
            _commandsManager = commandsManager;

            NavigateToTagCommand = _commandsManager.NavigateToGenreCommand;
            ChangeCover = new RelayCommand(() => Artist.ChangeCover());
            EditCommand = new RelayCommand(() => Edit());
        }

        private void Edit()
        {
            Error = ArtistName.IsNullOrWhiteSpace();
            if (!Error)
            {
                Artist.Name = ArtistName;
                DataAccess.Connection.UpdateArtist(Artist);
                CloseWindow();
            }
        }

        public override async void Update(BaseModel parameter = null)
        {
            if(parameter.IsNull() || parameter is not ArtistModel)
            {
                throw new Exception("The parameter of the window view model is not of type " + typeof(ArtistModel));
            }
            Artist = (ArtistModel)parameter;
            ArtistName = Artist.Name;
            Artist.Tags = await DataAccess.Connection.GetArtistTag(Artist.Id);
        }
    }
}
