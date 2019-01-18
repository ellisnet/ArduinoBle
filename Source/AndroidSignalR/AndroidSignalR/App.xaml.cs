using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using CodeBrix.Prism;
using Prism;
using Prism.DryIoc;
using Prism.Ioc;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace AndroidSignalR
{
    public partial class App : PrismApplication
    {
        public App() : this(null) { }  //This constructor should never be called at run-time

        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override void OnInitialized()
        {
            InitializeComponent();
            NavigationService.NavigateAsync($"{nameof(NavigationPage)}/{nameof(Views.MainPage)}");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //Register my views and services here
            containerRegistry.RegisterForNavigation<Views.MainPage>();
        }

        protected override void OnStart()
        {
            CodeBrixInfo.OnApplicationStart();
        }

        protected override void OnSleep()
        {
            CodeBrixInfo.OnApplicationSleep();
        }

        protected override void OnResume()
        {
            CodeBrixInfo.OnApplicationResume();
        }
    }
}
