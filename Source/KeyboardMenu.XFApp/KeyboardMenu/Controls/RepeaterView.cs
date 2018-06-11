using System;
using Xamarin.Forms;
using System.Collections;
using System.Collections.Specialized;

namespace KeyboardMenu.Controls
{
    public delegate void RepeaterViewItemAddedEventHandler(object sender, RepeaterViewItemAddedEventArgs args);

    public class RepeaterView : StackLayout
    {
        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
          propertyName: "ItemsSource",
          returnType: typeof(IEnumerable),
          declaringType: typeof(RepeaterView),
          defaultValue: null,
          defaultBindingMode: BindingMode.OneWay,
          propertyChanged: ItemsChanged);

        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(
          propertyName: "ItemTemplate",
          returnType: typeof(DataTemplate),
          declaringType: typeof(RepeaterView),
          defaultValue: default(DataTemplate));

        public event RepeaterViewItemAddedEventHandler ItemCreated;

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)GetValue(ItemTemplateProperty);
            set => SetValue(ItemTemplateProperty, value);
        }

        private static void ItemsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            // ReSharper disable once UnusedVariable
            var testValue = oldValue as IEnumerable;
            var newValueAsEnumerable = newValue as IEnumerable;

            var control = (RepeaterView)bindable;

            if (oldValue is INotifyCollectionChanged oldObservableCollection)
            {
                oldObservableCollection.CollectionChanged -= control.OnItemsSourceCollectionChanged;
            }

            if (newValue is INotifyCollectionChanged newObservableCollection)
            {
                newObservableCollection.CollectionChanged += control.OnItemsSourceCollectionChanged;
            }

            control.Children.Clear();

            if (newValueAsEnumerable != null)
            {
                foreach (var item in newValueAsEnumerable)
                {
                    var view = control.CreateChildViewFor(item);
                    control.Children.Add(view);
                    control.OnItemCreated(view);
                }
            }

            control.UpdateChildrenLayout();
            control.InvalidateLayout();
        }

        protected virtual void OnItemCreated(View view) =>
            ItemCreated?.Invoke(this, new RepeaterViewItemAddedEventArgs(view, view.BindingContext));

        private void OnItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var invalidate = false;

            if (e.OldItems != null)
            {
                Children.RemoveAt(e.OldStartingIndex);
                invalidate = true;
            }

            if (e.NewItems != null)
            {
                for (var i = 0; i < e.NewItems.Count; ++i)
                {
                    var item = e.NewItems[i];
                    var view = CreateChildViewFor(item);

                    Children.Insert(i + e.NewStartingIndex, view);
                    OnItemCreated(view);
                }

                invalidate = true;
            }

            if (invalidate)
            {
                UpdateChildrenLayout();
                InvalidateLayout();
            }
        }

        private View CreateChildViewFor(object item)
        {
            ItemTemplate.SetValue(BindingContextProperty, item);
            return (View)ItemTemplate.CreateContent();
        }
    }

    public class RepeaterViewItemAddedEventArgs : EventArgs
    {
        public View View { get; }

        public object Model { get; }

        public RepeaterViewItemAddedEventArgs(View view, object model)
        {
            View = view;
            Model = model;
        }
    }
}
