using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using LastFmNamespace.Interfaces;

namespace LastFmNamespace.Services
{
    public class DefaultHttpService : IHttpService
    {
        private string _userAgent = string.Empty;

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

        public string GetHostServer(string url)
        {
            var uri = new Uri(url);
            return uri.Host;
        }

        public void HandleHttpError(HttpStatusCode httpStatusCode)
        {

        }

        public async Task<HttpResponseMessage> GetAsync(Uri url, int timeout = 2000)
        {
            using var client = new HttpClient();
            if (!string.IsNullOrWhiteSpace(_userAgent))
                client.DefaultRequestHeaders.Add("User-Agent", _userAgent);
            client.Timeout = TimeSpan.FromMilliseconds(timeout);
            try
            {
                return await client.GetAsync(url);
            }
            catch (TaskCanceledException)
            {
                return new(HttpStatusCode.RequestTimeout);
            }
            catch(Exception)
            {
                return new(HttpStatusCode.BadRequest);
            }
        }

        public async Task<HttpResponseMessage> PostAsync(Uri url, HttpContent httpContent, int timeout = 2000)
        {
            using var client = new HttpClient();
            if (!string.IsNullOrWhiteSpace(_userAgent))
                client.DefaultRequestHeaders.Add("User-Agent", _userAgent);
            client.Timeout = TimeSpan.FromMilliseconds(timeout);
            try
            {
                return await client.PostAsync(url, httpContent);
            }
            catch (TaskCanceledException)
            {
                return new(HttpStatusCode.RequestTimeout);
            }
            catch (Exception)
            {
                return new(HttpStatusCode.BadRequest);
            }
        }

        public void SetUserAgent(string userAgent)
        {
            _userAgent = userAgent;
        }

        public async Task<HttpResponseMessage> GetAsync(string url, int timeout = 2000)
        {
            return await GetAsync(new Uri(url), timeout);
        }

        public async Task<HttpResponseMessage> PostAsync(string url, HttpContent httpContent, int timeout = 2000)
        {
            return await PostAsync(new Uri(url), httpContent, timeout);
        }
    }
}
