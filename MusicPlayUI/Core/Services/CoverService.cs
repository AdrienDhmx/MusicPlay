using MusicPlayModels.MusicModels;
using MusicPlayUI.Core.Factories;
using MessageControl;
using FilesProcessor;
using MusicFilesProcessor.Helpers;

namespace MusicPlayUI.Core.Services
{
    public static class CoverService
    {
        public static bool ChangeCover(this TrackModel track)
        {
            string oldCover = track.Artwork;
            bool result = CoverProcessor.ChangeCover(track);
            if (result)
            {
                CoverProcessor.DeleteAllCoversVersion(oldCover);
                MessageHelper.PublishMessage(MessageFactory.CoverChangedMessage(track.Title, true));
            }
            return result;
        }

        public static bool ChangeCover(this AlbumModel album)
        {
            string oldCover = album.AlbumCover;
            bool result = CoverProcessor.ChangeCover(album);
            if (result)
            {
                CoverProcessor.DeleteAllCoversVersion(oldCover);
                MessageHelper.PublishMessage(MessageFactory.CoverChangedMessage(album.Name, false));
            }
            return result;
        }

        public static bool ChangeCover(this ArtistModel artist)
        {
            string oldCover = artist.Cover;
            bool result = CoverProcessor.ChangeCover(artist);
            if (result)
            {
                CoverProcessor.DeleteAllCoversVersion(oldCover);
                MessageHelper.PublishMessage(MessageFactory.CoverChangedMessage(artist.Name, false));
            }
            return result;
        }

        public static string ChangeCover(this PlaylistModel playlist)
        {
            string oldCover = playlist.Cover;
            bool result = CoverProcessor.ChangeCover(playlist);
            if (result)
            {
                CoverProcessor.DeleteAllCoversVersion(oldCover);
            }
            return playlist.Cover;
        }

        public static void DeleteArtwork(this TrackModel track)
        {
            track.RemoveCover();
            MessageHelper.PublishMessage(MessageFactory.CoverDeletedMessage(track.Title, true));
        }
    }
}
