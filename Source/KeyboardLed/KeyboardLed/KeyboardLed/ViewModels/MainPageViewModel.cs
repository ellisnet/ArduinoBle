using System;
using System.Threading.Tasks;
using Acr.UserDialogs;
using CodeBrix.Prism.Abstract;
using CodeBrix.Prism.Observables;
using CodeBrix.Prism.Services;
using KeyboardLed.Models;
using KeyboardLed.Services;
using Prism.Commands;
using Prism.Navigation;

namespace KeyboardLed.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private IBleKeyboardService _keyboardService;
        private SimpleObserver<KeyboardMessage> _keyboardObserver;

        #region Commands and their implementations

        private DelegateCommand _leftRedCommand;
        public DelegateCommand LeftRedCommand => LazyCreateCommand(ref _leftRedCommand, () =>
            {
                _keyboardService?.SendLedMessage(LedMessage.LeftRed);
            });

        private DelegateCommand _leftGreenCommand;
        public DelegateCommand LeftGreenCommand => LazyCreateCommand(ref _leftGreenCommand, () =>
        {
            _keyboardService?.SendLedMessage(LedMessage.LeftGreen);
        });

        private DelegateCommand _leftBlueCommand;
        public DelegateCommand LeftBlueCommand => LazyCreateCommand(ref _leftBlueCommand, () =>
        {
            _keyboardService?.SendLedMessage(LedMessage.LeftBlue);
        });

        private DelegateCommand _leftOffCommand;
        public DelegateCommand LeftOffCommand => LazyCreateCommand(ref _leftOffCommand, () =>
        {
            _keyboardService?.SendLedMessage(LedMessage.LeftOff);
        });

        private DelegateCommand _rightRedCommand;
        public DelegateCommand RightRedCommand => LazyCreateCommand(ref _rightRedCommand, () =>
        {
            _keyboardService?.SendLedMessage(LedMessage.RightRed);
        });

        private DelegateCommand _rightGreenCommand;
        public DelegateCommand RightGreenCommand => LazyCreateCommand(ref _rightGreenCommand, () =>
        {
            _keyboardService?.SendLedMessage(LedMessage.RightGreen);
        });

        private DelegateCommand _rightBlueCommand;
        public DelegateCommand RightBlueCommand => LazyCreateCommand(ref _rightBlueCommand, () =>
        {
            _keyboardService?.SendLedMessage(LedMessage.RightBlue);
        });

        private DelegateCommand _rightOffCommand;
        public DelegateCommand RightOffCommand => LazyCreateCommand(ref _rightOffCommand, () =>
        {
            _keyboardService?.SendLedMessage(LedMessage.RightOff);
        });

        #endregion

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            await Task.Delay(100);
            using (DialogService.Loading("Scanning for keyboard..."))
            {
                _keyboardObserver = new SimpleObserver<KeyboardMessage>(
                    (keyboardMessage) =>
                    {
                        if (keyboardMessage != null)
                        {
                            DialogService.Toast(keyboardMessage.Message.ToString());
                        }                        
                    }, 
                    () => { }, //Not doing anything with OnCompleted yet
                    (exception) => { }); //Not doing anything with OnError yet
                _keyboardObserver.GetSubscription(_keyboardService.GetMessageNotifier());
                await _keyboardService.ScanForKeyboard();
            }
        }

        public MainPageViewModel(
            INavigationService navigationService, 
            ILoggerService loggerService, 
            IUserDialogs dialogService,
            IBleKeyboardService keyboardService) 
            : base(navigationService, loggerService, dialogService)
        {
            _keyboardService = keyboardService ?? throw new ArgumentNullException(nameof(keyboardService));
        }

        public override void Destroy()
        {
            _keyboardObserver?.Dispose();
            _keyboardObserver = null;
            _keyboardService?.Dispose();
            _keyboardService = null;
            base.Destroy();
        }
    }
}
