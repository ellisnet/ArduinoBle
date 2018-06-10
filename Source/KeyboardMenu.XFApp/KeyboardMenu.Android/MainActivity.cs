using Acr.UserDialogs;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using FFImageLoading.Forms.Platform;
using FFImageLoading.Svg.Forms;
using FFImageLoading.Transformations;
using KeyboardMenu.Services;
using Plugin.CurrentActivity;
using Plugin.Permissions;
using Prism.Ioc;

namespace KeyboardMenu.Droid
{
    [Activity(
        Label = "KeyboardMenu", 
        Icon = "@mipmap/icon", 
        Theme = "@style/MainTheme", 
        MainLauncher = true, 
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            // ReSharper disable once RedundantNameQualifier
            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App(new AndroidConfigService(this, bundle)));
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }

    public class AndroidConfigService : BaseAppConfigService
    {
        public override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //Nothing to register yet
        }

        public AndroidConfigService(Activity mainActivity, Bundle bundle)
        {
            //Initialize plug-ins
            CrossCurrentActivity.Current.Init(mainActivity, bundle);
            UserDialogs.Init(mainActivity);

            //Initialize SVG image and transformation support
            CachedImageRenderer.Init(enableFastRenderer: true);
            // ReSharper disable UnusedVariable
            var ignoreImage = typeof(SvgCachedImage);
            var ignoreTransformation = typeof(GrayscaleTransformation);
            // ReSharper restore UnusedVariable
        }
    }
}
