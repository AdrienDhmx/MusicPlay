using MessageControl;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FilesProcessor.Helpers
{
    public class ConnectivityHelper
    {
        private bool _tooManyRequest;
        private List<string> _serversWithTooManyRequest;
        private string _lastServer;

        public ConnectivityHelper()
        {
            _tooManyRequest = false;
            _serversWithTooManyRequest = new List<string>();
        }

        /// <summary>
        /// Check if there is internet access by sending a ping to google.com
        /// </summary>
        /// <returns></returns>
        public static bool CheckInternetAccess()
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

        /// <summary>
        /// Send an async request to the specified <paramref name="url"/>.
        /// If the request is from a server that already send a "too many request" htttp error (http error 429) then the request is ignored.
        /// </summary>
        /// <param name="url"> The url to send the resquest to. </param>
        /// <param name="timeout"> The time in milliseconds to wait before the request times out. </param>
        /// <returns> The HttpResponseMessage of the request, or in case of an earlier 429 error from the same host server as the current url an empty response with that error is returned. </returns>
        public async Task<HttpResponseMessage> SendRequestAsync(string url, int timeout = 2000)
        {
            string server = GetHostServer(url);
            if(!_tooManyRequest && !_serversWithTooManyRequest.Contains(server))
            {
                _lastServer = server;

                using var client = new HttpClient();
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

        /// <summary>
        /// Get the host server (DNS or IP address) of the <paramref name="url"/>
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetHostServer(string url)
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
                    message = "Error: an unknown error has occured on the server.";
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
    }
}
