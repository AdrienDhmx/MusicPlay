using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using MusicPlay.Database.Helpers;
using MusicPlay.Database.Models;

namespace MusicFilesProcessor.Helpers
{
    public static partial class ImageHelper
    {
        public static readonly int threshold = 200000;

        private static string CreateValidPath(this string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    // try to open the file
                    FileStream stream = File.Open(path, FileMode.Open);
                    stream.Close();
                    stream.Dispose();
                }
                catch (Exception)
                {
                    string ext = Path.GetExtension(path);
                    path = path.Replace(ext, string.Empty);

                    // add a guid to make the filename unique, the file already existing will not be deleted now
                    path += Guid.NewGuid().ToString();
                    path += ext;
                }
                
            }
            return path;
        }

        public static string GetAlbumCover(string album, string filePath)
        {
            if (Path.HasExtension(filePath))
                filePath = DirectoryHelper.GetDirectory(filePath);

            string coverPath = GetAlbumCoverFromDirectory(filePath, album);
            if (!string.IsNullOrWhiteSpace(coverPath))
            {
                return coverPath;
            }
            return string.Empty;
        }

        /// <summary>
        /// Save a file to the designated path by copying it and creating 2 smaller versions of it
        /// </summary>
        /// <param name="file"></param>
        /// <param name="newPath"></param>
        public static void SaveFileToNewPath(this string file, string newPath)
        {
            if(file.ValidFilePath() && !string.IsNullOrWhiteSpace(newPath))
            {
                if(file != newPath)
                    File.Copy(file, newPath, true); // copy original version

                string mediumFileNamePath = GetModifiedCoverPath(newPath, true);
                string thumbnailFileNamePath = GetModifiedCoverPath(newPath, false);
                ImageProcessor.FormatImage(file, mediumFileNamePath, thumbnailFileNamePath, threshold);
            }
        }

        public static void SaveSmallerImageVersions(this string file)
        {
            if (file.ValidFilePath())
            {
                string mediumFileNamePath = GetModifiedCoverPath(file, true);
                string thumbnailFileNamePath = GetModifiedCoverPath(file, false);
                ImageProcessor.FormatImage(file, mediumFileNamePath, thumbnailFileNamePath, threshold);
            }
        }

        public static async Task SaveAllCovers<T>(List<string> covers, T playableModel, Action<string> saveFirstCallback) where T : PlayableModel
        {
            bool saved = false;
            for (int i = 0; i < 10 && i < covers.Count; i++)
            {
                string path = playableModel.GetNewCoverPath();
                string image = covers[i];
                if (await ConnectivityHelper.Instance.DownloadImage(image, path))
                {
                    SaveSmallerImageVersions(path);
                    if (!saved)
                    {
                        saveFirstCallback(path);
                        saved = true;
                    }
                }
            }
        }

        /// <summary>
        /// Update the entity Cover/Artwork with a copy of the file parameter in its cover folder
        /// </summary>
        /// <param name="playableModel"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task UpdateCoverWithFile(this PlayableModel playableModel, string file)
        {
            if (!file.ValidFilePath())
                return;

            string path = playableModel.GetNewCoverPath();
            SaveFileToNewPath(file, path);

            if (playableModel is Album album)
                await Album.UpdateCover(album, path);
            else if (playableModel is Artist artist)
                await Artist.UpdateCover(artist, path);
            else if (playableModel is Track track)
                await Track.UpdateArtwork(track, path);
            else
                throw new Exception("This type doesn't support the UpdateCoverWithFile method yet!");
        }

        public static string GetModifiedCoverPath(this string path, bool medium)
        {
            if (!ValidFilePath(path))
                return string.Empty;

            string fileName = Path.GetFileNameWithoutExtension(path);
            string ext = Path.GetExtension(path);
            string directory = path.Replace(fileName + ext, string.Empty);

            if (medium)
            {
                fileName += 'M';
            }
            else
            {
                fileName += 'T';
            }

            return directory + fileName + ext;
        }

        private static string FormatSyntax(this string text)
        {
            text = text.Replace("|", "-");
            text = text.Replace("/", ",");
            text = text.Replace("\\", ",");
            text = text.Replace(":", "_");
            text = text.Replace("\"", " ");
            return text;
        }

        public static string FormatFileNameForCoverSearch(this string text, bool removeCover = false)
        {
            if (string.IsNullOrWhiteSpace(text)) 
                return "";

            text = text.Replace(" ", string.Empty).ToLower();
            text = SpecialCharacters().Replace(text, string.Empty); // Remove any special characters
            text = ParenthesesWithContent().Replace(text, string.Empty); // Remove any parentheses and their content
            text = Numbers().Replace(text, string.Empty); // Remove all numbers
            if (removeCover)
            {
                text = text.Replace("cover", string.Empty);
            }
            return text.Trim();
        }

        public static string GetAlbumCoverFromDirectory(string path, string albumName)
        {
            if (!Directory.Exists(path) || string.IsNullOrWhiteSpace(path))
            {
                return "";
            }

            string albumNameForSearch = albumName.FormatFileNameForCoverSearch(true);
            foreach (string file in Directory.EnumerateFiles(Path.GetDirectoryName(path)))
            {
                // not a supported image file
                if (!CoverHelper.SupportedImageExt.Contains(Path.GetExtension(file)))
                    continue;

                string filename = Path.GetFileNameWithoutExtension(file).FormatFileNameForCoverSearch();
                
                if (filename.Contains(albumNameForSearch) || filename == "cover" || filename.Contains("albumcover"))
                {
                    return file;
                }
            }

            return "";
        }

        public static string GetTrackCoverFromDirectory(string path, string title, string album)
        {
            if (!Directory.Exists(path) || string.IsNullOrWhiteSpace(path))
            {
                return "";
            }

            album = album.FormatFileNameForCoverSearch();
            string titleForSearch = title.FormatFileNameForCoverSearch(true);

            if (string.IsNullOrWhiteSpace(titleForSearch))
                return "";

            List<string> files = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories).ToList();
            foreach (string file in files)
            {
                // not a supported image file
                if (!CoverHelper.SupportedImageExt.Contains(Path.GetExtension(file)))
                    continue;


                string filename = Path.GetFileNameWithoutExtension(file).FormatFileNameForCoverSearch(true);
                if (filename.Contains(album) || album.Contains(filename) || string.IsNullOrWhiteSpace(filename))
                    continue;

                if (filename.Contains(titleForSearch) || titleForSearch.Contains(filename))
                {
                    return file;
                }
            }

            return "";
        }

        public static string GetArtistCoverFromDirectory(string path, string artist)
        {
            if (!Directory.Exists(path) || string.IsNullOrWhiteSpace(path))
            {
                return "";
            }

            string artistForSearch = artist.FormatFileNameForCoverSearch(true);
            List<string> files = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories).ToList();
            foreach (string file in files)
            {
                // not a supported image file
                if (!CoverHelper.SupportedImageExt.Contains(Path.GetExtension(file)))
                    continue;

                string filename = Path.GetFileNameWithoutExtension(file).FormatFileNameForCoverSearch();

                if (filename.Contains(artistForSearch) || filename.Contains("artist"))
                {
                    return file;
                }
            }

            return "";
        }

        /// <summary>
        /// Make sure the path is not null or white space and that it exists
        /// </summary>
        /// <param name="path"></param>
        /// <param name="acceptWhiteSpace"></param>
        /// <returns></returns>
        public static bool ValidFilePath(this string path, bool acceptWhiteSpace = false)
        {
            if (!acceptWhiteSpace && string.IsNullOrWhiteSpace(path)) return false;

            if(acceptWhiteSpace && path.Trim() == string.Empty) return true;

            return File.Exists(path);
        }

        [GeneratedRegex(@"[-,._&'|:\\/;`~!?<>%*^$#@]*")]
        private static partial Regex SpecialCharacters();
        [GeneratedRegex(@"(\([^()]*\))*")]
        private static partial Regex ParenthesesWithContent();
        [GeneratedRegex(@"[1-9]*")]
        private static partial Regex Numbers();
    }
}
