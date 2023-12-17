using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioHandler.Models;
using MusicPlayUI.Core.Enums;

namespace MusicPlayUI.Core.Factories
{
    public static class EQModelsFactory
    {
        public static readonly EQPresetModel Acoustic = new List<double>() { 3.5, 3.2, 2.2, -0.9, 0.5, 0.3, 2, 2.5, 2, 0 }.CreateEQPreset("Acoustic", (int)SettingsValueEnum.Acoustic);
        public static readonly EQPresetModel Classic = new List<double>() { 4.8, 4.5, 3.5, 0.4, 1.8, 1.6, 3.3, -1.5, -3.7, -5.3 }.CreateEQPreset("Classic", (int)SettingsValueEnum.Classic);
        public static readonly EQPresetModel Electronic = new List<double>() { 4, 3.5, 0.9, -0.6, -2.6, 1.8, 0.4, 0.9, 3.5, 4.3 }.CreateEQPreset("Electronic", (int)SettingsValueEnum.Electronic);
        public static readonly EQPresetModel Jazz = new List<double>() { 3.8, 2.3, 0.9, 1.8, -1.8, -1.8, -0.6, 1.1, 2.6, 3.5 }.CreateEQPreset("Jazz", (int)SettingsValueEnum.Jazz);
        public static readonly EQPresetModel Metal = new List<double>() { -1.3, 1.9, 1.7, -1.7, -3.7, -4.1, -1, 2.1, 4.7, 1.9 }.CreateEQPreset("Metal", (int)SettingsValueEnum.Metal);
        public static readonly EQPresetModel Piano = new List<double>() { 1.6, 0.6, -1.6, 1.1, 1.6, -0.6, 2.3, 3.0, 1.3, 2.0 }.CreateEQPreset("Piano", (int)SettingsValueEnum.Piano);
        public static readonly EQPresetModel Pop = new List<double>() { -1.4, -1.9, 0.8, 2.5, 3.6, 2.3, 0.5, -1, -1.9, -0.1 }.CreateEQPreset("Pop", (int)SettingsValueEnum.Pop);
        public static readonly EQPresetModel Rock = new List<double>() { 3.9, 2.8, -1.5, -3.8, -6.6, -1.1, 0.6, 3.5, 4.6, 5 }.CreateEQPreset("Rock", (int)SettingsValueEnum.Rock);

        public static List<EQPresetModel> GetPreMadeEQPresets()
        {
            return new()
            {
                Acoustic,
                Classic,
                Electronic,
                Jazz,
                Metal,
                Piano,
                Pop,
                Rock,
            };
        }

        public static EQPresetModel CreateEQPreset(this List<double> BandGains, string name, int id)
        {
            List<EQEffectModel> bands = new();
            int hz = 32;
            for (int i = 0; i < BandGains.Count; i++)
            {
                bands.Add(new(i, hz, 1, BandGains[i]));
                hz *= 2;

                if (hz == 128) hz = 125;
            }

            return new()
            {
                Name = name,
                Effects = bands,
                Id = id
            };
        }

        public static EQPresetModel GetPreMadePreset(int id)
        {
            return (SettingsValueEnum)id switch
            {
                SettingsValueEnum.Acoustic => Acoustic,
                SettingsValueEnum.Classic => Classic,
                SettingsValueEnum.Electronic => Electronic,
                SettingsValueEnum.Jazz => Jazz,
                SettingsValueEnum.Metal => Metal,
                SettingsValueEnum.Piano => Piano,
                SettingsValueEnum.Pop => Pop,
                SettingsValueEnum.Rock => Rock,
                SettingsValueEnum.UNKNOWN => new(),
                _ => throw new NotImplementedException(),
            };
                
        }
    }
}
