using MusicPlayModels;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.MVVM.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MusicPlayUI.MVVM.Models
{
    public class MenuModel : BaseModel
    {
        private bool _isSelected;

        public PathGeometry Icon { get; set; }
        public string Name { get; set; }
        public ViewNameEnum Enum { get; set; }
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }
    }
}
