using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace AVC.Wpf.MVVM.View
{
    public partial class VolumeSliderView : UserControl
    {
        public VolumeSliderView()
        {
            InitializeComponent();
        }

        private void DeviceVolumeSlider_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            Slider slider = (Slider) sender;
            double currentValue = slider.Value;
            switch (e.Delta)
            {
                case > 0:
                    currentValue += 5;
                    break;
                case < 0:
                    currentValue -= 5;
                    break;
            }

            slider.Value = Math.Max(slider.Minimum, Math.Min(slider.Maximum, currentValue));
        }

        private void AppVolumeSlider1_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}