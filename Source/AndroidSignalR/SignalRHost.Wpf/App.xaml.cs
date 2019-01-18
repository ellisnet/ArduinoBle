using System.Windows;
using Microsoft.Owin.Hosting;
using SignalRHost.Wpf.ViewModels;

namespace SignalRHost.Wpf
{
    public partial class App : Application
    {
        public static readonly string HostBaseAddress = "http://192.168.1.19:9001/";
        public static readonly string SignalRRoot = "signalr";

        public MainViewModel MainView { get; set; }

        public App()
        {
            //Start the self-hosted SignalR service
            WebApp.Start<Startup>(url: HostBaseAddress);
        }
    }
}
