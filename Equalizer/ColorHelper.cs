using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Equalizer
{
    public static class ColorHelper
    {
        public static SolidColorBrush AdjustHue(SolidColorBrush startBrush, double hueIndex)
        {
            // Extract the color from the SolidColorBrush
            Color startColor = startBrush.Color;

            // Convert Color to HSV for easier manipulation
            double hue, saturation, value;
            ColorToHSV(startColor, out hue, out saturation, out value);

            // Adjust hue
            hue = (hue + hueIndex) % 360;

            // Convert back to RGB
            Color newColor = ColorFromHSV(hue, saturation, value);

            // Create a new SolidColorBrush with the adjusted color
            return new SolidColorBrush(newColor);
        }

        private static void ColorToHSV(Color color, out double hue, out double saturation, out double value)
        {
            double r = color.R / 255.0;
            double g = color.G / 255.0;
            double b = color.B / 255.0;

            double min = Math.Min(Math.Min(r, g), b);
            double max = Math.Max(Math.Max(r, g), b);
            double delta = max - min;

            // Calculate hue
            if (delta == 0)
                hue = 0;
            else if (max == r)
                hue = (60 * ((g - b) / delta) + 360) % 360;
            else if (max == g)
                hue = (60 * ((b - r) / delta) + 120) % 360;
            else
                hue = (60 * ((r - g) / delta) + 240) % 360;

            // Calculate saturation
            if (max == 0)
                saturation = 0;
            else
                saturation = (delta / max) * 100;

            // Calculate value
            value = max * 100;
        }

        private static Color ColorFromHSV(double hue, double saturation, double value)
        {
            double chroma = (saturation / 100) * (value / 100);
            double huePrime = hue / 60;
            double x = chroma * (1 - Math.Abs(huePrime % 2 - 1));
            double m = (value / 100) - chroma;

            double r, g, b;
            if (0 <= huePrime && huePrime < 1)
            {
                r = chroma;
                g = x;
                b = 0;
            }
            else if (1 <= huePrime && huePrime < 2)
            {
                r = x;
                g = chroma;
                b = 0;
            }
            else if (2 <= huePrime && huePrime < 3)
            {
                r = 0;
                g = chroma;
                b = x;
            }
            else if (3 <= huePrime && huePrime < 4)
            {
                r = 0;
                g = x;
                b = chroma;
            }
            else if (4 <= huePrime && huePrime < 5)
            {
                r = x;
                g = 0;
                b = chroma;
            }
            else // 5 <= huePrime && huePrime < 6
            {
                r = chroma;
                g = 0;
                b = x;
            }

            r = (r + m) * 255;
            g = (g + m) * 255;
            b = (b + m) * 255;

            return Color.FromRgb((byte)r, (byte)g, (byte)b);
        }
    }

}
