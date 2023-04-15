using DataBaseConnection.DataAccess;
using MusicPlayModels.MusicModels;
using Microsoft.Win32;
using MusicFilesProcessor.Helpers;
using MusicPlay.Language;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilesProcessor
{
    public static class CoverProcessor
    {
        private static string OpenFileDialog()
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

        public static bool ChangeCover(this TrackModel track)
        {
            string cover = OpenFileDialog();
            if (!string.IsNullOrWhiteSpace(cover))
            {
                string filemane = ImageHelper.CreateCoverFilename();
                string newPath = ImageHelper.CreateCoverPath(filemane);
                cover.SaveFileToNewPath(newPath);

                track.Artwork = newPath;
                DataAccess.Connection.UpdateTrackArtwork(track);
                return true;
            }
            return false;
        }

        public static bool ChangeCover(this PlaylistModel playlist)
        {
            string cover = OpenFileDialog();
            if (!string.IsNullOrWhiteSpace(cover))
            {
                string filemane = ImageHelper.CreateCoverFilename();
                string newPath = ImageHelper.CreateCoverPath(filemane);
                cover.SaveFileToNewPath(newPath);

                playlist.Cover = newPath;
                return true;
            }
            return false;
        }

        public static bool ChangeCover(this AlbumModel album)
        {
            string cover = OpenFileDialog();
            if (!string.IsNullOrWhiteSpace(cover))
            {
                string filemane = ImageHelper.CreateCoverFilename();
                string newpath = ImageHelper.CreateCoverPath(filemane);

                cover.SaveFileToNewPath(newpath);

                album.AlbumCover = newpath;
                DataAccess.Connection.UpdateAlbumCover(album);
                return true;
            }
            return false;
        }

        public static bool ChangeCover(this ArtistModel artist)
        {
            string cover = OpenFileDialog();
            if (!string.IsNullOrWhiteSpace(cover))
            {
                string filemane = ImageHelper.CreateCoverFilename();
                string newpath = ImageHelper.CreateCoverPath(filemane);

                cover.SaveFileToNewPath(newpath);

                artist.Cover = newpath;
                DataAccess.Connection.UpdateArtistCover(artist);
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
            if (!path.ValidPath())
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
        public static void RemoveCover(this TrackModel track)
        {
            track.Artwork = "";
            DataAccess.Connection.UpdateTrackArtwork(track);
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
