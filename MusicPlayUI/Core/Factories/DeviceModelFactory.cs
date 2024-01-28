﻿using MusicPlay.Database.Enums;
using MusicPlay.Database.Models.AudioModels;
using MusicPlayUI.MVVM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Windows.Data.Text;
using Windows.Security.Authentication.OnlineId;

namespace MusicPlayUI.Core.Factories
{
    public static class DeviceModelFactory
    {
        public static FullDeviceModel CreateDeviceModal(this AudioDeviceModel device)
        {
            return new(device, GetIcon(device.DeviceType));
        }

        public static List<FullDeviceModel> CreateDeviceModel(this List<AudioDeviceModel> devices)
        {
            List<FullDeviceModel> result = new List<FullDeviceModel>();
            foreach (var d in devices)
            {
                result.Add(d.CreateDeviceModal());
            }
            return result;
        }

        public static AudioDeviceModel ToDeviceModel(this FullDeviceModel fullDeviceModel)
        {
            AudioDeviceModel output = new(fullDeviceModel.Name);
            output.DeviceType = fullDeviceModel.Type;
            output.Name = fullDeviceModel.Name;
            output.IsDefault = fullDeviceModel.IsDefault;
            output.IsInitialized = fullDeviceModel.IsInitialized;
            output.Index = fullDeviceModel.Index;
            return output;
        }

        private static Geometry GetIcon(AudioDeviceTypeEnum deviceType)
        {
            switch (deviceType)
            {
                case AudioDeviceTypeEnum.UNKNOWN:
                    return (PathGeometry)App.IconDic["UnknownDevice"];
                case AudioDeviceTypeEnum.HeadPhones:
                    return (PathGeometry)App.IconDic["Headphones"];
                case AudioDeviceTypeEnum.HeadSet:
                    return (PathGeometry)App.IconDic["Headset"];
                case AudioDeviceTypeEnum.Speakers:
                    return (Geometry)App.IconDic["Speakers"];
                case AudioDeviceTypeEnum.HDMI:
                    return (PathGeometry)App.IconDic["HDMI"];
                case AudioDeviceTypeEnum.Network:
                    return (PathGeometry)App.IconDic["UnknownDevice"];
                case AudioDeviceTypeEnum.SPDIF:
                    return (PathGeometry)App.IconDic["SPDIF"];
                default:
                    return (PathGeometry)App.IconDic["UnknownDevice"];
            }
        }
    }
}
