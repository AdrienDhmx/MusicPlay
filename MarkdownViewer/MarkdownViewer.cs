﻿using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using MarkdownViewerControl.Models;

namespace MarkdownViewerControl
{
    public class MarkdownViewer : TextBlock
    {
        private const char MarkdownTitleToken = '#';
        private const string MarkdownQuoteBlockToken = "> ";
        private const string LinkRegex = @"\[(?<placeholder>.*?)\]\((?<url>.*?)\)";

        public Brush LinkColor
        {
            get { return (Brush)GetValue(LinkColorProperty); }
            set { SetValue(LinkColorProperty, value); }
        }

        public bool AreLinksUnderlined
        {
            get { return (bool)GetValue(AreLinksUnderlinedProperty); }
            set { SetValue(AreLinksUnderlinedProperty, value); }
        }

        public Brush Header1Foreground
        {
            get { return (Brush)GetValue(Header1ForegroundProperty); }
            set { SetValue(Header1ForegroundProperty, value); }
        }

        public Brush Header2Foreground
        {
            get { return (Brush)GetValue(Header2ForegroundProperty); }
            set { SetValue(Header2ForegroundProperty, value); }
        }

        public Brush Header3Foreground
        {
            get { return (Brush)GetValue(Header3ForegroundProperty); }
            set { SetValue(Header3ForegroundProperty, value); }
        }

        public Brush Header4Foreground
        {
            get { return (Brush)GetValue(Header4ForegroundProperty); }
            set { SetValue(Header4ForegroundProperty, value); }
        }

        public Brush Header5Foreground
        {
            get { return (Brush)GetValue(Header5ForegroundProperty); }
            set { SetValue(Header5ForegroundProperty, value); }
        }

        public Brush Header6Foreground
        {
            get { return (Brush)GetValue(Header6ForegroundProperty); }
            set { SetValue(Header6ForegroundProperty, value); }
        }

        public double Header1FontSize
        {
            get { return (double)GetValue(Header1FontSizeProperty); }
            set { SetValue(Header1FontSizeProperty, value); }
        }

        public double Header2FontSize
        {
            get { return (double)GetValue(Header2FontSizeProperty); }
            set { SetValue(Header2FontSizeProperty, value); }
        }

        public double Header3FontSize
        {
            get { return (double)GetValue(Header3FontSizeProperty); }
            set { SetValue(Header3FontSizeProperty, value); }
        }

        public double Header4FontSize
        {
            get { return (double)GetValue(Header4FontSizeProperty); }
            set { SetValue(Header4FontSizeProperty, value); }
        }

        public double Header5FontSize
        {
            get { return (double)GetValue(Header5FontSizeProperty); }
            set { SetValue(Header5FontSizeProperty, value); }
        }

        public double Header6FontSize
        {
            get { return (double)GetValue(Header6FontSizeProperty); }
            set { SetValue(Header6FontSizeProperty, value); }
        }

        public static readonly DependencyProperty Header6FontSizeProperty =
            DependencyProperty.Register("Header6FontSize", typeof(double), typeof(MarkdownViewer), new PropertyMetadata(18d));


        public static readonly DependencyProperty Header5FontSizeProperty =
            DependencyProperty.Register("Header5FontSize", typeof(double), typeof(MarkdownViewer), new PropertyMetadata(20d));


        public static readonly DependencyProperty Header4FontSizeProperty =
            DependencyProperty.Register("Header4FontSize", typeof(double), typeof(MarkdownViewer), new PropertyMetadata(25d));


        public static readonly DependencyProperty Header3FontSizeProperty =
            DependencyProperty.Register("Header3FontSize", typeof(double), typeof(MarkdownViewer), new PropertyMetadata(30d));


        public static readonly DependencyProperty Header2FontSizeProperty =
            DependencyProperty.Register("Header2FontSize", typeof(double), typeof(MarkdownViewer), new PropertyMetadata(35d));


        public static readonly DependencyProperty Header1FontSizeProperty =
            DependencyProperty.Register("Header1FontSize", typeof(double), typeof(MarkdownViewer), new PropertyMetadata(40d));


