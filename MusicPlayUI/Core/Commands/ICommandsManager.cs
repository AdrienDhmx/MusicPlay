﻿using System.Windows.Input;
using MusicPlayUI.Core.Enums;

namespace MusicPlayUI.Core.Commands
{
    public interface ICommandsManager
    {
        ICommand ClosePopupCommand { get; }
        ICommand DecreaseVolumeCommand { get; }
        ICommand EscapeFullScreenCommand { get; }
        ICommand FavoriteCommand { get; }
        ICommand IncreaseVolumeCommand { get; }
        ICommand LeaveCommand { get; }
        ICommand MaximizeCommand { get; }
        ICommand MinimizeCommand { get; }
        ICommand MuteVolumeCommand { get; }

        ICommand PlayNewQueueCommand { get; }
        ICommand PlayNewQueueShuffledCommand { get; }

        ICommand NavigateCommand { get; }
        ICommand NavigateBackCommand { get; }
        ICommand NavigateForwardCommand { get; }

        ICommand NavigateToAlbumByIdCommand { get; }
        ICommand NavigateToArtistByIdCommand { get; }
        ICommand NavigateToGenreCommand { get; }
        ICommand NavigateToAlbumCommand { get; }
        ICommand NavigateToArtistCommand { get; }
        ICommand NavigateToPlaylistCommand { get; }
        ICommand OpenAlbumPopupCommand { get; }
        ICommand OpenArtistPopupCommand { get; }
        ICommand OpenTrackPopupCommand { get; }
        ICommand OpenTagPopupCommand { get; }
        ICommand OpenPlaylistPopupCommand { get; }
        ICommand NextTrackCommand { get; }
        ICommand ToggleMenuDrawerCommand { get; }
        ICommand ToggleQueueDrawerCommand { get; }

        ICommand UpdateAlbumCover { get; }

        ICommand PlayPauseCommand { get; }
        ICommand PreviousTrackCommand { get; }
        ICommand RatingCommand { get; }
        ICommand RepeatCommand { get; }
        ICommand ShuffleCommand { get; }
        ICommand ToggleFullScreenCommand { get; }

        ICommand ToggleThemeCommand { get; }

        ICommand GetCommand(CommandEnums commandEnums);
    }
}