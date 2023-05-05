using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace MusicFilesProcessor.Helpers
{
    public static class ImageHelper
    {
        private static readonly List<string> ImageExtension = new() { ".png", ".jpg", ".bmp", ".gif", ".webp" };
        public static readonly string imgFormat = ".png";
        public static readonly int treshold = 200000;

        public static string CreateCoverPath(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            fileName = fileName.FormatSyntax();

            string path = Path.Combine(DirectoryHelper.AppCoverFolder, fileName);
            DirectoryHelper.CheckDirectory(DirectoryHelper.AppCoverFolder);
            path = CreateValidPath(path);
            return path;
        }

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

        public static (string, string) GetAlbumCover(string album, string filepath)
        {
            if (Path.HasExtension(filepath))
                filepath = DirectoryHelper.GetDirectory(filepath);

            string coverPath = GetAlbumCoverFromDirectory(filepath, album);
            if (!string.IsNullOrWhiteSpace(coverPath))
            {
                string newPath = CreateCoverPath(CreateCoverFilename());
                coverPath.SaveFileToNewPath(newPath);
                return (coverPath, newPath);
            }
            return ("", "");
        }

        /// <summary>
        /// Save a file to the designated path by copying it and creating 2 smaller versions of it
        /// </summary>
        /// <param name="file"></param>
        /// <param name="newPath"></param>
        public static void SaveFileToNewPath(this string file, string newPath)
        {
            if(file.ValidPath() && !string.IsNullOrWhiteSpace(newPath))
            {
                if(file != newPath)
                    File.Copy(file, newPath, true); // copy original version

                string mediumFileNamePath = GetModifiedCoverPath(newPath, true);
                string thumbnailFileNamePath = GetModifiedCoverPath(newPath, false);
                ImageProcessor.FormatImage(file, mediumFileNamePath, thumbnailFileNamePath, treshold);
            }
        }

        public static string GetModifiedCoverPath(this string path, bool medium)
        {
            if (!ValidPath(path))
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

        public static string CreateCoverFilename()
        {
            return Path.GetRandomFileName() + imgFormat;
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

        private static string FormatForCoverSearch(this string text, bool removeCover = false)
        {
            if (string.IsNullOrWhiteSpace(text)) 
                return "";

            text = text.Replace(" ", string.Empty).ToLower();
            text = Regex.Replace(text, @"[-,._&'|:]*", string.Empty); // Remove any of these
            text = Regex.Replace(text, @"(\([^()]*\))*", string.Empty); // Remove any parentheses and their content
            text = Regex.Replace(text, @"[1-9]*", string.Empty); // Remove all numbers
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

            string albumNameForSearch = albumName.FormatForCoverSearch(true);
            foreach (string file in Directory.EnumerateFiles(Path.GetDirectoryName(path)).Where(s => ImageExtension.Contains(Path.GetExtension(s).ToLower())))
            {
                string filename = Path.GetFileNameWithoutExtension(file).FormatForCoverSearch();
                
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

            album = album.FormatForCoverSearch();
            string titleForSearch = title.FormatForCoverSearch(true);

            if (string.IsNullOrWhiteSpace(titleForSearch))
                return "";

            List<string> files = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories).ToList().Where(s => ImageExtension.Contains(Path.GetExtension(s).ToLower())).ToList();
            foreach (string file in files)
            {
                string filename = Path.GetFileNameWithoutExtension(file).FormatForCoverSearch(true);
                if (filename.Contains(album) || album.Contains(filename) || string.IsNullOrWhiteSpace(filename))
                    continue;

                if (filename.Contains(titleForSearch) || titleForSearch.Contains(filename))
                {
                    string newPath = CreateCoverPath(CreateCoverFilename());
                    file.SaveFileToNewPath(newPath);
                    return newPath;
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

            string artistForSearch = artist.FormatForCoverSearch(true);
            List<string> files = Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories).ToList().Where(s => ImageExtension.Contains(Path.GetExtension(s).ToLower())).ToList();
            foreach (string file in files)
            {
                string filename = Path.GetFileNameWithoutExtension(file).FormatForCoverSearch();

                if (filename.Contains(artistForSearch) || filename.Contains("artist"))
                {
                    string newPath = CreateCoverPath(CreateCoverFilename());
                    file.SaveFileToNewPath(newPath);
                    return newPath;
                }
            }

            return "";
        }

        public static bool ValidPath(this string path, bool acceptWhiteSpace = false)
        {
            if (!acceptWhiteSpace && string.IsNullOrWhiteSpace(path)) return false;

            if(acceptWhiteSpace && path.Trim() == string.Empty) return true;

            return File.Exists(path);
        }
    }
}
