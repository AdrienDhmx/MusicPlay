using MusicPlayModels;
using MusicPlayModels.MusicModels;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.MVVM.ViewModels;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayUI.MVVM.Models
{
    public class NavigationModel
    {
        public BaseModel Parameter { get; set; }
        public ViewModel View { get; set; }
        public ViewNameEnum ViewName { get; set; }

        public NavigationModel(ViewModel view, BaseModel parameter, ViewNameEnum viewName)
        {
            Parameter = parameter;
            View = view;
            ViewName = viewName;
        }

        public NavigationModel(ViewModel view)
        {
            View = view;
        }

        public NavigationModel()
        {

        }
    }
}
