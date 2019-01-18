using Microsoft.Owin.Cors;
using Owin;

namespace SignalRHost.Wpf
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();
        }
    }
}
