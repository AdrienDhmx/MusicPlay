using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Net.Http;
using System.Security.Policy;
using MusicFilesProcessor;
using DynamicScrollViewer;
using MusicPlayUI.Animations;
using MusicFilesProcessor.Helpers;

namespace MusicPlayUI.Controls
{
    public class AsyncImage : Border, IIsInViewport
    {
        public static Dictionary<string, BitmapImage> ImageCache { get; private set; } = new Dictionary<string, BitmapImage>();



        public static readonly DependencyProperty ImagePathProperty =
            DependencyProperty.Register(
                nameof(ImagePath), typeof(string), typeof(AsyncImage),
                new PropertyMetadata(async (o, e) =>
                {
                    if (e.NewValue is string imagePath)
                        await ((AsyncImage)o).LoadImageAsync(imagePath);
                    else if (o is AsyncImage asyncImage)
                        asyncImage.SetPlaceholder(asyncImage.Placeholder, false);
                }));

        public string ImagePath
        {
            get { return (string)GetValue(ImagePathProperty); }
            set { SetValue(ImagePathProperty, value); }
        }

        public static readonly DependencyProperty IsInViewportProperty =
            DependencyProperty.Register("IsInViewport", typeof(bool), typeof(AsyncImage), new PropertyMetadata(true, OnIsInViewportChanged));

        public FrameworkElement Placeholder
        {
            get { return (FrameworkElement)GetValue(PlaceholderProperty); }
            set { SetValue(PlaceholderProperty, value); }
        }

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register("Placeholder", typeof(FrameworkElement), typeof(AsyncImage), new PropertyMetadata(null, OnPlaceholderChanged));

        public static readonly DependencyProperty PlaceholderAnimationEnabledProperty =
            DependencyProperty.Register("PlaceholderAnimationEnabled", typeof(bool), typeof(AsyncImage), new PropertyMetadata(true));

        public bool CacheImage
        {
            get { return (bool)GetValue(CacheImageProperty); }
            set { SetValue(CacheImageProperty, value); }
        }

        public static readonly DependencyProperty CacheImageProperty =
            DependencyProperty.Register("CacheImage", typeof(bool), typeof(AsyncImage), new PropertyMetadata(false));

        public bool IsInViewport
        {
            get { return (bool)GetValue(IsInViewportProperty); }
            set { SetValue(IsInViewportProperty, value); }
        }

        public bool PlaceholderAnimationEnabled
        {
            get { return (bool)GetValue(PlaceholderAnimationEnabledProperty); }
            set { SetValue(PlaceholderAnimationEnabledProperty, value); }
        }

