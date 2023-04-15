using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MusicPlay.Language;
using System.Windows.Media;
using System.Runtime.CompilerServices;
using MusicPlayUI.MVVM.Models;
using MusicPlayUI.Core.Enums;
using MessageControl.Model;
using MessageControl;
using MusicPlayUI.ThemeColors;
using TagLib.Ape;

namespace MusicPlayUI.Core.Factories
{
    public static class MessageFactory
    {
        private static PathGeometry AppThemeIcon => (PathGeometry)App.IconDic["PaletteIcon"]; // 0
        private static PathGeometry CoverChangedIcon => (PathGeometry)App.IconDic["ImageIcon"]; // 1
        private static PathGeometry ErrorIcon => (PathGeometry)App.IconDic["WarningIcon"]; // 2
        private static PathGeometry LanguageIcon => (PathGeometry)App.IconDic["TranslateIcon"]; // 3
        private static PathGeometry ImportLibraryDoneIcon => (PathGeometry)App.IconDic["ImportDoneIcon"]; // 4
        private static PathGeometry SettingIcon => (PathGeometry)App.IconDic["SettingsIcon"]; // 5
        private static PathGeometry CircledAddIcon => (PathGeometry)App.IconDic["CircledAddIcon"]; // 6
        private static PathGeometry CircledRemoveIcon => (PathGeometry)App.IconDic["CircledRemoveIcon"]; // 7

        public static void RegisterErrorMessagesStyle()
        { 
            DefaultMessageFactory.RegisterErrorMessageStyle(MessageColors.ErrorContainer, MessageColors.OnErrorContainer, MessageColors.ErrorHover, ErrorIcon);
        }

        public static MessageModel ErrorMessage(ErrorEnum errorType)
        {
            string message = errorType.GetErrorMessage();
            return message.CreateErrorMessage();
        }

        public static MessageModel ScanDone(int numberOfTrackImported)
        {
            string message;
            if (numberOfTrackImported == 0)
            {
                message = $"No new files were found.";
                return message.CreateWarningMessage(2);
            }
            else 
            { 
                message = $"{numberOfTrackImported} {Resources.Scan_Done_Msg}"; 
                return message.CreateSuccessMessage(4);
            }
        }

        public static MessageModel TrackDeleted(int numberOfTrackDeleted)
        {
            string message = $"{numberOfTrackDeleted} {Resources.TrackNotFoundMsg}";
            return message.CreateErrorMessage();
        }

        public static MessageModel DataBaseCleared()
        {
            string message = $"{Resources.DataBase_Cleared_Msg}";
            return message.CreateWarningMessage(2);
        }

        public static MessageModel AppThemeChanged(this string themeName)
        {
            string message = $"{Resources.Theme_Changed_Msg} {themeName}.";
            return message.CreateInfoMessage(0);
        }

        public static MessageModel AppThemeLightChange()
        {
            string message = $"It's time for a change of theme !";

            return message.CreateInfoMessage(0);
        }

        public static MessageModel StartingViewChanged(this string startingViewName)
        {
            string message = $"{Resources.Starting_View_Changed_Msg} {startingViewName}.";
            return message.CreateInfoMessage(5);
        }

        public static MessageModel LanguageChanged(this string language)
        {
            string message = $"{Resources.Language_Changed_Msg} {language}.";
            return message.CreateInfoMessage(3);
        }

        public static MessageModel PlaylistCreated(this string playlistName)
        {
            string message = $"{Resources.Playlist_Created_Msg} \"{playlistName}\"";
            return message.CreateSuccessMessage(6);
        }

        public static MessageModel PlaylistCreatedWithAction(this string playlistName, Action<bool> callback, string actionMessage)
        {
            string message = $"{Resources.Playlist_Created_Msg} \"{playlistName}\"";
            return message.CreateSuccessMessageWithAction(6, callback, actionMessage);
        }

