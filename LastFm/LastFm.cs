using LastFmNamespace.Interfaces;
using LastFmNamespace.Services;
using LastFmNamespace.UrlBuilders;
using UriBuilder = LastFmNamespace.UrlBuilders.UriBuilder;

namespace LastFmNamespace
{
    public class LastFm
    {
        public static LastFm? Instance { get; private set; }

        private readonly string API_KEY;
        private readonly string _appName;
        private readonly string _appVersion;
        private readonly IHttpService _httpService;

        private readonly string baseApiUrl = "http://ws.audioscrobbler.com/2.0/";

        private string LastFmUrl => baseApiUrl + "?api_key=" + API_KEY + "&format=json";

        private ArtistServices? _artist;
        public ArtistServices Artist
        {
            get
            {
                return _artist ??= new(new ArtistUrlBuilder(LastFmUrl), _httpService);
            }
        }

        public LastFm(string apiKey, string appName, string appVersion)
        {
            API_KEY = apiKey;
            _appName = appName;
            _appVersion = appVersion;
            _httpService = new DefaultHttpService();
            InitHttpService();
            Instance = this;
        }

        public LastFm(string apiKey, string appName, string appVersion, IHttpService httpService)
        {
            API_KEY = apiKey;
            _appName = appName;
            _appVersion = appVersion;
            _httpService = httpService;
            InitHttpService();
            Instance = this;
        }

        private void InitHttpService()
        {
            _httpService.SetUserAgent(_appName + "/" + _appVersion);
        }
    }
}
