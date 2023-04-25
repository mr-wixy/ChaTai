using ChaTai.Blazor.Data;
using ChaTai.Core;
using Foundation;

namespace ChaTai.App
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp()
        {
            GlobalVariable.Platform = PlatformType.Mac;
            return MauiProgram.CreateMauiApp();
        }
    }
}