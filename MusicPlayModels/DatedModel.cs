using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayModels
{
    public abstract class DatedModel : BaseModel
    {
        public DateTime CreationDate { get; set; }
        public DateTime UpdateDate { get; set; }

        protected DatedModel(DateTime creationDate, DateTime updateDate)
        {
            CreationDate = creationDate;
            UpdateDate = updateDate;
        }

        public DatedModel()
        {
            
        }

    }
}
