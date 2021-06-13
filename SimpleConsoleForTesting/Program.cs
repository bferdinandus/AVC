using System;
using System.Collections.Generic;
using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using AudioSwitcher.AudioApi.Session;

namespace SimpleConsoleForTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            CoreAudioController audioController = new CoreAudioController();

            IEnumerable<CoreAudioDevice> devices = audioController.GetDevices(DeviceType.Playback, DeviceState.Active);

            foreach (CoreAudioDevice device in devices)
            {
                Console.WriteLine($"name: {device.Name} || fullname: {device.FullName} || volume: {device.Volume}");


                IEnumerable<IAudioSession> audioSessions = device.GetCapability<IAudioSessionController>().ActiveSessions();
                foreach (IAudioSession audioSession in audioSessions)
                {
                    Console.WriteLine(audioSession.DisplayName);
                }
            }



        }
    }
}