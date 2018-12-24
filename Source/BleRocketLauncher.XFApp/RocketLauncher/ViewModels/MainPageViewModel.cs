using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Acr.UserDialogs;
using CodeBrix.Prism.Abstract;
using CodeBrix.Prism.Services;
using Prism.Commands;
using Prism.Navigation;
using RocketLauncher.Services;

namespace RocketLauncher.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private ILauncherService _launcherService;

        #region Bindable properties

        private bool _isLaunching;
        public bool IsLaunching
        {
            get => _isLaunching;
            set => SetProperty(ref _isLaunching, value);
        }

        private bool _isConnected;
        public bool IsConnected
        {
            get => _isConnected;
            set => SetProperty(ref _isConnected, value);
        }

        private bool _rocket1Launched;
        public bool Rocket1Launched
        {
            get => _rocket1Launched;
            set
            {
                SetProperty(ref _rocket1Launched, value);
                NotifyPropertyChanged(nameof(AllRocketsLaunched));
            }
        }

        private bool _rocket2Launched;
        public bool Rocket2Launched
        {
            get => _rocket2Launched;
            set
            {
                SetProperty(ref _rocket2Launched, value);
                NotifyPropertyChanged(nameof(AllRocketsLaunched));
            }
        }

        private bool _rocket3Launched;
        public bool Rocket3Launched
        {
            get => _rocket3Launched;
            set
            {
                SetProperty(ref _rocket3Launched, value);
                NotifyPropertyChanged(nameof(AllRocketsLaunched));
            }
        }

        private bool _rocket4Launched;
        public bool Rocket4Launched
        {
            get => _rocket4Launched;
            set
            {
                SetProperty(ref _rocket4Launched, value);
                NotifyPropertyChanged(nameof(AllRocketsLaunched));
            }
        }

        private bool _rocket5Launched;
        public bool Rocket5Launched
        {
            get => _rocket5Launched;
            set
            {
                SetProperty(ref _rocket5Launched, value);
                NotifyPropertyChanged(nameof(AllRocketsLaunched));
            }
        }

        private bool _rocket6Launched;
        public bool Rocket6Launched
        {
            get => _rocket6Launched;
            set
            {
                SetProperty(ref _rocket6Launched, value);
                NotifyPropertyChanged(nameof(AllRocketsLaunched));
            }
        }

        private bool _rocket7Launched;
        public bool Rocket7Launched
        {
            get => _rocket7Launched;
            set
            {
                SetProperty(ref _rocket7Launched, value);
                NotifyPropertyChanged(nameof(AllRocketsLaunched));
            }
        }

        private bool _rocket8Launched;
        public bool Rocket8Launched
        {
            get => _rocket8Launched;
            set
            {
                SetProperty(ref _rocket8Launched, value);
                NotifyPropertyChanged(nameof(AllRocketsLaunched));
            }
        }

        public bool AllRocketsLaunched =>
            Rocket1Launched
            && Rocket2Launched
            && Rocket3Launched
            && Rocket4Launched
            && Rocket5Launched
            && Rocket6Launched
            && Rocket7Launched
            && Rocket8Launched;

        #endregion

        #region Commands and their implementations

        #region Launch1Command

        private DelegateCommand _launch1Command;
        public DelegateCommand Launch1Command =>
            LazyCreateCommand(ref _launch1Command, DoLaunch1, CanLaunch1)
                .ObservesProperty(() => IsConnected)
                .ObservesProperty(() => Rocket1Launched)
                .ObservesProperty(() => IsLaunching);

        public async void DoLaunch1()
        {
            if (CanLaunch1())
            {
                IsLaunching = await _launcherService.SendLaunchMessage("L0", () =>
                {
                    DialogService.Toast("Rocket 1 launched.");
                    Rocket1Launched = true;
                    IsLaunching = false;
                });
            }
        }

        public bool CanLaunch1() => IsConnected && (!IsLaunching) && (!Rocket1Launched);

        #endregion

        #region Launch2Command

        private DelegateCommand _launch2Command;
        public DelegateCommand Launch2Command =>
            LazyCreateCommand(ref _launch2Command, DoLaunch2, CanLaunch2)
                .ObservesProperty(() => IsConnected)
                .ObservesProperty(() => Rocket2Launched)
                .ObservesProperty(() => IsLaunching);

        public async void DoLaunch2()
        {
            if (CanLaunch2())
            {
                IsLaunching = await _launcherService.SendLaunchMessage("L1", () =>
                {
                    DialogService.Toast("Rocket 2 launched.");
                    Rocket2Launched = true;
                    IsLaunching = false;
                });
            }
        }

        public bool CanLaunch2() => IsConnected && (!IsLaunching) && (!Rocket2Launched);

        #endregion

        #region Launch3Command

        private DelegateCommand _launch3Command;
        public DelegateCommand Launch3Command =>
            LazyCreateCommand(ref _launch3Command, DoLaunch3, CanLaunch3)
                .ObservesProperty(() => IsConnected)
                .ObservesProperty(() => Rocket3Launched)
                .ObservesProperty(() => IsLaunching);

        public async void DoLaunch3()
        {
            if (CanLaunch3())
            {
                IsLaunching = await _launcherService.SendLaunchMessage("L2", () =>
                {
                    DialogService.Toast("Rocket 3 launched.");
                    Rocket3Launched = true;
                    IsLaunching = false;
                });
            }
        }

        public bool CanLaunch3() => IsConnected && (!IsLaunching) && (!Rocket3Launched);

        #endregion

        #region Launch4Command

        private DelegateCommand _launch4Command;
        public DelegateCommand Launch4Command =>
            LazyCreateCommand(ref _launch4Command, DoLaunch4, CanLaunch4)
                .ObservesProperty(() => IsConnected)
                .ObservesProperty(() => Rocket4Launched)
                .ObservesProperty(() => IsLaunching);

        public async void DoLaunch4()
        {
            if (CanLaunch4())
            {
                IsLaunching = await _launcherService.SendLaunchMessage("L3", () =>
                {
                    DialogService.Toast("Rocket 4 launched.");
                    Rocket4Launched = true;
                    IsLaunching = false;
                });
            }
        }

        public bool CanLaunch4() => IsConnected && (!IsLaunching) && (!Rocket4Launched);

        #endregion

        #region Launch5Command

        private DelegateCommand _launch5Command;
        public DelegateCommand Launch5Command =>
            LazyCreateCommand(ref _launch5Command, DoLaunch5, CanLaunch5)
                .ObservesProperty(() => IsConnected)
                .ObservesProperty(() => Rocket5Launched)
                .ObservesProperty(() => IsLaunching);

        public async void DoLaunch5()
        {
            if (CanLaunch5())
            {
                IsLaunching = await _launcherService.SendLaunchMessage("L4", () =>
                {
                    DialogService.Toast("Rocket 5 launched.");
                    Rocket5Launched = true;
                    IsLaunching = false;
                });
            }
        }

        public bool CanLaunch5() => IsConnected && (!IsLaunching) && (!Rocket5Launched);

        #endregion

        #region Launch6Command

        private DelegateCommand _launch6Command;
        public DelegateCommand Launch6Command =>
            LazyCreateCommand(ref _launch6Command, DoLaunch6, CanLaunch6)
                .ObservesProperty(() => IsConnected)
                .ObservesProperty(() => Rocket6Launched)
                .ObservesProperty(() => IsLaunching);

        public async void DoLaunch6()
        {
            if (CanLaunch6())
            {
                IsLaunching = await _launcherService.SendLaunchMessage("L5", () =>
                {
                    DialogService.Toast("Rocket 6 launched.");
                    Rocket6Launched = true;
                    IsLaunching = false;
                });
            }
        }

        public bool CanLaunch6() => IsConnected && (!IsLaunching) && (!Rocket6Launched);

        #endregion

        #region Launch7Command

        private DelegateCommand _launch7Command;
        public DelegateCommand Launch7Command =>
            LazyCreateCommand(ref _launch7Command, DoLaunch7, CanLaunch7)
                .ObservesProperty(() => IsConnected)
                .ObservesProperty(() => Rocket7Launched)
                .ObservesProperty(() => IsLaunching);

        public async void DoLaunch7()
        {
            if (CanLaunch7())
            {
                IsLaunching = await _launcherService.SendLaunchMessage("L6", () =>
                {
                    DialogService.Toast("Rocket 7 launched.");
                    Rocket7Launched = true;
                    IsLaunching = false;
                });
            }
        }

        public bool CanLaunch7() => IsConnected && (!IsLaunching) && (!Rocket7Launched);

        #endregion

        #region Launch8Command

        private DelegateCommand _launch8Command;
        public DelegateCommand Launch8Command =>
            LazyCreateCommand(ref _launch8Command, DoLaunch8, CanLaunch8)
                .ObservesProperty(() => IsConnected)
                .ObservesProperty(() => Rocket8Launched)
                .ObservesProperty(() => IsLaunching);

        public async void DoLaunch8()
        {
            if (CanLaunch8())
            {
                IsLaunching = await _launcherService.SendLaunchMessage("L7", () =>
                {
                    DialogService.Toast("Rocket 8 launched.");
                    Rocket8Launched = true;
                    IsLaunching = false;
                });
            }
        }

        public bool CanLaunch8() => IsConnected && (!IsLaunching) && (!Rocket8Launched);

        #endregion

        #region LaunchAllCommand

        private DelegateCommand _launchAllCommand;
        public DelegateCommand LaunchAllCommand =>
            LazyCreateCommand(ref _launchAllCommand, DoLaunchAll, CanLaunchAll)
                .ObservesProperty(() => IsConnected)
                .ObservesProperty(() => AllRocketsLaunched)
                .ObservesProperty(() => IsLaunching);

        public async void DoLaunchAll()
        {
            if (CanLaunchAll())
            {
                IsLaunching = await _launcherService.SendLaunchMessage("L8", () =>
                {
                    DialogService.Toast("All rockets launched.");
                    Rocket1Launched = true;
                    Rocket2Launched = true;
                    Rocket3Launched = true;
                    Rocket4Launched = true;
                    Rocket5Launched = true;
                    Rocket6Launched = true;
                    Rocket7Launched = true;
                    Rocket8Launched = true;
                    IsLaunching = false;
                });
            }
        }

        public bool CanLaunchAll() => IsConnected && (!IsLaunching) && (!AllRocketsLaunched);

        #endregion

        #endregion

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            await Task.Delay(2000);  //Wait a couple of seconds for UI to load

            IsConnected = _launcherService.IsConnected
                || (await _launcherService.Connect());

            //TODO: I need to figure out why await _launcherService.Connect() sometimes returns
            // false; even when successfully connected.
            if (!IsConnected)
            {
                await Task.Delay(2000);
                IsConnected = IsConnected || _launcherService.IsConnected;
            }
        }

        public MainPageViewModel(
            INavigationService navigationService,
            ILoggerService loggerService,
            IUserDialogs dialogService,
            ILauncherService launcherService)
            : base(navigationService, loggerService, dialogService)
        {
            _launcherService = launcherService ?? throw new ArgumentNullException(nameof(launcherService));
        }

        public override void Destroy()
        {
            _launcherService = null;
            base.Destroy();
        }
    }
}
