using System;
using System.Threading.Tasks;
using System.Windows.Input;

using MusicPlay.Database.Helpers;
using MusicPlay.Database.Models;
using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Services;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.MVVM.ViewModels.WindowViewModels;

namespace MusicPlayUI.MVVM.ViewModels
{
    public class EditArtistViewModel : WindowViewModel
    {
        private readonly ICommandsManager _commandsManager;

        private Artist _artist;
        public Artist Artist
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
            EditCommand = new RelayCommand(async () => await Edit());
        }

        private async Task Edit()
        {
            Error = ArtistName.IsNullOrWhiteSpace();
            if (!Error)
            {
                await Artist.Update(a => a.Name = ArtistName, Artist);
                CloseWindow();
            }
        }

        public override void Update(BaseModel parameter = null)
        {
            if(parameter.IsNull() || parameter is not MusicPlay.Database.Models.Artist)
            {
                throw new Exception("The parameter of the window view model is not of type " + typeof(Artist));
            }
            Artist = (Artist)parameter;
            ArtistName = Artist.Name;
            //Artist.TrackTag = await DataAccess.Connection.GetArtistTag(Artist.Id);
        }
    }
}
