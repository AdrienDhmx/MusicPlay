using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayModels.MusicModels
{
    public class TagModel : PlayableModel
    {
        private string _name = "";
        public string Name {
            get => _name;
            set
            {
                SetField(ref _name, value);
            } 
        }

        public TagModel(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public TagModel() { }

        public override Dictionary<string, object> CreateTable()
        {
            return base.CreateTable().AddRange(new() { { nameof(Name), Name } });
        }
    }
}
