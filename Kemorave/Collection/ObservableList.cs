using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Kemorave.Collection
{
    /// <summary>
    /// An observable list that support modification from non-UI threads 
    /// <para/>
    /// Used after initializing <see cref="ThreadingHelp.Initialize(IMainUIThreadIInvoker)"/>
    /// </summary>
    public class ObservableList<T> : System.Collections.ObjectModel.ObservableCollection<T>
    {
        public static PropertyChangedEventArgs HasItemsPropertyChangedEventArgs = new PropertyChangedEventArgs("HasItems");
        public event EventHandler OnClear;
        public bool HasItems => Count > 0;
        /// <summary>
        /// An observable list that support modification from non-UI threads 
        /// <para/>
        /// Used after initializing <see cref="ThreadingHelp.Initialize(IMainUIThreadIInvoker)"/>
        /// </summary>
        public ObservableList()
        {
            CollectionChanged += OnCollectionChanged; ;
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (Count == 0 || Count == 1)
            {
                OnPropertyChanged(HasItemsPropertyChangedEventArgs);
            }
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
            ThreadingHelp.RunOnMainUIThread(() =>
            {
                base.Add(item);
            });
        }
        public new void Remove(T item)
        {
            //Thread.Sleep(10);
            ThreadingHelp.RunOnMainUIThread(() =>
            {
                base.Remove(item);
            });
        }

        public new void Move(int oldIndex, int newIndex)
        {
            ThreadingHelp.RunOnMainUIThread(() =>
            {
                base.MoveItem(oldIndex, newIndex);
            });
        }
        public new void MoveItem(int oldIndex, int newIndex)
        {
            ThreadingHelp.RunOnMainUIThread(() =>
            {
                base.MoveItem(oldIndex, newIndex);
            });
        }
        public new void InsertItem(int index, T newItem)
        {
            ThreadingHelp.RunOnMainUIThread(() =>
            {
                base.InsertItem(index, newItem);
            });
        }
        public new void Clear()
        {
            ThreadingHelp.RunOnMainUIThread(() =>
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
            {
                Clear();
            }

            foreach (T item in list)
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
            {
                Clear();
            }

            foreach (T item in list)
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
            {
                Clear();
            }

            foreach (T item in list)
            {
                Add(item);
            }
        }

        #endregion
    }
}