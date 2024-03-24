using MusicPlay.Database.Enums;
using MusicPlay.Database.Models;

namespace MusicPlayUI.MVVM.Models
{
    public class SortModel : BaseModel
    {
        public SortEnum Type { get; }

        public string Name { get; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                SetField(ref _isSelected, value);
            }
        }

        private bool _isAscending;
        public bool IsAscending
        {
            get => _isAscending;
            set
            {
                SetField(ref _isAscending, value);
            }
        }

        public SortModel(int id, SortEnum sortType, string sortName, bool isSelected, bool isAscending)
        {
            Id = id;
            Type = sortType;
            Name = sortName;
            IsSelected= isSelected;
            IsAscending = isAscending;
        }
    }
}
