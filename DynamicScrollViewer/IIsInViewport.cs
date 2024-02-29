using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicScrollViewer
{
    public interface IIsInViewport
    {
        public bool IsInViewport { get; set; }
    }
}
