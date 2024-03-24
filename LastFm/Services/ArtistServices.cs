using System.Text.RegularExpressions;
using LastFmNamespace.Interfaces;
using LastFmNamespace.Models;
using LastFmNamespace.UrlBuilders;
using Newtonsoft.Json;

namespace LastFmNamespace.Services
{
    public partial class ArtistServices(ArtistUrlBuilder artistUrlBuilder, IHttpService httpService)
    {
        private readonly ArtistUrlBuilder _artistUrlBuilder = artistUrlBuilder;
        private readonly IHttpService _httpService = httpService;

        public const string artistInfoMethod = "artistInfo";

        /// <summary>
        /// Get the json response of Last.Fm GetArtistInfo API method
        /// </summary>
        /// <param name="artistName"></param>
        /// <returns></returns>
        public async Task<string?> GetArtistInfoJson(string artistName)
        {
            Uri url = _artistUrlBuilder.GetArtistInfoUrl(artistName);
            HttpResponseMessage response = await _httpService.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                _httpService.HandleHttpError(response.StatusCode);
                return null;
            }

            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// Deserialize the json response of <see cref="GetArtistInfoJson(string)"/> and retrieve the artist images
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public async Task<Root?> DeserializeArtistInfoJson(string json, bool fetchImages = true)
        {
            Root? data = JsonConvert.DeserializeObject<Root>(json);
            if (data != null && data.Artist != null)
            {
                if(fetchImages)
                {
                    data.Artist.Images = await GetArtistImages(data.Artist.Url);
                }
                data.Method = artistInfoMethod;
            }
            return data;
        }

        /// <summary>
        /// Return the deserialized response of Last.Fm GetArtistInfo API method with the artist 
        /// images retrieved from the html of the its Last.Fm gallery page
        /// </summary>
        /// <param name="artistName"></param>
        /// <returns></returns>
        public async Task<Root?> GetArtistInfo(string artistName)
        {
            string? json = await GetArtistInfoJson(artistName);

            if (json == null)
                return null;

            return await DeserializeArtistInfoJson(json);
        }

        public async Task<List<string>> GetArtistImages(string artistUrl)
        {
            string ArtistPhotosGalleryUrl = artistUrl + "/+images";

            HttpResponseMessage response = await _httpService.GetAsync(new(ArtistPhotosGalleryUrl));
            if (!response.IsSuccessStatusCode)
            {
                return [];
            }
            string rawHtml = await response.Content.ReadAsStringAsync();
            List<string> images = new List<string>();

            Regex regex = LastFmImageUrlRegex();
            foreach (Match match in regex.Matches(rawHtml).Cast<Match>())
            {
                if(match.Success && !images.Contains(match.Value))
                {
                    images.Add(match.Value);
                }
            }

            return images;
        }

        [GeneratedRegex(@"https://lastfm\.freetls\.fastly\.net/i/u/(?!.*\.jpg)[^""\s]+")]
        private static partial Regex LastFmImageUrlRegex();
    }
}
