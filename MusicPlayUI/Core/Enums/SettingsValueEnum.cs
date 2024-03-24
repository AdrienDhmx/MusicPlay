using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayUI.Core.Enums
{
    public enum SettingsValueEnum
    {
        // Error
        UNKNOWN = -1,

        // App Themes
        DefaultTheme = 0,
        WaterTheme = 1,
        ForestTheme = 2,
        FallenLeavesTheme = 3,
        TurquoiseTheme = 4,
        RedWineTheme = 5,

        // Languages (10 - 29)
        English = 10,
        French = 11,
        Spanish = 12,
        German = 13,
        ChineseMandarin = 14,
        ChinseCantonese = 15,
        Korean = 16,
        Japanese = 17,

        // Queue Covers
        NoCovers = 50,
        AlbumCovers = 51,
        ArtworkCovers = 52,
        AutoCovers = 53,
    
        // Pre made Equalizer presets
        Acoustic = -100,
        Classic = -101,
        Electronic = -102,
        Jazz = -103,
        Metal = -104,
        Piano = -105,
        Pop = -106,
        Rock = -107,

        // IDEAS:
        // - File directories = Cover and Lyrics directories
        // - Lyrics Settings = default fontsize, default website (only AZLyrics for now though), AutoSave

    }

    public static class SettingsValueEnumExt
    {
        public static string GetLanguageCulture(this SettingsValueEnum settingsValue)
        {
            switch (settingsValue)
            {
                case SettingsValueEnum.UNKNOWN:
                    return "en";
                case SettingsValueEnum.English:
                    return "en";
                case SettingsValueEnum.French:
                    return "fr";
                case SettingsValueEnum.Spanish:
                    return "es";
                case SettingsValueEnum.German:
                    return "de";
                case SettingsValueEnum.ChineseMandarin:
                    return "ch-md";
                case SettingsValueEnum.ChinseCantonese:
                    return "ch-ct";
                case SettingsValueEnum.Korean:
                    return "kr";
                case SettingsValueEnum.Japanese:
                    return "jp";
                default:
                    return "en";
            }
        }
    }
}
