using FFImageLoading.Forms.Platform;
using FFImageLoading.Svg.Forms;
using FFImageLoading.Transformations;
using KeyboardMenu.Services;
using Prism.Ioc;

namespace KeyboardMenu.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            LoadApplication(new KeyboardMenu.App(new UwpConfigService()));
        }
    }

    public class UwpConfigService : BaseAppConfigService
    {
        public override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //Nothing to register yet
        }

        public UwpConfigService()
        {
            //Initialize SVG image and transformation support
            CachedImageRenderer.Init();
            // ReSharper disable UnusedVariable
            var ignoreImage = typeof(SvgCachedImage);
            var ignoreTransformation = typeof(GrayscaleTransformation);
            // ReSharper restore UnusedVariable
        }
    }
}
