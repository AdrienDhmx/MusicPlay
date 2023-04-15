using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Factories;
using MusicPlayUI.Core.Services;
using MusicPlayUI.MVVM.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;
using MusicPlayUI.Core.Services.Interfaces;

namespace MusicPlayUI.MVVM.ViewModels
{
    public class MainMenuViewModel : ViewModel
    {
        private readonly INavigationService _navigationService;

        public ObservableCollection<MenuModel> Menu { get; set; } = new(MenuModelFactory.CreateMenu());

        public ICommand MenuNavigateCommand { get; }
        public ICommand NavigateBackCommand { get; }
        public MainMenuViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
            _navigationService.CurrentViewChanged += UpdateMenuUi;

            MenuNavigateCommand = new RelayCommand<MenuModel>((menu) => _navigationService.NavigateTo(menu.Enum));
            NavigateBackCommand = new RelayCommand(() => _navigationService.NavigateBack());

            _navigationService.NavigateTo((ViewNameEnum)ConfigurationService.GetPreference(SettingsEnum.MainStartingView));

            UpdateMenuUi(_navigationService.CurrentViewName);
        }

        private void UpdateMenuUi(ViewNameEnum selectedMenu)
        {
            foreach (MenuModel menu in Menu)
            {
                if (menu.IsSelected)
                {
                    menu.IsSelected = false;
                }
                if (menu.Enum == selectedMenu)
                {
                    menu.IsSelected = true;
                }
            }
        }

        public override void Dispose()
        {

        }
    }
}
