using AudioEngine.Enums;
using AudioEngine.Models;
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
        public static FullDeviceModel CreateDeviceModal(this DeviceModel device)
        {
            return new(device, GetIcon(device.Type));
        }

        public static List<FullDeviceModel> CreateDeviceModel(this List<DeviceModel> devices)
        {
            List<FullDeviceModel> result = new List<FullDeviceModel>();
            foreach (var d in devices)
            {
                result.Add(d.CreateDeviceModal());
            }
            return result;
        }

        public static DeviceModel ToDeviceModel(this FullDeviceModel fullDeviceModel)
        {
            DeviceModel output = new();
            output.Type = fullDeviceModel.Type;
            output.Name = fullDeviceModel.Name;
            output.IsDefault = fullDeviceModel.IsDefault;
            output.IsInitialized = fullDeviceModel.IsInitialized;
            output.Index = fullDeviceModel.Index;
            return output;
        }

        private static Geometry GetIcon(DeviceTypeEnum deviceType)
        {
            switch (deviceType)
            {
                case DeviceTypeEnum.UNKNOWN:
                    return (PathGeometry)App.IconDic["UnknownDevice"];
                case DeviceTypeEnum.HeadPhones:
                    return (PathGeometry)App.IconDic["Headphones"];
                case DeviceTypeEnum.HeadSet:
                    return (PathGeometry)App.IconDic["Headset"];
                case DeviceTypeEnum.Speakers:
                    return (Geometry)App.IconDic["Speakers"];
                case DeviceTypeEnum.HDMI:
                    return (PathGeometry)App.IconDic["HDMI"];
                case DeviceTypeEnum.Network:
                    return (PathGeometry)App.IconDic["UnknownDevice"];
                case DeviceTypeEnum.SPDIF:
                    return (PathGeometry)App.IconDic["SPDIF"];
                default:
                    return (PathGeometry)App.IconDic["UnknownDevice"];
            }
        }
    }
}
