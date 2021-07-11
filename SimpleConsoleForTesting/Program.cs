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
            IAudioController audioController = new CoreAudioController();

            IEnumerable<IDevice> devices = audioController.GetDevices(DeviceType.Playback, DeviceState.Active);

            foreach (IDevice device in devices)
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