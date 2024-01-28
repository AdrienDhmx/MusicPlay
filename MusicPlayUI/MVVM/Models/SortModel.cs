using MusicPlay.Database.Enums;
using MusicPlay.Database.Models;

namespace MusicPlayUI.MVVM.Models
{
    public class SortModel : BaseModel
    {
        public SortEnum SortType { get; }

        public string SortName { get; }

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

        public SortModel(SortEnum sortType, string sortName, bool isSelected, bool isAscending)
        {
            SortType = sortType;
            SortName = sortName;
            IsSelected= isSelected;
            IsAscending = isAscending;
        }
    }
}
