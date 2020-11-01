using System;
using CoreAudio;

namespace SimpleConsoleForTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            MMDeviceEnumerator deviceEnumerator = new MMDeviceEnumerator();
            MMDevice device = deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia);
            SessionCollection sessions = device.AudioSessionManager2.Sessions;
            foreach (AudioSessionControl2 session in sessions)
            {
                Console.WriteLine($"Session Identifier: {session.GetSessionIdentifier}");
                Console.WriteLine($"Session Name: {session.DisplayName}");
                Console.WriteLine($"Session IconPath: {session.IconPath}");
                Console.WriteLine("===================");
            }
        }
    }
}