using AVC.Core.ViewModels;
using MvvmCross.ViewModels;

namespace AVC.Core
{
    public class App : MvxApplication
    {
        public override void Initialize()
        {
            RegisterAppStart<GuestBookViewModel>();
        }
    }
}