using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BluetoothLevel.XFApp.ViewModels;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace BluetoothLevel.XFApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LevelPage : ContentPage
	{
	    private int indicatorHeight = 0;
        private SKColor indicatorColor = SKColor.Empty;

		public LevelPage ()
		{
			InitializeComponent ();
		}

	    protected override void OnBindingContextChanged()
	    {
	        base.OnBindingContextChanged();
	        if (BindingContext is ILevelValueProvider provider)
	        {
	            provider.IndicatorUpdateAction = (height, color) =>
	            {
	                indicatorHeight = height;
	                indicatorColor = color;
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        LevelIndicator.InvalidateSurface();
                    });                    
	            };
	        }
	    }

	    private void LevelIndicator_OnPaintSurface(object sender, SKPaintSurfaceEventArgs args)
	    {
	        SKImageInfo info = args.Info;
	        SKSurface surface = args.Surface;
	        SKCanvas canvas = surface.Canvas;

	        canvas.Clear();

	        int canvasHeight = info.Height;
	        int canvasWidth = info.Width;

	        int scaledHeight = (int) (Math.Round((indicatorHeight / 2000.0d) * canvasHeight));
	        if (scaledHeight < 0)
	        {
	            scaledHeight = 0;
	        }
            else if (scaledHeight > canvasHeight)
	        {
	            scaledHeight = canvasHeight;
	        }

	        SKPaint paint = new SKPaint
	        {
	            Style = SKPaintStyle.Fill,
	            Color = indicatorColor
	        };

            SKRect rect = new SKRect(0, (canvasHeight - scaledHeight), canvasWidth, canvasHeight);

            canvas.DrawRect(rect, paint);
        }
	}
}