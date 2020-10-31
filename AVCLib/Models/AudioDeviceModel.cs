using System;
using CoreAudio;

namespace AVCLib.Models
{
    public class AudioDeviceModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public bool Selected { get; set; }
        public bool Muted { get; set; }
        public int Volume { get; set; }
        public event Action<string> OutputDeviceVolumeChanged;

        public void UpdateVolume(AudioVolumeNotificationData data)
        {
            Volume = (int) (data.MasterVolume * 100);
            Muted = data.Muted;

            if (Selected) OutputDeviceVolumeChanged?.Invoke(Id);
        }
    }
}