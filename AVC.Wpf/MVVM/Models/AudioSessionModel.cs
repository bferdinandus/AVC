using CoreAudio;

namespace AVC.Wpf.MVVM.Models
{
    public class AudioSessionModel
    {
        private string _displayName;

        public string DisplayName {
            get => IsSystemSoundsSession ? "System Sound" : _displayName;
            set => _displayName = value;
        }

        public string Id { get; set; }
        public string IconPath { get; set; }
        public uint ProcessId { get; set; }
        public AudioSessionState State { get; set; }
        public bool Muted { get; set; }
        public int Volume { get; set; }
        public bool IsSystemSoundsSession { get; set; }
        public bool Selected { get; set; }
    }
}