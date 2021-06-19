using System;
using AudioSwitcher.AudioApi.Session;

namespace AVC.Core.Models
{
    public class AudioSessionModel
    {
        private string _displayName;
        public string Id { get; set; }

        public string DisplayName
        {
            get => IsSystemSoundsSession ? "System Sounds" : _displayName;
            set => _displayName = value;
        }

        public string IconPath { get; set; }
        public int ProcessId { get; set; }
        public AudioSessionState State { get; set; }
        public bool Muted { get; set; }
        public int Volume { get; set; }
        public bool IsSystemSoundsSession { get; set; }

        public bool Selected { get; set; }

    }
}