using MessageControl;
using MusicPlayUI.Core.Commands;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Factories;
using MusicPlayUI.Core.Services;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.MVVM.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace MusicPlayUI.MVVM.ViewModels.SettingsViewModels
{
    public class GeneralSettingsViewModel : BaseSettingViewModel
    {
        private string _appliedStartingView;
        public string AppliedStartingView
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_appliedStartingView))
                {
                    _appliedStartingView = AppliedSetting(_startingViews.Find(t => t.Value == (ViewNameEnum)ConfigurationService.GetPreference(SettingsEnum.MainStartingView)).Name);
                }
                return _appliedStartingView;
            }
            set
            {
                _appliedStartingView = AppliedSetting(value);
                OnPropertyChanged(nameof(AppliedStartingView));
            }
        }

        private List<SettingValueModel<ViewNameEnum>> _startingViews = SettingsModelFactory.GetStartingViews();
        public List<SettingValueModel<ViewNameEnum>> StartingViews
        {
            get { return _startingViews; }
            set
            {
                _startingViews = value;
                OnPropertyChanged(nameof(StartingViews));
            }
        }

        private SettingValueModel<ViewNameEnum> _selectedStartingView;
        public SettingValueModel<ViewNameEnum> SelectedStartingView
        {
            get { return _selectedStartingView; }
            set
            {
                _selectedStartingView = value;
                OnPropertyChanged(nameof(SelectedStartingView));
            }
        }

        private ObservableCollection<SettingValueModel<SettingsValueEnum>> _queueCovers = new(SettingsModelFactory.GetQueueCovers());
        public ObservableCollection<SettingValueModel<SettingsValueEnum>> QueueCovers
        {
            get { return _queueCovers; }
            set
            {
                _queueCovers = value;
                OnPropertyChanged(nameof(QueueCovers));
            }
        }


        private SettingValueModel<SettingsValueEnum> _selectedQueueCover;
        public SettingValueModel<SettingsValueEnum> SelectedQueueCover
        {
            get { return _selectedQueueCover; }
            set
            {
                _selectedQueueCover = value;
                OnPropertyChanged(nameof(SelectedQueueCover));
            }
        }

        private string _appliedQueueCover;
        public string AppliedQueueCover
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_appliedQueueCover))
                {
                    var value = QueueCovers.ToList().Find(t => t.Value == (SettingsValueEnum)ConfigurationService.GetPreference(SettingsEnum.QueueCovers));
                    if (value != null)
                        _appliedQueueCover = value.Name;
                }
                return _appliedQueueCover;
            }
            set
            {
                _appliedQueueCover = AppliedSetting(value);
                OnPropertyChanged(nameof(AppliedQueueCover));
            }
        }

        private string _userName = ConfigurationService.GetStringPreference(SettingsEnum.UserName);
        public string UserName
        {
            get { return _userName; }
            set
            {
                _userName = value;
                ConfigurationService.SetPreference(SettingsEnum.UserName, value, true);
                OnPropertyChanged(nameof(UserName));
            }
        }

        public ICommand SetSelectedStartingViewCommand { get; }
        public ICommand SetSelectedQueueCoverCommand { get; }
        public GeneralSettingsViewModel()
        {
            SetSelectedStartingViewCommand = new RelayCommand<SettingValueModel<ViewNameEnum>>((view) => UpdateSelectedStartingView(view));
            SetSelectedQueueCoverCommand = new RelayCommand<SettingValueModel<SettingsValueEnum>>((view) => UpdateSelectedQueueCover(view));

            UpdateSelectedStartingView(StartingViews.Find(v => v.Value == (ViewNameEnum)ConfigurationService.GetPreference(SettingsEnum.MainStartingView)), false);
            UpdateSelectedQueueCover(QueueCovers.ToList().Find(v => v.Value == (SettingsValueEnum)ConfigurationService.GetPreference(SettingsEnum.QueueCovers)), false);
        }

        public override void Dispose()
        {
            StartingViews.Clear();
        }

        private void UpdateSelectedStartingView(SettingValueModel<ViewNameEnum> startingView, bool save = true)
        {
            if (startingView is null) return;

            foreach (SettingValueModel<ViewNameEnum> t in StartingViews)
            {
                if (t.Name == startingView.Name)
                    t.IsSelected = true;
                else
                    t.IsSelected = false;
            }
            SelectedStartingView = startingView;
            StartingViews = new(StartingViews);

            if (save)
            {
                SetPreference(SettingsEnum.MainStartingView, ((int)SelectedStartingView.Value).ToString());
                MessageHelper.PublishMessage(MessageFactory.StartingViewChanged(SelectedStartingView.Name));
            }
            AppliedStartingView = SelectedStartingView.Name;
        }

        private void UpdateSelectedQueueCover(SettingValueModel<SettingsValueEnum> queueCover, bool save = true)
        {
            if (queueCover == null) return;

            foreach (SettingValueModel<SettingsValueEnum> t in QueueCovers)
            {
                if (t.Name == queueCover.Name)
                    t.IsSelected = true;
                else
                    t.IsSelected = false;
            }
            SelectedQueueCover = queueCover;

            if (save)
            {
                SetPreference(SettingsEnum.QueueCovers, ((int)SelectedQueueCover.Value).ToString());
            }
            AppliedQueueCover = SelectedQueueCover.Name;
        }
    }
}
