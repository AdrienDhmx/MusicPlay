using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using MusicPlay.Database.Helpers;

namespace MusicPlayUI.MVVM.Views.Templates
{
    /// <summary>
    /// Interaction logic for TextPreviewTemplate.xaml
    /// </summary>
    public partial class TextPreviewTemplate : UserControl
    {
        private const int _animationDurationInMs = 400;
        private bool _parsed = false;
        private Size _markdownDesiredSize = Size.Empty;

        public TextPreviewTemplate()
        {
            InitializeComponent();

            Loaded += OnLoaded;
            SizeChanged += OnLoaded;
            Unloaded += TextPreviewTemplate_Unloaded;
        }

        private void TextPreviewTemplate_Unloaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            SizeChanged -= OnLoaded;
            Unloaded -= TextPreviewTemplate_Unloaded;
        }

        protected override void OnChildDesiredSizeChanged(UIElement child)
        {
            base.OnChildDesiredSizeChanged(child);
            markdownViewer.MeasureDesiredSize(new Size(this.ActualWidth, double.PositiveInfinity));
            _markdownDesiredSize = markdownViewer.DesiredSize;
            
            Init();
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(d is TextPreviewTemplate textPreview && e.NewValue is string newText)
            {
                textPreview._parsed = false;
                textPreview.Init();
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            _parsed = false;
            Init();
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set {   SetValue(TextProperty, value); }
        }

        public double PreviewHeight
        {
            get { return (double)GetValue(PreviewHeightProperty); }
            set { SetValue(PreviewHeightProperty, value); }
        }

        public bool IsExtended
        {
            get { return (bool)GetValue(IsExtendedProperty); }
            set { SetValue(IsExtendedProperty, value); }
        }

        public bool CanExtend
        {
            get { return (bool)GetValue(CanExtendProperty); }
            set { SetValue(CanExtendProperty, value); }
        }

        public bool PreviewFirstParagraph
        {
            get { return (bool)GetValue(PreviewFirstParagraphProperty); }
            set { SetValue(PreviewFirstParagraphProperty, value); }
        }

        public static readonly DependencyProperty PreviewFirstParagraphProperty =
            DependencyProperty.Register("PreviewFirstParagraph", typeof(bool), typeof(TextPreviewTemplate), new PropertyMetadata(false));

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(TextPreviewTemplate), new PropertyMetadata("", OnTextChanged));

        public static readonly DependencyProperty PreviewHeightProperty =
            DependencyProperty.Register("PreviewHeight", typeof(double), typeof(TextPreviewTemplate), new PropertyMetadata(94d));

        public static readonly DependencyProperty IsExtendedProperty =
            DependencyProperty.Register("IsExtended", typeof(bool), typeof(TextPreviewTemplate), new PropertyMetadata(false));

        public static readonly DependencyProperty CanExtendProperty =
            DependencyProperty.Register("CanExtend", typeof(bool), typeof(TextPreviewTemplate), new PropertyMetadata(true));


        private void Init()
        {
            if (Text.IsNotNullOrWhiteSpace() && !_parsed)
            {
                markdownViewer.ParseMarkdown(Text);
                _parsed = true;
            }

            markdownViewer.MeasureDesiredSize(new Size(this.ActualWidth, double.PositiveInfinity));
            _markdownDesiredSize = markdownViewer.DesiredSize;
            CanExtend = _markdownDesiredSize.Height > PreviewHeight + 0.6 * PreviewHeight; // add a margin so that the extend option is not just for a single line
            if (!CanExtend) // text is too short to be extended
            {
                TextContainer.Height = _markdownDesiredSize.Height;
                CacheBorder.Height = _markdownDesiredSize.Height;

                CacheBorder.Visibility = Visibility.Collapsed;
                ExtendButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                if(PreviewFirstParagraph && Text.Split("\n").Length > 1)
                {
                    string firstParagraph = Text.Split("\n")[0];
                    Size firstParagraphSize = MeasureText(firstParagraph);
                    if(firstParagraphSize.Height < PreviewHeight + 0.6 * PreviewHeight)
                    {
                        PreviewHeight = firstParagraphSize.Height;
                    }
                }

                TextContainer.Height = PreviewHeight;
                CacheBorder.Height = PreviewHeight;

                CacheBorder.IsHitTestVisible = false;
                CacheBorder.Visibility = Visibility.Visible;
                ExtendButton.Visibility = Visibility.Visible;

                ExtendButton.PreviewMouseLeftButtonUp -= OnCacheBorderClick;
                ExtendButton.PreviewMouseLeftButtonUp += OnCacheBorderClick;
            }
        }

        private Size MeasureText(string text)
        {
            TextBlock textBlock = new()
            {
                Text = text,
                TextWrapping = TextWrapping.Wrap,
                Width = this.ActualWidth,
                FontSize = markdownViewer.FontSize + 0.5, // take into account possible headers
                FontWeight = markdownViewer.FontWeight,
            };


            textBlock.Measure(new Size(this.ActualWidth, double.PositiveInfinity));
            return textBlock.DesiredSize;
        }

        private void OnCacheBorderClick(object sender, MouseButtonEventArgs e)
        {
            IsExtended = !IsExtended;

            if (IsExtended)
            {
                DoubleAnimation animation = new DoubleAnimation
                {
                    From = TextContainer.ActualHeight,
                    To = _markdownDesiredSize.Height,
                    Duration = TimeSpan.FromMilliseconds(_animationDurationInMs)
                };

                animation.Completed += (sender, e) =>
                {
                    CacheBorder.Opacity = 0;
                };

                Storyboard storyboard = new Storyboard();
                storyboard.Children.Add(animation);
                Storyboard.SetTarget(animation, TextContainer);
                Storyboard.SetTargetProperty(animation, new PropertyPath(Border.HeightProperty));
                storyboard.Begin();
            }
            else
            {
                DoubleAnimation animation = new DoubleAnimation
                {
                    From = TextContainer.ActualHeight,
                    To = PreviewHeight,
                    Duration = TimeSpan.FromMilliseconds(_animationDurationInMs)
                };

                Storyboard storyboard = new Storyboard();
                storyboard.Children.Add(animation);
                Storyboard.SetTarget(animation, TextContainer);
                Storyboard.SetTargetProperty(animation, new PropertyPath(Border.HeightProperty));

                CacheBorder.Opacity = 1;
                storyboard.Begin();
            }
        }
    }
}
