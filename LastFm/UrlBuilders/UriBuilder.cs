using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LastFmNamespace.UrlBuilders
{
    public class UriBuilder(string baseUrl, Dictionary<string, string> query)
    {
        public string Url { get; private set; } = baseUrl;
        public Dictionary<string, string> Query = query;

        public Uri Uri => new(this.ToString());

        public static UriBuilder FromUrl(string url)
        {
            var uri = new Uri(url);
            string Url = $"{uri.Scheme}://{uri.Host}{(uri.IsDefaultPort ? "" : $":{uri.Port}")}{uri.AbsolutePath}";

            // Parse query parameters
            var queryParameters = uri.Query.TrimStart('?').Split('&');
            Dictionary<string, string> Query = []; 
            foreach (var parameter in queryParameters)
            {
                var keyValue = parameter.Split('=');
                if (keyValue.Length == 2)
                {
                    Query.Add(Uri.UnescapeDataString(keyValue[0]), Uri.UnescapeDataString(keyValue[1]));
                }
            }
            return new(Url, Query);
        }

        public static UriBuilder FromUriBuilder(UriBuilder uriBuilder)
        {
            return new(uriBuilder.Url, uriBuilder.Query);
        }

        /// <summary>
        /// Add the a new parameter to the current Query. <br></br>
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddQueryParam(string key, string value)
        {
            if (!Query.TryAdd(key, value))
            {
                Query[key] = value;
            }
        }

        /// <summary>
        /// Create a new UriBuilder from the current one and add the param to this new instance.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns>A new instance of UriBuilder</returns>
        public UriBuilder CloneAddQueryParam(string key, string value)
        {
            UriBuilder newInstance = new(Url.Clone().ToString()!, new(Query)) ;
            newInstance.AddQueryParam(key, value);
            return newInstance;
        }

        public override string ToString()
        {
            string url = Url;

            if (Query.Count > 0)
            {
                url += "?";
                foreach (var kvp in Query)
                {
                    url += $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}&";
                }
                url = url[..^1]; // Remove the trailing "&"
            }

            return url;
        }
    }
}
