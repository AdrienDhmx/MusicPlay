using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LastFmNamespace.UrlBuilders;
using LastFmNamespace.Interfaces;
using LastFmNamespace.Models;
using Newtonsoft.Json;

namespace LastFmNamespace.Services
{
    public class AlbumServices(AlbumUrlBuilder albumUrlBuilder, IHttpService httpService)
    {
        private readonly AlbumUrlBuilder _albumUrlBuilder = albumUrlBuilder;
        private readonly IHttpService _httpService = httpService;

        public async Task<Root?> GetInfo(string albumName, string artistName)
        {
            Uri url = _albumUrlBuilder.GetInfoUrl(albumName, artistName);
            HttpResponseMessage response = await _httpService.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            string json = await response.Content.ReadAsStringAsync();

            Root? data = JsonConvert.DeserializeObject<Root>(json);
            return data;
        }
    }
}
