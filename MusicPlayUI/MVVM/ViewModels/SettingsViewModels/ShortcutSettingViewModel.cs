using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MusicPlayModels;
using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.MVVM.Models;

namespace MusicPlayUI.MVVM.ViewModels.SettingsViewModels
{
    public class ShortcutSettingViewModel : BaseSettingViewModel
    {
        private ObservableCollection<ShortcutModel> _shortcuts;
        public ObservableCollection<ShortcutModel> Shortcuts
        {
            get { return _shortcuts; }
            set
            {
                _shortcuts = value;
                OnPropertyChanged(nameof(Shortcuts));
            }
        }

        public ShortcutSettingViewModel()
        {
            Update();
        }

        public override void Update(BaseModel parameter = null)
        {
            List<ShortcutModel> shortcuts = new List< ShortcutModel >();
            foreach (KeyValuePair<CommandEnums, KeyBinding> kvp in App.ShortcutsManager.KeyBindings)
            {
                shortcuts.Add(new(kvp.Key, kvp.Value));
            }

            Shortcuts = new(shortcuts);
        }
    }
}
