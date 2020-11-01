using System;
using System.Windows.Forms;
using AVCLib.Services;

namespace ArduinoVolumeControl
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            AudioService audioService = new AudioService();

            Application.Run(new ArduinoVolumeControl(audioService));
        }
    }
}