        private static async void OnIsInViewportChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AsyncImage asyncImage)
            {
                if (e.NewValue is bool value && value)
                {
                    await asyncImage.LoadImageAsync(asyncImage.ImagePath);
                }
                else
                {
                    asyncImage.Image.Source = null;
                }
            }
        }

        private static async void OnPlaceholderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is AsyncImage asyncImage && e.NewValue is FrameworkElement placeholder)
            {
                asyncImage.ClearAnimation();
                // will set the placholder it there is no image
                await asyncImage.LoadImageAsync(asyncImage.ImagePath);
            }
        }

        private bool sizeWasNotComputed = false;
        private Image Image { get; set; } = new();
        public AsyncImage()
        {
        }

        protected async override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            if(sizeWasNotComputed)
            {
                await LoadImageAsync(ImagePath);
            }
        }

        private void SetImage(BitmapImage source)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                ClearAnimation();
                double height = ActualHeight == 0 ? Height : ActualHeight;
                double width = ActualWidth == 0 ? Width : ActualWidth;

                if(height == 0 || double.IsNaN(height) || width == 0 || double.IsNaN(width))
                {
                    sizeWasNotComputed = true;
                }    

                Image.Height = height;
                Image.Width = width;
                Image.Clip = new RectangleGeometry(new Rect(0, 0, width, height), CornerRadius.TopLeft, CornerRadius.BottomRight);
                Image.Source = source;
                Child = Image;
            });
        }

        private void ClearAnimation()
        {
            BeginAnimation(OpacityProperty, null);
        }

        private void SetPlaceholder(FrameworkElement placeholder, bool animate = true, bool fromSizeChanged = false)
        {
            if (placeholder == null)
                return;

            double height = placeholder.Height;
            double width = placeholder.Width;

            if(fromSizeChanged)
            {
                placeholder.SizeChanged -= (e, o) => SetPlaceholder(placeholder, animate, true);
            }

            if (height == 0 || double.IsNaN(height) || width == 0 || double.IsNaN(width))
            {
                if(!fromSizeChanged)
                {
                    placeholder.SizeChanged += (e, o) => SetPlaceholder(placeholder, animate, true);
                }

                height = Height;
                width = Width;
            }

            Child = placeholder;
            Child.Clip = new RectangleGeometry(new Rect(0, 0, width, height), CornerRadius.TopLeft, CornerRadius.BottomRight);
            if (PlaceholderAnimationEnabled && animate)
            {
                DoubleAnimation animation = new DoubleAnimation
                {
                    From = 0.4,
                    To = 0.8,
                    AutoReverse = true,
                    RepeatBehavior = RepeatBehavior.Forever,
                    Duration = new Duration(TimeSpan.FromSeconds(2)),
                    EasingFunction = new EaseInOutBack(),
                };

                BeginAnimation(OpacityProperty, animation);
            }
        }

        public async Task LoadImageAsync(string imagePath)
        {
            if (Visibility != Visibility.Visible)
                return;

            if (string.IsNullOrEmpty(imagePath) || !IsInViewport)
            {
                SetPlaceholder(Placeholder, false);
                return;
            }

            if (ImageCache.TryGetValue(imagePath, out BitmapImage value))
            {
                SetImage(value);
                return;
            }
            else if (imagePath.StartsWith("Resources\\"))
            {
                using var stream = Application.GetResourceStream(new Uri("/MusicPlay;Component/" + imagePath, UriKind.Relative)).Stream;
                var bi = new BitmapImage();
                bi.BeginInit();
                bi.CacheOption = BitmapCacheOption.OnDemand;
                bi.StreamSource = stream;
                bi.EndInit();
                bi.Freeze();
                ImageCache.TryAdd(imagePath, bi);
                SetImage(bi);
                return;
            }
            else if(imagePath.StartsWith("https:\\"))
            {
                SetPlaceholder(Placeholder);
                LoadRemoteImageAsync(imagePath);
                return;
            }
            else if (!imagePath.ValidFilePath())
            {
                SetPlaceholder(Placeholder, false);
                return;
            }

            BitmapImage image = await Task.Run(() =>
            {
                using FileStream fileStream = File.OpenRead(imagePath);
                var biImg = new BitmapImage();
                biImg.BeginInit();
                biImg.CacheOption = BitmapCacheOption.OnLoad;
                biImg.StreamSource = fileStream;
                biImg.EndInit();
                biImg.Freeze();
                return biImg;
            });
            if(CacheImage)
                ImageCache.TryAdd(imagePath, image);
            SetImage(image);
        }

        private async void LoadRemoteImageAsync(string url)
        {
            if (ImageCache.TryGetValue(url, out BitmapImage value))
            {
                SetImage(value);
            }
            else
            {
                BitmapImage bitmapImage = await DownloadImageAsync(url);
                ImageCache[url] = bitmapImage;
                SetImage(bitmapImage);
            }
        }

        private async Task<BitmapImage> DownloadImageAsync(string url)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        using (Stream stream = await response.Content.ReadAsStreamAsync())
                        {
                            BitmapImage bitmapImage = new BitmapImage();
                            bitmapImage.BeginInit();
                            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                            bitmapImage.StreamSource = stream;
                            bitmapImage.EndInit();
                            bitmapImage.Freeze(); // Freezing the image makes it accessible on non-UI threads
                            return bitmapImage;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading image: {ex.Message}");
            }

            return null;
        }

        public static void LoadResourceImage(string resource)
        {
            if (resource.StartsWith("Resources\\"))
            {
                using (var stream = Application.GetResourceStream(new Uri("/MusicPlay;Component/" + resource, UriKind.Relative)).Stream)
                {
                    var bi = new BitmapImage();
                    bi.BeginInit();
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.StreamSource = stream;
                    bi.EndInit();
                    bi.Freeze();
                    ImageCache[resource] = bi;
                }
            }
        }
    }
}
