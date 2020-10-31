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
        private readonly bool _initialized;

        public ArduinoVolumeControl(AudioService audioService)
        {
            _audioService = audioService;

            InitializeComponent();

            SwitchOutputDropDown.DataSource = OutputDevices;
            SwitchOutputDropDown.DisplayMember = "FullName";

            UpdateOutputDevices();

            _initialized = true;
        }

        private BindingList<AudioDeviceModel> OutputDevices { get; } = new BindingList<AudioDeviceModel>();

        private void UpdateOutputDevices()
        {
            OutputDevices.Clear();

            int selectedDeviceIndex = 0;
            foreach (AudioDeviceModel outputDevice in _audioService.GetActiveOutputDevices())
            {
                OutputDevices.Add(outputDevice);

                if (outputDevice.Selected)
                {
                    selectedDeviceIndex = OutputDevices.Count - 1;

                    // attach slider update function to the device update volume event
                    _audioService.AttachFormUpdateFunction(outputDevice.Id, UpdateSwitchOutputVolumeSlider);
                    UpdateSwitchOutputVolumeSlider(outputDevice.Id);
                }
            }

            SwitchOutputDropDown.SelectedIndex = selectedDeviceIndex;
        }

        private void SwitchOutputDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            AudioDeviceModel selectedDevice = (AudioDeviceModel) SwitchOutputDropDown.SelectedItem;

            if (_initialized)
            {
                // activate the system output device only after the initialisation
                _audioService.SelectDeviceById(selectedDevice.Id);
            }

            // attach slider update function to the device update volume event
            _audioService.AttachFormUpdateFunction(selectedDevice.Id, UpdateSwitchOutputVolumeSlider);
            UpdateSwitchOutputVolumeSlider(selectedDevice.Id);
        }

        private void UpdateSwitchOutputVolumeSlider(string selectedDeviceId)
        {
            if (SwitchOutputVolumeSlider.InvokeRequired)
            {
                SwitchOutputVolumeSlider.Invoke((MethodInvoker) (() => UpdateSwitchOutputVolumeSlider(selectedDeviceId)));
            }
            else
            {
                SwitchOutputVolumeSlider.Value = _audioService.GetVolume(selectedDeviceId);
            }
        }

        private void SwitchOutputVolumeSlider_Scroll(object sender, EventArgs e)
        {
            _audioService.SetVolume(((AudioDeviceModel) SwitchOutputDropDown.SelectedItem).Id, SwitchOutputVolumeSlider.Value);
        }
    }
}