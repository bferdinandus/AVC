namespace AVC.Wpf.PubSubMessages
{
    public abstract class BaseVolumeUpdate
    {
        public int Volume { get; }

        protected BaseVolumeUpdate(int volume)
        {
            Volume = volume;
        }
    }
}