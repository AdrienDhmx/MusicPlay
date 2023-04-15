using MessageControl;
using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Factories;
using MusicPlayUI.Core.Services;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.MVVM.Models;
using System.Collections.Generic;
using System.Windows.Input;

namespace MusicPlayUI.MVVM.ViewModels.SettingsViewModels
{
    public class LanguageSettingViewModel : BaseSettingViewModel
    {
        private string _appliedLanguage;
        public string AppliedLanguage
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_appliedLanguage))
                {
                    _appliedLanguage = AppliedSetting(_languages.Find(t => t.Value == (SettingsValueEnum)ConfigurationService.GetPreference(SettingsEnum.Language)).Name);
                }
                return _appliedLanguage;
            }
            set
            {
                _appliedLanguage = AppliedSetting(value);
                OnPropertyChanged(nameof(AppliedLanguage));
            }
        }

        private List<SettingValueModel<SettingsValueEnum>> _languages = SettingsModelFactory.GetLanguages();
        public List<SettingValueModel<SettingsValueEnum>> Languages
        {
            get { return _languages; }
            set
            {
                _languages = value;
                OnPropertyChanged(nameof(Languages));
            }
        }

        private SettingValueModel<SettingsValueEnum> _selectedLanguage;
        public SettingValueModel<SettingsValueEnum> SelectedLanguage
        {
            get { return _selectedLanguage; }
            set
            {
                PreviousSelectedLanguage = _selectedLanguage;
                _selectedLanguage = value;
                OnPropertyChanged(nameof(SelectedLanguage));
            }
        }

        private SettingValueModel<SettingsValueEnum> PreviousSelectedLanguage { get; set; }

        public ICommand SetSelectedLanguageCommand { get; }
        public LanguageSettingViewModel()
        {
            SetSelectedLanguageCommand = new RelayCommand<SettingValueModel<SettingsValueEnum>>((theme) => UpdateSelectedLanguage(theme, true));

            UpdateSelectedLanguage(Languages.Find(l => l.Value == (SettingsValueEnum)ConfigurationService.GetPreference(SettingsEnum.Language)));
        }

        private void UpdateSelectedLanguage(SettingValueModel<SettingsValueEnum> language, bool save = false)
        {

            foreach (SettingValueModel<SettingsValueEnum> t in Languages)
            {
                if (t.Name == language.Name)
                    t.IsSelected = true;
                else
                    t.IsSelected = false;
            }
            SelectedLanguage = language;
            Languages = new(Languages);

            if(save)
            {
                SetPreference(SettingsEnum.Language, ((int)language.Value).ToString());
                // change app language
                LanguageService.SetLanguage(SelectedLanguage.Value.GetLanguageCulture());

                // reload this view
                Languages = SettingsModelFactory.GetLanguages();

                MessageHelper.PublishMessage(MessageFactory.LanguageChanged(SelectedLanguage.Name));
            }
        }
    }
}
