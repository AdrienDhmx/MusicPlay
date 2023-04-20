using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Enums;

namespace MusicPlayUI.MVVM.Models
{
    public class ShortcutModel
    {
        public CommandEnums Command { get; private set; }

        public string CommandName => Command.CommandToString();

        public Key Key { get; set; }

        public ModifierKeys Modifier { get; set; }

        public bool HasModifier => Modifier != ModifierKeys.None;

        public string KeyName => Key.KeyToString();

        public string ModifierName => Modifier.ModifierToString();

        public ShortcutModel(CommandEnums command, Core.Commands.KeyGesture keyGesture)
        {
            Command = command;
            Key = keyGesture.Key;
            Modifier = keyGesture.Modifier;
        }

        public ShortcutModel(CommandEnums command, KeyBinding keyGesture)
        {
            Command = command;
            Key = keyGesture.Key;
            Modifier = keyGesture.Modifiers;
        }

        public ShortcutModel()
        {
            
        }
    }
}
