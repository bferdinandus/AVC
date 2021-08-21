namespace AVC.Core.PubSubMessages
{
    public class AudioServiceDeviceVolumeUpdate : BaseVolumeUpdate
    {
        public string Name { get; }

        public AudioServiceDeviceVolumeUpdate(int volume, string name) : base(volume)
        {
            Name = name;
        }
    }
}