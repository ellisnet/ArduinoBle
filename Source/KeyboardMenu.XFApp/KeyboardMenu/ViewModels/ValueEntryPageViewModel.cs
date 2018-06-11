using System;
using System.Linq;
using System.Reactive.Linq;
using Acr.UserDialogs;
using KeyboardMenu.Interfaces;
using KeyboardMenu.Models;
using Prism.Commands;
using Prism.Navigation;

namespace KeyboardMenu.ViewModels
{
    public class ValueEntryPageViewModel : ViewModelBase, IValueEntryCompleted, IObserver<KeyboardMessage>
    {
        private IDisposable _keyboardSubscription;

        #region Bindable properties

        private string _enteredValue;
        public string EnteredValue
        {
            get => _enteredValue;
            set => SetProperty(ref _enteredValue, value);
        }

        public ChoiceItem ChoiceItem { get; set; }
        public string ItemName => ChoiceItem?.Name ?? String.Empty;
        public string EntryPrompt => $"Please provide a value for {ItemName}:";

        #endregion

        #region Commands and their implementations

        private DelegateCommand _valueEntryCompletedCommand;

        public DelegateCommand ValueEntryCompletedCommand => _valueEntryCompletedCommand ??
           (_valueEntryCompletedCommand = new DelegateCommand(DoValueEntryCompleted));

        public async void DoValueEntryCompleted()
        {
            bool valueUpdated = false;
            if (double.TryParse(_enteredValue, out double value))
            {
                ChoiceItem.Value = $"{value:F3}";
                valueUpdated = true;
            }

            await GoBack(new NavigationParameters
            {
                { NavParamKey.ValueUpdated, valueUpdated }
            });
        }

        #endregion

        public void OnCompleted()
        {
            //Nothing to do here
        }

        public void OnError(Exception error)
        {
            //Nothing to do here
        }

        public void OnNext(KeyboardMessage value)
        {
            if (value != null)
            {
                switch (value.Message)
                {
                    case BleMessage.KeyStar:
                        EnteredValue += ".";
                        break;
                    case BleMessage.KeyHash:
                        DoValueEntryCompleted();
                        break;
                    case BleMessage.Key0:
                        EnteredValue += "0";
                        break;
                    case BleMessage.Key1:
                        EnteredValue += "1";
                        break;
                    case BleMessage.Key2:
                        EnteredValue += "2";
                        break;
                    case BleMessage.Key3:
                        EnteredValue += "3";
                        break;
                    case BleMessage.Key4:
                        EnteredValue += "4";
                        break;
                    case BleMessage.Key5:
                        EnteredValue += "5";
                        break;
                    case BleMessage.Key6:
                        EnteredValue += "6";
                        break;
                    case BleMessage.Key7:
                        EnteredValue += "7";
                        break;
                    case BleMessage.Key8:
                        EnteredValue += "8";
                        break;
                    case BleMessage.Key9:
                        EnteredValue += "9";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public override void OnNavigatingTo(NavigationParameters parameters)
        {
            base.OnNavigatingTo(parameters);
            if ((parameters?.Any() ?? false)
                && parameters.ContainsKey(NavParamKey.SelectedChoiceItem)
                && parameters[NavParamKey.SelectedChoiceItem] is ChoiceItem item)
            {
                ChoiceItem = item;
                NotifyPropertyChanged(nameof(ChoiceItem));
                NotifyPropertyChanged(nameof(ItemName));
                NotifyPropertyChanged(nameof(EntryPrompt));
                EnteredValue = item.Value ?? String.Empty;
            }
        }

        public ValueEntryPageViewModel(
            INavigationService navigationService,
            IUserDialogs userDialogService,
            IBleKeyboardService keyboardService)
            : base(navigationService, userDialogService)
        {
            if (keyboardService == null)
            {
                throw new ArgumentNullException(nameof(keyboardService));
            }
            else
            {
                _keyboardSubscription = keyboardService
                    .GetMessageNotifier()
                    .Where(w => KeyboardMessage.KeyboardMessages.Contains(w.Message))
                    .Subscribe(this);
            }

            PageTitle = DefaultPageTitle;
        }

        public override void Destroy()
        {
            _keyboardSubscription?.Dispose();
            _keyboardSubscription = null;

            base.Destroy();
        }
    }
}
