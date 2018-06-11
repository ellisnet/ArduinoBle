using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace KeyboardMenu.Extensions
{
    public static class CollectionExtensions
    {
        public static void AddRange<T>(this ObservableCollection<T> collection, IList<T> items)
        {
            if (collection == null) { throw new ArgumentNullException(nameof(collection));}
            if (items == null) { throw new ArgumentNullException(nameof(items)); }

            foreach (T item in items)
            {
                collection.Add(item);
            }
        }

        public static void ResetItems<T>(this ObservableCollection<T> collection, IList<T> items)
        {
            if (collection == null) { throw new ArgumentNullException(nameof(collection)); }
            if (items == null) { throw new ArgumentNullException(nameof(items)); }

            T[] itemsToRemove = collection.Select(s => s).ToArray();
            foreach (T item in itemsToRemove)
            {
                collection.Remove(item);
            }

            foreach (T item in items)
            {
                collection.Add(item);
            }
        }
    }
}
