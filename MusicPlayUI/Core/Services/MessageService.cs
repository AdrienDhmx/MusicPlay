using Resources = MusicPlay.Language.Resources;
using MusicPlayModels;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Services.Interfaces;
using MusicPlayUI.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Documents;

namespace MusicPlayUI.Core.Services
{
    public class MessageService : ObservableObject
    {
        private static readonly Random rng = new();

        private static string WelcomeMessage { get; set; } = "";

        public static string GetWelcomeMessage()
        {
            if (!string.IsNullOrWhiteSpace(WelcomeMessage))
                return WelcomeMessage;

            string message;
            string userName = ConfigurationService.GetStringPreference(SettingsEnum.UserName);
            if (DateTime.Now.TimeOfDay.Hours <= 10)
            {
                message = Resources.Good_Morning;
            }
            else if (DateTime.Now.TimeOfDay.Hours >= 18)
            {
                message = Resources.Good_Evening;
            }
            else
            {

                int result = rng.Next(0, 3);
                if (result == 0)
                    message = WelcomeMessage = Resources.Welcome_Back;
                else if (result == 1)
                    message = WelcomeMessage = Resources.Hi;
                else
                    message = WelcomeMessage = Resources.Welcome;
            }

            if (string.IsNullOrWhiteSpace(userName))
            {
                return message + "!";
            }
            else 
            { 
                return message + " " + userName + "!"; 
            }
        }
    }
}
