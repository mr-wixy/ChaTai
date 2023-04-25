using Android.App;
using Android.Runtime;
using ChaTai.Blazor.Data;
using ChaTai.Core;

namespace ChaTai.App
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override MauiApp CreateMauiApp()
        {
            GlobalVariable.Platform = PlatformType.Android;
            return MauiProgram.CreateMauiApp();
        }
    }
}