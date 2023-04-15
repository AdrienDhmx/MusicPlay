using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayUI.MVVM.Models
{
    public class TrackInfoModel
    {
        public TrackInfoModel(string infoName, string infoValue)
        {
            InfoName = infoName;
            InfoValue = infoValue;
        }

        public string InfoName { get; set; }
        public string InfoValue { get; set; }
    }
}
