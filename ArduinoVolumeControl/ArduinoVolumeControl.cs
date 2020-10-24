using System;
using System.ComponentModel;
using System.Windows.Forms;
using AVCLib.Models;
using AVCLib.Services;

namespace ArduinoVolumeControl
{
    public partial class ArduinoVolumeControl : Form
    {
        private readonly AudioService _audioService;

        private BindingList<AudioDeviceModel> OutputDevices { get; } = new BindingList<AudioDeviceModel>();

        public ArduinoVolumeControl(AudioService audioService)
        {
            _audioService = audioService;

            InitializeComponent();

            SwitchOutputDropDown.DataSource = OutputDevices;
            SwitchOutputDropDown.DisplayMember = "FullName";

            UpdateOutputDevices();
        }

        private void UpdateOutputDevices()
        {
            OutputDevices.Clear();

            int selectedDeviceIndex = 0;
            foreach (AudioDeviceModel outputDevice in _audioService.GetActiveOutputDevices())
            {
                OutputDevices.Add(outputDevice);

                if (outputDevice.Selected)
                {
                    selectedDeviceIndex = OutputDevices.Count-1;
                }
            }
            SwitchOutputDropDown.SelectedIndex = selectedDeviceIndex;
        }

        private void SwitchOutputDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            AudioDeviceModel selectedDevice = (AudioDeviceModel) SwitchOutputDropDown.SelectedItem;

            _audioService.SelectDeviceById(selectedDevice.Id);
            UpdateSwitchOutputVolumeSlider();
        }

        private void UpdateSwitchOutputVolumeSlider()
        {
            AudioDeviceModel device = _audioService.GetSelectedDevice();
            SwitchOutputVolumeSlider.Value = (int)(device.Volume * 100);
        }

        private void SwitchOutputVolumeSlider_Scroll(object sender, EventArgs e)
        {
            AudioDeviceModel device = _audioService.GetSelectedDevice();
            _audioService.SetVolume(device.Id, SwitchOutputVolumeSlider.Value / 100f);
        }
    }
}