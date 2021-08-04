
namespace AVC.Wpf.Services
{
    public class VolumeUpdateMessage
    {
        public int Volume { get; }
        public bool DoUpdateDeviceVolume { get; }

        public VolumeUpdateMessage(int volume, bool doUpdateDeviceVolume)
        {
            Volume = volume;
            DoUpdateDeviceVolume = doUpdateDeviceVolume;
        }

    }
}