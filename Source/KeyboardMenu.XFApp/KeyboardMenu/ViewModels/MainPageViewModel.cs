using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Timers;
using Acr.UserDialogs;
using KeyboardMenu.Extensions;
using KeyboardMenu.Interfaces;
using KeyboardMenu.Models;
using KeyboardMenu.Views;
using Prism.Commands;
using Prism.Navigation;

namespace KeyboardMenu.ViewModels
{
    public class MainPageViewModel : ViewModelBase, IObserver<KeyboardMessage>
    {
        private readonly IList<MenuItem> _menuItems;
        private readonly IBleKeyboardService _keyboardService;
        private bool _scannedForKeyboard;
        private IDisposable _keyboardSubscription;
        private Timer _scanTimer;
        private int _selectedChoiceIndex;

        #region Bindable properties

        private bool _inMenuMode;
        public bool InMenuMode
        {
            get => _inMenuMode;
            private set => SetProperty(ref _inMenuMode, value);
        }

        public ObservableCollection<SelectableItemViewModel<MenuItem>> TopMenuItems { get; set; } 
            = new ObservableCollection<SelectableItemViewModel<MenuItem>>();

        public ObservableCollection<SelectableItemViewModel<SubMenuItem>> BottomMenuItems { get; set; } 
            = new ObservableCollection<SelectableItemViewModel<SubMenuItem>>();

        public ObservableCollection<SelectableItemViewModel<ChoiceItem>> ChoiceItems { get; set; } 
            = new ObservableCollection<SelectableItemViewModel<ChoiceItem>>();

        #endregion

        #region Commands and their implementations

        #region DoNothingCommand

        private DelegateCommand _doNothingCommand;
        public DelegateCommand DoNothingCommand => _doNothingCommand ??
            (_doNothingCommand = new DelegateCommand(() => { }));

