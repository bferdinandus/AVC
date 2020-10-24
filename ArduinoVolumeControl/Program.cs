using System;
using System.Windows.Forms;
using AVCLib.Services;

namespace ArduinoVolumeControl
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ArduinoVolumeControl(new AudioService()));
        }
    }
}