using KeyboardMenu.Interfaces;

namespace KeyboardMenu.Models
{
    public class ChoiceItem : ISelectableItem
    {
        public int Order { get; set; }
        public string Name => $"Item {Order}";
        public string IconFile { get; set; }
        public string Value { get; set; }
        public bool IsActive { get; set; }
    }
}
