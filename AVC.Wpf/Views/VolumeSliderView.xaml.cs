using System.Windows.Controls;
using System.Windows.Input;
using AVC.Core.ViewModels;
using MvvmCross.Platforms.Wpf.Presenters.Attributes;
using MvvmCross.Platforms.Wpf.Views;
using MvvmCross.ViewModels;

namespace AVC.Wpf.Views
{
    [MvxContentPresentation]
    [MvxViewFor(typeof(VolumeSliderViewModel))]
    public partial class VolumeSliderView : MvxWpfView
    {
        public VolumeSliderView()
        {
            InitializeComponent();
        }

        private void DeviceVolumeSlider_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            switch (e.Delta)
            {
                case > 0:
                    ((Slider) sender).Value += 5;
                    break;
                case < 0:
                    ((Slider) sender).Value -= 5;
                    break;
            }
        }
    }
}