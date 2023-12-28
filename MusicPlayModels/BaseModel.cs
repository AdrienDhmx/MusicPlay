using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicPlayModels.MusicModels;

namespace MusicPlayModels
{
    public class BaseModel : ObservableObject
    {
        public int Id { get; set; } = -1;

        public override bool Equals(object obj)
        {
            if (obj is BaseModel model && model.Id == Id)
            {
                return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
