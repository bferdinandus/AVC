using System;
using System.Collections.Generic;
using System.Linq;
using AVCLib.Models;
using CoreAudio;

namespace AVCLib.Services
{
    public class AudioService
    {
        private readonly MMDeviceEnumerator _deviceEnumerator = new MMDeviceEnumerator();
        private readonly List<AudioDeviceModel> _outputDevices = new List<AudioDeviceModel>();
        private readonly List<AudioSessionModel> _audioSessions = new List<AudioSessionModel>();

        public List<AudioDeviceModel> GetActiveOutputDevices()
        {
            MMDeviceCollection endPoints = _deviceEnumerator.EnumerateAudioEndPoints(EDataFlow.eRender, DEVICE_STATE.DEVICE_STATE_ACTIVE);

            foreach (MMDevice endPoint in endPoints)
            {
                AudioDeviceModel device = new AudioDeviceModel
                {
                    Id = endPoint.ID,
                    FullName = endPoint.FriendlyName,
                    Selected = endPoint.Selected,
                    Volume = (int) (endPoint.AudioEndpointVolume.MasterVolumeLevelScalar * 100),
                    Muted = endPoint.AudioEndpointVolume.Mute
                };
                endPoint.AudioEndpointVolume.OnVolumeNotification += device.UpdateVolume;

                _outputDevices.Add(device);
            }

            return _outputDevices;
        }

        public List<AudioSessionModel> GetAudioSessionsForCurrentDevice()
        {
            MMDevice device = _deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);

            SessionCollection sessions = device.AudioSessionManager2.Sessions;

            foreach (AudioSessionControl2 session in sessions)
            {
                Console.WriteLine($"Session Identifier: {session.GetSessionIdentifier}");
                Console.WriteLine($"Session Name: {session.DisplayName}");
                Console.WriteLine($"Session IconPath: {session.IconPath}");
                SimpleAudioVolume vol = session.SimpleAudioVolume;
                AudioSessionModel audioSession = new AudioSessionModel
                {
                    DisplayName = session.DisplayName,
                    State = session.State,
                    IconPath = session.IconPath,
                    SessionIdentifier = session.GetSessionIdentifier,
                    SessionInstanceIdentifier = session.GetSessionInstanceIdentifier,
                    ProcessID = session.GetProcessID,
                    IsSystemSoundsSession = session.IsSystemSoundsSession,
                    Volume = (int) (vol.MasterVolume * 100),
                    Muted = vol.Mute
                };
                session.OnChannelVolumeChanged += audioSession.ChannelVolumeChanged;
                session.OnSimpleVolumeChanged += audioSession.SimpleVolumeChanged;
                session.OnStateChanged += audioSession.StateChanged;

                _audioSessions.Add(audioSession);
            }

            return _audioSessions;
        }

        public AudioDeviceModel GetSelectedDevice()
        {
            MMDevice device = _deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);

            AudioDeviceModel model = _outputDevices.Single(m => m.Id == device.ID);

            return model;
        }

        public void SelectDeviceById(string id)
        {
            // mark al local models as not selected
            _outputDevices.ForEach(m => m.Selected = false);
            // mark requested model as selected
            _outputDevices.Single(m => m.Id == id).Selected = true;
            // select the output device
            _deviceEnumerator.GetDevice(id).Selected = true;
        }

        public void SetVolume(string id, int value)
        {
            //_deviceModels.Single(m => m.Id == id).Selected = true;
            _deviceEnumerator.GetDevice(id).AudioEndpointVolume.MasterVolumeLevelScalar = value / 100F;
        }

        public int GetDeviceVolume(string id)
        {
            return _outputDevices.Single(m => m.Id == id).Volume;
        }

        public int GetAudioSessionVolume(string id)
        {
            return _audioSessions.Single(m => m.SessionIdentifier == id).Volume;
        }

        public void AttachOutputDeviceVolumeChanged(string id, Action<string> callbackFunction)
        {
            _outputDevices.Single(m => m.Id == id).OnOutputDeviceVolumeChanged += callbackFunction;
        }

        public void DetachOutputDeviceVolumeChanged(string id, Action<string> callbackFunction)
        {
            _outputDevices.Single(m => m.Id == id).OnOutputDeviceVolumeChanged -= callbackFunction;
        }
    }
}