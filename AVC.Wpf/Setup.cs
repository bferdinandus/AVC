using Microsoft.Extensions.Logging;
using MvvmCross.IoC;
using MvvmCross.Platforms.Wpf.Binding;
using MvvmCross.Platforms.Wpf.Core;

namespace AVC.Wpf
{
    public class Setup : MvxWpfSetup<Core.App>
    {
        protected override ILoggerProvider CreateLogProvider()
        {
            return null;
        }

        protected override ILoggerFactory CreateLogFactory()
        {
            return null;
        }

        // protected override void InitializeLastChance(IMvxIoCProvider iocProvider)
        // {
        //     base.InitializeLastChance(iocProvider);
        //
        //     var builder = new MvxWindowsBindingBuilder();
        //     builder.DoRegistration(iocProvider);
        // }

    }

}