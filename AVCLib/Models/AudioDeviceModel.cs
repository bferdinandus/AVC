using System;

namespace AVCLib.Models
{
    public class AudioDeviceModel
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public bool Selected { get; set; }
        public bool Muted { get; set; }
        public int Volume { get; set; }
        public event Action<Guid> OnOutputDeviceVolumeChanged;

        public bool UpdateVolume(double volume)
        {
            Volume = (int) volume;

            if (Selected)
            {
                OnOutputDeviceVolumeChanged?.Invoke(Id);
            }

            return true;
        }
    }
}