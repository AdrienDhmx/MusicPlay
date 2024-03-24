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
                Dictionary<string, string> fileNames = [];
                foreach (string filePath in files)
                {
                    if (File.Exists(filePath))
                    {
                        fileNames.Add(filePath, Path.GetFileName(filePath));
                    }
                }
                KeyValuePair<string, string> mostRecentSave = fileNames.OrderDescending().First();
                string json = File.ReadAllText(mostRecentSave.Key);

                string fileType = mostRecentSave.Value.Split('_')[1].Split(".")[0];

                return fileType switch
                {
                    artistInfo => await LastFm.Instance.Artist.DeserializeArtistInfoJson(json, false),
                    _ => null,
                };
            }

            if (playableModel is Artist artist)
            {
                return await GetArtistExtData(artist);
            }
            return null;
        }

        public static async Task<string> UpdateWithExternalData(this Artist artist, Root data)
        {
            if (data.IsNull())
                return null;

            if(data.Method == artistInfo)
            {
                string appliedCover = null;
                await ImageHelper.SaveAllCovers(data.Artist.Images, artist, async (cover) =>
                {
                    // this callback is called once to update the cover of the model
                    // but I take this opportunity to also update the others properties
                    await Artist.Update((artist) =>
                    {
                        artist.Biography = data.Artist.Bio.Content;
                        artist.Cover = appliedCover = cover;
                    }, artist);
                });
                return appliedCover;
            }
            return null;
        }

        private static async Task<Root> GetArtistExtData(Artist artist)
        {
            string jsonData = await LastFm.Instance.Artist.GetArtistInfoJson(artist.Name);
            if (jsonData.IsNull())
                return null;

            Root data = await LastFm.Instance.Artist.DeserializeArtistInfoJson(jsonData);
            // try to get the biography from wikipedia as it's often of better quality than LastFM
            WikiPage wikiPage = await WikiAPIService.GetMarkdownWikiPage(artist.Name);
            if (ShouldUseWikiBiography(wikiPage?.extract))
            {
                data.Artist.Bio.Content = wikiPage.extract;
            }
            else
            {
                // LastFM biography contains a little of HTML sometines (link mainly)
                data.Artist.Bio.Content = new Html2Markdown.Converter().Convert(data.Artist.Bio.Content);
            }

            // save to file
            await SaveLastFmJson(artist, jsonData, artistInfo);
            await UpdateWithExternalData(artist, data);
            return data;
        }

        private static async Task SaveLastFmJson(PlayableModel playableModel, string json, string filePrefix)
        {
            string lastFmFolder = playableModel.GetLastFmFolder();
            string filename = DateTime.Now.Ticks.ToString() + '_' + filePrefix + ".lastfm.json";
            await File.WriteAllTextAsync(lastFmFolder + filename, json);
        }

        private static bool ShouldUseWikiBiography(string biography)
        {
            // may refer to pages means that the keyword(s) used to get the page has multiple pages for different topic,
            // therefor wikimedia gives back a list of all the possible pages we are looking for
            // however we won't try to go looking for the one we want
            if (biography.IsNullOrWhiteSpace() || biography.Split("\n").Contains("may refer to"))
                return false;

            // sometimes the keyword(s) have another meaning that the one we want and it's that other topic wikimedia gives us back
            // (probably because there are no pages about the one we are looking for)
            // therefor we need to make sure the page is related to music/artists...
            List<string> validTopics = ["artist", "composer", "music", "album", "orchestra", "dj"];
            return validTopics.Any(topic => biography.Contains(topic, StringComparison.OrdinalIgnoreCase));
        }
    }
}
