using System;
using CoreAudio;
using Prism.Mvvm;

namespace AVC.Core.Models
{
    public class AudioDeviceModel : BindableBase, IDisposable
    {
        private string _id;
        private string _fullName;
        private bool _muted;
        private string _iconPath;
        private int _volume;
        private bool _selected;

        private readonly AudioEndpointVolume _deviceAudioEndpointVolume;

        public string Id {
            get => _id;
            init => SetProperty(ref _id, value);
        }

        public string FullName {
            get => _fullName;
            init => SetProperty(ref _fullName, value);
        }

        public bool Muted {
            get => _muted;
            init => SetProperty(ref _muted, value);
        }

        public int Volume {
            get => _volume;
            set => SetProperty(ref _volume, value);
        }

        public bool Selected {
            get => _selected;
            set => SetProperty(ref _selected, value);
        }

        public string IconPath {
            get => _iconPath;
            init => SetProperty(ref _iconPath, value);
        }

        public AudioDeviceModel(MMDevice device)
        {
            _deviceAudioEndpointVolume = device.AudioEndpointVolume;

            Id = device.ID;
            FullName = device.FriendlyName;
            Selected = false;
            IconPath = device.IconPath;

            if (_deviceAudioEndpointVolume == null) {
                return;
            }

            Volume = (int) (_deviceAudioEndpointVolume.MasterVolumeLevelScalar * 100);
            Muted = _deviceAudioEndpointVolume.Mute;

            if (device.Properties != null && device.Properties.Contains(PKEY.PKEY_Device_DeviceDesc)) {
                FullName = (string) device.Properties[PKEY.PKEY_Device_DeviceDesc]?.Value;
            } else {
                FullName = device.FriendlyName;
            }

            _deviceAudioEndpointVolume.OnVolumeNotification += OnVolumeChangedEvent;
        }

        private void OnVolumeChangedEvent(AudioVolumeNotificationData data)
        {
            // update the model
            if (Volume != (int) (data.MasterVolume * 100)) {
                Volume = (int) (data.MasterVolume * 100);
            }
        }

        private void Dispose(bool disposing)
        {
            if (disposing) {
                _deviceAudioEndpointVolume?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~AudioDeviceModel()
        {
            Dispose(false);
        }
    }
}