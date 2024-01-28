using System.IO;
using FilesProcessor.Providers;
using FilesProcessor.Providers.Models;
using LastFmNamespace;
using LastFmNamespace.Models;
using MusicFilesProcessor.Helpers;
using MusicPlay.Database.Helpers;
using MusicPlay.Database.Models;

namespace MusicFilesProcessor
{
    public static class ExternalDataProvider
    {
        public const string artistInfo = LastFmNamespace.Services.ArtistServices.artistInfoMethod;

        public static bool HasAlreadyFetchedData(this PlayableModel playableModel)
        {
            string lastFmFolder = playableModel.GetLastFmFolder();
            return Directory.GetFiles(lastFmFolder).IsNotNullOrEmpty();
        }

        public static async Task<Root> GetExternalData(this PlayableModel playableModel)
        {
            if (playableModel.HasAlreadyFetchedData())
            {
                string[] files = playableModel.GetLastFmFiles();
                string mostRecentSave = files.OrderDescending().ElementAt(0);
                string json = File.ReadAllText(mostRecentSave);

                string fileType = mostRecentSave.Split('_')[1];

                return fileType switch
                {
                    artistInfo => await LastFm.Instance.Artist.DeserializeArtistInfoJson(json),
                    _ => null,
                };
            }

            if (playableModel is Artist artist)
            {
                return await GetArtistExtData(artist);
            }
            return null;
        }

        public static async Task UpdateWithExternalData(this Artist artist, Root data)
        {
            if (data.IsNull())
                return;

            if(data.Method == artistInfo)
            {
                await ImageHelper.SaveAllCovers(data.Artist.Images, artist, async (cover) =>
                {
                    // this callback is called once to update the cover of the model
                    // but I take this opportunity to also update the others properties
                    await Artist.Update((artist) =>
                    {
                        artist.Biography = data.Artist.Bio.Content;
                        artist.Cover = cover;

                    }, artist);
                });
            }
        }

        private static async Task<Root> GetArtistExtData(Artist artist)
        {
            string jsonData = await LastFm.Instance.Artist.GetArtistInfoJson(artist.Name);
            if (jsonData.IsNull())
                return null;

            Root data = await LastFm.Instance.Artist.DeserializeArtistInfoJson(jsonData);
            WikiPage wikiPage = await WikiAPIService.GetMarkdownWikiPage(artist.Name);
            if (ShouldUseWikiBiography(wikiPage?.extract))
            {
                data.Artist.Bio.Content = wikiPage.extract;
            }

            // save json to file
            await SaveLastFmJson(artist, jsonData, artistInfo);
            return data;
        }

        private static async Task SaveLastFmJson(PlayableModel playableModel, string json, string filePrefix)
        {
            string lastFmFolder = playableModel.GetLastFmFolder();
            string filename = DateTime.Now.Ticks.ToString() + '_' + filePrefix + ".lastfm";
            await File.WriteAllTextAsync(lastFmFolder + filename, json);
        }

        private static bool ShouldUseWikiBiography(string biography)
        {
            if (biography.IsNullOrWhiteSpace())
                return false;

            List<string> validTopics = ["artist", "composer", "music", "album", "orchestra", "dj"];
            return validTopics.Any(topic => biography.Contains(topic, StringComparison.OrdinalIgnoreCase));
        }
    }
}
