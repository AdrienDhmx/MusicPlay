using MusicPlay.Database.Enums;
using MusicPlay.Database.Models;
using MusicPlay.Language;
using MusicPlayUI.Core.Enums;
using MusicPlayUI.Core.Services;
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
        private static SolidColorBrush RedColor => (SolidColorBrush)AppTheme.AppThemeDic["ErrorHover"];
        private static SolidColorBrush MainHoverColor => (SolidColorBrush)AppTheme.AppThemeDic["PrimaryHover"];
        private static SolidColorBrush OnRedColor => (SolidColorBrush)AppTheme.AppThemeDic["Error"];
        private static SolidColorBrush OnMainHoverColor => (SolidColorBrush)AppTheme.AppThemeDic["Primary"];

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
                case ModelTypeEnum.Tag:
                    dataType = "tag";
                    break;
                case ModelTypeEnum.EQPreset:
                    dataType = "preset";
                    break;
                default:
                    break;
            }
            return CreateConfirmModel(Resources.Delete, $"{Resources.Delete} {dataType}", $"\"{dataToDelete}\" {Resources.PermanentlyDeletedWarning}", actionColor: RedColor);
        }

        public static ConfirmActionModel CreateConfirmDeleteFolderModel(this Folder folder)
        {
            string title = $"Deleting Folder \"{folder.Name}\"";
            string contentMessage = $"Deleting this folder will also delete the {folder.TrackImportedCount} tracks imported from it.";
            return CreateConfirmModel(Resources.Delete, title, contentMessage, RedColor);
        }

        public static ConfirmActionModel CreateConfirmClearDataBaseModel()
        {
            return CreateConfirmModel(Resources.ClearDatabase, Resources.ClearDatabase,
               $"{Resources.TheDatabase} {Resources.PermanentlyDeletedWarning}", RedColor);
        }
    }
}
