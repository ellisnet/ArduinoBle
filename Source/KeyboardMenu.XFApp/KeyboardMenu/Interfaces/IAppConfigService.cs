using Prism;
using Prism.Ioc;

namespace KeyboardMenu.Interfaces
{
    public interface IAppConfigService : IPlatformInitializer, IContainerProvider
    {
        void SetContainer(IContainerProvider container);
        IContainerProvider Container { get; }
        T Resolve<T>(string name = null);
    }
}
