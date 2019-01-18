using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Acr.UserDialogs;
using AndroidSignalR.Services;
using CodeBrix.Prism.Abstract;
using CodeBrix.Prism.Observables;
using CodeBrix.Prism.Services;
using Prism.Navigation;
using SignalR.Models;

namespace AndroidSignalR.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private SignalRKeyboardService _keyboardService;
        private SimpleObserver<HardwareKeyMessage> _hardwareKeyObserver;

        #region Bindable properties

        private string _incomingMessages = String.Empty;
        public string IncomingMessages
        {
            get => _incomingMessages;
            set => SetProperty(ref _incomingMessages, value);
        }

        #endregion

        public void OnKeyboardMessageNext(HardwareKeyMessage message)
        {
            Debug.WriteLine($"Incoming: {message.Stroke}");
            IncomingMessages += (IncomingMessages == String.Empty) ? $"{message.Stroke}" : $"\n{message.Stroke}";
        }
         
        public async void OnKeyboardMessageError(Exception e) => await ShowErrorAsync(e);

        public void OnKeyboardMessageCompleted() {} //Not doing anything here

        public MainPageViewModel(
            INavigationService navigationService,
            ILoggerService loggerService,
            IUserDialogs dialogService)
            : base(navigationService, loggerService, dialogService)
        {
            _keyboardService = new SignalRKeyboardService();
            _hardwareKeyObserver = new SimpleObserver<HardwareKeyMessage>(OnKeyboardMessageNext,
                OnKeyboardMessageCompleted, OnKeyboardMessageError);
            _hardwareKeyObserver.GetSubscription(_keyboardService.GetObservable());
            new Task(async () => { await _keyboardService.Connect(); }).Start();            
        }
    }
}
