using AVC.Core.ViewModels;
using MvvmCross.Platforms.Wpf.Presenters.Attributes;
using MvvmCross.Platforms.Wpf.Views;
using MvvmCross.ViewModels;

namespace AVC.Wpf.Views
{
    [MvxContentPresentation]
    [MvxViewFor(typeof(GuestBookViewModel))]
    public partial class GuestBookView : MvxWpfView
    {
        public GuestBookView()
        {
            InitializeComponent();
        }
    }
}