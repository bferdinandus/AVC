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

        private List<AudioDeviceModel> _audioDevices = new();
        private List<AudioSessionModel> _audioSessions = new();
        private int _deviceVolumeSliderValue;
        private Guid _deviceSelectionComboBoxSelectedValue;
        private int _appVolumeSlider1Value;
        private Guid _appSelectionComboBox1SelectedValue;

        public List<AudioDeviceModel> AudioDevices
        {
            get => _audioDevices;
            set => SetProperty(ref _audioDevices, value);
        }

        public List<AudioSessionModel> AudioSessions
        {
            get => _audioSessions;
            set => SetProperty(ref _audioSessions, value);
        }

        public int DeviceVolumeSliderValue
        {
            get => _deviceVolumeSliderValue;
            set
            {
                SetProperty(ref _deviceVolumeSliderValue, value);
                _audioService.SetDeviceVolume(DeviceSelectionComboBoxSelectedValue, DeviceVolumeSliderValue);
            }
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

        public int AppVolumeSlider1Value
        {
            get => _appVolumeSlider1Value;
            set
            {
                SetProperty(ref _appVolumeSlider1Value, value);
                //TODO: update app volume
                //_audioService.SetDeviceVolume(DeviceSelectionComboBoxSelectedValue, AppVolumeSlider1Value);
            }
        }

        public Guid AppSelectionComboBox1SelectedValue
        {
            get => _appSelectionComboBox1SelectedValue;
            set => SetProperty(ref _appSelectionComboBox1SelectedValue, value);
        }

        public VolumeSliderViewModel()
        {
            _audioService = new AudioService();
        }

        public override async Task Initialize()
        {
            await base.Initialize();

            AudioDevices = _audioService.GetActiveOutputDevices();
            DeviceSelectionComboBoxSelectedValue = AudioDevices.Single(a => a.Selected).Id;
        }

        public void AudioDeviceChanged()
        {
            _audioService.SelectDeviceById(DeviceSelectionComboBoxSelectedValue);
            DeviceVolumeSliderValue = AudioDevices.Single(a => a.Id == DeviceSelectionComboBoxSelectedValue).Volume;
            // todo: populate app selection 1 combobox
        }
    }
}