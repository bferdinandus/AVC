using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using AVC.Core.Services;
using AVC.Core.ViewModels;
using MvvmCross;
using MvvmCross.IoC;
using MvvmCross.Plugin.Messenger;
using MvvmCross.ViewModels;

namespace AVC.Core
{
    public class App : MvxApplication
    {
        public override void Initialize()
        {
            // Mvx.IoCProvider.RegisterType => Transient
            Mvx.IoCProvider.RegisterSingleton<IAudioController>(new CoreAudioController());
            Mvx.IoCProvider.RegisterSingleton<IMvxMessenger>(new MvxMessengerHub());

            Mvx.IoCProvider.LazyConstructAndRegisterSingleton<IAudioService, AudioService>();

            RegisterAppStart<VolumeSliderViewModel>();
        }
    }
}