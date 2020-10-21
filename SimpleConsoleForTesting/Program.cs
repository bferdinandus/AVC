using System;
using System.Collections.Generic;
using System.Threading;
using AVCLib.Models;
using AVCLib.Services;

namespace SimpleConsoleForTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            AudioService audioService = new AudioService();
            List<AudioDeviceModel> devices = audioService.GetActiveDevices();

            Console.Clear();
            Console.WriteLine("Press the number in front of the device to activate it. Press q to exit.");


            Console.CursorVisible = false;
            while (true)
            {
                Console.SetCursorPosition(0, 1);

                for (int i = 0; i < devices.Count; i++)
                {
                    Console.WriteLine($"{i+1}. Device {devices[i].FullName}, active: {devices[i].Active}, ID: {devices[i].Id}");
                }

                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    switch (key.Key)
                    {
                        case ConsoleKey.D1:
                            audioService.SelectDeviceById(devices[0].Id);
                            devices = audioService.GetActiveDevices();
                            break;
                        case ConsoleKey.D2:
                            audioService.SelectDeviceById(devices[1].Id);
                            devices = audioService.GetActiveDevices();
                            break;
                        case ConsoleKey.Q:
                            Console.CursorVisible = true;
                            return;
                    }
                }

                Thread.Sleep(10);
            }
        }
    }
}