        #endregion

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
                if (InMenuMode && KeyboardMessage.JoystickMessages.Contains(value.Message))
                {
                    //Navigate the menu
                }
                else if (KeyboardMessage.JoystickMessages.Contains(value.Message))
                {
                    //Navigate the selection list
                    bool updateList = false;
                    if (value.Message == BleMessage.JoystickSouth && _selectedChoiceIndex < ChoiceItems.Count)
                    {
                        _selectedChoiceIndex++;
                        updateList = true;
                    }
                    else if (value.Message == BleMessage.JoystickNorth && _selectedChoiceIndex > 1)
                    {
                        _selectedChoiceIndex--;
                        updateList = true;
                    }
                    else if (value.Message == BleMessage.JoystickButton
                             && _selectedChoiceIndex > 0
                             && _selectedChoiceIndex <= ChoiceItems.Count)
                    {
                        ChoiceItem itemToEdit = ChoiceItems.FirstOrDefault(f => f.Order == _selectedChoiceIndex)?.Item;
                        if (itemToEdit != null)
                        {
                            ChoiceItemSelected(itemToEdit);
                        }
                    }

                    if (updateList)
                    {
                        foreach (SelectableItemViewModel<ChoiceItem> item in ChoiceItems)
                        {
                            item.Item.IsActive = (item.Order == _selectedChoiceIndex);
                            item.NotifyChanged();
                        }
                    }
                }
                else if (KeyboardMessage.ButtonMessages.Contains(value.Message))
                {
                    //Handle button presses
                    if (value.Message == BleMessage.ButtonGreen) { InMenuMode = !InMenuMode;} //Toggle menu on and off
                }
            }
        }

        public void TopMenuItemSelected(MenuItem menuItem)
        {
            bool itemChanged = false;
            IList<SubMenuItem> subMenuItems = null;
            foreach (MenuItem item in _menuItems)
            {
                if (item == menuItem)
                {
                    itemChanged = !item.IsActive;
                    item.IsActive = true;
                    subMenuItems = item.SubMenuItems;
                }
                else
                {
                    item.IsActive = false;
                }
            }

            if (itemChanged)
            {
                TopMenuItems.ResetItems(_menuItems
                    .Select(s => new SelectableItemViewModel<MenuItem>(s, TopMenuItemSelected)).ToArray());
                if (subMenuItems != null)
                {
                    for (int i = 0; i < subMenuItems.Count; i++)
                    {
                        subMenuItems[i].IsActive = (i == 0);
                    }

                    SubMenuItem selectedSubMenuItem = subMenuItems[0];

                    ResetBottomMenuItems(subMenuItems);
                    ResetChoiceItems(selectedSubMenuItem.ChoiceItems);
                }
                NotifyPropertyChanged(nameof(TopMenuItems));
                NotifyPropertyChanged(nameof(BottomMenuItems));
                NotifyPropertyChanged(nameof(ChoiceItems));
            }
        }

        public void BottomMenuItemSelected(SubMenuItem menuItem)
        {
            bool itemChanged = false;
            IList<SubMenuItem> subMenuItems = _menuItems.Single(s => s.IsActive).SubMenuItems;
            IList<ChoiceItem> choiceItems = null;

            foreach (SubMenuItem item in subMenuItems)
            {
                if (item == menuItem)
                {
                    itemChanged = !item.IsActive;
                    item.IsActive = true;
                    choiceItems = item.ChoiceItems;
                }
                else
                {
                    item.IsActive = false;
                }
            }

            if (itemChanged)
            {
                ResetBottomMenuItems(subMenuItems);
                ResetChoiceItems(choiceItems);
                NotifyPropertyChanged(nameof(BottomMenuItems));
                NotifyPropertyChanged(nameof(ChoiceItems));
            }
        }

        public async void ChoiceItemSelected(ChoiceItem item)
        {
            await NavigateToPage<ValueEntryPage>(new NavigationParameters
            {
                {NavParamKey.SelectedChoiceItem, item}
            });
        }

        private void ResetBottomMenuItems(IList<SubMenuItem> items)
        {
            BottomMenuItems.ResetItems(items.Select(s => new SelectableItemViewModel<SubMenuItem>(s, BottomMenuItemSelected)).ToArray());
        }

        private void ResetChoiceItems(IList<ChoiceItem> items)
        {
            _selectedChoiceIndex = 0;
            foreach (ChoiceItem choice in items)
            {
                choice.IsActive = false;
            }
            ChoiceItems.ResetItems(items.Select(s => new SelectableItemViewModel<ChoiceItem>(s, ChoiceItemSelected)).ToArray());
        }

        public override void OnNavigatedTo(NavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if ((parameters?.Any() ?? false)
                && parameters.ContainsKey(NavParamKey.ValueUpdated)
                && bool.TryParse(parameters[NavParamKey.ValueUpdated].ToString(), out bool parsed)
                && parsed)
            {
                _selectedChoiceIndex = 0;
                foreach (SelectableItemViewModel<ChoiceItem> item in ChoiceItems)
                {
                    item.Item.IsActive = false;
                    item.NotifyChanged();
                }
                NotifyPropertyChanged(nameof(ChoiceItems));
            }

            if (!_scannedForKeyboard)
            {
                _scannedForKeyboard = true;

                _scanTimer = new Timer(6000) {AutoReset = false};
                _scanTimer.Elapsed += async (sender, args) =>
                {
                    await _keyboardService.ScanForKeyboard();
                };
                _scanTimer.Start();
            }
        }

        public MainPageViewModel(
            INavigationService navigationService,
            IUserDialogs userDialogService,
            IBleKeyboardService keyboardService)
            : base(navigationService, userDialogService)
        {
            _keyboardService = keyboardService ?? throw new ArgumentNullException(nameof(keyboardService));

            _keyboardSubscription = _keyboardService
                .GetMessageNotifier()
                .Where(w => KeyboardMessage.ButtonMessages.Contains(w.Message)
                    || KeyboardMessage.JoystickMessages.Contains(w.Message))
                .Subscribe(this);

            PageTitle = DefaultPageTitle;

            //Initialize collections
            _menuItems = MenuItem.GetMenuItems();
            TopMenuItems.AddRange(_menuItems
                .Select(s => new SelectableItemViewModel<MenuItem>(s, TopMenuItemSelected)).ToArray());

            IList<SubMenuItem> subMenuItems = _menuItems.Single(s => s.IsActive).SubMenuItems;
            ResetBottomMenuItems(subMenuItems);
            ResetChoiceItems(subMenuItems.Single(s => s.IsActive).ChoiceItems);
        }

        public override void Destroy()
        {
            _keyboardSubscription?.Dispose();
            _keyboardSubscription = null;

            base.Destroy();
        }
    }
}
