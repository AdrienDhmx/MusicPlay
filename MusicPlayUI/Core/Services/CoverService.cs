using MusicPlayUI.Core.Factories;
using MessageControl;
using FilesProcessor;
using MusicFilesProcessor.Helpers;
using MusicPlay.Database.Models;
using System.Threading.Tasks;

namespace MusicPlayUI.Core.Services
{
    public static class CoverService
    {
        public static async Task<bool> ChangeCover(this Track track)
        {
            string oldCover = track.Artwork;
            bool result = await CoverProcessor.ChangeCover(track);
            if (result)
            {
                CoverProcessor.DeleteAllCoversVersion(oldCover);
                MessageHelper.PublishMessage(MessageFactory.CoverChangedMessage(track.Title, true));
            }
            return result;
        }

        public static async Task<bool> ChangeCover(this Album album)
        {
            string oldCover = album.AlbumCover;
            bool result = await CoverProcessor.ChangeCover(album);
            if (result)
            {
                CoverProcessor.DeleteAllCoversVersion(oldCover);
                MessageHelper.PublishMessage(MessageFactory.CoverChangedMessage(album.Name, false));
            }
            return result;
        }

        public static async Task<bool> ChangeCover(this Artist artist)
        {
            string oldCover = artist.Cover;
            bool result = await CoverProcessor.ChangeCover(artist);
            if (result)
            {
                CoverProcessor.DeleteAllCoversVersion(oldCover);
                MessageHelper.PublishMessage(MessageFactory.CoverChangedMessage(artist.Name, false));
            }
            return result;
        }

        public static async Task<string> ChangeCover(this Playlist playlist, bool update = true)
        {
            string oldCover = playlist.Cover;
            if(update)
            {
                bool result = await CoverProcessor.ChangeCover(playlist);
                if (result)
                {
                    CoverProcessor.DeleteAllCoversVersion(oldCover);
                }
            } 
            else
            {
                string cover = CoverProcessor.OpenFileDialog();
                if (!string.IsNullOrWhiteSpace(cover))
                {
                    playlist.Cover = cover;
                }
            }
            return playlist.Cover;
        }

        public static async Task DeleteArtwork(this Track track)
        {
            await track.RemoveCover();
            MessageHelper.PublishMessage(MessageFactory.CoverDeletedMessage(track.Title, true));
        }
    }
}
