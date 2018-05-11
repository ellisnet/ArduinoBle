using System;
using System.Diagnostics;
using System.Reactive.Linq;
using Acr.UserDialogs;
using BluetoothLevel.XFApp.Models;
using BluetoothLevel.XFApp.Services;
using BluetoothLevel.XFApp.ViewModels.Interfaces;
using Prism.Commands;
using Prism.Navigation;
using SkiaSharp;
using Xamarin.Forms;

namespace BluetoothLevel.XFApp.ViewModels
{
    public class LevelPageViewModel : BaseViewModel, IObserver<LevelMeasurement>, ILevelValueProvider
    {
        // ReSharper disable InconsistentNaming
        private static readonly SKColor SafeColor = SKColor.Parse("#6ce26c");
        private static readonly SKColor WarningColor = SKColor.Parse("#ffee62");
        private static readonly SKColor AlarmColor = SKColor.Parse("#ff0000");
        // ReSharper restore InconsistentNaming

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

        private int _resolution = 0;
        public int Resolution
        {
            get => _resolution;
            set
            {
                if (_resolution != value)
                {
                    Debug.WriteLine($"Switching to a resolution of: {value}");
                    _measurementSubscription?.Dispose();
                    _measurementSubscription = _levelApiService
                        .GetMeasurementNotifier()
                        .Where(w => w.ValueDelta >= value || w.MeasurementType == MeasurementType.Final)
                        .Subscribe(this);
                }
                SetProperty(ref _resolution, value);
            }
        }

        public Action<int, SKColor> IndicatorUpdateAction { get; set; }

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
            SKColor indicatorColor = SafeColor;
            if (Math.Abs(value.CurrentValue) > 300)
            {
                indicatorColor = AlarmColor;
            }
            else if (Math.Abs(value.CurrentValue) > 15)
            {
                indicatorColor = WarningColor;
            }

            int indicatorValue = value.CurrentValue + 1000;
            if (indicatorValue < 0)
            {
                indicatorValue = 0;
            }
            else if (indicatorValue > 2000)
            {
                indicatorValue = 2000;
            }

            IndicatorUpdateAction?.Invoke(indicatorValue, indicatorColor);

            if (value.MeasurementType == MeasurementType.Final)
            {
                Debug.WriteLine($"A final level measurement of: {value.CurrentValue}");
                LevelBorderColor = Color.Blue;
                IsLevelIdle = true;
            }
            else if (value.MeasurementType == MeasurementType.Intermediate)
            {
                Debug.WriteLine($"An intermediate level measurement of: {value.CurrentValue}");
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
