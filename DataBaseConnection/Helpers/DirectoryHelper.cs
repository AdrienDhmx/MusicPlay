using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Serialization;
using MessageControl;
using MusicPlay.Database.Models;

namespace MusicPlay.Database.Helpers
{
    public static class DirectoryHelper
    {
#if DEBUG
        public static readonly string AppName = "MusicPlay_DEBUG";
#else
        public static readonly string AppName = "MusicPlay";
#endif

        /// <summary>
        /// C:\User\UserName\Music\
        /// </summary>
        public static readonly string MusicFolder = Enum.GetValues(typeof(Environment.SpecialFolder))
                                                 .Cast<Environment.SpecialFolder>()
                                                 .Select(specialFolder => new
                                                 {
                                                     Name = specialFolder.ToString(),
                                                     Path = Environment.GetFolderPath(specialFolder)
                                                 }).Where(i => i.Name == "MyMusic").First().Path;

        private static string _appFolder;
        public static string AppFolder
        {
            get
            {
                if (_appFolder.IsNull())
                {
                    _appFolder = MusicFolder + "/" + AppName + "/";
                }
                return _appFolder;
            }
        }

        private static readonly string ArtistFolderName = "artists";
        private static readonly string AlbumsFolderName = "albums";
        private static readonly string PlaylistsFolderName = "playlists";
        private static readonly string TrackFolderName = "track_covers";
        private static readonly string CoverFolderName = "covers";
        private static readonly string BlurredCoverFolderName = $"${CoverFolderName}/blurred";
        private static readonly string LastFmFolderName = "lastfm";

        public static readonly string LyricsDirectory = Path.Combine(AppFolder, "Lyrics");
        public static readonly string TimedLyricsDirectory = Path.Combine(LyricsDirectory, "Timed Lyrics");

        /// <summary>
        /// Check whether the directory exists, if it doesn't it's created.
        /// </summary>
        /// <param name="directory"></param>
        /// <returns>The directory</returns>
        public static string CheckDirectory(this string directory)
        {
            if (string.IsNullOrEmpty(directory))
                return string.Empty;

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            return directory;
        }

        /// <summary>
        /// Get the last folder name of the file passed in.
        /// </summary>
        /// <param name="file"></param>
        /// <returns> C:\User\UserName\Music\ArtistName\ this => AlbumName <= this \Filename.ext </returns>
        public static string GetFolderName(this string file)
        {
            string output = "";

            // if the file doesn't exist it may be a directory
            if (File.Exists(file))
            {
                file = file.Replace(Path.GetFileName(file), string.Empty);
                file = file[..^1]; //  To get rid of the last "\"
            }

            string value = "";
            foreach (char c in file.Reverse())
            {
                if (c == '\\')
                {
                    break;
                }
                else
                {
                    value += c; // add the char
                }
            }

            foreach (char c in value.Reverse())
            {
                output += c;
            }
            return output;
        }

        /// <summary>
        /// Check if the following directory exists: importedFolder\artistName.
        /// </summary>
        /// <param name="artistName"></param>
        /// <returns>The directory if it exists or an empty string.</returns>
        public static string TryGetArtistDirectory(this string artistName, string importedFolder)
        {
            string output = Path.Combine(importedFolder, artistName);
            if (Directory.Exists(output))
            {
                return output;
            }
            return "";
        }

        /// <summary>
        /// Try to get the first folder after the ImportPath the file has
        /// </summary>
        /// <param name="file"></param>
        /// <param name="ImportPath"></param>
        /// <returns> ImportPath = C:\User\UserName\Music\   
        ///           file = C:User\UserName\Music\Folder1\Folder2\FolderX\FileName   
        ///           return = ImportPath\Folder1 
        /// </returns>
        public static string TryGetBaseDirectory(this string file, string ImportPath)
        {
            string output = "";

            ImportPath = ImportPath.Split('\\').Last();
            // Get rid of the file name
            file = file.Replace(Path.GetFileName(file), string.Empty);
            file = file[..^1]; //  To get rid of the last "\"

            List<string> Folders = [.. file.Split('\\')]; // Get all the folders name

            if (Folders.Count > 0)
            {
                if (Folders.Any(f => f.Equals(ImportPath, StringComparison.CurrentCultureIgnoreCase))) // If a folder has the name of the artistName
                {
                    int ImportPathFolderIndex = Folders.IndexOf(ImportPath); // Find the index of that folder
                    if (ImportPathFolderIndex + 1 < Folders.Count) // If the importPath is not the last folder in the list
                    {
                        for (int i = 0; i <= ImportPathFolderIndex + 1; i++)
                        {
                            output += Folders[i] + "\\"; // Recreate the path until that folder + the next folder
                        }
                    }
                    return output;
                }
            }
            return null;
        }

        /// <summary>
        /// Retrieve the directory of the file
        /// </summary>
        /// <param name="file"></param>
        /// <returns> The directory of the file </returns>
        public static string GetDirectory(this string file)
        {
            if (!string.IsNullOrWhiteSpace(file))
            {
                return file.Replace(Path.GetFileName(file), string.Empty);
            }
            return null;
        }

        /// <summary>
        /// Get the assigned folder (path) of the entity. <br></br>
        /// Note: Only Artist and Album are supported
        /// </summary>
        /// <typeparam name="T">The type of the entity</typeparam>
        /// <param name="playableModel">The entity to get the assigned folder of</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">The type of the argument is not supported (i.e.: Artist and Album)</exception>
        public static string GetFolder(this PlayableModel playableModel)
        {
            string folderName = null;
            if(playableModel.GetType() == typeof(Artist))
            {
                folderName = ArtistFolderName;
            }
            else if(playableModel.GetType() == typeof(Album))
            {
                folderName = AlbumsFolderName;
            }
            else if(playableModel.GetType() == typeof(Track))
            {
                return CheckDirectory(AppFolder + TrackFolderName + "/");
            }
            else if (playableModel.GetType() == typeof(Playlist))
            {
                folderName = PlaylistsFolderName;
            }

            if (folderName.IsNotNullOrWhiteSpace())
            {
                string directory = AppFolder + folderName + "/" + playableModel.Id + "/";
                return CheckDirectory(directory);
            }
            else
            {
                throw new ArgumentException($"The type {playableModel.GetType()} is not valid, only {typeof(Artist)}, {typeof(Album)} and {typeof(Track)} are supported.");
            }
        }

        public static string GetCoverFolder(this PlayableModel playableModel)
        {
            string folder = playableModel.GetFolder() + CoverFolderName + "/";
            return CheckDirectory(folder);
        }

        public static string GetBlurredCoverFolder(this PlayableModel playableModel)
        {
            string folder = playableModel.GetFolder() + BlurredCoverFolderName + "/";
            return CheckDirectory(folder);
        }

        public static string GetLastFmFolder(this PlayableModel playableModel)
        {
            string folder = playableModel.GetFolder() + LastFmFolderName + "/";
            return CheckDirectory(folder);
        }

        public static string[] GetLastFmFiles(this PlayableModel playableModel)
        {
            return Directory.GetFiles(GetLastFmFolder(playableModel));
        }

        public static void DeleteFolder(this PlayableModel playableModel)
        {
            try
            {
                Directory.Delete(playableModel.GetFolder(), true);
            }
            catch (IOException ioException)
            {
                // $"Error while deleting folder its file: {ioException.Message}".CreateErrorMessage().Publish();
            }
        }
    }
}
