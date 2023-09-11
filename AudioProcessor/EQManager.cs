using System.Runtime.InteropServices;
using System.Threading.Channels;
using AudioHandler.Models;
using ManagedBass;
using ManagedBass.Fx;
using MusicPlayModels;

namespace AudioHandler
{
    public class EQManager : ObservableObject, IDisposable
    {
        private int stream = -1;
        private int fxHandle = -1;

        public event Action EQBandChanged;
        private void OnEQBandChanged()
        {
            EQBandChanged?.Invoke();
        }

        public event Action PresetChanged;
        private void OnPresetChanged()
        {
            PresetChanged?.Invoke();
        }

        private EQPresetModel _preset = new();
        public EQPresetModel Preset
        {
            get => _preset;
            private set
            {
                SetField(ref _preset, value);
                OnPresetChanged();
            }
        }

        private bool _enabled = false;
        public bool Enabled 
        {
            get => _enabled;
            set
            {
                _enabled = value;

                if (value)
                    ApplyPreset();
                else
                    RemovePreset();
            }
        }

        public EQManager()
        {
            // load bass fx
            _ = BassFx.Version;
            Bass.Configure(Configuration.FloatDSP, true);
        }

        private void SetNewPreset(EQPresetModel preset)
        {
            if (fxHandle != -1)
                RemovePreset(); // remove old preset
            Preset = preset;
        }

        public void ApplyPreset(int stream, EQPresetModel preset = null)
        {
            this.stream = stream; // update the stream and the preset even if not enabled
            preset ??= Preset;
            SetNewPreset(preset);
            ApplyPreset(); // will not be applied if not Enabled if false
        }

        public void ApplyPreset(EQPresetModel preset)
        {
            SetNewPreset(preset);
            ApplyPreset(); // will not be applied if not Enabled if false
        }

        private void ApplyPreset()
        {
            if(!Enabled) return;

            int bandCount = 0;

            fxHandle = Bass.ChannelSetFX(stream, EffectType.PeakEQ, 1);
            foreach (EQEffectModel effect in Preset.Effects)
            {
                effect.Band = bandCount;
                if (!ApplyEffect(effect))
                {
                    var error = Bass.LastError;
                }
                bandCount++;
            }
        }

        private bool ApplyEffect(EQEffectModel effect)
        {
            if(fxHandle == -1)
            {
                fxHandle = Bass.ChannelSetFX(stream, EffectType.PeakEQ, 1);
            }

            return Bass.FXSetParameters(fxHandle, effect.EQEffectToEQParamater());
        }


        public void AddEffect(EQEffectModel effect)
        {
            effect.Band = Preset.Effects.Count;
            Preset.Effects.Add(effect);
            ApplyEffect(effect);
            OnPresetChanged();
        }

        public void RemovePreset()
        {
            bool success = Bass.ChannelRemoveFX(stream, fxHandle);

            if (success) fxHandle = -1;
        }

        public void RemoveEffect(EQEffectModel effect)
        {
            int index = -1;
            for (int i = 0; i < Preset.Effects.Count; i++)
            {
                if (Preset.Effects[i].Band == effect.Band)
                {
                    index = i;
                }
                else if(index != -1)
                {
                    Preset.Effects[i].Band = i - 1;
                }
            }

            if (index == -1) return;
                
            Preset.Effects.RemoveAt(index);

            Bass.ChannelRemoveFX(stream, fxHandle);
            
            ApplyPreset();
            OnPresetChanged();
        }

        public void UpdatePreset(EQEffectModel effect)
        {
            // update the data
            EQEffectModel eff = Preset.Effects.Find(e => e.Band == effect.Band);
            eff.Gain = effect.Gain;
            eff.CenterFrequency = effect.CenterFrequency;
            eff.BandWidth = effect.BandWidth;

            if (Enabled)
            {
                if (fxHandle == -1)
                {
                    ApplyPreset();
                } 
                else
                {
                    // update correct band
                    PeakEQParameters eq = new();
                    // get values of the selected band
                    eq.lBand = effect.Band;
                    if (Bass.FXGetParameters(fxHandle, eq))
                    {
                        eq.fGain = (float)effect.Gain;
                        Bass.FXSetParameters(fxHandle, eq);
                    }
                    else
                    {
                        bool success = Bass.FXSetParameters(fxHandle, eff.EQEffectToEQParamater());
                    }
                }
            }
            OnEQBandChanged();
        }

        public void ResetPreset()
        {
            ApplyPreset(stream, new());
        }

        public void Dispose()
        {
            RemovePreset();
            //Preset.PropertyChanged -= Preset_PropertyChanged;
        }
    }

    static class EQEffectHelper
    {
        public static PeakEQParameters EQEffectToEQParamater(this EQEffectModel effect)
        {
            PeakEQParameters parameters = new();
            parameters.lBand = effect.Band;
            parameters.fBandwidth = (float)effect.BandWidth;
            parameters.fCenter = (float)effect.CenterFrequency;
            parameters.fGain = (float)effect.Gain;
            parameters.lChannel = effect.Channels;

            return parameters;
        }
    }
}
