using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicPlayUI.Core.Enums;
using System.Windows.Input;
using MusicPlayUI.Core.Commands;
using MusicPlayModels;

namespace MusicPlayUI.MVVM.Models
{
    public class ShortcutCommand : BaseModel, ICloneable
    {
        public Key Key { get; set; }
        public ModifierKeys Modifier { get; set; } = ModifierKeys.None;
        public ICommand Command { get; set; }
        public object CommandParameter { get; set; }

        public CommandEnums CommandEnums { get; private set; }
        public string Name { get; private set; }

        public bool HasModifier => Modifier != ModifierKeys.None;
        public string ModifierName => Modifier.ModifierToString();
        public string KeyName => Key.KeyToString();

        public ShortcutCommand(Key keys, ModifierKeys modifierKeys, ICommand command, object commandParameter, CommandEnums commandEnums)
        {
            Key = keys;
            Modifier = modifierKeys;
            Command = command;
            CommandParameter = commandParameter;
            CommandEnums = commandEnums;
            Name = CommandEnums.CommandToString();
        }

        public ShortcutCommand(Key keys, ModifierKeys modifierKeys, ICommand command, CommandEnums commandEnums)
        {
            Key = keys;
            Modifier = modifierKeys;
            Command = command;
            CommandParameter = null;
            CommandEnums = commandEnums;
            Name = CommandEnums.CommandToString();
        }

        public ShortcutCommand(Key keys, ICommand command, object commandParameter, CommandEnums commandEnums)
        {
            Key = keys;
            Command = command;
            CommandParameter = commandParameter;
            CommandEnums = commandEnums;
            Name = CommandEnums.CommandToString();
        }

        public ShortcutCommand(Key keys, ICommand command, CommandEnums commandEnums)
        {
            Key = keys;
            Command = command;
            CommandParameter = null;
            CommandEnums = commandEnums;
            Name = CommandEnums.CommandToString();
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
