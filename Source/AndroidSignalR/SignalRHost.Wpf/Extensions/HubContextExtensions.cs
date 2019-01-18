using Microsoft.AspNet.SignalR.Hubs;
using SignalR.Models;

namespace SignalRHost.Wpf.Extensions
{
    public static class HubContextExtensions
    {
        public static void SendMessage(this IHubConnectionContext<dynamic> context, HardwareKeyMessage message)
        {
            if (message != null)
            {
                context.All.SendMessage(message);
            }
        }
    }
}
