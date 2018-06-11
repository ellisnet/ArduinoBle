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
using Xamarin.Forms;
using MenuItem = KeyboardMenu.Models.MenuItem;

namespace KeyboardMenu.ViewModels
{
    public class MainPageViewModel : ViewModelBase, IObserver<KeyboardMessage>
    {
        private static readonly string closeIconName = "closemenu.svg";

        private readonly IList<MenuItem> _menuItems;
        private readonly IBleKeyboardService _keyboardService;
        private bool _scannedForKeyboard;
        private IDisposable _keyboardSubscription;
        private Timer _scanTimer;
        private int _selectedChoiceIndex;

        private int _selectedTopMenuIndex;
        private int _selectedBottomMenuIndex;
        private int _tempTopMenuIndex;
        private int _tempBottomMenuIndex;
        private int _menuRowIndex;

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

        // ReSharper disable InconsistentNaming
        public string MenuImage1_1 => GetIconFilePath(GetMenuIcon(1, 1));
        public string MenuImage1_2 => GetIconFilePath(GetMenuIcon(1, 2));
        public string MenuImage1_3 => GetIconFilePath(GetMenuIcon(1, 3));
        public string MenuImage1_4 => GetIconFilePath(GetMenuIcon(1, 4));
        public string MenuImage1_5 => GetIconFilePath(GetMenuIcon(1, 5));
        public string MenuImage2_1 => GetIconFilePath(GetMenuIcon(2, 1));
        public string MenuImage2_2 => GetIconFilePath(GetMenuIcon(2, 2));
        public string MenuImage2_3 => GetIconFilePath(GetMenuIcon(2, 3));
        public string MenuImage2_4 => GetIconFilePath(GetMenuIcon(2, 4));
        public string MenuImage2_5 => GetIconFilePath(GetMenuIcon(2, 5));
        public string MenuImage3_1 => GetIconFilePath(GetMenuIcon(3, 1));
        public string MenuImage3_2 => GetIconFilePath(GetMenuIcon(3, 2));
        public string MenuImage3_3 => GetIconFilePath(GetMenuIcon(3, 3));
        public string MenuImage3_4 => GetIconFilePath(GetMenuIcon(3, 4));
        public string MenuImage3_5 => GetIconFilePath(GetMenuIcon(3, 5));
        public string MenuImage4_1 => GetIconFilePath(GetMenuIcon(4, 1));
        public string MenuImage4_2 => GetIconFilePath(GetMenuIcon(4, 2));
        public string MenuImage4_3 => GetIconFilePath(GetMenuIcon(4, 3));
        public string MenuImage4_4 => GetIconFilePath(GetMenuIcon(4, 4));
        public string MenuImage4_5 => GetIconFilePath(GetMenuIcon(4, 5));
        public string MenuImage5_1 => GetIconFilePath(GetMenuIcon(5, 1));
        public string MenuImage5_2 => GetIconFilePath(GetMenuIcon(5, 2));
        public string MenuImage5_3 => GetIconFilePath(GetMenuIcon(5, 3));
        public string MenuImage5_4 => GetIconFilePath(GetMenuIcon(5, 4));
        public string MenuImage5_5 => GetIconFilePath(GetMenuIcon(5, 5));
        // ReSharper restore InconsistentNaming

        #endregion

        #region Commands and their implementations

        #region DoNothingCommand

        private DelegateCommand _doNothingCommand;
        public DelegateCommand DoNothingCommand => _doNothingCommand ??
            (_doNothingCommand = new DelegateCommand(() => { }));

        #endregion

        #endregion

