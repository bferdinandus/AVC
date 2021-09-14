using System;
using Prism.Mvvm;

namespace AVC.Core.Models
{
    public class AudioDeviceModel : BindableBase, IDisposable
    {
        private Guid _id;
        private string _fullName;
        private bool _muted;
        private string _iconPath;
        private IDisposable _volumeChangedSubscription;
        private int _volume;
        private bool _selected;

        public Guid Id {
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

        public IDisposable VolumeChangedSubscription {
            init => _volumeChangedSubscription = value;
        }

        public void Dispose()
        {
            _volumeChangedSubscription?.Dispose();
        }
    }
}