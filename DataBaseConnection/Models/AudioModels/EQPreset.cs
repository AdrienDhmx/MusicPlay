using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using MusicPlay.Database.DatabaseAccess;
using MusicPlay.Database.Models;

namespace AudioHandler.Models
{
    [Table("EQPreset")]
    public class EQPreset : BaseModel, ICloneable
    {
        private readonly List<EQBand> _defaultEffects = new()
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

        private List<EQBand> _effects;
        public List<EQBand> Effects
        {
            get => _effects;
            set 
            {
                SetField(ref _effects, value);
            }
        }

        public EQPreset()
        {
            _effects = new(_defaultEffects);
        }

        public EQPreset(List<EQBand> effects, string name)
        {
            _effects = new(effects);
            Name = name;
        }

        public object Clone()
        {
            return new EQPreset(new(Effects), Name) { Id = this.Id };
        }

        public static async Task Insert(EQPreset preset)
        {
            using DatabaseContext context = new();
            context.EQPresetModels.Add(preset);
            await context.SaveChangesAsync();
        }

        public static EQPreset Get(int id)
        {
            using DatabaseContext context = new();
            return context.EQPresetModels.Find(id);
        }

        public static async Task<List<EQPreset>> GetAll()
        {
            using DatabaseContext context = new();
            return await context.EQPresetModels.ToListAsync();
        }
    }
}
