using System;
using System.Windows;
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
            throw new System.NotImplementedException();
        }
    }
}