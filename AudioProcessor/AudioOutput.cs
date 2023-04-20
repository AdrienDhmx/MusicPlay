using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using AudioHandler.Enums;
using AudioHandler.Models;
using ManagedBass;

namespace AudioHandler
{
    public static class AudioOutput
    {
        /// <summary>
        /// Get all the available audio endpoints
        /// </summary>
        /// <returns></returns>
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


        /// <summary>
        /// Get the default audio endpoint currently available
        /// </summary>
        /// <returns>The default device that has been found</returns>
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
            return type switch
            {
                DeviceType.Network => DeviceTypeEnum.Network,
                DeviceType.Speakers => DeviceTypeEnum.Speakers,
                DeviceType.Headphones => DeviceTypeEnum.HeadPhones,
                DeviceType.Headset => DeviceTypeEnum.HeadSet,
                DeviceType.SPDIF => DeviceTypeEnum.SPDIF,
                DeviceType.HDMI => DeviceTypeEnum.HDMI,
                DeviceType.Handset => DeviceTypeEnum.HeadSet,
                _ => DeviceTypeEnum.UNKNOWN,
            };
        }

    }
}
