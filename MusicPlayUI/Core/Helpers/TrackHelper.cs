using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicPlay.Database.Models;
using MusicPlayUI.Core.Services;

namespace MusicPlayUI.Core.Helpers
{
    public static class TrackHelper
    {
        public static string GetCover<T>(this T track) where T : Track
        {
            if (ConfigurationService.AutoCover)
            {
                if (!string.IsNullOrWhiteSpace(track.Artwork))
                {
                    return track.Artwork;
                }
                else
                {
                    return track.Album.AlbumCover;
                }
            }
            else if (ConfigurationService.AlbumCoverOnly)
            {
                return track.Album.AlbumCover;
            }
            else
            {
                return track.Artwork;
            }
        }
    }
}
