using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicPlayModels.MusicModels;

namespace MusicPlayModels.Interfaces
{
    public interface ITaggable
    {
        public List<TagModel> Tags { get; set; }
    }
}
