using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using MusicFilesProcessor.Helpers;
using System.Web;
using FilesProcessor.Providers.Models;
using System.Net.Http.Json;
using MusicPlay.Database.Helpers;
using System.Text.RegularExpressions;

namespace FilesProcessor.Providers
{
    public static class WikiAPIService
    {
        private static readonly string wikiAPIUrl = @"https://en.wikipedia.org/w/api.php?action=query&&exlimit=1&explaintext=1&format=json&inprop=url&prop=extracts|info|langlinks&variant&lllimit=50&&titles=";

        public static async Task<WikiPage> GetMarkdownWikiPage(string topic)
        {
            WikiPage page = await GetWikiPage(topic);
            if(page == null) 
                return null;

            if (page.extract.IsNotNullOrWhiteSpace())
            {
                // try to only get the interesting paragraph of the wiki page
                // so as to not have something too long
                string discographyHeader = "== discography ==";
                string recordingHeader = "== recordings ==";
                string seeAlsoHeader = "== see also ==";
                string bandMembersHeader = "== band members ==";
                string referencesHeader = "== references ==";
                page.extract = CutTextAfterMarker(page.extract, seeAlsoHeader);
                page.extract = CutTextAfterMarker(page.extract, discographyHeader);
                page.extract = CutTextAfterMarker(page.extract, recordingHeader);
                page.extract = CutTextAfterMarker(page.extract, bandMembersHeader);
                page.extract = CutTextAfterMarker(page.extract, referencesHeader);

                page.extract = ReplaceWikiHeadingsByMarkdownHeadings(page.extract);
                page.extract += $"\n\n- From the Wikipedia article about [{page.title}]({page.fullurl})";
            }

            return page;
        }

        private static string CutTextAfterMarker(string text, string marker)
        {
            int markerIndex = text.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
            if (markerIndex != -1)
            {
                return text.Substring(0, markerIndex);
            }
            return text;
        }

        private static string ReplaceWikiHeadingsByMarkdownHeadings(string input)
        {
            // Use regular expression to match "== ... ==" and replace it with "## ..."
            string pattern = @"==+\s*(.*?)\s*==+";
            return Regex.Replace(input, pattern, match =>
            {
                int numberOfEquals = (match.Value.Length - match.Groups[1].Length) / 2; // Divide by 2 to get the number of '=' characters
                string replacement = new string('#', numberOfEquals) + " " + match.Groups[1].Value;
                return replacement;
            });
        }

        private static async Task<WikiPage> GetWikiPage(string topic)
        {
            HttpResponseMessage response = await SendWikiRequest(topic);
            if (response.IsSuccessStatusCode)
            {
                WikiQueryResult result = await response.Content.ReadFromJsonAsync<WikiQueryResult>();
                if (result != null && result.query != null && result.query.pages != null 
                    && result.query?.pages?.FirstOrDefault().Key != "-1")
                {
                    return result.query.pages.Values.FirstOrDefault(); // only take the first page                
                }
                return null;
            }
            else
            {
                ConnectivityHelper.Instance.HandleHttpError(response.StatusCode);
                return null;
            }
        }

        private static async Task<HttpResponseMessage> SendWikiRequest(string topic)
        {
            string titleParameter = HttpUtility.UrlEncode(topic);
            string url = wikiAPIUrl + titleParameter;
            HttpResponseMessage response = await ConnectivityHelper.Instance.GetAsync(url);
            if (response.IsSuccessStatusCode)
                return response;

            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
            TextInfo textInfo = cultureInfo.TextInfo;
            topic = textInfo.ToTitleCase(topic.ToLower());
            titleParameter = HttpUtility.UrlEncode(topic);
            url = wikiAPIUrl + titleParameter;
            response = await ConnectivityHelper.Instance.GetAsync(url);
            return response;
        }
    }
}
