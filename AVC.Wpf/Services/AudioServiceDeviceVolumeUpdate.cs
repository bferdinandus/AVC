namespace AVC.Wpf.Services
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