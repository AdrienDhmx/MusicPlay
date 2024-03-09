using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicPlayUI.Core.Models;
using MusicPlayUI.Core.Services;

namespace MusicPlayUI.MVVM.ViewModels.PopupViewModels
{
    public class PopupViewModel : ViewModel
    {
        public override NavigationState State
        {
            get => App.State.CurrentPopup?.State;
        }

        public static bool AreCoversEnabled => ConfigurationService.AreCoversEnabled;

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
