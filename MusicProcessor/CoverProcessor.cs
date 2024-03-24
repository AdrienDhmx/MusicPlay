using Microsoft.Win32;
using MusicFilesProcessor.Helpers;
using MusicPlay.Language;
using System.IO;
using MusicPlay.Database.Models;

namespace FilesProcessor
{
    public static class CoverProcessor
    {
        public static string OpenFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Images (*.BMP;*.JPG;*.GIF,*.PNG,*.TIFF)|*.BMP;*.JPG;*.GIF;*.PNG;*.TIFF";
            openFileDialog.Multiselect = false;
            openFileDialog.Title = Resources.Select_an_Image;
            bool? result = openFileDialog.ShowDialog();
            if (result.Value && result is not null)
            {
                return openFileDialog.FileName;
            }
            return "";
        }

        public static async Task<bool> ChangeCover(this PlayableModel playableModel)
        {
            string cover = OpenFileDialog();
            if (!string.IsNullOrWhiteSpace(cover))
            {
                await playableModel.UpdateCoverWithFile(cover);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Try to delete all 3 versions of the cover
        /// </summary>
        /// <param name="path"></param>
        public static void DeleteAllCoversVersion(this string path)
        {
            if (!path.ValidFilePath())
                return; 

            DeleteCover(path);
            string modifiedVersion = ImageHelper.GetModifiedCoverPath(path, true);
            DeleteCover(modifiedVersion);
            modifiedVersion = ImageHelper.GetModifiedCoverPath(path, false);
            DeleteCover(modifiedVersion);
        }

        /// <summary>
        /// Remove the cover from the database for this <paramref name="track"/>
        /// without deleting the file.
        /// </summary>
        /// <param name="track"></param>
        public static async Task RemoveCover(this Track track)
        {
            await Track.UpdateArtwork(track, "");
        }

        /// <summary>
        /// Try to delete the file by calling <see cref="File.Delete(string)"/>
        /// </summary>
        /// <param name="path"></param>
        public static bool DeleteCover(this string path)
        {
            try
            {
                File.Delete(path);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
