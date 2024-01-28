using MusicPlay.Database.Models;

using MusicPlayUI.Core.Enums;
using System;
using System.Windows.Media;

namespace MusicPlayUI.MVVM.Models
{
    public class MenuModel : BaseModel
    {
        private bool _isSelected;

        public PathGeometry Icon { get; set; }
        public string Name { get; set; }
        public ViewNameEnum Enum { get; set; }
        public Type Type {  get; set; }
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
