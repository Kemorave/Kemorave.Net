using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;

namespace Kemorave.Collection
{
    public class ObservableList<T> : System.Collections.ObjectModel.ObservableCollection<T>
    {
        public static void Initialize(Kemorave.IMainUIThreadIInvoker invoker)
        {
            Threading.MainUIThreadIInvoker = invoker;
        }
        public static PropertyChangedEventArgs HasItemsPropertyChangedEventArgs = new PropertyChangedEventArgs("HasItems");
        public event EventHandler OnClear;
        public bool HasItems { get { return Count > 0; } }
        /// <summary>
        /// An observable list that support modification from non-UI threads 
        /// Used after <see cref="Initialize"/>
        /// </summary>
        public ObservableList()
        {
            CollectionChanged += ThreadSafeCollection_CollectionChanged; ;
        }

        private void ThreadSafeCollection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (Count == 0 || Count == 1)
                OnPropertyChanged(HasItemsPropertyChangedEventArgs);

        }


        public ObservableList(IEnumerable<T> list)
        {
            //SearchPropertyName = PropertyName;
            AddRange(list);
            //InitCommands();
        }
        public ObservableList(T[] list)
        {
            // SearchPropertyName = PropertyName;
            AddRange(list);
            //InitCommands();
        }

        public new void Add(T item)
        {
            //Thread.Sleep(10);
            Threading.RunOnMainUIThread(() =>
            {
                base.Add(item);
            });
        }
        public new void Remove(T item)
        {
            //Thread.Sleep(10);
            Threading.RunOnMainUIThread(() =>
            {
                base.Remove(item);
            });
        }

        public new void Move(int oldIndex, int newIndex)
        {
            Threading.RunOnMainUIThread(() =>
            {
                base.MoveItem(oldIndex, newIndex);
            });
        }
        public new void MoveItem(int oldIndex, int newIndex)
        {
            Threading.RunOnMainUIThread(() =>
            {
                base.MoveItem(oldIndex, newIndex);
            });
        }
        public new void InsertItem(int index, T newItem)
        {
            Threading.RunOnMainUIThread(() =>
            {
                base.InsertItem(index, newItem);
            });
        }
        public new void Clear()
        {
            Threading.RunOnMainUIThread(() =>
            {
                base.ClearItems();
                OnPropertyChanged(new PropertyChangedEventArgs("Count"));
                OnClear?.Invoke(this, null);
            });
        }
        #region ext

        public void AddRange(IEnumerable<T> list, bool ClearOld = false)
        {
            if (list == null)
            {
                return;
            }
            if (ClearOld)
                Clear();
            foreach (var item in list)
            {
                this.Add(item);
            }
        }
        public void AddRange(IList<T> list, bool ClearOld = false)
        {
            if (list == null)
            {
                return;
            }
            if (ClearOld)
                Clear();
            foreach (var item in list)
            {
                this.Add(item);
            }
        }
        public void AddRange(T[] list, bool ClearOld = false)
        {
            if (list == null)
            {
                return;
            }
            if (ClearOld)
                Clear();
            foreach (var item in list)
            {
                Add(item);
            }
        }

        #endregion
    }
}