using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MusicPlayUI.Core.Commands
{
    public static class ShortcutHelper
    {
        public static bool Match(this KeyBinding k1, KeyBinding k2)
        {
            return k1.Key == k2.Key && k1.Modifiers == k2.Modifiers;
        }

        public static string KeyGestureToString(this KeyBinding key)
        {
            if (key.Modifiers == ModifierKeys.None)
                return key.Key.ToString();

            return $"{key.Key} + {key.Modifiers}";
        }

        public static KeyBinding CreatekeyBinding(Key key, ModifierKeys modifier, ICommand command)
        {
            return new()
            {
                Key = key,
                Modifiers = modifier,
                Command = command,
            };
        }

        public static KeyBinding CreatekeyBinding(Key key, ModifierKeys modifier, ICommand command, object paramter)
        {
            return new()
            {
                Key = key,
                Modifiers = modifier,
                Command = command,
                CommandParameter = paramter,
            };
        }

        public static KeyGesture Parse(this string value)
        {
            ModifierKeys modifierKeys = ModifierKeys.None;
            if (value.Contains('+'))
            {
                string[] values = value.Split('+');
                if (!Enum.TryParse(values[1], out modifierKeys))
                {
                    throw new ArgumentException($"The value is not is not a valid modifier key: {values[1]}");
                }

                value = values[0];
            }

            if (!Enum.TryParse(value, out Key key))
            {
                throw new ArgumentException($"The value is not is not a valid key: {value}");
            }

            return new(key, modifierKeys);
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

        public static string KeyToString(this Key key)
        {
            return key switch
            {
                Key.None => string.Empty,
                Key.D0 => "0",
                Key.D1 => "1",
                Key.D2 => "2",
                Key.D3 => "3",
                Key.D4 => "4",
                Key.D5 => "5",
                Key.D6 => "6",
                Key.D7 => "7",
                Key.D8 => "8",
                Key.D9 => "9",
                Key.NumPad0 => "0",
                Key.NumPad1 => "1",
                Key.NumPad2 => "2",
                Key.NumPad3 => "3",
                Key.NumPad4 => "4",
                Key.NumPad5 => "5",
                Key.NumPad6 => "6",
                Key.NumPad7 => "7",
                Key.NumPad8 => "8",
                Key.NumPad9 => "9",
                Key.Multiply => "*",
                Key.Add => "+",
                Key.Separator => "|",
                Key.Subtract => "-",
                Key.Decimal => ".",
                Key.Divide => "/",
                _ => key.ToString(),
            };
        }

    }
}
