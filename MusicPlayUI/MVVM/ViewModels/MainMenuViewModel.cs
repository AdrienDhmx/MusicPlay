using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Factories;
using MusicPlayUI.Core.Services;
using MusicPlayUI.MVVM.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.MVVM.ViewModels.SettingsViewModels;

namespace MusicPlayUI.MVVM.ViewModels
{
    public class MainMenuViewModel : ViewModel
    {
        public ObservableCollection<MenuModel> Menu { get; set; } = new(MenuModelFactory.CreateMenu());

        public ICommand MenuNavigateCommand { get; }
        public ICommand NavigateBackCommand { get; }
        public MainMenuViewModel()
        {
            MenuNavigateCommand = new RelayCommand<MenuModel>((menu) => 
            {
                switch (menu.Enum)
                {
                    case ViewNameEnum.Home:
                        App.State.NavigateTo<HomeViewModel>();
                        break;
                    case ViewNameEnum.Artists:
                        App.State.NavigateTo<ArtistLibraryViewModel>();
                        break;
                    case ViewNameEnum.Albums:
                        App.State.NavigateTo<AlbumLibraryViewModel>();
                        break;
                    case ViewNameEnum.Playlists:
                        App.State.NavigateTo<PlaylistLibraryViewModel>();
                        break;
                    case ViewNameEnum.Genres:
                        App.State.NavigateTo<GenreLibraryViewModel>();
                        break;
                    case ViewNameEnum.NowPlaying:
                        App.State.NavigateTo<NowPlayingViewModel>();
                        break;
                    case ViewNameEnum.Settings:
                        App.State.NavigateTo<SettingsViewModel, GeneralSettingsViewModel>();
                        break;
                }
            });
            NavigateBackCommand = new RelayCommand(App.State.NavigateBack);

            App.State.CurrentViewChanged += UpdateMenuUi;

            App.State.NavigateTo<HomeViewModel>();

            UpdateMenuUi();
        }

        private void UpdateMenuUi()
        {
            foreach (MenuModel menu in Menu)
            {
                if (menu.IsSelected)
                {
                    menu.IsSelected = false;
                }
                if (menu.Type == App.State.CurrentView.ViewModel.GetType())
                {
                    menu.IsSelected = true;
                }
            }
        }

        public override void Dispose()
        {
            App.State.CurrentViewChanged -= UpdateMenuUi;
        }
    }
}
