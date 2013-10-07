// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SortableObservableCollection.cs" company="None">
//   None
// </copyright>
// <summary>
//   Defines the SortableObservableCollection type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TfsWorkbench.Core.Helpers
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;

    /// <summary>
    /// Initializes instance of SortableObservableCollection&lt;T&gt;
    /// </summary>
    /// <typeparam name="T">The collection item type</typeparam>
    public class SortableObservableCollection<T> : ObservableCollection<T>
    {
        /// <summary>
        /// Sorts this instance.
        /// </summary>
        public void Sort()
        {
            this.Sort(0, this.Count, null);
        }

        /// <summary>
        /// Sorts the specified comparer.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        public void Sort(IComparer<T> comparer)
        {
            this.Sort(0, this.Count, comparer);
        }

        /// <summary>
        /// Sorts the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="count">The count.</param>
        /// <param name="comparer">The comparer.</param>
        public void Sort(int index, int count, IComparer<T> comparer)
        {
            var list = this.Items as List<T>;

            if (list != null)
            {
                list.Sort(index, count, comparer);
            }

            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}