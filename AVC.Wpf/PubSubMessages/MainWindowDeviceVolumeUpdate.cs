namespace AVC.Wpf.PubSubMessages
{
    public class MainWindowDeviceVolumeUpdate : BaseVolumeUpdate
    {
        public string Name { get; }

        public MainWindowDeviceVolumeUpdate(int volume, string name) : base(volume)
        {
            Name = name;
        }
    }
}