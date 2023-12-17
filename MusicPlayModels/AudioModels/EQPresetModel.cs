using MusicPlayModels;

namespace AudioHandler.Models
{
    public class EQPresetModel : BaseModel, ICloneable
    {
        private readonly List<EQEffectModel> _defaultEffects = new()
        {
            new (0, 1000, 1, 0),
            //new (1, 64, 1.2, 0),
            //new (2, 125, 1.2, 0),
            //new (3, 250, 1.2, 0),
            //new (4, 500, 1.2, 0),
            //new (5, 1000, 1.2, 0),
            //new (6, 2000, 1.2, 0),
            //new (7, 4000, 1.2, 0),
            //new (8, 8000, 1.2, 0),
            //new (9, 16000, 1.2, 0),
        };

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                SetField(ref _name, value);
            }
        }

        private List<EQEffectModel> _effects;
        public List<EQEffectModel> Effects
        {
            get => _effects;
            set 
            {
                SetField(ref _effects, value);
            }
        }

        public EQPresetModel()
        {
            _effects = new(_defaultEffects);
        }

        public EQPresetModel(List<EQEffectModel> effects, string name)
        {
            _effects = new(effects);
            Name = name;
        }

        public object Clone()
        {
            return new EQPresetModel(new(Effects), Name) { Id = this.Id };
        }
    }
}
