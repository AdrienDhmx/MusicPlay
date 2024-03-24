using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UriBuilder = LastFmNamespace.UrlBuilders.UriBuilder;

namespace LastFmNamespace.UrlBuilders
{
    public class AlbumUrlBuilder(string baseUrl)
    {
        private readonly string _baseUrl = baseUrl;

        public Uri GetInfoUrl(string albumName, string artistName)
        {
            UriBuilder uriBuilder = UriBuilder.FromUrl(_baseUrl);
            uriBuilder.AddQueryParam("method", "album.getinfo");
            uriBuilder.AddQueryParam("album", albumName);
            uriBuilder.AddQueryParam("artist", artistName);
            return uriBuilder.Uri;
        }
    }
}
