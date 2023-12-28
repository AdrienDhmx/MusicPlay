using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using AudioHandler.Enums;
using AudioHandler.Models;
using ManagedBass;
using MusicPlayModels.AudioModels;
using MusicPlayModels.Enums;

namespace AudioHandler
{
    public static class AudioOutput
    {
        /// <summary>
        /// Get all the available audio endpoints
        /// </summary>
        /// <returns></returns>
        public static List<AudioDeviceModel> GetAllDevices()
        {
            List<AudioDeviceModel> devices = new();
            
            for(int i = 1; i < Bass.DeviceCount; i++)
            {
                DeviceInfo deviceInfo = Bass.GetDeviceInfo(i);
                if (deviceInfo.IsEnabled)
                {
                    AudioDeviceTypeEnum deviceType = GetDeviceType(deviceInfo.Type);
                    devices.Add(new(deviceInfo.Name)
                    { 
                        Name = deviceInfo.Name, 
                        DeviceType = deviceType, 
                        IsDefault = deviceInfo.IsDefault, 
                        IsInitialized = deviceInfo.IsInitialized, 
                        Index = i
                    });
                }
            }
            return devices;
        }


        /// <summary>
        /// Get the default audio endpoint available
        /// </summary>
        /// <returns>The default device that has been found</returns>
        public static AudioDeviceModel GetDefaultDevice()
        {
            AudioDeviceModel device = null;
            int deviceCount = Bass.DeviceCount;
            for (int i = 1; i < deviceCount; i++)
            {
                DeviceInfo deviceInfo = Bass.GetDeviceInfo(i);
                if (deviceInfo.IsDefault)
                {
                    AudioDeviceTypeEnum deviceType = GetDeviceType(deviceInfo.Type);
                    device = new(deviceInfo.Name) 
                    { 
                        Name = deviceInfo.Name,
                        DeviceType = deviceType,
                        IsDefault = deviceInfo.IsDefault,
                        IsInitialized = deviceInfo.IsInitialized,
                        Index = i
                    };
                    return device;
                }
            }
            return device;
        }

        private static AudioDeviceTypeEnum GetDeviceType(DeviceType type)
        {
            return type switch
            {
                DeviceType.Network => AudioDeviceTypeEnum.Network,
                DeviceType.Speakers => AudioDeviceTypeEnum.Speakers,
                DeviceType.Headphones => AudioDeviceTypeEnum.HeadPhones,
                DeviceType.Headset => AudioDeviceTypeEnum.HeadSet,
                DeviceType.SPDIF => AudioDeviceTypeEnum.SPDIF,
                DeviceType.HDMI => AudioDeviceTypeEnum.HDMI,
                DeviceType.Handset => AudioDeviceTypeEnum.HeadSet,
                _ => AudioDeviceTypeEnum.UNKNOWN,
            };
        }

    }
}
