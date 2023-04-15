using System.Windows.Markup;
using MusicPlay.Language;
using MusicPlayUI.Core.Enums;

namespace MusicPlayUI.Core.Services
{
    public static class LanguageService
    {
        public static void SetLanguage(string language = "en")
        {
            Resources.Culture = new System.Globalization.CultureInfo(language);
        }

        public static string CurrentCulture => ((SettingsValueEnum)ConfigurationService.GetPreference(SettingsEnum.Language)).GetLanguageCulture();
        public static XmlLanguage CurrentXamlCulture => XmlLanguage.GetLanguage(CurrentCulture);

        public static string GetCurrentLanguage(this string culture)
        {
            return culture switch
            {
                "en" => Resources.English_Lang,
                "fr" => Resources.French_Lang,
                "ko" => Resources.Korean_Lang,
                "es" => Resources.Spanish_Lang,
                "de" => Resources.German_Lang,
                "zh-CN" => Resources.Chinese_Mandarin_Lang,
                "jp" => Resources.Japanese_Lang,
                _ => Resources.English_Lang,
            };
        }
    }
}
