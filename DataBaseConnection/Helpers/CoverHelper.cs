using MusicPlay.Database.Models;

namespace MusicPlay.Database.Helpers
{
    public static class CoverHelper
    {
        public static readonly List<string> SupportedImageExt = [".png", ".jpg", ".bmp", ".gif", ".webp"];
        public static readonly string DefaultImgFormat = ".png";

        /// <summary>
        /// Get all the covers of the entity
        /// </summary>
        /// <param name="playableModel">The entity to get the covers of</param>
        /// <returns></returns>
        public static List<string> GetCovers(this PlayableModel playableModel)
        {
            List<string> covers = [];
            foreach (string file in Directory.EnumerateFiles(playableModel.GetCoverFolder()))
            {
                if (SupportedImageExt.Contains(Path.GetExtension(file)))
                {
                    covers.Add(file);
                }
            }
            return covers;
        }

        /// <summary>
        /// Create a random file name with no '.' and with the ".png" extension
        /// </summary>
        /// <returns></returns>
        public static string GetCoverFilename()
        {
            return Path.GetRandomFileName().Replace(".", "") + DefaultImgFormat;
        }

        /// <summary>
        /// Get the full path (folder + filename) for a cover of the entity
        /// </summary>
        /// <param name="playableModel"></param>
        /// <returns></returns>
        public static string GetNewCoverPath(this PlayableModel playableModel)
        {
            return playableModel.GetCoverFolder() + GetCoverFilename();
        }
    }
}
