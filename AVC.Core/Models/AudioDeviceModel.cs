using System;

namespace AVC.Core.Models
{
    public class AudioDeviceModel
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public bool Muted { get; set; }
        public int Volume { get; set; }
        public bool Selected { get; set; }
        public string IconPath { get; set; }
    }
}