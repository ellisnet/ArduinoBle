using Acr.UserDialogs;
using BluetoothLevel.XFApp.Services;
using BluetoothLevel.XFApp.ViewModels;
using BluetoothLevel.XFApp.Views;
using Prism;
using Prism.Ioc;
using Prism.Unity;
using Xamarin.Forms;

namespace BluetoothLevel.XFApp
{
	public partial class App : PrismApplication
	{
		public App (IPlatformInitializer initializer) : base(initializer) { }

	    protected override async void OnInitialized()
	    {
	        InitializeComponent();
	        await NavigationService.NavigateAsync($"{nameof(NavigationPage)}/{nameof(ScanPage)}");
	    }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
	    {
	        containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<ScanPage, ScanPageViewModel>();
	        containerRegistry.RegisterForNavigation<LevelPage, LevelPageViewModel>();

            containerRegistry.RegisterInstance(typeof(IUserDialogs), UserDialogs.Instance);
	        containerRegistry.RegisterInstance(typeof(ILevelApiService), new BleLevelApiService(UserDialogs.Instance));
        }

        protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

	    protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
