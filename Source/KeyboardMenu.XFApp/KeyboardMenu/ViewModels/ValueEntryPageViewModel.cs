using System;
using System.Linq;
using Acr.UserDialogs;
using KeyboardMenu.Interfaces;
using KeyboardMenu.Models;
using Prism.Commands;
using Prism.Navigation;

namespace KeyboardMenu.ViewModels
{
    public class ValueEntryPageViewModel : ViewModelBase, IValueEntryCompleted
    {
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
            IUserDialogs userDialogService)
            : base(navigationService, userDialogService)
        {
            PageTitle = DefaultPageTitle;
        }
    }
}
