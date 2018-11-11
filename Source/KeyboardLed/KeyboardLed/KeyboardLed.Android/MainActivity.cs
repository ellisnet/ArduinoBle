using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using CodeBrix.Prism.Ioc;
using KeyboardLed.Droid.Services;
using KeyboardLed.Services;
using Plugin.Permissions;
using Prism;
using Prism.Ioc;

namespace KeyboardLed.Droid
{
    [Activity(
        Label = "KeyboardLed", 
        Icon = "@mipmap/icon", 
        Theme = "@style/MainTheme", 
        MainLauncher = true, 
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, savedInstanceState);

            //Initialization step required for Xamarin.Essentials (used by CodeBrix)
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            //Initialization step required for CodeBrix
            CodeBrix.Prism.Platform.Init(this, savedInstanceState);

            LoadApplication(new App(new AndroidInitializer()));
        }

        //Override of OnRequestPermissionsResult required by Xamarin.Essentials (used by CodeBrix)
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }

    //Platform initializer for Prism (and CodeBrix)
    public class AndroidInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry container)
        {
            // Register CodeBrix pages and services
            CodeBrix.Prism.Platform.RegisterTypes(container);

            // Register any platform-specific services created for this application
            container.RegisterDisposable<IBleKeyboardService, BleKeyboardService>();
        }
    }
}