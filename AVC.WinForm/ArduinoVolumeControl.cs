using System;
using System.ComponentModel;
using System.Windows.Forms;
using AVC.Core.Models;
using AVC.Core.Services;

namespace AVC.WinForm
{
    public partial class ArduinoVolumeControl : Form
    {
        private readonly AudioService _audioService;
        private readonly bool _initialized;
        private AudioDeviceModel _selectedDevice;
        private AudioSessionModel _selectedAudioSession1;
        private AudioSessionModel _selectedAudioSession2;

        public ArduinoVolumeControl(AudioService audioService)
        {
            _audioService = audioService;

            InitializeComponent();

            SwitchOutputDropDown.DataSource = OutputDevices;
            SwitchOutputDropDown.DisplayMember = "FullName";
            AudioSessionDropDown1.DataSource = AudioSessions1;
            AudioSessionDropDown1.DisplayMember = "DisplayName";
            AudioSessionDropDown2.DataSource = AudioSessions2;
            AudioSessionDropDown2.DisplayMember = "DisplayName";
            AudioSessionVolumeSlider1.Value = 0;
            AudioSessionVolumeSlider1.Enabled = false;
            AudioSessionVolumeSlider2.Value = 0;
            AudioSessionVolumeSlider2.Enabled = false;

            UpdateOutputDevices();
            UpdateAudioSessions();

            _initialized = true;
        }

        private BindingList<AudioDeviceModel> OutputDevices { get; } = new();
        private BindingList<AudioSessionModel> AudioSessions1 { get; } = new();
        private BindingList<AudioSessionModel> AudioSessions2 { get; } = new();

        private void UpdateAudioSessions()
        {
            AudioSessions1.Clear();
            AudioSessions2.Clear();

            // add "empty" item
            AudioSessions1.Add(new AudioSessionModel {DisplayName = "-"});
            AudioSessions2.Add(new AudioSessionModel {DisplayName = "-"});

            foreach (AudioSessionModel session in _audioService.GetAudioSessionsForCurrentDevice())
            {
                AudioSessions1.Add(session);
                AudioSessions2.Add(session);
            }

            _selectedAudioSession1 = AudioSessions1[0];
            _selectedAudioSession2 = AudioSessions2[0];
        }

        private void UpdateOutputDevices()
        {
            OutputDevices.Clear();

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

            if (_selectedDevice != null)
            {
                // first detach the current selected output device
                _audioService.DetachOutputDeviceVolumeChanged(_selectedDevice.Id, UpdateSwitchOutputVolumeSlider);
            }

            // then get the new selected device
            _selectedDevice = (AudioDeviceModel) ((ComboBox)sender).SelectedItem;

            if (_selectedDevice == null)
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
            // update the audio sessions
            UpdateAudioSessions();

            //update the slider
            UpdateSwitchOutputVolumeSlider(_selectedDevice.Id);
        }

        private void AudioSessionDropDown1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            if (_selectedAudioSession1.Id != null)
            {
                // first detach the current selected session
                _audioService.DetachSessionVolumeChanged(_selectedAudioSession1.Id, UpdateAudioSessionVolumeSlider1);
                _selectedAudioSession1.Selected = false;
            }

            // then get the new selected audioSession
            _selectedAudioSession1 = (AudioSessionModel) ((ComboBox)sender).SelectedItem;
            _selectedAudioSession1.Selected = true;

            if (_selectedAudioSession1.Id == null)
            {
                AudioSessionVolumeSlider1.Value = 0;
                AudioSessionVolumeSlider1.Enabled = false;
            }
            else
            {
                AudioSessionVolumeSlider1.Enabled = true;
                // attach slider update function to the session update volume event
                _audioService.AttachSessionVolumeChanged(_selectedAudioSession1.Id, UpdateAudioSessionVolumeSlider1);

                UpdateAudioSessionVolumeSlider1(_selectedAudioSession1.Id);
            }
        }

        private void AudioSessionDropDown2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            if (_selectedAudioSession2.Id != null)
            {
                // first detach the current selected session
                _audioService.DetachSessionVolumeChanged(_selectedAudioSession2.Id, UpdateAudioSessionVolumeSlider2);
                _selectedAudioSession2.Selected = false;
            }

            // then get the new selected audioSession
            _selectedAudioSession2 = (AudioSessionModel) ((ComboBox)sender).SelectedItem;

            if (_selectedAudioSession2.Id == null)
            {
                AudioSessionVolumeSlider2.Value = 0;
                AudioSessionVolumeSlider2.Enabled = false;
            }
            else
            {
                AudioSessionVolumeSlider2.Enabled = true;
                // attach slider update function to the session update volume event
                _audioService.AttachSessionVolumeChanged(_selectedAudioSession2.Id, UpdateAudioSessionVolumeSlider2);

                UpdateAudioSessionVolumeSlider2(_selectedAudioSession2.Id);
            }
        }

        private void UpdateSwitchOutputVolumeSlider(Guid id)
        {
            if (SwitchOutputVolumeSlider.InvokeRequired)
            {
                SwitchOutputVolumeSlider.Invoke((MethodInvoker) (() => UpdateSwitchOutputVolumeSlider(id)));
            }
            else
            {
                SwitchOutputVolumeSlider.Value = _audioService.GetDeviceVolume(id);
            }
        }

        private void UpdateAudioSessionVolumeSlider1(string id)
        {
            if (AudioSessionVolumeSlider1.InvokeRequired)
            {
                AudioSessionVolumeSlider1.Invoke((MethodInvoker) (() => UpdateAudioSessionVolumeSlider1(id)));
            }
            else
            {
                AudioSessionVolumeSlider1.Value = _audioService.GetAudioSessionVolume(id);
            }
        }

        private void UpdateAudioSessionVolumeSlider2(string id)
        {
            if (AudioSessionVolumeSlider2.InvokeRequired)
            {
                AudioSessionVolumeSlider2.Invoke((MethodInvoker) (() => UpdateAudioSessionVolumeSlider2(id)));
            }
            else
            {
                AudioSessionVolumeSlider2.Value = _audioService.GetAudioSessionVolume(id);
            }
        }

        private void SwitchOutputVolumeSlider_Scroll(object sender, EventArgs e)
        {
            _audioService.SetDeviceVolume(_selectedDevice.Id, SwitchOutputVolumeSlider.Value);
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

        private void AudioSessionVolumeSlider1_Scroll(object sender, EventArgs e)
        {
            _audioService.SetSessionVolume(_selectedAudioSession1.Id, AudioSessionVolumeSlider1.Value);
        }

        private void AudioSessionVolumeSlider2_Scroll(object sender, EventArgs e)
        {
            _audioService.SetSessionVolume(_selectedAudioSession2.Id, AudioSessionVolumeSlider2.Value);
        }
    }
}