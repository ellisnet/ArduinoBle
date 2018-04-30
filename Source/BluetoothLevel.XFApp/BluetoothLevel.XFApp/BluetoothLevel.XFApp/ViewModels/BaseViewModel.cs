using System;
using Acr.UserDialogs;
using Prism.Mvvm;
using Prism.Navigation;

namespace BluetoothLevel.XFApp.ViewModels
{
    public abstract class BaseViewModel : BindableBase, IDestructible
    {
        protected INavigationService NavigationService;
        protected IUserDialogs UserDialogs;

        protected BaseViewModel(INavigationService navigationService, IUserDialogs userDialogs)
        {
            NavigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            UserDialogs = userDialogs ?? throw new ArgumentNullException(nameof(userDialogs));
        }

        public virtual void Destroy()
        {
            //nothing to do here yet
        }
    }

    public static class DesignTimeViewModelLocator
    {
        public static ScanPageViewModel ScanPage => null;
        public static LevelPageViewModel LevelPage => null;
    }
}
