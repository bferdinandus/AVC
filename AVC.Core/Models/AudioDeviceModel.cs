using System;
using Prism.Mvvm;

namespace AVC.Core.Models
{
    public class AudioDeviceModel : BindableBase
    {
        private Guid _id;
        private string _fullName;
        private bool _muted;
        private int _volume;
        private bool _selected;
        private string _iconPath;

        public Guid Id {
            get => _id;
            set => SetProperty(ref _id, value);
        }

        public string FullName {
            get => _fullName;
            set => SetProperty(ref _fullName, value);
        }

        public bool Muted {
            get => _muted;
            set => SetProperty(ref _muted, value);
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
            set => SetProperty(ref _iconPath, value);
        }
    }
}