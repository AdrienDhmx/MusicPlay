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
        private readonly KeyGesture _defaultNavigateBackGesture = new(Key.B, ModifierKeys.Alt); 
        
        private readonly KeyGesture _defaultToggleQueueDrawer = new(Key.Q, ModifierKeys.Alt);

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
        public KeyBinding NavigateBackGesture { get; private set; }
              
        public KeyBinding ToggleQueueDrawer { get; private set; }

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

            PlayPauseGesture = _defaultPlayPauseGesture.CreateKeyBinding(_commandsManager.PlayPauseCommand);
            _keyBindings.Add(CommandEnums.PlayPause, PlayPauseGesture);

            NextTrackGesture = _defaultNextTrackGesture.CreateKeyBinding(_commandsManager.NextTrackCommand);
            _keyBindings.Add(CommandEnums.NexTrack, NextTrackGesture);

            PreviousTrackGesture = _defaultPreviousTrackGesture.CreateKeyBinding(_commandsManager.PreviousTrackCommand);
            _keyBindings.Add(CommandEnums.PreviousTrack, PreviousTrackGesture);

            ShuffleGesture = _defaultShuffleGesture.CreateKeyBinding(_commandsManager.ShuffleCommand);
            _keyBindings.Add(CommandEnums.Shuffle, ShuffleGesture);

            RepeatGesture = _defaultRepeatGesture.CreateKeyBinding(_commandsManager.RepeatCommand); 
            _keyBindings.Add(CommandEnums.Repeat, RepeatGesture);

            DecreaseVolumeGesture = _defaultDecreaseVolumeGesture.CreateKeyBinding(_commandsManager.DecreaseVolumeCommand);
            _keyBindings.Add(CommandEnums.DecreaseVolume, DecreaseVolumeGesture);

            IncreaseVolumeGesture = _defaultIncreaseVolumeGesture.CreateKeyBinding(_commandsManager.IncreaseVolumeCommand); 
            _keyBindings.Add(CommandEnums.IncreaseVolume, IncreaseVolumeGesture);

            MuteVolumeGesture = _defaultMuteVolumeGesture.CreateKeyBinding(_commandsManager.MuteVolumeCommand);
            _keyBindings.Add(CommandEnums.MuteVolume, MuteVolumeGesture);

            FavoriteGesture = _defaultFavoriteGesture.CreateKeyBinding(_commandsManager.FavoriteCommand);
            _keyBindings.Add(CommandEnums.ToggleFavorite, FavoriteGesture);

            Rating0Gesture = _defaultRating0Gesture.CreateKeyBinding(_commandsManager.RatingCommand, "0");
            _keyBindings.Add(CommandEnums.Rating0, Rating0Gesture);

            Rating1Gesture = _defaultRating1Gesture.CreateKeyBinding(_commandsManager.RatingCommand, "1");
            _keyBindings.Add(CommandEnums.Rating1, Rating1Gesture);

            Rating2Gesture = _defaultRating2Gesture.CreateKeyBinding(_commandsManager.RatingCommand, "2");
            _keyBindings.Add(CommandEnums.Rating2, Rating2Gesture);

            Rating3Gesture = _defaultRating3Gesture.CreateKeyBinding(_commandsManager.RatingCommand, "3");
            _keyBindings.Add(CommandEnums.Rating3, Rating3Gesture);

            Rating4Gesture = _defaultRating4Gesture.CreateKeyBinding(_commandsManager.RatingCommand, "4"); 
            _keyBindings.Add(CommandEnums.Rating4, Rating4Gesture);

            Rating5Gesture = _defaultRating5Gesture.CreateKeyBinding(_commandsManager.RatingCommand, "5");
            _keyBindings.Add(CommandEnums.Rating5, Rating5Gesture);

            NavigateHomeGesture = _defaultNavigateHomeGesture.CreateKeyBinding(_commandsManager.NavigateCommand, ViewNameEnum.Home); 
            _keyBindings.Add(CommandEnums.Home, NavigateHomeGesture);

            NavigateToAlbumsGesture = _defaultNavigateToAlbumsGesture.CreateKeyBinding(_commandsManager.NavigateCommand, ViewNameEnum.Albums);
            _keyBindings.Add(CommandEnums.Albums, NavigateToAlbumsGesture);

            NavigateToArtistsGesture = _defaultNavigateToArtistsGesture.CreateKeyBinding(_commandsManager.NavigateCommand, ViewNameEnum.Artists);
            _keyBindings.Add(CommandEnums.Artists, NavigateToArtistsGesture);

            NavigateToImportGesture = _defaultNavigateToImportGesture.CreateKeyBinding(_commandsManager.NavigateCommand, ViewNameEnum.Import);
            _keyBindings.Add(CommandEnums.Import, NavigateToImportGesture);

            NavigateToNowPlayingGesture = _defaultNavigateToNowPlayingGesture.CreateKeyBinding(_commandsManager.NavigateCommand, ViewNameEnum.NowPlaying);
            _keyBindings.Add(CommandEnums.NowPlaying, NavigateToNowPlayingGesture);

            NavigateToPlaylistsGesture = _defaultNavigateToPlaylistsGesture.CreateKeyBinding(_commandsManager.NavigateCommand, ViewNameEnum.Playlists);
            _keyBindings.Add(CommandEnums.Playlists, NavigateToPlaylistsGesture);

            NavigateToSettingsGesture = _defaultNavigateToSettingsGesture.CreateKeyBinding(_commandsManager.NavigateCommand, ViewNameEnum.Settings); 
            _keyBindings.Add(CommandEnums.Settings, NavigateToSettingsGesture);

            NavigateBackGesture = _defaultNavigateBackGesture.CreateKeyBinding(_commandsManager.NavigateBackCommand);
            _keyBindings.Add(CommandEnums.NavigateBack, NavigateBackGesture);

            EscapeFullScreenGesture = _defaultEscapeFullScreenGesture.CreateKeyBinding(_commandsManager.EscapeFullScreenCommand);
            _keyBindings.Add(CommandEnums.EscapeFullScreen, EscapeFullScreenGesture);

            ToggleFullScreenGesture = _defaultToggleFullScreenGesture.CreateKeyBinding(_commandsManager.ToggleFullScreenCommand);
            _keyBindings.Add(CommandEnums.ToggleFullScreen, ToggleFullScreenGesture);

            ToggleQueueDrawer = _defaultToggleQueueDrawer.CreateKeyBinding(_commandsManager.ToggleQueueDrawerCommand);
            _keyBindings.Add(CommandEnums.ToggleQueueDrawer, ToggleQueueDrawer);

            _window.InputBindings.Clear();
            _window.InputBindings.AddRange(_keyBindings.Values);
            AddMediaShortCuts();
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
