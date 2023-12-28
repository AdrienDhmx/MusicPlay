using System.IO;

namespace MusicFilesProcessor.Helpers
{
    public static class DirectoryHelper
    {
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

        /// <summary>
        /// C:\User\UserName\Music\MusicPlay\
        /// </summary>
        public static readonly string AppFolder = Path.Combine(MusicFolder, "MusicPlay");

        /// <summary>
        /// C:\User\UserName\Music\MusicPlay\.settings (hidden folder)
        /// </summary>
        public static readonly string AppSettingsFolder = Path.Combine(AppFolder, ".settings");

        /// <summary>
        /// C:\User\UserName\Music\MusicPlay\Covers\
        /// </summary>
        public static readonly string AppCoverFolder = Path.Combine(AppFolder, "Covers");

        public static readonly string AlbumCoverDirectory = Path.Combine(AppCoverFolder, "AlbumCover");
        public static readonly string TrackCoverDirectory = Path.Combine(AppCoverFolder, "TrackCover");
        public static readonly string ArtistCoverDirectory = Path.Combine(AppCoverFolder, "ArtistCover");
        public static readonly string AvatarCoverDirectory = Path.Combine(AppCoverFolder, "Avatar");
        public static readonly string BlurredCoverDirectory = Path.Combine(AppCoverFolder, "Blurred");
        public static readonly string PlaylistCoverDirectory = Path.Combine(AppCoverFolder, "PlaylistCover");
        public static readonly string LyricsDirectory = Path.Combine(AppFolder, "Lyrics");
        public static readonly string TimedLyricsDirectory = Path.Combine(LyricsDirectory, "Timed Lyrics");

        /// <summary>
        /// Create the directory passed in
        /// </summary>
        /// <param name="path"></param>
        public static void CreateDirectory(this string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// Check whether the directory exists, if it doesn't create it.
        /// </summary>
        /// <param name="directory"></param>
        public static void CheckDirectory(this string directory)
        {
            if (string.IsNullOrEmpty(directory))
                return;

            if (!Directory.Exists(directory))
            {
                CreateDirectory(directory);
            }
        }

        /// <summary>
        /// Get the last folder name of the file passed in.
        /// </summary>
        /// <param name="file"></param>
        /// <returns> C:\User\UserName\Music\ArtistName\ this => AlbumName <= this \Filename </returns>
        public static string GetFolderName(this string file)
        {
            string output = "";

            // if the file doesn't exist it may be a directory
            if(File.Exists(file))
            {
                file = file.Replace(Path.GetFileName(file), string.Empty);
                file = file.Substring(0, file.Length - 1); //  To get rid of the last "\"
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
        /// Check if the following directory exists: importedFolder\artist.
        /// </summary>
        /// <param name="artist"></param>
        /// <returns>The directory if it exists or an empty string.</returns>
        public static string TryGetArtistDirectory(this string artist, string importedFolder)
        {
            string output = Path.Combine(importedFolder, artist);
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
            file = file.Substring(0, file.Length - 1); //  To get rid of the last "\"

            List<string> Folders = file.Split('\\').ToList(); // Get all the folders name

            if (Folders.Count > 0)
            {
                if (Folders.Any(f => f.ToLower() == ImportPath.ToLower())) // If a folder has the name of the artist
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
    }
}
