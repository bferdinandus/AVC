
namespace AVC.Wpf.Services
{
    public class VolumeUpdateMessage
    {
        public int Volume { get; private set; }
        public bool DoUpdateDeviceVolume { get; set; }

        public VolumeUpdateMessage(object sender, int volume, bool doUpdateDeviceVolume)
        {
            Volume = volume;
            DoUpdateDeviceVolume = doUpdateDeviceVolume;
        }

    }
}