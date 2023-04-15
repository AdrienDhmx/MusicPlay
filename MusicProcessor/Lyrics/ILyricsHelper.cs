using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MusicFilesProcessor.Lyrics
{
    public interface ILyricsHelper
    {
        public string BaseURL { get; }
       
        /// <summary>
        /// create the url for the lyrics corresponding to the title of the tracks and its performer
        /// </summary>
        /// <param name="title"></param>
        /// <param name="artist"></param>
        /// <returns></returns>
        string GetUrl(string title, string artist);

        /// <summary>
        /// read a web page to remove all html tags and try to find the lyrics
        /// </summary>
        /// <param name="page"> the html page to read </param>
        /// <returns> the lyrics found in the page, otherwise an empty string </returns>
        string ReadWebPage(string page);
    }
}
