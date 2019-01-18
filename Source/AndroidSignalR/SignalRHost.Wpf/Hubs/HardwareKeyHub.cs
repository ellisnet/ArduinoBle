using System.Diagnostics;
using System.Windows;
using Microsoft.AspNet.SignalR;
using SignalR.Models;
using SignalRHost.Wpf.Extensions;
using SignalRHost.Wpf.ViewModels;

namespace SignalRHost.Wpf.Hubs
{
    public class HardwareKeyHub : Hub
    {
        public void SendMessage(HardwareKeyMessage message) => Clients.SendMessage(message);

        public void ReceiveMessage(HardwareKeyMessage message)
        {
            if (message != null)
            {
                Debug.WriteLine($"Incoming hardware key message:\n{message.Stroke}");
                MainViewModel vm = (Application.Current as App)?.MainView;
                vm?.ProcessIncomingMessage(message);
            }
        }
    }
}
