using System;
using System.Threading.Tasks;
using KeyboardMenu.Models;

namespace KeyboardMenu.Interfaces
{
    public interface IBleKeyboardService
    {
        Task<bool> ScanForKeyboard();
        IObservable<KeyboardMessage> GetMessageNotifier();
    }
}
