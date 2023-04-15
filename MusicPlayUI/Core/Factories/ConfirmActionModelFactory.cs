using MusicPlay.Language;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.MVVM.Models;
using System;
using System.Collections.Generic;
using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Windows.ApplicationModel.Background;

namespace MusicPlayUI.Core.Factories
{
    public static class ConfirmActionModelFactory
    {
        private static SolidColorBrush RedColor => (SolidColorBrush)App.appThemeDic["ErrorHover"];
        private static SolidColorBrush MainHoverColor => (SolidColorBrush)App.appThemeDic["PrimaryHover"];
        private static SolidColorBrush OnRedColor => (SolidColorBrush)App.appThemeDic["Error"];
        private static SolidColorBrush OnMainHoverColor => (SolidColorBrush)App.appThemeDic["Primary"];

        public static ConfirmActionModel CreateConfirmModel(this string confirmAction, string message, string messageDetail = "", Brush actionColor = null)
        {
            SolidColorBrush confirmActionForeground = OnMainHoverColor;
            actionColor ??= MainHoverColor;

            if(actionColor == RedColor)
            {
                confirmActionForeground = OnRedColor;
            }

            return new ConfirmActionModel()
            {
                Message = message,
                MessageDetail = messageDetail,
                ConfirmAction = confirmAction,
                ConfirmActionColor = actionColor,
                ConfirmActionForeground = confirmActionForeground
            };
        }

        public static ConfirmActionModel CreateConfirmDeleteModel(this string dataToDelete, ModelTypeEnum modelTypeEnum)
        {
            string dataType = "";
            switch (modelTypeEnum)
            {
                case ModelTypeEnum.Track:
                    dataType = Resources.Track;
                    break;
                case ModelTypeEnum.Album:
                    dataType = Resources.Album;
                    break;
                case ModelTypeEnum.Artist:
                    dataType = Resources.Artist;
                    break;
                case ModelTypeEnum.Playlist:
                    dataType = Resources.Playlist;
                    break;
                default:
                    break;
            }
            return CreateConfirmModel(Resources.Delete, $"{Resources.Delete} {dataType}", $"\"{dataToDelete}\" {Resources.PermanentlyDeletedWarning}", actionColor: RedColor);
        }

        public static ConfirmActionModel CreateConfirmClearDataBaseModel()
        {
            return CreateConfirmModel(Resources.ClearDatabase, Resources.ClearDatabase,
               $"{Resources.TheDatabase} {Resources.PermanentlyDeletedWarning}", RedColor);
        }
    }
}
