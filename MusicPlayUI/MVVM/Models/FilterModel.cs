using MusicPlayModels;
using MusicPlayUI.Core.Enums;

namespace MusicPlayUI.MVVM.Models
{
    public class FilterModel : BaseModel
    {
        public string Name { get; set; }
        public int ValueId { get;}
        public FilterEnum FilterType { get; }

        public FilterModel(string name, int valueId, FilterEnum filterType)
        {
            Name = name;
            ValueId = valueId;
            FilterType = filterType;
        }

        public FilterModel(int valueId, FilterEnum filterType)
        {
            ValueId = valueId;
            FilterType = filterType;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj is not FilterModel) return false;

            FilterModel model = obj as FilterModel;
            return this.FilterType == model.FilterType && this.ValueId == model.ValueId;
        }
    }
}
