using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Services;
using MusicPlayUI.MVVM.Models;
using MusicPlayUI.MVVM.ViewModels;

namespace MusicPlayUI.Core.Commands
{
    public class ShortcutsManager
    {
        public static readonly int SettingsEnumStartCommandEnum = 100;
        public static readonly int ShortcutQuantity = 27;
        private readonly ICommandsManager _commandsManager; // command manager that define all the commands
        private readonly Window _window; // window to listen to the key down event from

        private Key LastKeyDown { get; set; } = Key.None;
        private ModifierKeys LastModifierDown { get; set; } = ModifierKeys.None;

        public bool NextIsChangeOfKey { get; set; } = false;
        public event Action<Key> KeyDown; 

        private List<ShortcutCommand> _shortcutCommands = new();
        public List<ShortcutCommand> ShortcutCommands => _shortcutCommands;

        public ShortcutsManager(ICommandsManager commandsManager, Window window)
        {
            _commandsManager = commandsManager;

            _window = window;
            _window.PreviewKeyDown += OnPreviewKeyDownHandler;
            _window.PreviewKeyUp += OnPreviewKeyUpHandler;
            Init();
        }

        private void OnPreviewKeyUpHandler(object sender, KeyEventArgs e)
        {
            ModifierKeys modifier = e.Key.ToModifier();
            if (e.Key == Key.System)
            {
                modifier = e.SystemKey.ToModifier();
            }

            if (modifier == ModifierKeys.None)
            {
                LastKeyDown = Key.None;
            }
            else
            {
                LastModifierDown = ModifierKeys.None;
            }
        }

        private void OnPreviewKeyDownHandler(object sender, KeyEventArgs e)
        {
            IInputElement element = Keyboard.FocusedElement;

            if (NextIsChangeOfKey) // the user is changing a shortcut keys
            {              
                ModifierKeys modifier = e.Key.ToModifier();
                Key keyDown = e.Key;
                if(e.Key == Key.System)
                {
                    modifier = e.SystemKey.ToModifier();
                    keyDown = e.SystemKey;
                }

                if (modifier != ModifierKeys.None) // modifier are not allowed (chosen with UI)
                {
                    KeyDown?.Invoke(Key.None);
                } 
                else
                {
                    KeyDown?.Invoke(keyDown); // will notify the listener (view model linked to UI to change keys of a command)
                }
                e.Handled = true;
            }
            // no text box focused and key not already handled
            else if (element != null && element as TextBox == null)
            {
                ModifierKeys modifier = e.Key.ToModifier();
                if(e.Key == Key.System)
                {
                    modifier = e.SystemKey.ToModifier();

                    if (modifier == ModifierKeys.None)
                    {
                        LastKeyDown = e.SystemKey;
                    }
                    else
                    {
                        LastModifierDown = modifier;
                    }
                }
                else
                {
                    if (modifier == ModifierKeys.None)
                    {
                        LastKeyDown = e.Key;
                    }
                    else
                    {
                        LastModifierDown = modifier;
                    }
                }

                if (HandleKeysDown())
                {
                    e.Handled = true;
                }
            }
        }

        private bool HandleKeysDown()
        {
            foreach (ShortcutCommand shortcut in _shortcutCommands)
            {
                if(LastModifierDown == shortcut.Modifier && LastKeyDown == shortcut.Key)
                {
                    if(shortcut.Command == null)
                    {
                        ChangeShortcut(GetDefaultShortcut(CommandEnums.ToggleTheme));
                    }
                    shortcut.Command.Execute(shortcut.CommandParameter);
                    return true;
                }
            }
            return false;
        }

        private void Init()
        {
            ShortcutCommand shortcut;
            for (int i = SettingsEnumStartCommandEnum; i < SettingsEnumStartCommandEnum + ShortcutQuantity; i++)
            {
                string savedShortcut = ConfigurationService.GetStringPreference((SettingsEnum)i);
                if (string.IsNullOrEmpty(savedShortcut))
                {
                    ResetToDefault();
                    break;
                }
                else
                {
                    shortcut = savedShortcut.ParseToShortcut(_commandsManager.GetCommand);
                }

                if (shortcut != null)
                {
                    _shortcutCommands.Add(shortcut);
                }
            }

            AddMediaShortCuts();
        }

        public void ResetToDefault()
        {
            _shortcutCommands = new();

            ShortcutCommand shortcut;
            for (int i = SettingsEnumStartCommandEnum; i < SettingsEnumStartCommandEnum + ShortcutQuantity; i++)
            {
                shortcut = GetDefaultShortcut((CommandEnums)(i - SettingsEnumStartCommandEnum));
                ConfigurationService.SetPreference((SettingsEnum)i, shortcut.ShortcutToString());

                _shortcutCommands.Add(shortcut);
            }

            AddMediaShortCuts();
        }

        public void ResetToDefault(ShortcutCommand shortcut)
        {
            // remove current shortcut
            int index = _shortcutCommands.IndexOf(shortcut);
            _shortcutCommands.RemoveAt(index);

            // insert default
            ShortcutCommand defaultShorcut = GetDefaultShortcut(shortcut.CommandEnums);
            _shortcutCommands.Insert(index, defaultShorcut);

            // save in settings
            ConfigurationService.SetPreference((SettingsEnum)(SettingsEnumStartCommandEnum + defaultShorcut.CommandEnums), defaultShorcut.ShortcutToString());
        }