        private void NotifyMenuIconsChanged()
        {
            NotifyPropertyChanged(nameof(MenuImage1_1));
            NotifyPropertyChanged(nameof(MenuImage1_2));
            NotifyPropertyChanged(nameof(MenuImage1_3));
            NotifyPropertyChanged(nameof(MenuImage1_4));
            NotifyPropertyChanged(nameof(MenuImage1_5));
            NotifyPropertyChanged(nameof(MenuImage2_1));
            NotifyPropertyChanged(nameof(MenuImage2_2));
            NotifyPropertyChanged(nameof(MenuImage2_3));
            NotifyPropertyChanged(nameof(MenuImage2_4));
            NotifyPropertyChanged(nameof(MenuImage2_5));
            NotifyPropertyChanged(nameof(MenuImage3_1));
            NotifyPropertyChanged(nameof(MenuImage3_2));
            NotifyPropertyChanged(nameof(MenuImage3_3));
            NotifyPropertyChanged(nameof(MenuImage3_4));
            NotifyPropertyChanged(nameof(MenuImage3_5));
            NotifyPropertyChanged(nameof(MenuImage4_1));
            NotifyPropertyChanged(nameof(MenuImage4_2));
            NotifyPropertyChanged(nameof(MenuImage4_3));
            NotifyPropertyChanged(nameof(MenuImage4_4));
            NotifyPropertyChanged(nameof(MenuImage4_5));
            NotifyPropertyChanged(nameof(MenuImage5_1));
            NotifyPropertyChanged(nameof(MenuImage5_2));
            NotifyPropertyChanged(nameof(MenuImage5_3));
            NotifyPropertyChanged(nameof(MenuImage5_4));
            NotifyPropertyChanged(nameof(MenuImage5_5));
        }

        private string GetIconFilePath(string filename) => (String.IsNullOrWhiteSpace(filename))
            ? null : $"Icons/{filename}";

