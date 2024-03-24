using ManagedBass;
using MusicPlay.Database.Models;

namespace AudioHandler.Models
{
    public class EQBand : BaseModel, ICloneable
    {
        private const double defaultCenterFrequency = 1000.0;
        private const double defaultBandWidth = 1;
        private const double defaultGain = 0;
        private const FXChannelFlags defaultChannels = FXChannelFlags.All;

        public const int MinFrequency = 20;
        public const int MaxFrequency = 20000;

        public double HalfBandWidth => BandWidth / 2;
        public double LowerPointHz => DownOctave(HalfBandWidth);
        public double UpperPointHz => UpOctave(HalfBandWidth);
        public double BandWidthHz => UpperPointHz - LowerPointHz;
        public double Q => CenterFrequency / BandWidthHz;

        public int EQPresetId { get; set; }

        private double UpOctave(double octave)
        {
            return CenterFrequency * Math.Pow(2, octave);
        }

        private double DownOctave(double octave)
        {
            return CenterFrequency / Math.Pow(2, octave);
        }

        public string CenterFrequencyName => $"{Math.Round(CenterFrequency)}Hz";

        public string BandWidthName => $"{Math.Round(BandWidth, 1)} Octave";

        public string GainName => $"{Math.Round(Gain, 1)}Db";

        private int _band;
        public int Band
        {
            get => _band;
            set
            {
                SetField(ref _band, value);
            }
        }

        private FXChannelFlags _channels = defaultChannels;
        public FXChannelFlags Channels 
        {
            get => _channels;
            set
            {
                SetField(ref _channels, value);
            }
        }

        private double _centerFrequency = defaultCenterFrequency;
        public double CenterFrequency 
        {
            get => _centerFrequency;
            set
            {
                if (value > MaxFrequency) value = MaxFrequency;
                else if(value < MinFrequency) value = MinFrequency;

                SetField(ref _centerFrequency, value);
                OnPropertyChanged(nameof(CenterFrequencyName));
            }
        }

        private double _bandWidth = defaultBandWidth;
        public double BandWidth 
        {
            get => _bandWidth;
            set
            {
                SetField(ref _bandWidth, value);
                OnPropertyChanged(nameof(BandWidthName));
            }
        }

        private double _gain = defaultGain;
        public double Gain 
        {
            get => _gain; 
            set
            {
                SetField(ref _gain, value);
                OnPropertyChanged(nameof(GainName));
            }
        }

        public EffectType FXType => EffectType.PeakEQ;

        public EQBand(int band, double centerFrequency, double bandWidth, double gain)
        {
            Band = band;
            CenterFrequency = centerFrequency;
            BandWidth = bandWidth;
            Gain = gain;
        }

        public EQBand(int band, double bandWidth)
        {
            Band = band;
            BandWidth = bandWidth;
        }

        public EQBand()
        {
                
        }

        public virtual object Clone()
        {
            return new EQBand()
            {
                CenterFrequency = this.CenterFrequency,
                BandWidth = this.BandWidth,
                Gain = this.Gain,
                Band = this.Band,
                Channels = this.Channels,
                Id = this.Id,
            };
        }
    }
}
