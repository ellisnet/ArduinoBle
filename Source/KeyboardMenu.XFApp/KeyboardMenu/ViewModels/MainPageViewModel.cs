using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Acr.UserDialogs;
using KeyboardMenu.Extensions;
using KeyboardMenu.Models;
using KeyboardMenu.Views;
using Prism.Navigation;

namespace KeyboardMenu.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private readonly IList<MenuItem> _menuItems;

        #region Bindable properties

        public ObservableCollection<SelectableItemViewModel<MenuItem>> TopMenuItems { get; set; } 
            = new ObservableCollection<SelectableItemViewModel<MenuItem>>();

        public ObservableCollection<SelectableItemViewModel<SubMenuItem>> BottomMenuItems { get; set; } 
            = new ObservableCollection<SelectableItemViewModel<SubMenuItem>>();

        public ObservableCollection<SelectableItemViewModel<ChoiceItem>> ChoiceItems { get; set; } 
            = new ObservableCollection<SelectableItemViewModel<ChoiceItem>>();

        #endregion

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
                    foreach (ChoiceItem choice in selectedSubMenuItem.ChoiceItems)
                    {
                        choice.IsActive = false;
                    }

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
                foreach (ChoiceItem choice in choiceItems)
                {
                    choice.IsActive = false;
                }

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
                foreach (SelectableItemViewModel<ChoiceItem> item in ChoiceItems)
                {
                    item.NotifyValueChanged();
                }
                NotifyPropertyChanged(nameof(ChoiceItems));
            }
        }

        public MainPageViewModel(
            INavigationService navigationService,
            IUserDialogs userDialogService)
            : base(navigationService, userDialogService)
        {
            PageTitle = DefaultPageTitle;

            //Initialize collections
            _menuItems = MenuItem.GetMenuItems();
            TopMenuItems.AddRange(_menuItems
                .Select(s => new SelectableItemViewModel<MenuItem>(s, TopMenuItemSelected)).ToArray());

            IList<SubMenuItem> subMenuItems = _menuItems.Single(s => s.IsActive).SubMenuItems;
            ResetBottomMenuItems(subMenuItems);
            ResetChoiceItems(subMenuItems.Single(s => s.IsActive).ChoiceItems);
        }
    }
}
