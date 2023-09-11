using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Equalizer
{
    public static class DrawHelper
    {
        public static readonly SolidColorBrush Tranparent = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
        public static readonly double LineMinThickness = 2;
        public static readonly double BackgroundLineOpacity = 0.2;

        public static void DrawPoint(this Canvas canvas, Point point, double diameter, Brush fill, 
            MouseButtonEventHandler onLeftMouseDown, MouseButtonEventHandler onLeftMouseUp, 
            MouseButtonEventHandler onRightMouseDown, MouseButtonEventHandler onRightMouseUp,
            string name = "")
        {
            Ellipse ellipse = new()
            {
                Height = diameter,
                Width = diameter,
                Fill = fill,
                Stroke = fill,
                Name = name,
                Cursor = Cursors.Hand,
            };

            ellipse.MouseLeftButtonDown += onLeftMouseDown;
            ellipse.MouseLeftButtonUp += onLeftMouseUp;
            ellipse.MouseRightButtonDown += onRightMouseDown;
            ellipse.MouseRightButtonUp += onRightMouseUp;
            ellipse.OverridesDefaultStyle = true;

            Canvas.SetLeft(ellipse, point.X - ellipse.Width / 2);
            Canvas.SetBottom(ellipse, point.Y - ellipse.Width / 2);

            canvas.Children.Add(ellipse);
        }

        public static void DrawPoint(this Canvas canvas, Point point, double diameter, Brush fill)
        {
            Ellipse ellipse = new()
            {
                Height = diameter,
                Width = diameter,
                Fill = fill,
                Stroke = Tranparent,
            };

            Canvas.SetLeft(ellipse, point.X - ellipse.Width / 2);
            Canvas.SetBottom(ellipse, point.Y - ellipse.Width / 2);

            canvas.Children.Add(ellipse);
        }

        public static TextBlock CreateTextBlock(string text, int fontSize, Brush foreground, string name = "")
        {
            TextBlock textBlock = new()
            {
                Text = text,
                Foreground = foreground,
                FontSize = fontSize
            };

            if (!string.IsNullOrWhiteSpace(name))
            {
                textBlock.Name = name;
            }

            return textBlock;
        }

        public static void DrawText(this Canvas canvas, TextBlock textBlock, Point point, bool alignLeft = true, bool center = true)
        {
            if (center)
            {
                double textWidth = MeasureStringWidth(textBlock);
                if (alignLeft)
                    point.X -= textWidth / 2;
                else
                    point.X += textWidth / 2;
                point.Y -= textBlock.FontSize / 2;
            }

            if(alignLeft)
                Canvas.SetLeft(textBlock, point.X);
            else 
                Canvas.SetRight(textBlock, point.X);

            Canvas.SetBottom(textBlock, point.Y);

            canvas.Children.Add(textBlock);
        }


        public static void DrawRectangle(this Canvas canvas, double X1, double Y1, double width, double height, Brush color, double opacity = 0.2)
        {
            double smallSide = width >= height ? height : width;

            Rectangle rectangle = new Rectangle();
            rectangle.Width = width;
            rectangle.Height = height;
            rectangle.RadiusX = 1;
            rectangle.RadiusY = 1;

            rectangle.Stroke = color;
            rectangle.StrokeThickness = 1;
            rectangle.Fill = color;
            rectangle.Opacity = opacity;

            rectangle.IsHitTestVisible = false;

            Canvas.SetLeft(rectangle, X1 - smallSide / 2);
            Canvas.SetBottom(rectangle, Y1 - smallSide / 2);

            canvas.Children.Add(rectangle);
        }

        private static double MeasureStringWidth(TextBlock textBlock)
        {
            System.Drawing.Font drawingFont = new System.Drawing.Font(
                        textBlock.FontFamily.ToString(),
                        (float)textBlock.FontSize,
                        System.Drawing.FontStyle.Regular,
                        System.Drawing.GraphicsUnit.Pixel // You can adjust this based on your needs
                    );

            return GraphicsHelper.MeasureString(textBlock.Text, drawingFont).Width;
        }
    }
}
