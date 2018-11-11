using System;
using System.Threading.Tasks;
using KeyboardLed.Models;

namespace KeyboardLed.Services
{
    public interface IBleKeyboardService : IDisposable
    {
        Task<bool> ScanForKeyboard();
        IObservable<KeyboardMessage> GetMessageNotifier();
        Task<bool> SendLedMessage(LedMessage message);
    }
}
