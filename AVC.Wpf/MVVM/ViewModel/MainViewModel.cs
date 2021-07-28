using AVC.Wpf.Core;

namespace AVC.Wpf.MVVM.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        public VolumeSliderViewModel VolumeSliderViewModel { get; set; }
        private object _currentView;

        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel(VolumeSliderViewModel volumeSliderViewModel)
        {
            VolumeSliderViewModel = volumeSliderViewModel;

            CurrentView = VolumeSliderViewModel;
        }
    }
}