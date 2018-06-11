namespace KeyboardMenu.Interfaces
{
    public interface ISelectableItem
    {
        int Order { get; }
        string IconFile { get; }
        string Name { get; }
        bool IsActive { get; }
        string Value { get; }
    }
}
