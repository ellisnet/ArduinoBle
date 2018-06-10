using FFImageLoading.Forms.Platform;
using FFImageLoading.Svg.Forms;
using FFImageLoading.Transformations;
using Foundation;
using KeyboardMenu.Services;
using Prism.Ioc;
using UIKit;

namespace KeyboardMenu.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App(new IosConfigService()));

            return base.FinishedLaunching(app, options);
        }
    }

    public class IosConfigService : BaseAppConfigService
    {
        public override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //Nothing to register yet
        }

        public IosConfigService()
        {
            //Initialize SVG image and transformation support
            CachedImageRenderer.Init();
            // ReSharper disable UnusedVariable
            var ignoreImage = typeof(SvgCachedImage);
            var ignoreTransformation = typeof(GrayscaleTransformation);
            // ReSharper restore UnusedVariable
        }
    }
}
