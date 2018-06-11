using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using KeyboardMenu.Interfaces;
using Prism.Commands;

namespace KeyboardMenu.ViewModels
{
    public class SelectableItemViewModel<T> : INotifyPropertyChanged where T : ISelectableItem
    {
        private readonly Action<T> _selectAction;

        public T Item { get; }
        public int Order => Item.Order;
        public string IconFile => $"Icons/{Item.IconFile}";
        public string Name => Item.Name;
        public bool IsActive => Item.IsActive;
        public string Value => Item.Value;

        private DelegateCommand _itemSelectedCommand;
        public DelegateCommand ItemSelectedCommand => _itemSelectedCommand
            ?? (_itemSelectedCommand = new DelegateCommand(() => _selectAction.Invoke(Item)));

        public void NotifyValueChanged()
        {
            OnPropertyChanged(nameof(Value));
        }

        public SelectableItemViewModel(T item, Action<T> selectAction)
        {
            if (item == null) { throw new ArgumentNullException(nameof(item)); }
            Item = item;
            _selectAction = selectAction ?? throw new ArgumentNullException(nameof(selectAction));
        }

        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
