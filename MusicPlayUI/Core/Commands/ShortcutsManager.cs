using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Services;
using SQLitePCL;
using Windows.Devices.PointOfService;

namespace MusicPlayUI.Core.Commands
{
    public class KeyGesture
    {
        public Key Key { get; private set; }
        public ModifierKeys Modifier { get; private set; }

        public KeyGesture(Key key, ModifierKeys modifier)
        {
            Key = key;
            Modifier = modifier;
        }
    }

    public class ShortcutsManager
    {
        private const int SettingsEnumStartCommandEnum = 100;
        private readonly ICommandsManager _commandsManager;
        private readonly Window _window;

        private Dictionary<CommandEnums, KeyBinding> _keyBindings = new();
        public Dictionary<CommandEnums, KeyBinding> KeyBindings => _keyBindings;

        private readonly KeyGesture _defaultPlayPauseGesture = new(Key.Space, ModifierKeys.None);
        private readonly KeyGesture _defaultNextTrackGesture = new(Key.Right, ModifierKeys.Shift);
        private readonly KeyGesture _defaultPreviousTrackGesture = new(Key.Left, ModifierKeys.Shift);

        private readonly KeyGesture _defaultShuffleGesture = new(Key.S, ModifierKeys.Shift);
        private readonly KeyGesture _defaultRepeatGesture = new(Key.R, ModifierKeys.Shift);

        private readonly KeyGesture _defaultDecreaseVolumeGesture = new(Key.Down, ModifierKeys.Shift);
        private readonly KeyGesture _defaultIncreaseVolumeGesture = new(Key.Up, ModifierKeys.Shift);
        private readonly KeyGesture _defaultMuteVolumeGesture = new(Key.M, ModifierKeys.Shift);

        private readonly KeyGesture _defaultFavoriteGesture = new(Key.F, ModifierKeys.Control);

        private readonly KeyGesture _defaultRating0Gesture = new(Key.D0, ModifierKeys.Control);
        private readonly KeyGesture _defaultRating1Gesture = new(Key.D1, ModifierKeys.Control);
        private readonly KeyGesture _defaultRating2Gesture = new(Key.D2, ModifierKeys.Control);
        private readonly KeyGesture _defaultRating3Gesture = new(Key.D3, ModifierKeys.Control);
        private readonly KeyGesture _defaultRating4Gesture = new(Key.D4, ModifierKeys.Control);
        private readonly KeyGesture _defaultRating5Gesture = new(Key.D5, ModifierKeys.Control);

        private readonly KeyGesture _defaultNavigateHomeGesture = new(Key.H, ModifierKeys.Alt);
        private readonly KeyGesture _defaultNavigateToAlbumsGesture = new(Key.D, ModifierKeys.Alt);
        private readonly KeyGesture _defaultNavigateToArtistsGesture = new(Key.A, ModifierKeys.Alt);
        private readonly KeyGesture _defaultNavigateToPlaylistsGesture = new(Key.P, ModifierKeys.Alt);
        private readonly KeyGesture _defaultNavigateToNowPlayingGesture = new(Key.N, ModifierKeys.Alt);
        private readonly KeyGesture _defaultNavigateToImportGesture = new(Key.I, ModifierKeys.Alt);
        private readonly KeyGesture _defaultNavigateToSettingsGesture = new(Key.S, ModifierKeys.Alt);

        private readonly KeyGesture _defaultEscapeFullScreenGesture = new(Key.Escape, ModifierKeys.None);
        private readonly KeyGesture _defaultToggleFullScreenGesture = new(Key.F, ModifierKeys.Alt);

        public KeyBinding PlayPauseGesture { get; private set; }
        public KeyBinding NextTrackGesture { get; private set; }
        public KeyBinding PreviousTrackGesture { get; private set; }
               
        public KeyBinding ShuffleGesture { get; private set; }
        public KeyBinding RepeatGesture { get; private set; }
              
        public KeyBinding DecreaseVolumeGesture { get; private set; }
        public KeyBinding IncreaseVolumeGesture { get; private set; }
        public KeyBinding MuteVolumeGesture { get; private set; }
               
        public KeyBinding FavoriteGesture { get; private set; }
              
        public KeyBinding Rating0Gesture { get; private set; }
        public KeyBinding Rating1Gesture { get; private set; }
        public KeyBinding Rating2Gesture { get; private set; }
        public KeyBinding Rating3Gesture { get; private set; }
        public KeyBinding Rating4Gesture { get; private set; }
        public KeyBinding Rating5Gesture { get; private set; }
              
