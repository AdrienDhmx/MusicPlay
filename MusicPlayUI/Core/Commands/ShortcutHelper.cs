using System;
using System.Windows.Input;
using Humanizer;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.MVVM.Models;

namespace MusicPlayUI.Core.Commands
{
    public static class ShortcutHelper
    {
        public static Key ParseToKey(this string value)
        {
            if (!Enum.TryParse(value, out Key key))
            {
                throw new ArgumentException($"The value is not is not a valid key: {value}");
            }

            return key;
        }

        public static ModifierKeys ToModifier(this Key key)
        {
            return key switch
            {
                Key.LeftCtrl => ModifierKeys.Control,
                Key.RightCtrl => ModifierKeys.Control,
                Key.LeftAlt => ModifierKeys.Alt,
                Key.RightAlt => ModifierKeys.Alt,
                Key.LWin => ModifierKeys.Windows,
                Key.RWin => ModifierKeys.Windows,
                Key.LeftShift => ModifierKeys.Shift,
                Key.RightShift => ModifierKeys.Shift,
                _ => ModifierKeys.None,
            };
        }

        public static string ShortcutToString(this ShortcutCommand shortcut)
        {
            return shortcut.ModifierName + "||" + shortcut.KeyName + "||" + (shortcut.CommandEnums + ShortcutsManager.SettingsEnumStartCommandEnum).ToString() + "||" + shortcut.CommandParameter;
        }

        public static ShortcutCommand ParseToShortcut(this string value, Func<CommandEnums,ICommand> parseCommandEnumsFunc)
        {
            string[] values = value.Split("||");

            CommandEnums commandEnums = (CommandEnums)(int.Parse(values[2]) - ShortcutsManager.SettingsEnumStartCommandEnum);

            return new(values[1].ParseToKey(), values[0].StringToModifier(), parseCommandEnumsFunc(commandEnums), values[3], commandEnums);
        }

        public static string ModifierToString(this ModifierKeys modifierKeys)
        {
            return modifierKeys switch
            {
                ModifierKeys.None => string.Empty,
                ModifierKeys.Alt => "ALT",
                ModifierKeys.Control => "CTRL",
                ModifierKeys.Shift => "SHIFT",
                ModifierKeys.Windows => "WINDOWS",
                _ => string.Empty,
            };
        }

        public static ModifierKeys StringToModifier(this string modifierKeys)
        {
            return modifierKeys switch
            {
                "" => ModifierKeys.None,
                "ALT" => ModifierKeys.Alt,
                "CTRL" => ModifierKeys.Control,
                "SHIFT" => ModifierKeys.Shift,
                "WINDOWS" => ModifierKeys.Windows,
                _ => ModifierKeys.None,
            };
        }

        public static string KeyToString(this Key key)
        {
            return key switch
            {
                Key.None => string.Empty,
                //Key.D0 => "0",
                //Key.D1 => "1",
                //Key.D2 => "2",
                //Key.D3 => "3",
                //Key.D4 => "4",
                //Key.D5 => "5",
                //Key.D6 => "6",
                //Key.D7 => "7",
                //Key.D8 => "8",
                //Key.D9 => "9",
                //Key.NumPad0 => "0",
                //Key.NumPad1 => "1",
                //Key.NumPad2 => "2",
                //Key.NumPad3 => "3",
                //Key.NumPad4 => "4",
                //Key.NumPad5 => "5",
                //Key.NumPad6 => "6",
                //Key.NumPad7 => "7",
                //Key.NumPad8 => "8",
                //Key.NumPad9 => "9",
                //Key.Multiply => "*",
                //Key.Add => "+",
                //Key.Separator => "|",
                //Key.Subtract => "-",
                //Key.Decimal => ".",
                //Key.Divide => "/",
                _ => key.ToString(),
            };
        }

    }
}
