using System;
using Acr.UserDialogs;
using KeyboardMenu.Interfaces;
using KeyboardMenu.Services;
using Prism.Ioc;
using Prism.Unity;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]

namespace KeyboardMenu
{
    // ReSharper disable once RedundantExtendsListEntry
    public partial class App : PrismApplication
	{
	    private IContainerRegistry _containerRegistry;

	    public IAppConfigService AppConfigService { get; }

	    public App(IAppConfigService configSvc) : base(configSvc)
	    {
	        AppConfigService = configSvc ?? throw new ArgumentNullException(nameof(configSvc));
            AppConfigService.SetContainer(Container);
            _containerRegistry?.RegisterInstance(typeof(IAppConfigService), AppConfigService);
	    }

        protected override async void OnInitialized()
	    {
	        InitializeComponent();
	        await NavigationService.NavigateAsync($"{nameof(NavigationPage)}/{nameof(Views.MainPage)}");
	    }

	    protected override void RegisterTypes(IContainerRegistry containerRegistry)
	    {
	        _containerRegistry = containerRegistry;

            containerRegistry.RegisterForNavigation<NavigationPage>();
	        containerRegistry.RegisterForNavigation<Views.MainPage, ViewModels.MainPageViewModel>();
	        containerRegistry.RegisterForNavigation<Views.ValueEntryPage, ViewModels.ValueEntryPageViewModel>();

            containerRegistry.RegisterInstance(typeof(IUserDialogs), UserDialogs.Instance);
	        containerRegistry.RegisterInstance(typeof(IBleKeyboardService),
	            new BleKeyboardService(UserDialogs.Instance));
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
