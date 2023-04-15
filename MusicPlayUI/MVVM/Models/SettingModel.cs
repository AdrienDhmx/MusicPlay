using MusicPlayModels;
using MusicPlayUI.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayUI.MVVM.Models
{
    public class SettingModel : BaseModel
    {
        private bool _isSelected = false;

        public ViewNameEnum SettingViewEnum { get; set; }
        public string SettingName { get; set; }
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }
        public string SettingDescription { get; set; }

        public SettingModel(ViewNameEnum settingViewEnum, string settingName, string settingDescription, bool isSelected = false)
        {
            SettingViewEnum = settingViewEnum;
            SettingName = settingName;
            IsSelected = isSelected;
            SettingDescription = settingDescription;
        }
    }
}
