using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LastFmNamespace.Interfaces
{
    public interface IHttpService
    {
        public void SetUserAgent(string userAgent);
        public bool CheckInternetAccess();
        public Task<HttpResponseMessage> GetAsync(Uri url, int timeout = 2000);
        public Task<HttpResponseMessage> PostAsync(Uri url, HttpContent httpContent, int timeout = 2000);
        public string GetHostServer(string url);
        public void HandleHttpError(HttpStatusCode httpStatusCode);
    }
}
