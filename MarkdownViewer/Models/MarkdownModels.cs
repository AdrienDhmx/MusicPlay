using System;
using System.Diagnostics;
using System.Security.Policy;
using System.Windows.Documents;
using System.Windows.Media;

namespace MarkdownViewerControl.Models
{
    public class LinkModel
    {
        public LinkModel() { }

        public string Placeholder {  get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;

        public int StartIndexInBlock { get; set; } = 0;
        public int EndIndexInBlock { get; set; } = 0;

        public Hyperlink? GetHyperLink(Brush foreground, double fontSize, bool underlined)
        {
            if(Placeholder.Length == 0 || Url.Length == 0 || StartIndexInBlock >= EndIndexInBlock)
            {
                return null;
            }

            Hyperlink hyperlink = new Hyperlink()
            {
                NavigateUri = new Uri(Url),
                Foreground = foreground,
                FontSize = fontSize,
            };

            if (!underlined)
                hyperlink.TextDecorations = null;

            hyperlink.Inlines.Add(new Run(Placeholder));
            hyperlink.Click += (s, e) =>
            {
                try
                {
                    Process.Start(Url);
                }
                catch (Exception)
                {
                    Process.Start(new ProcessStartInfo(Url.Replace("&", "^&")) { UseShellExecute = true });
                }
            };
            return hyperlink;
        }
    }
}
