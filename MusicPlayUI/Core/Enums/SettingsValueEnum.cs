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

        // Languages (10 - 29)
        English = 10,
        French = 11,
        Spanish = 12,
        German = 13,
        ChineseMandarin = 14,
        ChinseCantonese = 15,
        Korean = 16,
        Japonese = 17,

        // Queue Covers
        NoCovers = 50,
        AlbumCovers = 51,
        ArtworkCovers = 52,
        AutoCovers = 53

        // IDEAS:
        // - File directories = Cover and Lyrics directories
        // - Lyrics Settings = default fontsize, default website (only AZLyrics for now though), AutoSave
        // - default blur radius or no blur at all

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
                case SettingsValueEnum.Japonese:
                    return "jp";
                default:
                    return "en";
            }
        }
    }
}
