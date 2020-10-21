using System.Collections.Generic;
using AVCLib.Models;
using CoreAudio;

namespace AVCLib.Services
{
    public class AudioService
    {
        MMDeviceEnumerator _deviceEnumerator = new MMDeviceEnumerator();

        public AudioService()
        {
        }

        public List<AudioDeviceModel> GetActiveDevices()
        {
            MMDeviceCollection endPoints = _deviceEnumerator.EnumerateAudioEndPoints(EDataFlow.eRender, DEVICE_STATE.DEVICE_STATE_ACTIVE);
            List<AudioDeviceModel> deviceModels = new List<AudioDeviceModel>();

            foreach (MMDevice endPoint in endPoints)
            {
                deviceModels.Add(new AudioDeviceModel
                {
                    Id = endPoint.ID,
                    FullName = endPoint.FriendlyName,
                    Active = endPoint.Selected,
                });
            }


            return deviceModels;
        }

        public AudioDeviceModel GetActiveDevice()
        {
            MMDevice device = _deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);

            return new AudioDeviceModel
            {
                Id = device.ID,
                FullName = device.FriendlyName,
                Active = device.Selected,
            };
        }

        public void SelectDeviceById(string id) => _deviceEnumerator.GetDevice(id).Selected = true;
    }
}