        public static readonly DependencyProperty Header6ForegroundProperty =
            DependencyProperty.Register("Header6Foreground", typeof(Brush), typeof(MarkdownViewer), new PropertyMetadata(Black));


        public static readonly DependencyProperty Header5ForegroundProperty =
            DependencyProperty.Register("Header5Foreground", typeof(Brush), typeof(MarkdownViewer), new PropertyMetadata(Black));


        public static readonly DependencyProperty Header4ForegroundProperty =
            DependencyProperty.Register("Header4Foreground", typeof(Brush), typeof(MarkdownViewer), new PropertyMetadata(Black));


        public static readonly DependencyProperty Header3ForegroundProperty =
            DependencyProperty.Register("Header3Foreground", typeof(Brush), typeof(MarkdownViewer), new PropertyMetadata(Black));


        public static readonly DependencyProperty Header2ForegroundProperty =
            DependencyProperty.Register("Header2Foreground", typeof(Brush), typeof(MarkdownViewer), new PropertyMetadata(Black));


        public static readonly DependencyProperty Header1ForegroundProperty =
            DependencyProperty.Register("Header1Foreground", typeof(Brush), typeof(MarkdownViewer), new PropertyMetadata(Black));



        public static readonly DependencyProperty LinkColorProperty =
            DependencyProperty.Register("LinkColor", typeof(Brush), typeof(MarkdownViewer), new PropertyMetadata(Black));

        public static readonly DependencyProperty AreLinksUnderlinedProperty =
            DependencyProperty.Register("AreLinksUnderlined", typeof(bool), typeof(MarkdownViewer), new PropertyMetadata(false));

