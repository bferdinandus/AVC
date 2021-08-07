namespace AVC.Wpf.PubSubMessages
{
    public class AudioServiceDeviceVolumeUpdate
    {
        public int Volume { get; }

        public AudioServiceDeviceVolumeUpdate(int volume)
        {
            Volume = volume;
        }
    }
}