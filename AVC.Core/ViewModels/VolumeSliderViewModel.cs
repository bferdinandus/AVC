using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AVC.Core.Models;
using AVC.Core.Services;
using MvvmCross.Commands;
using MvvmCross.ViewModels;

namespace AVC.Core.ViewModels
{
    public class VolumeSliderViewModel : MvxViewModel
    {
        private readonly AudioService _audioService;

        private int _deviceVolumeSliderValue = 100;
        private int _deviceSelectionComboBoxSelectedIndex = 2;
        private Guid _deviceSelectionComboBoxSelectedValue;
        private List<AudioDeviceModel> _audioDevices = new();

        public int DeviceVolumeSliderValue
        {
            get => _deviceVolumeSliderValue;
            set
            {
                SetProperty(ref _deviceVolumeSliderValue, value);
                _audioService.SetDeviceVolume(DeviceSelectionComboBoxSelectedValue, DeviceVolumeSliderValue);
            }
        }

        public int DeviceSelectionComboBoxSelectedIndex
        {
            get => _deviceSelectionComboBoxSelectedIndex;
            set => SetProperty(ref _deviceSelectionComboBoxSelectedIndex, value);
        }

        public Guid DeviceSelectionComboBoxSelectedValue
        {
            get => _deviceSelectionComboBoxSelectedValue;
            set
            {
                SetProperty(ref _deviceSelectionComboBoxSelectedValue, value);
                AudioDeviceChanged();
            }
        }

        public List<AudioDeviceModel> AudioDevices
        {
            get => _audioDevices;
            set => SetProperty(ref _audioDevices, value);
        }

        public IMvxCommand AudioDeviceChangedCommand { get; set; }

        public VolumeSliderViewModel()
        {
            _audioService = new AudioService();
        }

        public override async Task Initialize()
        {
            await base.Initialize();

            AudioDevices = _audioService.GetActiveOutputDevices();
            AudioDeviceChangedCommand = new MvxCommand(AudioDeviceChanged);
        }

        public void AudioDeviceChanged()
        {
            _audioService.SelectDeviceById(DeviceSelectionComboBoxSelectedValue);
            DeviceVolumeSliderValue = AudioDevices.Single(a => a.Id == DeviceSelectionComboBoxSelectedValue).Volume;
        }
    }
}