namespace AVC.Wpf.PubSubMessages
{
    public class AudioServiceDeviceVolumeUpdate : BaseVolumeUpdate
    {
        public AudioServiceDeviceVolumeUpdate(int volume) : base(volume) {}
    }
}