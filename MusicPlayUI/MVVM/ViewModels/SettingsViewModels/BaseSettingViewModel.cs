using MusicPlay.Language;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Services;

namespace MusicPlayUI.MVVM.ViewModels.SettingsViewModels
{
    public class BaseSettingViewModel : ViewModel
    {
        protected void SetPreference(SettingsEnum key, string value)
        {
            ConfigurationService.SetPreference(key, value);
        }

        protected string AppliedSetting(string settingValue)
        {
            return Resources.Applied + ": " + settingValue;
        }

        protected string BoolToString(bool value)
        {
            return value ? "1" : "0";
        }
    }
}
