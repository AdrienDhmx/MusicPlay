using Path = System.IO.Path;
using MusicFilesProcessor.Helpers;
using System.Drawing;
using System.Windows.Media;
using System.IO;
using Image = SixLabors.ImageSharp.Image;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using Rectangle = System.Drawing.Rectangle;
using System.Drawing.Drawing2D;
using SixLabors.ImageSharp.Formats.Png;
using System.Windows;
using MessageControl;
using System.Drawing.Imaging;
using MusicPlay.Database.Helpers;

namespace MusicFilesProcessor
{
    public static class ImageProcessor
    {
        public static bool Merge4ImagesInOne(string image1, string image2, string image3, string image4, string path)
        {
            bool result = true;
            int width = 600;
            int height = 600;
            if (!string.IsNullOrWhiteSpace(image1) && File.Exists(image1) &&
                !string.IsNullOrWhiteSpace(image2) && File.Exists(image2) &&
                !string.IsNullOrWhiteSpace(image3) && File.Exists(image3) &&
                !string.IsNullOrWhiteSpace(image4) && File.Exists(image4))
            {
                using (Bitmap bmp1 = ResizeImage(image1, 300, 300))
                using (Bitmap bmp2 = ResizeImage(image2, 300, 300))
                using (Bitmap bmp3 = ResizeImage(image3, 300, 300))
                using (Bitmap bmp4 = ResizeImage(image4, 300, 300))
                {
                    using(Bitmap dest = new(width, height))
                    using(Graphics g = Graphics.FromImage(dest))
                    {
                        g.DrawImage(bmp1, 0, 0, 300, 300);
                        g.DrawImage(bmp2, 300, 0, 300, 300);
                        g.DrawImage(bmp3, 0, 300, 300, 300);
                        g.DrawImage(bmp4, 300, 300, 300, 300);

                        dest.Save(path);
                    }
                }   
                ImageHelper.SaveFileToNewPath(path, path);
            }
            else
            {
                result = false;
            }

            return result;
        }

        private static Bitmap ResizeImage(string image, int width, int height)
        {
            Bitmap bmp = new(image);

            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.Default;
                graphics.InterpolationMode = InterpolationMode.Default;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(bmp, destRect, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            bmp.Dispose();
            return destImage;
        }

        public static string GetBlurredImage(this string imagePath, int blur)
        {
            // get the thumbnail size
            string mImagePath = imagePath.GetModifiedCoverPath(false);

            if (!ImageHelper.ValidFilePath(mImagePath))
            {
                // try with the medium size
                mImagePath = imagePath.GetModifiedCoverPath(true);

                if (!ImageHelper.ValidFilePath(mImagePath))
                {
                    mImagePath = imagePath;
                    if (!ImageHelper.ValidFilePath(mImagePath))
                        return "";
                }
            }

            string blurredImage = mImagePath.CreateBlurredImagePath();
            if (!File.Exists(blurredImage) && !string.IsNullOrWhiteSpace(blurredImage))
            {
                using Bitmap bmp = new(mImagePath);

                GaussianBlur gaussianBlur = new(bmp);
                Bitmap blurredBmp = gaussianBlur.Process(blur);

                blurredBmp.Save(blurredImage);

                blurredBmp.Dispose();
                bmp.Dispose();
            }
            return blurredImage;
        }

        private static string CreateBlurredImagePath(this string originalPath)
        {
            return DirectoryHelper.GetDirectory(originalPath) + Path.GetFileNameWithoutExtension(originalPath) + "_blurred.png";
        }

        public static SolidColorBrush CalculateMeanColor(this string imgPath)
        {
            if (!ImageHelper.ValidFilePath(imgPath)) return new();

            using Bitmap bmp = new(imgPath);

            var width = bmp.Width;
            var height = bmp.Height;
            int red = 0;
            int green = 0;
            int blue = 0;
            long[] totals = [0, 0, 0];
            int bppModifier = bmp.PixelFormat == PixelFormat.Format24bppRgb ? 3 : 4;

            BitmapData srcData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadOnly, bmp.PixelFormat);
            int stride = srcData.Stride;
            IntPtr Scan0 = srcData.Scan0;

            unsafe
            {
                byte* p = (byte*)(void*)Scan0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int idx = (y * stride) + x * bppModifier;
                        red = p[idx + 2];
                        green = p[idx + 1];
                        blue = p[idx];

                        totals[2] += red;
                        totals[1] += green;
                        totals[0] += blue;

                    }
                }
            }

