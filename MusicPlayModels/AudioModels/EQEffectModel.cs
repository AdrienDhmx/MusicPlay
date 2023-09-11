using ManagedBass;
using MusicPlayModels;

namespace AudioHandler.Models
{
    public class EQEffectModel : BaseModel, ICloneable
    {
        private const double defaultCenterfrequency = 1000.0;
        private const double defaultBandWidth = 2.5;
        private const double defaultGain = 0;
        private const FXChannelFlags defaultChannels = FXChannelFlags.All;

        public const int MinFrequency = 20;
        public const int MaxFrequency = 16000;

        public double HalfBandWidth => BandWidth / 2;
        public double LowerPointHz => DownOctave(HalfBandWidth);
        public double UpperPointHz => UpOctave(HalfBandWidth);
        public double BandWidthHz => UpperPointHz - LowerPointHz;
        public double Q => CenterFrequency / BandWidthHz;

        private double UpOctave(double octave)
        {
            return CenterFrequency * Math.Pow(2, octave);
        }

        private double DownOctave(double octave)
        {
            return CenterFrequency / Math.Pow(2, octave);
        }

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

        private double _centerFrequency = defaultCenterfrequency;
        public double CenterFrequency 
        {
            get => _centerFrequency;
            set
            {
                if (value > MaxFrequency) value = MaxFrequency;
                else if(value < MinFrequency) value = MinFrequency;

                SetField(ref _centerFrequency, value);
            }
        }

        private double _bandWidth = defaultBandWidth;
        public double BandWidth 
        {
            get => _bandWidth;
            set
            {
                SetField(ref _bandWidth, value);
            }
        }

        private double _gain = defaultGain;
        public double Gain 
        {
            get => _gain; 
            set
            {
                SetField(ref _gain, value);
            }
        }

        public EffectType FXType => EffectType.PeakEQ;

        public EQEffectModel(int band, double centerFrequency, double bandWidth, double gain)
        {
            Band = band;
            CenterFrequency = centerFrequency;
            BandWidth = bandWidth;
            Gain = gain;
        }

        public EQEffectModel(int band, double bandWidth)
        {
            Band = band;
            BandWidth = bandWidth;
        }

        public EQEffectModel()
        {
                
        }

        public virtual object Clone()
        {
            return new EQEffectModel()
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
