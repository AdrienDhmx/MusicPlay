using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicPlayUI.Core.Models;

namespace MusicPlayUI.MVVM.ViewModels.PopupViewModels
{
    public class PopupViewModel : ViewModel
    {
        public override NavigationState State
        {
            get => App.State.CurrentPopup?.State;
            set
            {
                if (App.State.CurrentPopup != null)
                {
                    App.State.CurrentPopup.State = value;
                    OnPropertyChanged(nameof(State));
                }
            }
        }

        public PopupViewModel()
        {

        }

        /// <summary>
        /// Closes the popup by calling App.State.ClosePopup()
        /// </summary>
        public static void ClosePopup()
        {
            App.State.ClosePopup();
        }
    }
}
