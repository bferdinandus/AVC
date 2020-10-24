using System.Collections.Generic;
using AVCLib.Models;
using CoreAudio;

namespace AVCLib.Services
{
    public class AudioService
    {
        private readonly MMDeviceEnumerator _deviceEnumerator = new MMDeviceEnumerator();

        public AudioService()
        {
        }

        public List<AudioDeviceModel> GetActiveOutputDevices()
        {
            MMDeviceCollection endPoints = _deviceEnumerator.EnumerateAudioEndPoints(EDataFlow.eRender, DEVICE_STATE.DEVICE_STATE_ACTIVE);
            List<AudioDeviceModel> deviceModels = new List<AudioDeviceModel>();

            foreach (MMDevice endPoint in endPoints)
            {
                deviceModels.Add(new AudioDeviceModel
                {
                    Id = endPoint.ID,
                    FullName = endPoint.FriendlyName,
                    Selected = endPoint.Selected,
                });
            }

            return deviceModels;
        }

        public AudioDeviceModel GetSelectedDevice()
        {
            MMDevice device = _deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);

            return new AudioDeviceModel
            {
                Id = device.ID,
                FullName = device.FriendlyName,
                Selected = device.Selected,
                Volume = device.AudioEndpointVolume.MasterVolumeLevelScalar,
                Mute = device.AudioEndpointVolume.Mute
            };
        }

        public void SelectDeviceById(string id) => _deviceEnumerator.GetDevice(id).Selected = true;

        public void SetVolume(string id, float value)
        {
            _deviceEnumerator.GetDevice(id).AudioEndpointVolume.MasterVolumeLevelScalar = value;
        }
    }
}