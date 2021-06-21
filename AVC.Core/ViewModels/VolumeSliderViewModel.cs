using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AVC.Core.Models;
using AVC.Core.Services;
using MvvmCross.ViewModels;

namespace AVC.Core.ViewModels
{
    public class VolumeSliderViewModel : MvxViewModel
    {
        private readonly AudioService _audioService;

        private ObservableCollection<AudioDeviceModel> _audioDevices = new();
        private ObservableCollection<AudioSessionModel> _audioSessions = new();
        private int _deviceVolumeSliderValue;
        private Guid _deviceSelectionComboBoxSelectedValue;
        private int _appVolumeSlider1Value;
        private string _appSelectionComboBox1SelectedValue;

        public ObservableCollection<AudioDeviceModel> AudioDevices
        {
            get => _audioDevices;
            set => SetProperty(ref _audioDevices, value);
        }

        public ObservableCollection<AudioSessionModel> AudioSessions
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
                _audioService.SetSessionVolume(AppSelectionComboBox1SelectedValue, AppVolumeSlider1Value);
            }
        }

        public string AppSelectionComboBox1SelectedValue
        {
            get => _appSelectionComboBox1SelectedValue;
            set
            {
                SetProperty(ref _appSelectionComboBox1SelectedValue, value);
                AppSessionChanged(value);
            }
        }

        public VolumeSliderViewModel()
        {
            _audioService = new AudioService();
        }

        public override async Task Initialize()
        {
            await base.Initialize();

            AudioDevices = new ObservableCollection<AudioDeviceModel>(_audioService.GetActiveOutputDevices());
            DeviceSelectionComboBoxSelectedValue = AudioDevices.Single(a => a.Selected).Id;
        }

        public void AudioDeviceChanged()
        {
            _audioService.SelectDeviceById(DeviceSelectionComboBoxSelectedValue);
            DeviceVolumeSliderValue = AudioDevices.Single(a => a.Id == DeviceSelectionComboBoxSelectedValue).Volume;

            AudioSessions = new ObservableCollection<AudioSessionModel>(_audioService.GetAudioSessionsForDevice(DeviceSelectionComboBoxSelectedValue));
        }

        private void AppSessionChanged(string value)
        {
            AudioSessionModel session = AudioSessions.Single(a => a.Id == value);
            AppVolumeSlider1Value = session.Volume;
        }
    }
}