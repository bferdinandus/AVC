using System;
using System.Collections.Generic;
using System.Linq;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using AudioSwitcher.AudioApi.Observables;
using AudioSwitcher.AudioApi.Session;
using AVC.Core.Models;

namespace AVC.Core.Services
{
    public class AudioService
    {
        private readonly List<AudioDeviceModel> _outputDevices = new();
        private readonly List<AudioSessionModel> _audioSessions = new();
        private readonly CoreAudioController _audioController;

        public AudioService()
        {
            _audioController = new CoreAudioController();
        }

        public List<AudioDeviceModel> GetActiveOutputDevices()
        {
            IEnumerable<CoreAudioDevice> devices = _audioController.GetDevices(DeviceType.Playback, DeviceState.Active);

            foreach (CoreAudioDevice device in devices)
            {
                AudioDeviceModel deviceModel = new()
                {
                    Id = device.Id,
                    FullName = device.Name,
                    Selected = device.IsDefaultDevice,
                    Volume = (int) device.Volume,
                    Muted = device.IsMuted,
                    IconPath = device.IconPath
                };

                // update the model (and somehow tell the viewModel)
                device.VolumeChanged.When(vc =>
                {
                    deviceModel.Volume = (int) vc.Device.Volume;
                    return true;
                });

                _outputDevices.Add(deviceModel);
            }

            return _outputDevices;
        }

        public List<AudioSessionModel> GetAudioSessionsForCurrentDevice()
        {
            return GetAudioSessionsForDevice(_outputDevices.Single(m => m.Selected).Id);
        }

        public List<AudioSessionModel> GetAudioSessionsForDevice(Guid id)
        {
            _audioSessions.Clear();

            CoreAudioDevice audioDevice = _audioController.GetDevice(id);
            IEnumerable<IAudioSession> sessions = audioDevice.GetCapability<IAudioSessionController>().All();

            foreach (IAudioSession session in sessions)
            {
                AudioSessionModel sessionModel = new()
                {
                    Id = session.Id,
                    DisplayName = session.DisplayName,
                    IconPath = session.IconPath,
                    State = session.SessionState,
                    ProcessId = session.ProcessId,
                    IsSystemSoundsSession = session.IsSystemSession,
                    Volume = (int) session.Volume,
                    Muted = session.IsMuted
                };
                // update the model (and somehow tell the viewModel)
                session.VolumeChanged.When(vc =>
                {
                    sessionModel.Volume = (int) vc.Session.Volume;
                    return true;
                });

                _audioSessions.Add(sessionModel);
            }

            return _audioSessions;
        }

        public void SelectDeviceById(Guid id)
        {
            // mark all local models as not selected
            _outputDevices.ForEach(m => m.Selected = false);
            // mark requested model as selected
            _outputDevices.Single(m => m.Id == id).Selected = true;
            // select the output device
            _audioController.GetDevice(id).SetAsDefault();
        }

        public void SetDeviceVolume(Guid id, int value)
        {
            _audioController.GetDevice(id).SetVolumeAsync(value);
        }

        public int GetDeviceVolume(Guid id)
        {
            return _outputDevices.Single(m => m.Id == id).Volume;
        }

        public int GetAudioSessionVolume(string id)
        {
            return _audioSessions.Single(m => m.Id == id).Volume;
        }

        public void SetSessionVolume(string id, int value)
        {
            CoreAudioDevice audioDevice = _audioController.GetDevice(_outputDevices.Single(m => m.Selected).Id);
            IAudioSession session = audioDevice.GetCapability<IAudioSessionController>().All().Single(s => s.Id == id);
            session.SetVolumeAsync(value);
        }
    }
}