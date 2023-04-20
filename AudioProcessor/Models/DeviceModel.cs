using AudioHandler.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Composition;

namespace AudioHandler.Models
{
    public class DeviceModel
    {
        public string Name { get; set; }

        public int Index { get; set; }

        public DeviceTypeEnum Type { get; set; }

        public bool IsInitialized { get; set; }

        public bool IsDefault { get; set; }

    }
}
