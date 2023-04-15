using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayUI.Core.Enums
{
    public enum ErrorEnum
    {
        NoConnection = 0,
        CorruptedFile = 1,
        NotEnoughDataForRadio = 2,
        RemoveTrackFromQueueError = 3,
        PlayingFromNotFound = 4,
        TrackAlreadyInPlaylist = 5,
    }
}
