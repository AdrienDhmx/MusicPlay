using MusicPlay.Database.Models;

namespace MusicFilesProcessor.Lyrics
{
    public interface ILyricsFileHelper
    {
        /// <summary>
        /// Create the theoretical absolute path to the lyrics file based on its name
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns> the path to the file (may not exist) </returns>
        public string GetFilePath(string fileName);

        /// <summary>
        /// Retrieve the lyrics from the file in the given path as well as its data
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public MusicPlay.Database.Models.Lyrics GetLyrics(string filePath);

        /// <summary>
        /// Retrieve only the lyrics from the file (if it exist) based on the fileName property
        /// </summary>
        /// <param name="lyrics"></param>
        /// <returns> The lyrics model with the lyrics newly retrieved if found </returns>
        public MusicPlay.Database.Models.Lyrics GetLyrics(MusicPlay.Database.Models.Lyrics lyrics, string fileName);

        /// <summary>
        /// Save the lyrics to the given filePath
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="lyrics"></param>
        public void SaveLyrics(string filePath, MusicPlay.Database.Models.Lyrics lyrics);
    }
}
