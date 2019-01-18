using Microsoft.AspNet.SignalR;
using SignalR.Models;
using SignalRHost.Wpf.Extensions;
using SignalRHost.Wpf.Hubs;

namespace SignalRHost.Wpf.Helpers
{
    public static class HardwareKeyMessageHelper
    {
        public static void SendMessage(HardwareKeyMessage message)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<HardwareKeyHub>();
            context?.Clients?.SendMessage(message);
        }
    }
}
