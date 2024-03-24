using MusicPlay.Database.Models;

using MusicPlayUI.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Converters;

namespace MusicPlayUI.MVVM.Models
{
    public class SettingModel : MenuModel
    {
        public string SettingDescription { get; set; }

        public SettingModel(ViewNameEnum viewNameEnum, string name, Type type, bool selected)
        {
            Enum = viewNameEnum;
            Name = name;
            Type = type;
            IsSelected = selected;
        }


        public SettingModel(ViewNameEnum viewNameEnum, string name, Type type)
        {
            Enum = viewNameEnum;
            Name = name;
            Type = type;
        }
    }
}
