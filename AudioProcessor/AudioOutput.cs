using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using AudioEngine.Enums;
using AudioEngine.Models;
using ManagedBass;

namespace AudioEngine
{
    public static class AudioOutput
    {
        public static List<DeviceModel> GetAllDevices()
        {
            List<DeviceModel> devices = new();
            
            for(int i = 1; i < Bass.DeviceCount; i++)
            {
                DeviceInfo deviceInfo = Bass.GetDeviceInfo(i);
                if (deviceInfo.IsEnabled)
                {
                    DeviceTypeEnum deviceType = GetdeviceType(deviceInfo.Type);
                    devices.Add(new()
                    { 
                        Name = deviceInfo.Name, 
                        Type = deviceType, 
                        IsDefault = deviceInfo.IsDefault, 
                        IsInitialized = deviceInfo.IsInitialized, 
                        Index = i
                    });
                }
            }
            return devices;
        }


        public static DeviceModel GetDefaultdevice()
        {
            DeviceModel device = new();
            int deviceCount = Bass.DeviceCount;
            for (int i = 1; i < deviceCount; i++)
            {
                DeviceInfo deviceInfo = Bass.GetDeviceInfo(i);
                if (deviceInfo.IsDefault)
                {
                    DeviceTypeEnum deviceType = GetdeviceType(deviceInfo.Type);
                    device = new() 
                    { 
                        Name = deviceInfo.Name, 
                        Type = deviceType, 
                        IsDefault = deviceInfo.IsDefault, 
                        IsInitialized = deviceInfo.IsInitialized, 
                        Index = i 
                    };
                    return device;
                }
            }
            return device;
        }

        private static DeviceTypeEnum GetdeviceType(DeviceType type)
        {
            switch (type)
            {
                case DeviceType.Network:
                    return DeviceTypeEnum.Network;
                case DeviceType.Speakers:
                    return DeviceTypeEnum.Speakers;
                case DeviceType.Headphones:
                    return DeviceTypeEnum.HeadPhones;
                case DeviceType.Headset:
                    return DeviceTypeEnum.HeadSet;
                case DeviceType.SPDIF:
                    return DeviceTypeEnum.SPDIF;
                case DeviceType.HDMI:
                    return DeviceTypeEnum.HDMI;
                case DeviceType.Handset:
                    return DeviceTypeEnum.HeadSet;
                default:
                    return DeviceTypeEnum.UNKNOWN;
            }
        }

    }
}
