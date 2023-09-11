using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayModels
{
    public class BaseModel : ObservableObject
    {
        public int Id { get; set; } = -1;
    }
}
