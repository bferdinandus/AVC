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
        private AudioDeviceModel _selectedDevice;

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

            // add "empty" item for testing
            // OutputDevices.Add(new AudioDeviceModel {FullName = "-"});

            foreach (AudioDeviceModel outputDevice in _audioService.GetActiveOutputDevices())
            {
                OutputDevices.Add(outputDevice);

                if (!outputDevice.Selected)
                {
                    continue;
                }

                _selectedDevice = outputDevice;

                // attach slider update function to the device update volume event
                _audioService.AttachOutputDeviceVolumeChanged(outputDevice.Id, UpdateSwitchOutputVolumeSlider);
                UpdateSwitchOutputVolumeSlider(outputDevice.Id);
            }

            SwitchOutputDropDown.SelectedItem = _selectedDevice;
        }

        private void SwitchOutputDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            if (!string.IsNullOrEmpty(_selectedDevice.Id))
            {
                // first detach the current selected output device
                _audioService.DetachOutputDeviceVolumeChanged(_selectedDevice.Id, UpdateSwitchOutputVolumeSlider);
            }

            // then get the new selected device
            _selectedDevice = (AudioDeviceModel) ((ComboBox)sender).SelectedItem;

            if (string.IsNullOrEmpty(_selectedDevice.Id))
            {
                SwitchOutputVolumeSlider.Value = 0;
                SwitchOutputVolumeSlider.Enabled = false;

                return;
            }

            SwitchOutputVolumeSlider.Enabled = true;

            // attach slider update function to the device update volume event
            _audioService.AttachOutputDeviceVolumeChanged(_selectedDevice.Id, UpdateSwitchOutputVolumeSlider);
            // activate the chosen device
            _audioService.SelectDeviceById(_selectedDevice.Id);
            UpdateSwitchOutputVolumeSlider(_selectedDevice.Id);
        }

        private void UpdateSwitchOutputVolumeSlider(string id)
        {
            if (SwitchOutputVolumeSlider.InvokeRequired)
            {
                SwitchOutputVolumeSlider.Invoke((MethodInvoker) (() => UpdateSwitchOutputVolumeSlider(id)));
            }
            else
            {
                SwitchOutputVolumeSlider.Value = _audioService.GetVolume(id);
            }
        }

        private void SwitchOutputVolumeSlider_Scroll(object sender, EventArgs e)
        {
            _audioService.SetVolume(_selectedDevice.Id, SwitchOutputVolumeSlider.Value);
        }


        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            //notifyIcon1.Visible = false;
        }

        private void ArduinoVolumeControl_Resize(object sender, EventArgs e)
        {
            //if the form is minimized
            //hide it from the task bar
            //and show the system tray icon (represented by the NotifyIcon control)
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
                //notifyIcon1.Visible = true;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}