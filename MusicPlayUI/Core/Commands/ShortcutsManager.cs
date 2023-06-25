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

        public ShortcutCommand PlayPauseGesture { get; private set; }
        public ShortcutCommand NextTrackGesture { get; private set; }
        public ShortcutCommand PreviousTrackGesture { get; private set; }

        public ShortcutCommand ShuffleGesture { get; private set; }
        public ShortcutCommand RepeatGesture { get; private set; }

        public ShortcutCommand DecreaseVolumeGesture { get; private set; }
        public ShortcutCommand IncreaseVolumeGesture { get; private set; }
        public ShortcutCommand MuteVolumeGesture { get; private set; }

        public ShortcutCommand FavoriteGesture { get; private set; }

        public ShortcutCommand Rating0Gesture { get; private set; }
        public ShortcutCommand Rating1Gesture { get; private set; }
        public ShortcutCommand Rating2Gesture { get; private set; }
        public ShortcutCommand Rating3Gesture { get; private set; }
        public ShortcutCommand Rating4Gesture { get; private set; }
        public ShortcutCommand Rating5Gesture { get; private set; }

        public ShortcutCommand NavigateHomeGesture { get; private set; }
        public ShortcutCommand NavigateToAlbumsGesture { get; private set; }
        public ShortcutCommand NavigateToArtistsGesture { get; private set; }
        public ShortcutCommand NavigateToPlaylistsGesture { get; private set; }
        public ShortcutCommand NavigateToNowPlayingGesture { get; private set; }
        public ShortcutCommand NavigateToImportGesture { get; private set; }
        public ShortcutCommand NavigateToSettingsGesture { get; private set; }
        public ShortcutCommand NavigateBackGesture { get; private set; }

        public ShortcutCommand ToggleQueueDrawer { get; private set; }

        public ShortcutCommand EscapeFullScreenGesture { get; private set; }
        public ShortcutCommand ToggleFullScreenGesture { get; private set; }

        public ShortcutCommand ToggleThemeGesture { get; private set; }

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

            if (NextIsChangeOfKey)
            {
                
                ModifierKeys modifier = e.Key.ToModifier();
                Key keyDown = e.Key;
                if(e.Key == Key.System)
                {
                    modifier = e.SystemKey.ToModifier();
                    keyDown = e.SystemKey;
                }

                if (modifier != ModifierKeys.None)
                {
                    KeyDown?.Invoke(Key.None);
                } 
                else
                {
                    KeyDown?.Invoke(keyDown);
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
                    shortcut = GetDefaultShortcut((CommandEnums)(i - SettingsEnumStartCommandEnum));

                    if(shortcut != null)
                        ConfigurationService.SetPreference((SettingsEnum)i, shortcut.ShortcutToString());
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
        }

        public void ResetToDefault()
        {
            _shortcutCommands = new();

            PlayPauseGesture = GetDefaultShortcut(CommandEnums.PlayPause);
            _shortcutCommands.Add(PlayPauseGesture);

            NextTrackGesture = GetDefaultShortcut(CommandEnums.NexTrack);
            _shortcutCommands.Add(NextTrackGesture);

            PreviousTrackGesture = GetDefaultShortcut(CommandEnums.PreviousTrack);
            _shortcutCommands.Add(PreviousTrackGesture);

            ShuffleGesture = GetDefaultShortcut(CommandEnums.Shuffle);
            _shortcutCommands.Add(ShuffleGesture);

            RepeatGesture = GetDefaultShortcut(CommandEnums.Repeat); 
            _shortcutCommands.Add(RepeatGesture);

            DecreaseVolumeGesture = GetDefaultShortcut(CommandEnums.DecreaseVolume);
            _shortcutCommands.Add(DecreaseVolumeGesture);

            IncreaseVolumeGesture = GetDefaultShortcut(CommandEnums.IncreaseVolume); 
            _shortcutCommands.Add(IncreaseVolumeGesture);

            MuteVolumeGesture = GetDefaultShortcut(CommandEnums.MuteVolume);
            _shortcutCommands.Add(MuteVolumeGesture);

            NavigateHomeGesture = GetDefaultShortcut(CommandEnums.Home); 
            _shortcutCommands.Add(NavigateHomeGesture);

            NavigateToAlbumsGesture = GetDefaultShortcut(CommandEnums.Albums);
            _shortcutCommands.Add(NavigateToAlbumsGesture);

            NavigateToArtistsGesture = GetDefaultShortcut(CommandEnums.Artists);
            _shortcutCommands.Add(NavigateToArtistsGesture);

            NavigateToImportGesture = GetDefaultShortcut(CommandEnums.Import);
            _shortcutCommands.Add(NavigateToImportGesture);

            NavigateToNowPlayingGesture = GetDefaultShortcut(CommandEnums.NowPlaying);
            _shortcutCommands.Add(NavigateToNowPlayingGesture);

            NavigateToPlaylistsGesture = GetDefaultShortcut(CommandEnums.Playlists);
            _shortcutCommands.Add(NavigateToPlaylistsGesture);

            NavigateToSettingsGesture = GetDefaultShortcut(CommandEnums.Settings); 
            _shortcutCommands.Add(NavigateToSettingsGesture);

            NavigateBackGesture = GetDefaultShortcut(CommandEnums.NavigateBack);
            _shortcutCommands.Add(NavigateBackGesture);

            EscapeFullScreenGesture = GetDefaultShortcut(CommandEnums.EscapeFullScreen);
            _shortcutCommands.Add(EscapeFullScreenGesture);

            ToggleFullScreenGesture = GetDefaultShortcut(CommandEnums.ToggleFullScreen);
            _shortcutCommands.Add(ToggleFullScreenGesture);

            ToggleQueueDrawer = GetDefaultShortcut(CommandEnums.ToggleQueueDrawer);
            _shortcutCommands.Add(ToggleQueueDrawer);

            FavoriteGesture = GetDefaultShortcut(CommandEnums.ToggleFavorite);
            _shortcutCommands.Add(FavoriteGesture);

            Rating0Gesture = GetDefaultShortcut(CommandEnums.Rating0);
            _shortcutCommands.Add(Rating0Gesture);

            Rating1Gesture = GetDefaultShortcut(CommandEnums.Rating1);
            _shortcutCommands.Add(Rating1Gesture);

            Rating2Gesture = GetDefaultShortcut(CommandEnums.Rating2);
            _shortcutCommands.Add(Rating2Gesture);

            Rating3Gesture = GetDefaultShortcut(CommandEnums.Rating3);
            _shortcutCommands.Add(Rating3Gesture);

            Rating4Gesture = GetDefaultShortcut(CommandEnums.Rating4); 
            _shortcutCommands.Add(Rating4Gesture);

            Rating5Gesture = GetDefaultShortcut(CommandEnums.Rating5);
            _shortcutCommands.Add(Rating5Gesture);

            ToggleThemeGesture = GetDefaultShortcut(CommandEnums.ToggleTheme);
            _shortcutCommands.Add(ToggleThemeGesture);

            AddMediaShortCuts();
        }

        public void ResetToDefault(ShortcutCommand shortcut)
        {
            int index = _shortcutCommands.IndexOf(shortcut);
            _shortcutCommands.RemoveAt(index);
            _shortcutCommands.Insert(index, GetDefaultShortcut(shortcut.CommandEnums));
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
                CommandEnums.Rating1 => new(Key.D1, _commandsManager.RatingCommand, "0", CommandEnums.Rating1),
                CommandEnums.Rating2 => new(Key.D2, _commandsManager.RatingCommand, "0", CommandEnums.Rating2),
                CommandEnums.Rating3 => new(Key.D3, _commandsManager.RatingCommand, "0", CommandEnums.Rating3),
                CommandEnums.Rating4 => new(Key.D4, _commandsManager.RatingCommand, "0", CommandEnums.Rating4),
                CommandEnums.Rating5 => new(Key.D5, _commandsManager.RatingCommand, "0", CommandEnums.Rating5),
                CommandEnums.Home => new(Key.H, _commandsManager.NavigateCommand, ViewNameEnum.Home, CommandEnums.Home),
                CommandEnums.Albums => new(Key.D, _commandsManager.NavigateCommand, ViewNameEnum.Albums, CommandEnums.Albums),
                CommandEnums.Artists => new(Key.A, _commandsManager.NavigateCommand, ViewNameEnum.Artists, CommandEnums.Artists),
                CommandEnums.Playlists => new(Key.P, _commandsManager.NavigateCommand, ViewNameEnum.Playlists, CommandEnums.Playlists),
                CommandEnums.NowPlaying => new(Key.N, _commandsManager.NavigateCommand, ViewNameEnum.NowPlaying, CommandEnums.NowPlaying),
                CommandEnums.Import => new(Key.I, _commandsManager.NavigateCommand, ViewNameEnum.Import, CommandEnums.Import),
                CommandEnums.Settings => new(Key.S, _commandsManager.NavigateCommand, ViewNameEnum.Settings, CommandEnums.Settings),
                CommandEnums.NavigateBack => new(Key.B, _commandsManager.NavigateBackCommand, CommandEnums.NavigateBack),
                CommandEnums.EscapeFullScreen => new(Key.Escape, _commandsManager.EscapeFullScreenCommand, CommandEnums.EscapeFullScreen),
                CommandEnums.ToggleFullScreen => new(Key.F, ModifierKeys.Control, _commandsManager.ToggleFullScreenCommand, CommandEnums.ToggleFullScreen),
                CommandEnums.ToggleQueueDrawer => new(Key.Q, ModifierKeys.Control, _commandsManager.ToggleQueueDrawerCommand, CommandEnums.ToggleQueueDrawer),
                CommandEnums.ToggleTheme => new(Key.L, ModifierKeys.Alt, _commandsManager.ToggleThemeCommand, CommandEnums.ToggleTheme),
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
            if (shortcut.Key == Key.None || IsGestureAlreadyTaken(shortcut.Key, shortcut.Modifier))
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