        private ShortcutCommand GetDefaultShortcut(CommandEnums command)
        {
            return command switch
            {
                CommandEnums.PlayPause => new(Key.Space, _commandsManager.PlayPauseCommand, CommandEnums.PlayPause),
                CommandEnums.NexTrack => new(Key.Right, _commandsManager.NextTrackCommand, CommandEnums.NexTrack),
                CommandEnums.PreviousTrack => new(Key.Left, _commandsManager.PreviousTrackCommand, CommandEnums.PreviousTrack),
                CommandEnums.Shuffle => new(Key.S, _commandsManager.ShuffleCommand, CommandEnums.Shuffle),
                CommandEnums.Repeat => new(Key.R, _commandsManager.RepeatCommand, CommandEnums.Repeat),
                CommandEnums.DecreaseVolume => new(Key.Down, _commandsManager.DecreaseVolumeCommand, 500, CommandEnums.DecreaseVolume),
                CommandEnums.IncreaseVolume => new(Key.Up, _commandsManager.IncreaseVolumeCommand, 500, CommandEnums.IncreaseVolume),
                CommandEnums.MuteVolume => new(Key.M, _commandsManager.MuteVolumeCommand, CommandEnums.MuteVolume),
                CommandEnums.ToggleFavorite => new(Key.F, _commandsManager.FavoriteCommand, CommandEnums.ToggleFavorite),
                CommandEnums.Rating0 => new(Key.D0, _commandsManager.RatingCommand, "0", CommandEnums.Rating0),
                CommandEnums.Rating1 => new(Key.D1, _commandsManager.RatingCommand, "1", CommandEnums.Rating1),
                CommandEnums.Rating2 => new(Key.D2, _commandsManager.RatingCommand, "2", CommandEnums.Rating2),
                CommandEnums.Rating3 => new(Key.D3, _commandsManager.RatingCommand, "3", CommandEnums.Rating3),
                CommandEnums.Rating4 => new(Key.D4, _commandsManager.RatingCommand, "4", CommandEnums.Rating4),
                CommandEnums.Rating5 => new(Key.D5, _commandsManager.RatingCommand, "5", CommandEnums.Rating5),
                CommandEnums.Home => new(Key.H, _commandsManager.NavigateCommand, typeof(HomeViewModel), CommandEnums.Home),
                CommandEnums.Albums => new(Key.D, _commandsManager.NavigateCommand, typeof(AlbumLibraryViewModel), CommandEnums.Albums),
                CommandEnums.Artists => new(Key.A, _commandsManager.NavigateCommand, typeof(ArtistLibraryViewModel), CommandEnums.Artists),
                CommandEnums.Playlists => new(Key.P, _commandsManager.NavigateCommand, typeof(PlaylistLibraryViewModel), CommandEnums.Playlists),
                CommandEnums.NowPlaying => new(Key.N, _commandsManager.NavigateCommand, typeof(NowPlayingViewModel), CommandEnums.NowPlaying),
                CommandEnums.Settings => new(Key.O, _commandsManager.NavigateCommand, typeof(SettingsViewModel), CommandEnums.Settings),
                CommandEnums.NavigateBack => new(Key.Left, ModifierKeys.Shift, _commandsManager.NavigateBackCommand, CommandEnums.NavigateBack),
                CommandEnums.NavigateForward => new(Key.Right, ModifierKeys.Shift, _commandsManager.NavigateForwardCommand, CommandEnums.NavigateForward),
                CommandEnums.EscapeFullScreen => new(Key.Escape, _commandsManager.EscapeFullScreenCommand, CommandEnums.EscapeFullScreen),
                CommandEnums.ToggleFullScreen => new(Key.F, ModifierKeys.Control, _commandsManager.ToggleFullScreenCommand, CommandEnums.ToggleFullScreen),
                CommandEnums.ToggleQueueDrawer => new(Key.Q, ModifierKeys.Control, _commandsManager.ToggleQueueDrawerCommand, CommandEnums.ToggleQueueDrawer),
                CommandEnums.ToggleTheme => new(Key.L, ModifierKeys.Control, _commandsManager.ToggleThemeCommand, CommandEnums.ToggleTheme),
                _ => null,
            };
        }

        private void AddMediaShortCuts()
        {
            _window.InputBindings.Add(new KeyBinding()
            {
                Key = Key.MediaPlayPause,
                Command = _commandsManager.PlayPauseCommand
            });

            _window.InputBindings.Add(new KeyBinding()
            {
                Key = Key.MediaNextTrack,
                Command = _commandsManager.NextTrackCommand
            });

            _window.InputBindings.Add(new KeyBinding()
            {
                Key = Key.MediaPreviousTrack,
                Command = _commandsManager.PreviousTrackCommand
            });
        }

        public bool ChangeShortcut(ShortcutCommand shortcut)
        {
            // divide key is used to save the shortcuts in the settings as a string 
            if (shortcut.Key == Key.None || IsGestureAlreadyTaken(shortcut.Key, shortcut.Modifier) || shortcut.Key == Key.Divide)
                return false;

            ReplaceShortcut(shortcut);
            ConfigurationService.SetPreference((SettingsEnum)(shortcut.CommandEnums + SettingsEnumStartCommandEnum), shortcut.ShortcutToString());

            return true;
        }

        private void ReplaceShortcut(ShortcutCommand shortcut)
        {
            for (int i = 0; i < _shortcutCommands.Count; i++)
            {
                if(shortcut.CommandEnums == _shortcutCommands[i].CommandEnums)
                {
                    _shortcutCommands[i] = shortcut;
                    return;
                }
            }
        }

        public bool IsGestureAlreadyTaken(Key key, ModifierKeys modifier)
        {
            return _shortcutCommands.Any(k => k.Key == key && k.Modifier == modifier);
        }
    }
}
