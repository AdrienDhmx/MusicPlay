using Resources = MusicPlay.Language.Resources;
using MusicPlayUI.Core.Enums;
using System;
using MusicPlay.Database.Models;

namespace MusicPlayUI.Core.Services
{
    public class MessageService : ObservableObject
    {
        private static readonly Random rng = new();

        private static string WelcomeMessage { get; set; } = "";
        private static bool IsEvening => DateTime.Now.TimeOfDay.Hours >= 18;
        private static bool IsMorning => DateTime.Now.TimeOfDay.Hours <= 11;

        public static string GetWelcomeMessage()
        {
            string message;
            string userName = ConfigurationService.GetStringPreference(SettingsEnum.UserName);

            if (IsMorning)
            {
                message = Resources.Good_Morning;
            }
            else if (IsEvening)
            {
                message = Resources.Good_Evening;
            }
            else
            {
                int result = rng.Next(0, 3);
                if (result == 0)
                    message = Resources.Welcome_Back;
                else if (result == 1)
                    message = Resources.Hi;
                else
                    message = Resources.Welcome;
            }


            // if the time has changed then change the welcome Message
            if (IsMorning || IsEvening || WelcomeMessage == "")
            {
                WelcomeMessage = message;
            }

            // add the user name if there is one
            // if the username has been updated it will change since this is called every time the Home view is opened
            if (string.IsNullOrWhiteSpace(userName))
            {
                return WelcomeMessage + "!";
            }
            else 
            { 
                return WelcomeMessage + " " + userName + "!"; 
            }
        }
    }
}