        static MarkdownViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MarkdownViewer), new FrameworkPropertyMetadata(typeof(MarkdownViewer)));
        }


        private static Brush Transparent => new SolidColorBrush(Colors.Transparent);
        private static Brush Black => new SolidColorBrush(Colors.Black);
        public MarkdownViewer()
        {
        }

        public string ParseMarkdown(string text)
        {
            if(text == null || text.Length == 0)
            {
                return string.Empty;
            }

            Text = null;
            Inlines.Clear();

            string finalRawContent = string.Empty;
            List<Inline> inlines = new List<Inline>();

            List<string> blocks = [.. text.Split('\n')];
            bool previousWasBreakLine = false;
            foreach (string block in blocks)
            {
                string blockTrimmed = block.Trim();
                double fontSize = FontSize;
                Brush foreground = Foreground;
                if (blockTrimmed.Length == 0) 
                {
                    if(!previousWasBreakLine) // make sure there are never 2 \n in a row
                    {
                        inlines.Add(new LineBreak());
                        previousWasBreakLine = true;
                        finalRawContent += "\n";
                    }
                    continue;
                }
                previousWasBreakLine = false;

                if (blockTrimmed.StartsWith(MarkdownTitleToken))
                {
                    (blockTrimmed, int titleTokenCount) = ParseHeader(blockTrimmed);

                    if(titleTokenCount > 0)
                    {
                        switch(titleTokenCount)
                        {
                            case 1:
                                foreground = Header1Foreground;
                                fontSize = Header1FontSize;
                                break;
                            case 2:
                                foreground = Header2Foreground;
                                fontSize = Header2FontSize;
                                break;
                            case 3:
                                foreground = Header3Foreground;
                                fontSize = Header3FontSize;
                                break;
                            case 4:
                                foreground = Header4Foreground;
                                fontSize = Header4FontSize;
                                break;
                            case 5:
                                foreground = Header5Foreground;
                                fontSize = Header5FontSize;
                                break;
                            case 6:
                                foreground = Header6Foreground;
                                fontSize = Header6FontSize;
                                break;
                        }
                    }
                }
                
                Regex regex = new(LinkRegex);
                List<LinkModel> links = [];
                foreach(Match linkMatch in regex.Matches(blockTrimmed).Cast<Match>())
                {
                    links.Add(new()
                    {
                        Placeholder = linkMatch.Groups["placeholder"].Value,
                        Url = linkMatch.Groups["url"].Value,
                        StartIndexInBlock = linkMatch.Index,
                        EndIndexInBlock = linkMatch.Index + linkMatch.Value.Length,
                    });
                }

                List<char> characters = [.. blockTrimmed.ToCharArray()];
                string runText = string.Empty;
                int linkIndex = 0;
                double inactiveAlphaPercentage = 0.9;
                Color linkColor = ((SolidColorBrush)LinkColor).Color;
                linkColor.A = (byte)(linkColor.A * inactiveAlphaPercentage);
                SolidColorBrush linkForeground = new(linkColor);
                for(int i = 0; i < characters.Count; ++i)
                {
                    if(linkIndex < links.Count && i == links[linkIndex].StartIndexInBlock)
                    {
                        inlines.Add(DefaultRun(runText, fontSize, foreground));
                        finalRawContent += runText;
                        finalRawContent += links[linkIndex].Placeholder;
                        Hyperlink? linkInline = links[linkIndex].GetHyperLink(linkForeground, fontSize, AreLinksUnderlined);
                        if (linkInline != null)
                        {
                            linkInline.Focusable = true;
                            linkInline.IsEnabled = true;
                            linkInline.ToolTip = links[linkIndex].Url;
                            // animated the opacity of the link by animating the alpha channel of the foreground
                            linkInline.MouseEnter += (sender, e) => AnimateAlpha((SolidColorBrush)((Hyperlink)sender).Foreground, 1.0);
                            linkInline.MouseLeave += (sender, e) => AnimateAlpha((SolidColorBrush)((Hyperlink)sender).Foreground, inactiveAlphaPercentage);
                            inlines.Add(linkInline);
                        }
                        runText = string.Empty;
                        i = links[linkIndex].EndIndexInBlock + 1;
                        linkIndex++;
                        continue;
                    }
                    runText += characters[i];
                }

                if(runText != string.Empty)
                {
                    inlines.Add(DefaultRun(runText, fontSize, foreground));
                    finalRawContent += runText;
                }
                inlines.Add(new LineBreak());
                finalRawContent += "\n";
            }
            Inlines.AddRange(inlines);
            return finalRawContent;
        }

        public void MeasureDesiredSize(Size availableSize)
        {
            this.Measure(availableSize);
        }

        private (string, int) ParseHeader(string block)
        {
            List<char> characters = [.. block.ToCharArray()];

            int titleTokenCount = 1; // the first char is the token
            int index;
            for (index = 1; index < characters.Count; index++)
            {
                if (characters[index] == MarkdownTitleToken && titleTokenCount < 6)
                {
                    titleTokenCount++;
                    continue;
                }

                if (characters[index] == ' ')
                {
                    index++;
                    break;
                }
                else
                {
                    // not valid title there is no space between token and text => ##title
                    index = characters.Count;
                }
            }

            // the loop stopped because it has been parsed
            if (index < characters.Count)
            {
                return (block[index..], titleTokenCount);
            }
            return (block, 0);
        }

        private Run DefaultRun(string text, double fontSize, Brush foreground) => new Run()
        {
            Text = text,
            Background = Transparent,
            FontSize = fontSize,
            Foreground = foreground,
        };

        private static void AnimateAlpha(SolidColorBrush brush, double targetAlpha)
        {
            ColorAnimation alphaAnimation = new ColorAnimation
            {
                To = Color.FromArgb((byte)(targetAlpha * 255), brush.Color.R, brush.Color.G, brush.Color.B),
                Duration = TimeSpan.FromSeconds(0.2),
                FillBehavior = FillBehavior.HoldEnd
            };

            (brush).BeginAnimation(SolidColorBrush.ColorProperty, alphaAnimation);
        }

        private static void AnimateOpacity(UIElement element, double targetOpacity)
        {
            DoubleAnimation opacityAnimation = new DoubleAnimation
            {
                To = targetOpacity,
                Duration = TimeSpan.FromSeconds(0.2), // Set your desired animation duration
                FillBehavior = FillBehavior.HoldEnd
            };

            element.BeginAnimation(UIElement.OpacityProperty, opacityAnimation);
        }
    }
}