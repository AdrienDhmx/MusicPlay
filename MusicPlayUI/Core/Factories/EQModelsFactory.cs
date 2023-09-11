using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioHandler.Models;

namespace MusicPlayUI.Core.Factories
{
    public static class EQModelsFactory
    {
        public static List<EQPresetModel> GetPreMadeEQPresets()
        {
            return new()
            {
                new List<double>() { 3.5, 3.2, 2.2, -0.9, 0.5, 0.3, 2, 2.5, 2, 0 }.CreateEQPreset("Acoustic"),
                new List<double>() { 4.8, 4.5, 3.5, 0.4, 1.8, 1.6, 3.3, -1.5, -3.7, -5.3 }.CreateEQPreset("Classic"),
                new List<double>() { 4, 3.5, 0.9, -0.6, -2.6, 1.8, 0.4, 0.9, 3.5, 4.3 }.CreateEQPreset("Electronic"),
                new List<double>() { 3.8, 2.3, 0.9, 1.8, -1.8, -1.8, -0.6, 1.1, 2.6, 3.5 }.CreateEQPreset("Jazz"),
                new List<double>() { -1.3, 1.9, 1.7, -1.7, -3.7, -4.1, -1, 2.1, 4.7, 1.9 }.CreateEQPreset("Metal"),
                new List<double>() { 1.6, 0.6, -1.6, 1.1, 1.6, -0.6, 2.3, 3.0, 1.3, 2.0 }.CreateEQPreset("Piano"),
                new List<double>() { -1.4, -1.9, 0.8, 2.5, 3.6, 2.3, 0.5, -1, -1.9, -0.1 }.CreateEQPreset("Pop"),
                new List<double>() { 3.9, 2.8, -1.5, -3.8, -6.6, -1.1, 0.6, 3.5, 4.6, 5 }.CreateEQPreset("Rock"),
            };
        }

        public static EQPresetModel CreateEQPreset(this List<double> Bandgains, string name)
        {
            if(Bandgains.Count < 10) return new();

            return new()
            {
                Name = name,
                Effects = new()
                {
                    new (0, 32, 1, Bandgains[0]),
                    new (1, 64, 1, Bandgains[1]),
                    new (2, 125, 1, Bandgains[2]),
                    new (3, 250, 1, Bandgains[3]),
                    new (4, 500, 1, Bandgains[4]),
                    new (5, 1000, 1, Bandgains[5]),
                    new (6, 2000, 1, Bandgains[6]),
                    new (7, 4000, 1, Bandgains[7]),
                    new (8, 8000, 1, Bandgains[8]),
                    new (9, 16000, 1, Bandgains[9]),
                }
            };
        }
    }
}
