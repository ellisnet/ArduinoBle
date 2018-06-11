using Prism.Commands;

namespace KeyboardMenu.Interfaces
{
    public interface IValueEntryCompleted
    {
        DelegateCommand ValueEntryCompletedCommand { get; }
    }
}
