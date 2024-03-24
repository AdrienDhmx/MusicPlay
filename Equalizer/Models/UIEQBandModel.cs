using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using AudioHandler.Models;

namespace Equalizer.Models
{
    public class UIEQBandModel : EQBand
    {
        public SolidColorBrush Brush { get; }

        public string BandName => $"Band {Band + 1}";

        public UIEQBandModel(EQBand eQBand, SolidColorBrush brush)
        {
            Brush = brush;
            Id = eQBand.Id;
            Band = eQBand.Band;
            CenterFrequency = eQBand.CenterFrequency;
            BandWidth = eQBand.BandWidth;
            Gain = eQBand.Gain;
            Channels = eQBand.Channels;
        }

        public override object Clone()
        {
            return new UIEQBandModel((EQBand)base.Clone(), Brush);
        }
    }

    public static class UIEQBandModelExt
    {
        public static UIEQBandModel ToUIEQBand(this EQBand effect, SolidColorBrush baseColor)
        {
            SolidColorBrush brush = ColorHelper.AdjustHue(baseColor, (effect.Band + 1) * 25);
            return new UIEQBandModel(effect, brush);
        }

        public static List<UIEQBandModel> ToUIEQBand(this List<EQBand> effects, SolidColorBrush baseColor)
        {
            List<UIEQBandModel> bands = new();

            foreach (EQBand effect in effects)
            {
                bands.Add(effect.ToUIEQBand(baseColor));
            }

            return bands;
        }
    }
}