        public KeyBinding NavigateHomeGesture { get; private set; }
        public KeyBinding NavigateToAlbumsGesture { get; private set; }
        public KeyBinding NavigateToArtistsGesture { get; private set; }
        public KeyBinding NavigateToPlaylistsGesture { get; private set; }
        public KeyBinding NavigateToNowPlayingGesture { get; private set; }
        public KeyBinding NavigateToImportGesture { get; private set; }
        public KeyBinding NavigateToSettingsGesture { get; private set; }
              
        public KeyBinding EscapeFullScreenGesture { get; private set; }
        public KeyBinding ToggleFullScreenGesture { get; private set; }

        public ShortcutsManager(ICommandsManager commandsManager, Window window)
        {
            _commandsManager = commandsManager;

            _window = window;
            ResetToDefault();
        }

        public void ResetToDefault()
        {
            _keyBindings = new();

            PlayPauseGesture = new(_commandsManager.PlayPauseCommand, _defaultPlayPauseGesture.Key, _defaultPlayPauseGesture.Modifier);
            _keyBindings.Add(CommandEnums.PlayPause, PlayPauseGesture);

            NextTrackGesture = new(_commandsManager.NextTrackCommand, _defaultNextTrackGesture.Key, _defaultNextTrackGesture.Modifier);
            _keyBindings.Add(CommandEnums.NexTrack, NextTrackGesture);

            PreviousTrackGesture =new(_commandsManager.PreviousTrackCommand, _defaultPreviousTrackGesture.Key, _defaultPreviousTrackGesture.Modifier);
            _keyBindings.Add(CommandEnums.PreviousTrack, PreviousTrackGesture);

            ShuffleGesture = new()
            {
                Key = _defaultShuffleGesture.Key,
                Modifiers = _defaultShuffleGesture.Modifier,
                Command = _commandsManager.ShuffleCommand
            };
            _keyBindings.Add(CommandEnums.Shuffle, ShuffleGesture);

            RepeatGesture = new()
            {
                Key = _defaultRepeatGesture.Key,
                Modifiers = _defaultRepeatGesture.Modifier,
                Command = _commandsManager.RepeatCommand
            }; 
            _keyBindings.Add(CommandEnums.Repeat, RepeatGesture);

            DecreaseVolumeGesture = new()
            {
                Key = _defaultDecreaseVolumeGesture.Key,
                Modifiers = _defaultDecreaseVolumeGesture.Modifier,
                Command = _commandsManager.DecreaseVolumeCommand
            };
            _keyBindings.Add(CommandEnums.DecreaseVolume, DecreaseVolumeGesture);

            IncreaseVolumeGesture = new()
            {
                Key = _defaultIncreaseVolumeGesture.Key,
                Modifiers = _defaultIncreaseVolumeGesture.Modifier,
                Command = _commandsManager.IncreaseVolumeCommand
            }; 
            _keyBindings.Add(CommandEnums.IncreaseVolume, IncreaseVolumeGesture);

            MuteVolumeGesture = new()
            {
                Key = _defaultMuteVolumeGesture.Key,
                Modifiers = _defaultMuteVolumeGesture.Modifier,
                Command = _commandsManager.MuteVolumeCommand
            }; 
            _keyBindings.Add(CommandEnums.MuteVolume, MuteVolumeGesture);

            FavoriteGesture = new()
            {
                Key = _defaultFavoriteGesture.Key,
                Modifiers = _defaultFavoriteGesture.Modifier,
                Command = _commandsManager.FavoriteCommand
            };
            _keyBindings.Add(CommandEnums.ToggleFavorite, FavoriteGesture);

            Rating0Gesture = new()
            {
                Key = _defaultRating0Gesture.Key,
                Modifiers = _defaultRating0Gesture.Modifier,
                Command = _commandsManager.RatingCommand,
                CommandParameter = "0",
            };
            _keyBindings.Add(CommandEnums.Rating0, Rating0Gesture);

            Rating1Gesture = new()
            {
                Key = _defaultRating1Gesture.Key,
                Modifiers = _defaultRating1Gesture.Modifier,
                Command = _commandsManager.RatingCommand,
                CommandParameter = "1",
            };
            _keyBindings.Add(CommandEnums.Rating1, Rating1Gesture);

            Rating2Gesture = new()
            {
                Key = _defaultRating2Gesture.Key,
                Modifiers = _defaultRating2Gesture.Modifier,
                Command = _commandsManager.RatingCommand,
                CommandParameter = "2",
            }; _keyBindings.Add(CommandEnums.Rating2, Rating2Gesture);

            Rating3Gesture = new()
            {
                Key = _defaultRating3Gesture.Key,
                Modifiers = _defaultRating3Gesture.Modifier,
                Command = _commandsManager.RatingCommand,
                CommandParameter = "3",
            }; _keyBindings.Add(CommandEnums.Rating3, Rating3Gesture);

            Rating4Gesture = new()
            {
                Key = _defaultRating4Gesture.Key,
                Modifiers = _defaultRating4Gesture.Modifier,
                Command = _commandsManager.RatingCommand,
                CommandParameter = "4",
            }; _keyBindings.Add(CommandEnums.Rating4, Rating4Gesture);

            Rating5Gesture = new()
            {
                Key = _defaultRating5Gesture.Key,
                Modifiers = _defaultRating5Gesture.Modifier,
                Command = _commandsManager.RatingCommand,
                CommandParameter = "5",
            }; _keyBindings.Add(CommandEnums.Rating5, Rating5Gesture);

            NavigateHomeGesture = new()
            {
                Key = _defaultNavigateHomeGesture.Key,
                Modifiers = _defaultNavigateHomeGesture.Modifier,
                Command = _commandsManager.NavigateCommand,
                CommandParameter = ViewNameEnum.Home,
            }; 
            _keyBindings.Add(CommandEnums.Home, NavigateHomeGesture);

            NavigateToAlbumsGesture = new()
            {
                Key = _defaultNavigateToAlbumsGesture.Key,
                Modifiers = _defaultNavigateToAlbumsGesture.Modifier,
                Command = _commandsManager.NavigateCommand,
                CommandParameter = ViewNameEnum.Albums,
            };
            _keyBindings.Add(CommandEnums.Albums, NavigateToAlbumsGesture);

            NavigateToArtistsGesture = new()
            {
                Key = _defaultNavigateToArtistsGesture.Key,
                Modifiers = _defaultNavigateToArtistsGesture.Modifier,
                Command = _commandsManager.NavigateCommand,
                CommandParameter = ViewNameEnum.Artists,
            };
            _keyBindings.Add(CommandEnums.Artists, NavigateToArtistsGesture);

            NavigateToImportGesture = new()
            {
                Key = _defaultNavigateToImportGesture.Key,
                Modifiers = _defaultNavigateToImportGesture.Modifier,
                Command = _commandsManager.NavigateCommand,
                CommandParameter = ViewNameEnum.Import,
            };
            _keyBindings.Add(CommandEnums.Import, NavigateToImportGesture);

            NavigateToNowPlayingGesture = new()
            {
                Key = _defaultNavigateToNowPlayingGesture.Key,
                Modifiers = _defaultNavigateToNowPlayingGesture.Modifier,
                Command = _commandsManager.NavigateCommand,
                CommandParameter = ViewNameEnum.NowPlaying,
            };
            _keyBindings.Add(CommandEnums.NowPlaying, NavigateToNowPlayingGesture);

            NavigateToPlaylistsGesture = new()
            {
                Key = _defaultNavigateToPlaylistsGesture.Key,
                Modifiers = _defaultNavigateToPlaylistsGesture.Modifier,
                Command = _commandsManager.NavigateCommand,
                CommandParameter = ViewNameEnum.Playlists,
            };
            _keyBindings.Add(CommandEnums.Playlists, NavigateToPlaylistsGesture);

            NavigateToSettingsGesture = new()
            {
                Key = _defaultNavigateToSettingsGesture.Key,
                Modifiers = _defaultNavigateToSettingsGesture.Modifier,
                Command = _commandsManager.NavigateCommand,
                CommandParameter = ViewNameEnum.Settings,
            }; _keyBindings.Add(CommandEnums.Settings, NavigateToSettingsGesture);

            EscapeFullScreenGesture = new()
            {
                Key = _defaultEscapeFullScreenGesture.Key,
                Modifiers = _defaultEscapeFullScreenGesture.Modifier,
                Command = _commandsManager.EscapeFullScreenCommand
            };
            _keyBindings.Add(CommandEnums.EscapeFullScreen, EscapeFullScreenGesture);

            ToggleFullScreenGesture = new()
            {
                Key = _defaultToggleFullScreenGesture.Key,
                Modifiers = _defaultToggleFullScreenGesture.Modifier,
                Command = _commandsManager.ToggleFullScreenCommand
            };
            _keyBindings.Add(CommandEnums.ToggleFullScreen, ToggleFullScreenGesture);

            _window.InputBindings.Clear();
            _window.InputBindings.AddRange(_keyBindings.Values);
            AddMediaShortCut();
        }

        private void AddMediaShortCut()
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

        public bool RegisterNewKeyBinding(KeyBinding keyGesture, CommandEnums command)
        {
            if (keyGesture == null || IsGestureAlreadyTaken(keyGesture))
                return false;

            _keyBindings.Remove(command);
            _keyBindings.Add(command, keyGesture);
            ConfigurationService.SetPreference((SettingsEnum)command + SettingsEnumStartCommandEnum, keyGesture.KeyGestureToString());

            return true;
        }

        public bool IsGestureAlreadyTaken(KeyBinding keyGesture)
        {
            return _keyBindings.Any(k => k.Value.Match(keyGesture));
        }
    }
}
