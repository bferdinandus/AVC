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
        private AudioSessionModel _selectedAudioSession1;
        private AudioSessionModel _selectedAudioSession2;
        private AudioSessionModel _selectedAudioSession3;

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
            AudioSessionDropDown3.DataSource = AudioSessions3;
            AudioSessionDropDown3.DisplayMember = "DisplayName";
            AudioSessionVolumeSlider1.Value = 0;
            AudioSessionVolumeSlider1.Enabled = false;
            AudioSessionVolumeSlider2.Value = 0;
            AudioSessionVolumeSlider2.Enabled = false;
            AudioSessionVolumeSlider3.Value = 0;
            AudioSessionVolumeSlider3.Enabled = false;

            UpdateOutputDevices();
            UpdateAudioSessions();

            _initialized = true;
        }

        private BindingList<AudioDeviceModel> OutputDevices { get; } = new();
        private BindingList<AudioSessionModel> AudioSessions1 { get; } = new();
        private BindingList<AudioSessionModel> AudioSessions2 { get; } = new();
        private BindingList<AudioSessionModel> AudioSessions3 { get; } = new();

        private void UpdateAudioSessions()
        {
            AudioSessions1.Clear();
            AudioSessions2.Clear();
            AudioSessions3.Clear();

            // add "empty" item
            AudioSessions1.Add(new AudioSessionModel {DisplayName = "-"});
            AudioSessions2.Add(new AudioSessionModel {DisplayName = "-"});
            AudioSessions3.Add(new AudioSessionModel {DisplayName = "-"});

            foreach (AudioSessionModel session in _audioService.GetAudioSessionsForCurrentDevice())
            {
                AudioSessions1.Add(session);
                AudioSessions2.Add(session);
                AudioSessions3.Add(session);
            }

            _selectedAudioSession1 = AudioSessions1[0];
            _selectedAudioSession2 = AudioSessions2[0];
            _selectedAudioSession3 = AudioSessions3[0];
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

        # region DropDownsIndexChanged

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
            UpdateSwitchOutputVolumeSlider(_selectedDevice.Id);
        }

        private void AudioSessionDropDown1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            if (!string.IsNullOrEmpty(_selectedAudioSession1.SessionIdentifier))
            {
                // todo: detach the volume changed notifier
            }

            // then get the new selected audioSession
            _selectedAudioSession1 = (AudioSessionModel) ((ComboBox)sender).SelectedItem;

            if (string.IsNullOrEmpty(_selectedAudioSession1.SessionIdentifier))
            {
                AudioSessionVolumeSlider1.Value = 0;
                AudioSessionVolumeSlider1.Enabled = false;

                return;
            }

            AudioSessionVolumeSlider1.Enabled = true;

            // todo: attach the volume changed notifier

            UpdateAudioSessionVolumeSlider1(_selectedAudioSession1.SessionIdentifier);
        }

        private void AudioSessionDropDown2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            if (!string.IsNullOrEmpty(_selectedAudioSession2.SessionIdentifier))
            {
                // todo: detach the volume changed notifier
            }

            // then get the new selected audioSession
            _selectedAudioSession2 = (AudioSessionModel) ((ComboBox)sender).SelectedItem;

            if (string.IsNullOrEmpty(_selectedAudioSession2.SessionIdentifier))
            {
                AudioSessionVolumeSlider2.Value = 0;
                AudioSessionVolumeSlider2.Enabled = false;

                return;
            }

            AudioSessionVolumeSlider2.Enabled = true;

            // todo: attach the volume changed notifier

            UpdateAudioSessionVolumeSlider2(_selectedAudioSession2.SessionIdentifier);
        }

        private void AudioSessionDropDown3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            if (!string.IsNullOrEmpty(_selectedAudioSession3.SessionIdentifier))
            {
                // todo: detach the volume changed notifier
            }

            // then get the new selected audioSession
            _selectedAudioSession3 = (AudioSessionModel) ((ComboBox)sender).SelectedItem;

            if (string.IsNullOrEmpty(_selectedAudioSession3.SessionIdentifier))
            {
                AudioSessionVolumeSlider3.Value = 0;
                AudioSessionVolumeSlider3.Enabled = false;

                return;
            }

            AudioSessionVolumeSlider3.Enabled = true;

            // todo: attach the volume changed notifier

            UpdateAudioSessionVolumeSlider3(_selectedAudioSession3.SessionIdentifier);
        }

        # endregion DropDownsIndexChanged

        # region UpdateVolumeSliders

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
            /*if (AudioSessionVolumeSlider1.InvokeRequired)
            {
                AudioSessionVolumeSlider1.Invoke((MethodInvoker) (() => UpdateAudioSessionVolumeSlider1(id)));
            }
            else
            {
                AudioSessionVolumeSlider1.Value = _audioService.GetAudioSessionVolume(id);
            }*/
        }

        private void UpdateAudioSessionVolumeSlider2(string id)
        {
            /*if (AudioSessionVolumeSlider2.InvokeRequired)
            {
                AudioSessionVolumeSlider2.Invoke((MethodInvoker) (() => UpdateAudioSessionVolumeSlider2(id)));
            }
            else
            {
                AudioSessionVolumeSlider2.Value = _audioService.GetAudioSessionVolume(id);
            }*/
        }

        private void UpdateAudioSessionVolumeSlider3(string id)
        {
            /*if (AudioSessionVolumeSlider3.InvokeRequired)
            {
                AudioSessionVolumeSlider3.Invoke((MethodInvoker) (() => UpdateAudioSessionVolumeSlider3(id)));
            }
            else
            {
                AudioSessionVolumeSlider3.Value = _audioService.GetAudioSessionVolume(id);
            }*/
        }

        # endregion UpdateVolumeSliders

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