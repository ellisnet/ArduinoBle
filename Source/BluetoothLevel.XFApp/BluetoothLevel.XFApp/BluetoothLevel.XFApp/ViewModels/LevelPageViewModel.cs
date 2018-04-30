using System;
using System.Diagnostics;
using Acr.UserDialogs;
using BluetoothLevel.XFApp.Models;
using BluetoothLevel.XFApp.Services;
using Prism.Commands;
using Prism.Navigation;

using Xamarin.Forms;

namespace BluetoothLevel.XFApp.ViewModels
{
    public class LevelPageViewModel : BaseViewModel, IObserver<LevelMeasurement>
    {
        private ILevelApiService _levelApiService;
        private IDisposable _measurementSubscription;

        #region Bindable properties

        private bool _isLevelIdle = true;
        public bool IsLevelIdle
        {
            get => _isLevelIdle;
            set => SetProperty(ref _isLevelIdle, value);
        }

        private Color _levelBorderColor = Color.Black;
        public Color LevelBorderColor
        {
            get => _levelBorderColor;
            set => SetProperty(ref _levelBorderColor, value);
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
            LevelBorderColor = Color.Black;
            IsLevelIdle = !(await _levelApiService.RequestMeasurement());
        }

        #endregion

        #endregion

        #region IObserver implementation

        public void OnCompleted()
        {
            //Not doing anything here yet
            //  If there was functionality for turning off the level, would maybe 
            //  implement something here based on that.
        }

        public void OnError(Exception error)
        {
            //Not doing anything here yet
        }

        public void OnNext(LevelMeasurement value)
        {
            if (value.MeasurementType == MeasurementType.Final)
            {
                Debug.WriteLine($"A final level measurement of: {value.Value}");
                LevelBorderColor = Color.Blue;
                IsLevelIdle = true;
            }
            else if (value.MeasurementType == MeasurementType.Intermediate)
            {
                Debug.WriteLine($"An intermediate level measurement of: {value.Value}");
            }
        }

        #endregion

        public LevelPageViewModel(
            INavigationService navigationService,
            IUserDialogs userDialogs,
            ILevelApiService levelApiService)
            : base(navigationService, userDialogs)
        {
            _levelApiService = levelApiService ?? throw new ArgumentNullException(nameof(levelApiService));
            //Set up my subscription of the LevelMeasurement observable
            _measurementSubscription = _levelApiService.GetMeasurementNotifier().Subscribe(this);
        }

        public override void Destroy()
        {
            _measurementSubscription?.Dispose();
            _measurementSubscription = null;
            _levelApiService = null;
            base.Destroy();
        }
    }
}
