using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Acr.UserDialogs;
using Plugin.BLE;
using Xamarin.Forms;

namespace ArduinoBle.XFApp
{
	public partial class App : Application
	{
	    public static void ShowToast(string toastMsg)
	    {
	        var toastConfig = new ToastConfig(toastMsg);
	        toastConfig.SetPosition(ToastPosition.Top);
	        toastConfig.SetDuration(2000);
	        toastConfig.SetBackgroundColor(System.Drawing.Color.FromArgb(12, 131, 193));

	        UserDialogs.Instance.Toast(toastConfig);
	    }

        public App ()
		{
			InitializeComponent();

		    var ble = CrossBluetoothLE.Current;
		    ble.StateChanged += (s, e) =>
		    {
		        ShowToast($"The bluetooth state changed to {e.NewState}");
		    };

            MainPage = new ArduinoBle.XFApp.MainPage();
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
