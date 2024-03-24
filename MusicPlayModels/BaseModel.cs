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

        public virtual Dictionary<string, object> CreateTable()
        {
            throw new NotSupportedException("CreateTable not supported for the base class.");
        }

        public virtual Dictionary<string, object> UpdateTable()
        {
            throw new NotSupportedException("UpdateTable not supported for the base class.");
        }

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
