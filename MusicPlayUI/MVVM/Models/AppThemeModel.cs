using MusicPlayUI.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MusicPlayUI.MVVM.Models
{
    public class AppThemeModel : SettingValueModel<SettingsValueEnum>
    {
        public Brush AccentColor { get; set; }

        public AppThemeModel(string name, string description, SettingsValueEnum value, Brush accentColor, bool isSelected = false) : base(name, description, value, isSelected)
        {
            AccentColor = accentColor;

            if (string.IsNullOrWhiteSpace(description))
            {
                base.Description = base.Name + " - " + AccentColor.ToString();
            }
        }
    }
}
