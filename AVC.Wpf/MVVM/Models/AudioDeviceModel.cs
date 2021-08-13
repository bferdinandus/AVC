using System;

namespace AVC.Wpf.MVVM.Models
{
    public class AudioDeviceModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public bool Muted { get; set; }
        public int Volume { get; set; }
        public bool Selected { get; set; }
        public string IconPath { get; set; }
    }
}