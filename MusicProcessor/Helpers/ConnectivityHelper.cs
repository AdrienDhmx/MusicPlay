using LastFmNamespace.Interfaces;
using MessageControl;
using MusicPlay.Database.Helpers;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Windows;

namespace MusicFilesProcessor.Helpers
{
    public class ConnectivityHelper : IHttpService
    {
        private bool _tooManyRequest;
        private List<string> _serversWithTooManyRequest;
        private string _lastServer;

        private string _defaultUserAgent = string.Empty;

        private static ConnectivityHelper _instance;
        public static ConnectivityHelper Instance
        {
            get => _instance ??= new ConnectivityHelper();
        }

        private ConnectivityHelper()
        {
            _tooManyRequest = false;
            _serversWithTooManyRequest = new List<string>();
        }

        /// <summary>
        /// Check if there is internet access by sending a ping to google.com
        /// </summary>
        /// <returns></returns>
        public bool CheckInternetAccess()
        {
            try
            {
                Ping myPing = new Ping();
                string host = "google.com";
                byte[] buffer = new byte[32];
                int timeout = 2000;
                PingOptions pingOptions = new PingOptions();
                PingReply reply = myPing.Send(host, timeout, buffer, pingOptions);
                return (reply.Status == IPStatus.Success);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<HttpResponseMessage> GetAsync(string url, int timeout = 2000)
        {
            return await GetAsync(new Uri(url), timeout);
        }

        /// <summary>
        /// Send an async request to the specified <paramref name="url"/>.
        /// If the request is from a server that already send a "too many request" htttp error (http error 429) then the request is ignored.
        /// </summary>
        /// <param name="url"> The url to send the request to. </param>
        /// <param name="timeout"> The time in milliseconds to wait before the request times out. </param>
        /// <returns> The HttpResponseMessage of the request, or in case of an earlier 429 error from the same host server as the current url an empty response with that error is returned. </returns>
        public async Task<HttpResponseMessage> GetAsync(Uri url, int timeout = 2000)
        {
            string server = url.Host;
            if(!_tooManyRequest && !_serversWithTooManyRequest.Contains(server))
            {
                _lastServer = server;

                using var client = new HttpClient();
                if (_defaultUserAgent.IsNotNullOrWhiteSpace())
                    client.DefaultRequestHeaders.Add("User-Agent", _defaultUserAgent);
                client.Timeout = TimeSpan.FromMilliseconds(timeout);
                try
                {
                    return await client.GetAsync(url);
                }
                catch (TaskCanceledException)
                {
                    return new(HttpStatusCode.RequestTimeout);
                }
            }
            else
            {
                return new(HttpStatusCode.TooManyRequests);
            }
        }

        public async Task<HttpResponseMessage> PostAsync(string url, HttpContent content, int timeout = 2000)
        {
            return await PostAsync(new Uri(url), content, timeout);
        }

        /// <summary>
        /// Send an async request to the specified <paramref name="url"/>.
        /// If the request is from a server that already send a "too many request" htttp error (http error 429) then the request is ignored.
        /// </summary>
        /// <param name="url"> The url to send the request to. </param>
        /// <param name="timeout"> The time in milliseconds to wait before the request times out. </param>
        /// <returns> The HttpResponseMessage of the request, or in case of an earlier 429 error from the same host server as the current url an empty response with that error is returned. </returns>
        public async Task<HttpResponseMessage> PostAsync(Uri url, HttpContent content, int timeout = 2000)
        {
            string server = url.Host;
            if (!_tooManyRequest && !_serversWithTooManyRequest.Contains(server))
            {
                _lastServer = server;

                using var client = new HttpClient();
                if(_defaultUserAgent.IsNotNullOrWhiteSpace())
                    client.DefaultRequestHeaders.Add("User-Agent", _defaultUserAgent);
                client.Timeout = TimeSpan.FromMilliseconds(timeout);
                try
                {
                    return await client.PostAsync(url, content);
                }
                catch (TaskCanceledException)
                {
                    return new(HttpStatusCode.RequestTimeout);
                }
            }
            else
            {
                return new(HttpStatusCode.TooManyRequests);
            }
        }

        public string GetHostServer(string url)
        {
            Uri Uri = new Uri(url);
            return Uri.Host;
        }

        /// <summary>
        /// Publish a message corresponding to the http error code using <see cref="MessageHelper.PublishMessage(MessageControl.Model.MessageModel)"/>
        /// with the default Error Message <see cref="DefaultMessageFactory.CreateErrorMessage(string)"/>
        /// </summary>
        /// <param name="httpStatusCode"></param>
        public void HandleHttpError(HttpStatusCode httpStatusCode)
        {
            string message;
            switch (httpStatusCode)
            {
                case HttpStatusCode.NotFound:
                    message = "Error: the requested resource does not exist on the server.";
                    break;
                case HttpStatusCode.RequestTimeout:
                    message = "Error: request timed out.";
                    break;
                case HttpStatusCode.Gone:
                    message = "Error: the requested resource is no longer available.";
                    break;
                case HttpStatusCode.TooManyRequests:
                    message = "Error: you have send too many request to the server.";
                    if(!_serversWithTooManyRequest.Contains(_lastServer))
                        _serversWithTooManyRequest.Add(_lastServer);
                    _tooManyRequest = true;
                    break;
                case HttpStatusCode.InternalServerError:
                    message = "Error: an unknown error has occurred on the server.";
                    break;
                case HttpStatusCode.ServiceUnavailable:
                    message = "Error: the server is temporarily unavailable.";
                    break;
                case HttpStatusCode.GatewayTimeout:
                    message = "Error: gateway timed out";
                    break;
                case HttpStatusCode.NetworkAuthenticationRequired:
                    message = "Error: you need to be authenticated to gain network access.";
                    break;
                default:
                    message = "Unknown HTTP error.";
                    break;
            }

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage(message));
            }));
        }

        /// <summary>
        /// Publish an error message saying that there is no access to the internet using <see cref="MessageHelper.PublishMessage(MessageControl.Model.MessageModel)"/>
        /// </summary>
        public static void PublishNoConnectionMsg()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage("Error: you are not connected to the internet."));
            }));
        }

        public void SetUserAgent(string userAgent)
        {
            _defaultUserAgent = userAgent;
        }

        public async Task<bool> DownloadImage(string imageUrl, string destinationPath)
        {
            using var httpClient = new HttpClient();
            try
            {
                var response = await httpClient.GetByteArrayAsync(imageUrl);

                using var stream = new FileStream(destinationPath, FileMode.Create);
                await stream.WriteAsync(response, 0, response.Length);
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error downloading image: {ex.Message}");
                return false;
            }
            return true;
        }
    }
}