        private string GetMenuIcon(int row, int column)
        {
            string result = null;

            if (InMenuMode && row > 0 && row < 6 && column > 0 && column < 6)
            {
                do
                {
                    if (_menuRowIndex == 0 && row == 1) { break; }
                    if (_menuRowIndex == 0 && row == 2) { break; }
                    if (_menuRowIndex == 1 && row == 1) { break; }
                    if (_menuRowIndex == 1 && row == 5) { break; }
                    if (_menuRowIndex == 2 && row == 4) { break; }
                    if (_menuRowIndex == 2 && row == 5) { break; }

                    if ((_menuRowIndex == 0 && row == 5 && column == 3)
                        || (_menuRowIndex == 1 && row == 4 && column == 3)
                        || (_menuRowIndex == 2 && row == 3 && column == 3))
                    {
                        result = closeIconName;
                        break;
                    }

                    if ((_menuRowIndex == 0 && row == 3)
                        || (_menuRowIndex == 1 && row == 2)
                        || (_menuRowIndex == 2 && row == 1))
                    {
                        //Need to figure out which top menu icon to show
                        int itemIndex = (column - 3) + _tempTopMenuIndex;
                        if (itemIndex > -1 && itemIndex < _menuItems.Count)
                        {
                            result = _menuItems[itemIndex].IconFile;
                        }
                    }

                    if ((_menuRowIndex == 0 && row == 4)
                        || (_menuRowIndex == 1 && row == 3)
                        || (_menuRowIndex == 2 && row == 2))
                    {
                        //Need to figure out which top menu icon to show
                        int itemIndex = (column - 3) + _tempBottomMenuIndex;
                        IList<SubMenuItem> bottomItems = _menuItems[_tempTopMenuIndex].SubMenuItems;
                        if (itemIndex > -1 && itemIndex < bottomItems.Count)
                        {
                            result = bottomItems[itemIndex].IconFile;
                        }
                    }

                } while (false);
            }

            return result;
        }

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
                    if (value.Message == BleMessage.JoystickWest)
                    {
                        if (_menuRowIndex == 0 && _tempTopMenuIndex > 0)
                        {
                            _tempTopMenuIndex--;
                            _tempBottomMenuIndex = 0;
                            NotifyMenuIconsChanged();
                        }
                        else if (_menuRowIndex == 1 && _tempBottomMenuIndex > 0)
                        {
                            _tempBottomMenuIndex--;
                            NotifyMenuIconsChanged();
                        }
                    }
                    else if (value.Message == BleMessage.JoystickEast)
                    {
                        if (_menuRowIndex == 0 && _tempTopMenuIndex < (_menuItems.Count - 1))
                        {
                            _tempTopMenuIndex++;
                            _tempBottomMenuIndex = 0;
                            NotifyMenuIconsChanged();
                        }
                        else if (_menuRowIndex == 1
                                 && _tempBottomMenuIndex < (_menuItems[_tempTopMenuIndex].SubMenuItems.Count - 1))
                        {
                            _tempBottomMenuIndex++;
                            NotifyMenuIconsChanged();
                        }
                    }
                    else if (value.Message == BleMessage.JoystickNorth && _menuRowIndex > 0)
                    {
                        _menuRowIndex--;
                        NotifyMenuIconsChanged();
                    }
                    else if (value.Message == BleMessage.JoystickSouth && _menuRowIndex < 2)
                    {
                        _menuRowIndex++;
                        NotifyMenuIconsChanged();
                    }
                    else if (value.Message == BleMessage.JoystickButton)
                    {
                        if (_menuRowIndex == 0 && _tempTopMenuIndex != _selectedTopMenuIndex)
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                TopMenuItemSelected(_menuItems[_tempTopMenuIndex]);
                            });                           
                        }
                        else if (_menuRowIndex == 1 && (_tempTopMenuIndex != _selectedTopMenuIndex))
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                TopMenuItemSelected(_menuItems[_tempTopMenuIndex]);
                                BottomMenuItemSelected(_menuItems[_tempTopMenuIndex].SubMenuItems[_tempBottomMenuIndex]);
                            });
                        }
                        else if (_menuRowIndex == 1 && (_tempBottomMenuIndex != _selectedBottomMenuIndex))
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                BottomMenuItemSelected(_menuItems[_tempTopMenuIndex].SubMenuItems[_tempBottomMenuIndex]);
                            });
                        }
                        InMenuMode = false;
                    }
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
                    if (value.Message == BleMessage.ButtonGreen)
                    {
                        _tempTopMenuIndex = _selectedTopMenuIndex;
                        _tempBottomMenuIndex = _selectedBottomMenuIndex;
                        _menuRowIndex = 0;

                        //Toggle menu on and off
                        InMenuMode = !InMenuMode;
                        NotifyMenuIconsChanged();
                    } 
                }
            }
        }

        public void TopMenuItemSelected(MenuItem menuItem)
        {
            bool itemChanged = false;
            IList<SubMenuItem> subMenuItems = null;
            int index = 0;
            foreach (MenuItem item in _menuItems)
            {
                if (item == menuItem)
                {
                    _selectedTopMenuIndex = index;
                    itemChanged = !item.IsActive;
                    item.IsActive = true;
                    subMenuItems = item.SubMenuItems;
                }
                else
                {
                    item.IsActive = false;
                }

                index++;
            }

            if (itemChanged)
            {
                _selectedBottomMenuIndex = 0;
                TopMenuItems.ResetItems(_menuItems
                    .Select(s => new SelectableItemViewModel<MenuItem>(s, TopMenuItemSelected)).ToArray());
                if (subMenuItems != null)
                {
                    for (int i = 0; i < subMenuItems.Count; i++)
                    {
                        subMenuItems[i].IsActive = (i == _selectedBottomMenuIndex);
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

            int index = 0;
            foreach (SubMenuItem item in subMenuItems)
            {
                if (item == menuItem)
                {
                    _selectedBottomMenuIndex = index;
                    itemChanged = !item.IsActive;
                    item.IsActive = true;
                    choiceItems = item.ChoiceItems;
                }
                else
                {
                    item.IsActive = false;
                }

                index++;
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
