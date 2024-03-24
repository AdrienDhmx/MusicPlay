using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LastFmNamespace.UrlBuilders
{
    public class ArtistUrlBuilder(string baseUrl)
    {
        private readonly string _baseUrl = baseUrl;

        public Uri GetArtistInfoUrl(string artistName)
        {
            UriBuilder uriBuilder = UriBuilder.FromUrl(_baseUrl);
            uriBuilder.AddQueryParam("method", "artist.getinfo");
            uriBuilder.AddQueryParam("artist", artistName);
            return uriBuilder.Uri;
        }
    }
}
