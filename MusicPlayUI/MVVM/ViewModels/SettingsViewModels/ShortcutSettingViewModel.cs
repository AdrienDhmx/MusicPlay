using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using MessageControl;
using MusicPlay.Database.Models;

using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Factories;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.MVVM.Models;

namespace MusicPlayUI.MVVM.ViewModels.SettingsViewModels
{
    public class ShortcutSettingViewModel : BaseSettingViewModel
    {
        private ObservableCollection<ShortcutCommand> _shortcuts;
        private readonly IModalService _modalService;

        public ObservableCollection<ShortcutCommand> Shortcuts
        {
            get { return _shortcuts; }
            set
            {
                _shortcuts = value;
                OnPropertyChanged(nameof(Shortcuts));
            }
        }

        public ICommand ResetAllShortcutsCommand { get; }
        public ICommand ResetShortcutCommand { get; }
        public ICommand ChangeShortcutCommand { get; }
        public ShortcutSettingViewModel(IModalService modalService)
        {
            _modalService = modalService;

            ResetAllShortcutsCommand = new RelayCommand(() => 
            {
                App.ShortcutsManager.ResetToDefault();
                Update();
            });
            ResetShortcutCommand = new RelayCommand<ShortcutCommand>((ShortcutCommand shortcut) => 
            {
                App.ShortcutsManager.ResetToDefault(shortcut);
                Update();
            });
            ChangeShortcutCommand = new RelayCommand<ShortcutCommand>((ShortcutCommand shortcut) =>
            {
                _modalService.OpenModal(ViewNameEnum.UpdateShortcut, UpdateShortcutModalClosed, (ShortcutCommand)shortcut.Clone());
            });
            Update();
        }

        private void UpdateShortcutModalClosed(bool canceled)
        {
            if(!canceled)
            {
                MessageHelper.PublishMessage(DefaultMessageFactory.CreateSuccessMessage("The shortcut has been updated!"));
                Update();
            }
        }

        public override void Update(BaseModel parameter = null)
        {
            Shortcuts = new(App.ShortcutsManager.ShortcutCommands);
        }
    }
}