        public static MessageModel TrackAddedToPlaylist(this string track, string playlistName)
        {
            string message = $"{Resources.The_Track} \"{track}\" {Resources.Has_been_Added_To} \"{playlistName}\"";
            return message.CreateInfoMessage(6);
        }

        public static MessageModel TracksAddedToPlaylist(this string playlistName, int trackNumber)
        {
            string message = $"{trackNumber} {Resources.Tracks} {Resources.Have_been_added_to} \"{playlistName}\"";
            return message.CreateInfoMessage(6);
        }

        public static MessageModel TrackRemovedFromPlaylist(this string track, string playlistName)
        {
            string message = $"{Resources.The_Track} \"{track}\" {Resources.Has_Been_Removed_From} \"{playlistName}\"";
            return message.CreateWarningMessage(7);
        }

        public static MessageModel CoverChangedMessage(this string name, bool artwork = false)
        {
            string message = $"{(artwork ? Resources.Artwork_Changed_Msg : Resources.Cover_Changed_Msg)} \"{name}\" ";
            return message.CreateSuccessMessage(1);
        }

        public static MessageModel CoverDeletedMessage(this string name, bool artwork)
        {
            string message = $"{(artwork ? Resources.ArtworkDeletedMsg : Resources.CoverDeletedMsg)} \"{name}\" ";
            return message.CreateWarningMessage(1);
        }

        public static MessageModel DataDeleted(this string name)
        {
            string message = $"\"{name}\" {Resources.Has_Been_Deleted}";
            return message.CreateWarningMessage(7);
        }

        public static MessageModel DataUpdated(this string name)
        {
            string message = $"{Resources.TheDataFor} \"{name}\" {Resources.Has_Been_Updated}";
            return message.CreateSuccessMessage(1);
        }

        public static MessageModel DataCreated(this string name, string type)
        {
            string message = $"1 {type} \"{name}\" {Resources.Has_Been_Created}";
            return message.CreateSuccessMessage(6);
        }

        public static MessageModel QueueChanged(this string name, bool added = true, bool album = false, bool artist = false)
        {
            string item = Resources.The_Track;
            if (album)
            {
                item = Resources.The_Album;
            }
            else if (artist)
            {
                item = Resources.The_Artist;
            }
            if (added)
            {
                string message = $"{item} \"{name}\" {Resources.Has_been_Added_To} {Resources.The_Queue}.";
                return message.CreateInfoMessage(6);
            }
            else
            {
                string message = $"{item} \"{name}\" {Resources.Has_Been_Removed_From} {Resources.The_Queue}.";
                return message.CreateWarningMessage(7);
            }
        }

        public static MessageModel TrackRemovedFromQueueWithUndo(this string name, Func<bool> callback)
        {
            string message = $"{Resources.The_Track} \"{name}\" {Resources.Has_Been_Removed_From} {Resources.The_Queue}.";
            return message.CreateWarningMessageWithUndo(7, callback);
        }

        private static MessageModel CreateErrorMessage(this string message)
        {
            MessageModel messageModel = new()
            {
                Message = message,
                Icon = ErrorIcon,
                Foreground = MessageColors.OnErrorContainer,
                IconBrush = MessageColors.OnErrorContainer,
                Background = MessageColors.ErrorContainer,
                MouseOverBackground = MessageColors.ErrorHover,
            };
            return messageModel;
        }

        private static MessageModel CreateSuccessMessage(this string message, int iconType)
        {
            MessageModel messageModel = new()
            {
                Message = message,
                Icon = GetIcon(iconType),
                Foreground = MessageColors.OnSuccessContainer,
                IconBrush = MessageColors.OnSuccessContainer,
                Background = MessageColors.SuccessContainer,
                MouseOverBackground = MessageColors.SuccessHover,
            };
            return messageModel;
        }

