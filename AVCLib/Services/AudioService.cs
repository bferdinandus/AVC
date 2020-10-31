using System;
using System.Collections.Generic;
using System.Linq;
using AVCLib.Models;
using CoreAudio;

namespace AVCLib.Services
{
    public class AudioService
    {
        private readonly MMDeviceEnumerator _deviceEnumerator = new MMDeviceEnumerator();
        private readonly List<AudioDeviceModel> _deviceModels = new List<AudioDeviceModel>();

        public List<AudioDeviceModel> GetActiveOutputDevices()
        {
            MMDeviceCollection endPoints = _deviceEnumerator.EnumerateAudioEndPoints(EDataFlow.eRender, DEVICE_STATE.DEVICE_STATE_ACTIVE);

            foreach (MMDevice endPoint in endPoints)
            {
                AudioDeviceModel device = new AudioDeviceModel
                {
                    Id = endPoint.ID,
                    FullName = endPoint.FriendlyName,
                    Selected = endPoint.Selected,
                    Volume = (int) (endPoint.AudioEndpointVolume.MasterVolumeLevelScalar * 100),
                    Muted = endPoint.AudioEndpointVolume.Mute
                };
                endPoint.AudioEndpointVolume.OnVolumeNotification += device.UpdateVolume;

                _deviceModels.Add(device);
            }

            return _deviceModels;
        }

        public AudioDeviceModel GetSelectedDevice()
        {
            MMDevice device = _deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);

            AudioDeviceModel model = _deviceModels.Single(m => m.Id == device.ID);

            return model;
        }

        public void SelectDeviceById(string id)
        {
            // mark al local models as not selected
            _deviceModels.ForEach(m => m.Selected = false);
            // mark requested model as selected
            _deviceModels.Single(m => m.Id == id).Selected = true;
            // select the output device
            _deviceEnumerator.GetDevice(id).Selected = true;
        }

        public void SetVolume(string id, int value)
        {
            //_deviceModels.Single(m => m.Id == id).Selected = true;
            _deviceEnumerator.GetDevice(id).AudioEndpointVolume.MasterVolumeLevelScalar = value / 100F;
        }

        public int GetVolume(string id)
        {
            return _deviceModels.Single(m => m.Id == id).Volume;
        }

        public void AttachOutputDeviceVolumeChanged(string id, Action<string> callbackFunction)
        {
            _deviceModels.Single(m => m.Id == id).OnOutputDeviceVolumeChanged += callbackFunction;
        }

        public void DetachOutputDeviceVolumeChanged(string id, Action<string> callbackFunction)
        {
            _deviceModels.Single(m => m.Id == id).OnOutputDeviceVolumeChanged -= callbackFunction;
        }
    }
}