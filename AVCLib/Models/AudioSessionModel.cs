using System;
using System.Diagnostics.CodeAnalysis;
using AudioSwitcher.AudioApi.Session;

namespace AVCLib.Models
{
    public class AudioSessionModel
    {
        public int ProcessId { get; set; }
        private string _displayName;
        public bool Muted { get; set; }
        public int Volume { get; set; }

        [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Wordt gebruikt door de ComboBox")]
        public string DisplayName
        {
            get => IsSystemSoundsSession ? "System Sounds" : _displayName;
            set => _displayName = value;
        }

        //public AudioSessionState State { get; set; }
        public string IconPath { get; set; }
        public string SessionIdentifier { get; set; }
        public string SessionInstanceIdentifier { get; set; }
        public bool IsSystemSoundsSession { get; set; }
        public AudioSessionState State { get; set; }
        public string Id { get; set; }

        // public event Action<string> OnSessionDeviceVolumeChanged;

        /*
        public void UpdateVolume(AudioVolumeNotificationData data)
        {
            Volume = (int) (data.MasterVolume * 100);
            Muted = data.Muted;

            // if (Selected)
            // {
            //     OnSessionDeviceVolumeChanged?.Invoke(Id);
            // }
        }
        */

        public void ChannelVolumeChanged(object sender, int channelcount, float[] newvolume, int changedchannel)
        {
            Console.WriteLine($"ChannelVolumeChanged: {sender} {channelcount} {newvolume} {changedchannel}");
        }

        public void SimpleVolumeChanged(object sender, float newVolume, bool newMute)
        {
            Volume = (int) (newVolume * 100);
            Muted = newMute;
            Console.WriteLine($"SimpleVolumeChanged: {sender} {newVolume} {newMute}");
        }

        // public void StateChanged(object sender, AudioSessionState newstate)
        // {
        //     Console.WriteLine($"StateChanged: {sender} {newstate}");
        // }
    }
}