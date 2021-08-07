namespace AVC.Wpf.PubSubMessages
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