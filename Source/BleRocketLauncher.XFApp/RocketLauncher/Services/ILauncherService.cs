using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RocketLauncher.Services
{
    public interface ILauncherService
    {
        bool IsConnected { get; }
        Task<bool> Connect();
        Task<bool> SendLaunchMessage(string message, Action launchCompleteAction = null);
    }
}
