using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MessageControl;
using MusicPlayModels;
using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.MVVM.Models;

namespace MusicPlayUI.MVVM.ViewModels.ModalViewModels
{
    public class UpdateShortcutViewModel : ModalViewModel
    {
        private ShortcutCommand _shortcut;
        public ShortcutCommand Shortcut
        {
            get { return _shortcut; }
            set
            {
                SetField(ref _shortcut, value);
            }
        }

        private string _key = "Press a Key...";
        public string Key
        {
            get { return _key; }
            set
            {
                SetField(ref _key, value);
            }
        }

        private ModifierKeys _modifier;
        public ModifierKeys Modifier
        {
            get { return _modifier; }
            set
            {
                SetField(ref _modifier, value);
            }
        }

        private string _modifierString;
        public string ModifierString
        {
            get { return _modifierString; }
            set
            {
                SetField(ref _modifierString, value);
            }
        }

        private bool _isKeyValid = true;
        public bool IsKeyValid
        {
            get { return _isKeyValid; }
            set
            {
                SetField(ref _isKeyValid, value);
            }
        }

        public ICommand ChangeModifierCommand { get; }
        public ICommand ChangeShortcutCommand { get; }
        public UpdateShortcutViewModel(IModalService modalService, INavigationService navigationService) : base(modalService, navigationService)
        {
            ChangeModifierCommand = new RelayCommand<string>((string modifier) =>
            {
                ModifierString = modifier == "None" ? "" : modifier;
                Modifier = modifier.StringToModifier();
            });
            ChangeShortcutCommand = new RelayCommand(ChangeShortcut);
        }

        private void KeyDown(Key key)
        {
            if(key != System.Windows.Input.Key.None)
            {
                Key = key.KeyToString();
                Shortcut.Key = key;
                IsKeyValid = true;
            }
            else
            {
                Key = "The key must not be a modifier!";
                IsKeyValid = false;
            }
        }


        private void ChangeShortcut()
        {
            if (IsKeyValid)
            {
                Shortcut.Modifier = Modifier;
                if (App.ShortcutsManager.ChangeShortcut(Shortcut))
                {
                    App.ShortcutsManager.NextIsChangeOfKey = false;
                    CloseModal();
                }
                else
                {
                    Key = "You have not made any changes!";
                    IsKeyValid = false;
                }
            }
        }

        public override void CloseModal(bool canceled = false)
        {
            App.ShortcutsManager.NextIsChangeOfKey = false;
            App.ShortcutsManager.KeyDown -= KeyDown;
            base.CloseModal(canceled);
        }

        public override void Update(BaseModel parameter = null)
        {
            Shortcut = parameter as ShortcutCommand;

            Key = Shortcut.KeyName;
            ModifierString = Shortcut.Modifier.ModifierToString();

            Modifier = Shortcut.Modifier;

            App.ShortcutsManager.NextIsChangeOfKey = true;
            App.ShortcutsManager.KeyDown += KeyDown;
        }
    }
}
