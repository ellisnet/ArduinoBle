using System;
using Acr.UserDialogs;
using BluetoothLevel.XFApp.Services;
using BluetoothLevel.XFApp.Views;
using Prism.Commands;
using Prism.Navigation;

namespace BluetoothLevel.XFApp.ViewModels
{
    public class ScanPageViewModel : BaseViewModel
    {
        private ILevelApiService _levelApiService;

        #region Bindable properties

        private bool _isReadyToScan = true;
        public bool IsReadyToScan
        {
            get => _isReadyToScan;
            set => SetProperty(ref _isReadyToScan, value);
        }

        #endregion

        #region Commands and their implementations

        #region ScanForLevelCommand

        private DelegateCommand _scanForLevelCommand;
        public DelegateCommand ScanForLevelCommand => _scanForLevelCommand
            ?? (_scanForLevelCommand = new DelegateCommand(DoScanForLevel, () => IsReadyToScan)
                                                          .ObservesProperty(() => IsReadyToScan));

        public async void DoScanForLevel()
        {
            IsReadyToScan = false;
            if (await _levelApiService.ScanForLevel())
            {
                await NavigationService.NavigateAsync($"{nameof(LevelPage)}");
            }
            else
            {
                IsReadyToScan = true;
            }
        }

        #endregion

        #endregion

        public ScanPageViewModel(
            INavigationService navigationService, 
            IUserDialogs userDialogs,
            ILevelApiService levelApiService)
            : base(navigationService, userDialogs)
        {
            _levelApiService = levelApiService ?? throw new ArgumentNullException(nameof(levelApiService));
        }

        public override void Destroy()
        {
            _levelApiService = null;
            base.Destroy();
        }
    }
}
