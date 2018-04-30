using System;
using System.Collections.Generic;
using System.Text;
using Acr.UserDialogs;
using BluetoothLevel.XFApp.Services;
using Prism.Commands;
using Prism.Navigation;

namespace BluetoothLevel.XFApp.ViewModels
{
    public class LevelPageViewModel : BaseViewModel
    {
        private ILevelApiService _levelApiService;

        #region Bindable properties

        private bool _isLevelIdle = true;
        public bool IsLevelIdle
        {
            get => _isLevelIdle;
            set => SetProperty(ref _isLevelIdle, value);
        }

        #endregion

        #region Commands and their implementations

        #region RequestCalibrationCommand 

        private DelegateCommand _requestCalibrationCommand;
        public DelegateCommand RequestCalibrationCommand => _requestCalibrationCommand
            ?? (_requestCalibrationCommand = new DelegateCommand(DoRequestCalibration, () => IsLevelIdle)
                                                                .ObservesProperty(() => IsLevelIdle));

        public async void DoRequestCalibration()
        {
            IsLevelIdle = false;
            await _levelApiService.RequestCalibration();
            IsLevelIdle = true;
        }

        #endregion

        #region RequestMeasurementCommand 

        private DelegateCommand _requestMeasurementCommand;
        public DelegateCommand RequestMeasurementCommand => _requestMeasurementCommand
            ?? (_requestMeasurementCommand = new DelegateCommand(DoRequestMeasurement, () => IsLevelIdle)
                                                                .ObservesProperty(() => IsLevelIdle));

        public async void DoRequestMeasurement()
        {
            IsLevelIdle = false;
            IsLevelIdle = !(await _levelApiService.RequestMeasurement());
        }

        #endregion

        #endregion

        public LevelPageViewModel(
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