            int count = width * height;
            int avgR = (int)(totals[2] / count);
            int avgG = (int)(totals[1] / count);
            int avgB = (int)(totals[0] / count);

            bmp.UnlockBits(srcData);
            bmp.Dispose();
            return new(System.Windows.Media.Color.FromRgb((byte)avgR, (byte)avgG, (byte)avgB));
        }

        public static SolidColorBrush GetEmphasizedColor(this SolidColorBrush color, double emphasizeBy = 1.05d, int maxChannelValue = 230)
        {
            int red;
            int green;
            int blue;

            // color dif with the highest value increased
            (double rRatio, double gRatio, double bRatio) = CalculateRatio(color.Color.R, color.Color.G, color.Color.B, emphasizeBy);

            double brightness = color.CalculateBrightness();

            // change brightness to have a color that's either brighter or darker than the original (it needs to be visible)
            red = (int)(rRatio * color.Color.R + (0.7 - brightness) * maxChannelValue * (1.05 - brightness));
            green = (int)(gRatio * color.Color.G + (0.7 - brightness) * maxChannelValue * (1.05 - brightness));
            blue = (int)(bRatio * color.Color.B + (0.7 - brightness) * maxChannelValue * (1.05 - brightness));

            if (brightness <= 0.8)
            {
                // increase the brightness
                brightness += 0.85 - brightness;
            }

            // accentuate or minimize rbg values while making sure they stay between valid values (0 - 255)
            red = FormatRGB(red, 1.1, (int)(maxChannelValue * brightness * rRatio));
            green = FormatRGB(green, 1.1, (int)(maxChannelValue * brightness * gRatio));
            blue = FormatRGB(blue, 1.1, (int)(maxChannelValue * brightness * bRatio));

            return new(System.Windows.Media.Color.FromRgb((byte)red, (byte)green, (byte)blue));
        }

        public static double CalculateBrightness(this SolidColorBrush color)
        {
            return (color.Color.R + color.Color.G + color.Color.B) / 765d;
        }

        public static bool IsGrey(this SolidColorBrush color, int margin = 40)
        {
            int r = color.Color.R;
            int g = color.Color.G;
            int b = color.Color.B;

            return Math.Abs(r - g) <= margin && Math.Abs(g - b) <= margin && Math.Abs(b - r) <= margin;
        }


        private static int FormatRGB(int value, double ratio, int maxvalue = 210, int minvalue = 20)
        {
            int max = (int)(ratio * maxvalue);
            int min = (int)(ratio * minvalue);

            value = (int)(value * ratio);

            int output = value > max ? max : value;
            return output < min ? min : output;
        }

        private static (double, double, double) CalculateRatio(double red, double green, double blue,double maxRatio = 1.05d)
        {
            double rRatio = maxRatio;
            double gRatio = maxRatio;
            double bRatio = maxRatio;

            if (red == green && red == blue) return (rRatio, gRatio, bRatio);

            if (red > green)
            {
                if (red > blue) // red higher
                {
                    rRatio = maxRatio;
                    gRatio = green / red;
                    bRatio = blue / red;
                    
                    if(gRatio > bRatio)
                    {
                        (rRatio, gRatio, bRatio) = AccentuateRatio(rRatio, gRatio, bRatio);
                    }
                    else
                    {
                        (rRatio, bRatio, gRatio) = AccentuateRatio(rRatio, bRatio, gRatio);
                    }
                }
                else
                {
                    if (green > blue) // green higher
                    {
                        gRatio = maxRatio;
                        rRatio = red / green;
                        bRatio = blue / green;

                        (rRatio, gRatio, bRatio) = AccentuateRatio(rRatio, gRatio, bRatio);
                    }
                    else // blue higher
                    {
                        bRatio = maxRatio;
                        rRatio = red / blue;
                        gRatio = green / blue;

                        (rRatio, bRatio, gRatio) = AccentuateRatio(rRatio, bRatio, gRatio);
                    }
                }
            }
            else
            {
                if (green > blue) // green higher
                {
                    gRatio = maxRatio;
                    rRatio = red / green;
                    bRatio = blue / green;

                    if (bRatio > rRatio)
                    {
                        (gRatio, bRatio, rRatio) = AccentuateRatio(gRatio, bRatio, rRatio);
                    }
                    else
                    {
                        (gRatio, rRatio, bRatio) = AccentuateRatio(gRatio, rRatio, bRatio);
                    }
                }
                else // blue higher
                {
                    bRatio = maxRatio;
                    rRatio = red / blue;
                    gRatio = green / blue;

                    (bRatio, gRatio, rRatio) = AccentuateRatio(bRatio, gRatio, rRatio);

                }
            }

            return (rRatio, gRatio, bRatio);
        }

        private static (double, double, double) AccentuateRatio(double maxRatio, double midRatio, double minRatio)
        {
            double newMid = midRatio;
            // change dif only if they are too close but not too close (if color is grey its stays grey)
            if(maxRatio - midRatio > 0.04 && maxRatio - midRatio < 0.2 && midRatio > 0.1)
            {
                newMid -= 0.05;
            }

            if(midRatio - minRatio > 0.04 && midRatio - minRatio < 0.2 && minRatio > 0.1) 
            {
                minRatio -= 0.05;
            }
            return (maxRatio, newMid, minRatio);
        }

        /// <summary>
        /// Format an image by resizing it and setting the bit-depth to 24 (RGB).
        /// </summary>
        /// <param name="imgPath"></param>
        /// <param name="newPath"></param>
        /// <param name="treshold"> The physical file size in bytes at which the image gets compressed (default = 512 000 bytes = 500 kb) </param>
        public static void FormatImage(string imgPath, string mediumSized, string thumbnailSized, long treshold = 512000)
        {
            try
            {
                const int mediumSizeThreshold = 400;
                const int thumbnailSizeThreshold = 200;
                PngEncoder pngEncoder = new() { CompressionLevel = PngCompressionLevel.NoCompression };

                FileInfo fileInfo = new(imgPath);
                if (fileInfo.Length > treshold)
                {
                    pngEncoder = new PngEncoder() // CompressionLevel prop is init only
                    {
                       CompressionLevel = PngCompressionLevel.DefaultCompression,
                    };
                }

                using (Image image = Image.Load(imgPath))
                {
                    if (image.Width > mediumSizeThreshold && image.Height > mediumSizeThreshold)
                    {
                        ResizeAndCompressImage(image, mediumSized, mediumSizeThreshold, pngEncoder);
                        if (image.Width > thumbnailSizeThreshold && image.Height > thumbnailSizeThreshold)
                            ResizeAndCompressImage(image, thumbnailSized, thumbnailSizeThreshold, pngEncoder);
                        else
                            image.Save(thumbnailSized);
                    }
                    else
                    {
                        image.Save(mediumSized);
                        image.Save(thumbnailSized);
                    }
                }
            }
            catch (Exception ex)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MessageHelper.PublishMessage(DefaultMessageFactory.CreateErrorMessage($"Error while compressing an image: {ex}"));
                });
            }
        }

        private static void ResizeImage(string image, string newPath, int desiredSize)
        {
            Bitmap bmp = new(image);

            (int width, int height) = GetNewSize(desiredSize, bmp.Width, bmp.Height);

            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.Default;
                graphics.InterpolationMode = InterpolationMode.Default;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(bmp, destRect, 0, 0, bmp.Width, bmp.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            destImage.Save(newPath, ImageFormat.Png);

            bmp.Dispose();
            destImage.Dispose();
        }

        private static void ResizeAndCompressImage(Image image, string newPath, int desiredSize, PngEncoder pngEncoder)
        {
            double heightWidthDif = image.Width - image.Height;
            double ratio;
            int width;
            int height;

            if(heightWidthDif == 0)
            {
                width = height = desiredSize;
            }
            else if(heightWidthDif > 0)
            {
                width = desiredSize;
                ratio = width / image.Width;
                height = (int)(image.Height * ratio);
            }
            else
            {
                height = desiredSize;
                ratio = height / image.Height;
                width = (int)(image.Width * ratio);
            }

            image.Mutate(x => x.Resize(width, height, KnownResamplers.Bicubic));

            image.Save(newPath, pngEncoder);
        }

        private static (int, int) GetNewSize(int desiredSize, int originalWidth, int originalHeight)
        {
            double heightWidthDif = originalWidth - originalHeight;
            double ratio;
            int width;
            int height;

            if (heightWidthDif == 0)
            {
                width = height = desiredSize;
            }
            else if (heightWidthDif > 0)
            {
                width = desiredSize;
                ratio = width / originalWidth;
                height = (int)(originalHeight * ratio);
            }
            else
            {
                height = desiredSize;
                ratio = height / originalHeight;
                width = (int)(originalWidth * ratio);
            }
            return (width, height);
        }

        public static ImageCodecInfo GetEncoder(string format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FilenameExtension.Contains(format, StringComparison.CurrentCultureIgnoreCase))
                {
                    return codec;
                }
            }
            return null;
        }
    }
}
