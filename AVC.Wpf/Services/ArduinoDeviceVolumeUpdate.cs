namespace AVC.Wpf.Services
{
    public class ArduinoDeviceVolumeUpdate
    {
        public int Volume { get; }

        public ArduinoDeviceVolumeUpdate(int volume)
        {
            Volume = volume;
        }
    }
}