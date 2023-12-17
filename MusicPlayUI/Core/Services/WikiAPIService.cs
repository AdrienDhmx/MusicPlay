using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using FilesProcessor.Helpers;
using MessageControl;
using MusicPlayUI.MVVM.Models;

namespace MusicPlayUI.Core.Services
{
    public static class WikiAPIService
    {
        private static readonly string wikiAPIUrl = @"https://en.wikipedia.org/w/api.php?action=query&prop=extracts&format=json&exintro=1&exlimit=1&explaintext=1&titles=";

        public static async Task<string> QueryExtract(string topic)
        {
            string titleParameter = HttpUtility.UrlEncode(topic);
            string url = wikiAPIUrl + titleParameter;

            string extract = await SendWikiRequest(url);

            if(extract == "")
            {
                // retry with title case
                CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                TextInfo textInfo = cultureInfo.TextInfo;
                topic = textInfo.ToTitleCase(topic.ToLower());
                titleParameter = HttpUtility.UrlEncode(topic);
                url = wikiAPIUrl + titleParameter;
                extract = await SendWikiRequest(url);

                if (string.IsNullOrWhiteSpace(extract))
                {
                    return "";
                }
            }

            return extract + "\n\n" + "From the Wikipedia article about " + topic;
        }

        private static async Task<string> SendWikiRequest(string url)
        {
            HttpResponseMessage response = await ConnectivityHelper.Instance.SendRequestAsync(url);

            if (response.IsSuccessStatusCode)
            {
                WikiQueryResult result = await response.Content.ReadFromJsonAsync<WikiQueryResult>();

                // page found
                if (result != null && result.query != null && result.query.pages != null && result.query?.pages?.FirstOrDefault().Key != "-1")
                {
                    WikiPage page = result.query.pages.FirstOrDefault().Value;

                    // page is not a Disambiguation page
                    if (!page.extract.Split('\n')[0].Contains("may refer to") && !string.IsNullOrWhiteSpace(page.extract.Trim()))
                    {
                        return page.extract.Trim();
                    }
                }
                return "";
            }
            else
            {
                ConnectivityHelper.Instance.HandleHttpError(response.StatusCode);
                return null;
            }
        }
    }
}
