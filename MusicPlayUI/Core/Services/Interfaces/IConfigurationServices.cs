using MusicPlayUI.Core.Enums;

namespace MusicPlayUI.Core.Services.Interfaces
{
    public interface IConfigurationServices
    {
        SettingsValueEnum GetPreference(SettingsEnum key);
        void InitializeAppTheme();
        bool SetPreference(SettingsEnum key, string value, string valueName);
    }
}