        private static MessageModel CreateSuccessMessageWithAction(this string message, int iconType, Action<bool> confirmCallBack, string confirmMessage)
        {
            MessageModel messageModel = new()
            {
                Message = message,
                Icon = GetIcon(iconType),
                Foreground = MessageColors.OnSuccessContainer,
                IconBrush = MessageColors.OnSuccessContainer,
                Background = MessageColors.SuccessContainer,
                MouseOverBackground = MessageColors.SuccessHover,
                IsInteractive = true,
                ConfirmCallBack = confirmCallBack,
                ConfirmMessage = confirmMessage,
                IsInteractiveWithCancel = false,
            };
            return messageModel;
        }

        private static MessageModel CreateInfoMessage(this string message, int iconType)
        {
            MessageModel messageModel = new()
            {
                Message = message,
                Icon = GetIcon(iconType),
                Foreground = MessageColors.OnInfoContainer,
                IconBrush = MessageColors.OnInfoContainer,
                Background = MessageColors.InfoContainer,
                MouseOverBackground = MessageColors.InfoHover,
            };
            return messageModel;
        }

        private static MessageModel CreateInfoMessageWithUndo(this string message, int iconType, Func<bool> CallBack)
        {
            MessageModel messageModel = new()
            {
                Message = message,
                Icon = GetIcon(iconType),
                Foreground = MessageColors.OnInfoContainer,
                IconBrush = MessageColors.OnInfoContainer,
                Background = MessageColors.InfoContainer,
                MouseOverBackground = MessageColors.InfoHover,
                IsUndoEnabled = true,
                UndoCallBack = CallBack,
                UndoMessage = "Undo"
            };
            return messageModel;
        }
        private static MessageModel CreateWarningMessage(this string message, int iconType)
        {
            MessageModel messageModel = new()
            {
                Message = message,
                Icon = GetIcon(iconType),
                Foreground = MessageColors.OnWarningContainer,
                IconBrush = MessageColors.OnWarningContainer,
                Background = MessageColors.WarningContainer,
                MouseOverBackground = MessageColors.WarningHover,
            };
            return messageModel;
        }

        private static MessageModel CreateWarningMessageWithUndo(this string message, int iconType, Func<bool> undoCallBack)
        {
            MessageModel messageModel = new()
            {
                Message = message,
                Icon = GetIcon(iconType),
                Foreground = MessageColors.OnWarningContainer,
                IconBrush = MessageColors.OnWarningContainer,
                Background = MessageColors.WarningContainer,
                MouseOverBackground = MessageColors.WarningHover,
                IsUndoEnabled = true,
                UndoCallBack = undoCallBack,
                UndoMessage = "Undo"
            };
            return messageModel;
        }

        private static PathGeometry GetIcon(int iconType)
        {
            switch (iconType)
            {
                case 0:
                    return AppThemeIcon;
                case 1:
                    return CoverChangedIcon;
                case 2:
                    return ErrorIcon;
                case 3:
                    return LanguageIcon;
                case 4:
                    return ImportLibraryDoneIcon;
                case 5:
                    return SettingIcon;
                case 6:
                    return CircledAddIcon;
                case 7:
                    return CircledRemoveIcon;
                default:
                    return SettingIcon;
            }
        }

        private static string GetErrorMessage(this ErrorEnum error)
        {
            switch (error)
            {
                case ErrorEnum.NoConnection:
                    return Resources.Error_No_Connection;
                case ErrorEnum.CorruptedFile:
                    return Resources.Error_Corrupted_File;
                case ErrorEnum.NotEnoughDataForRadio:
                    return Resources.Error_Not_Enough_Data_Radio;
                case ErrorEnum.RemoveTrackFromQueueError:
                    return "The Track couldn't be remove from the queue";
                case ErrorEnum.PlayingFromNotFound:
                    return "The queue playing from couldn't be found.";
                case ErrorEnum.TrackAlreadyInPlaylist:
                    return "The track is already in the playlist.";
                default:
                    return "An Unknown error has occured.";
            }
        }
    }
}