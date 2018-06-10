using Acr.UserDialogs;
using Prism.Navigation;

namespace KeyboardMenu.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public MainPageViewModel(
            INavigationService navigationService,
            IUserDialogs userDialogService)
            : base(navigationService, userDialogService)
        {
            PageTitle = DefaultPageTitle;
        }
    }
}
