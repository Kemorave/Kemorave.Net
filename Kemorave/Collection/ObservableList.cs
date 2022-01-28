using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Kemorave.Collection
{
	/// <summary>
	/// An observable list that support modification from non-UI threads 
	/// <para/>
	/// Used after initializing <see cref="ThreadingHelper.Initialize(IMainUIThreadIInvoker)"/>
	/// </summary>
	public class ObservableList<T> : System.Collections.ObjectModel.ObservableCollection<T>
	{
		public static PropertyChangedEventArgs HasItemsPropertyChangedEventArgs = new PropertyChangedEventArgs("HasItems");
		public event EventHandler OnClear;
		public bool HasItems => Count > 0;
		/// <summary>
		/// An observable list that support modification from non-UI threads 
		/// <para/>
		/// Used after initializing <see cref="ThreadingHelper.Initialize(IMainUIThreadIInvoker)"/>
		/// </summary>
		public ObservableList()
		{
			CollectionChanged += OnCollectionChanged; ;
		}
		public ObservableList(IEnumerable<T> list) : base(list??new List<T>())
		{
		}

		private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (Count == 0 || Count == 1)
			{
				OnPropertyChanged(HasItemsPropertyChangedEventArgs);
			}
		}



		public async Task AddAsync(T item)
		{
			;
			await ThreadingHelper.RunOnMainUIThread(() =>
			 {
				 base.Add(item);
			 });
		}
		public async Task RemoveAsync(T item)
		{
			//Thread.Sleep(10);
			await ThreadingHelper.RunOnMainUIThread(() =>
			 {
				 base.Remove(item);
			 });
		}

		public async Task MoveAsync(int oldIndex, int newIndex)
		{
			await ThreadingHelper.RunOnMainUIThread(() =>
			 {
				 base.MoveItem(oldIndex, newIndex);
			 });
		}
		public async Task MoveItemAsync(int oldIndex, int newIndex)
		{
			await ThreadingHelper.RunOnMainUIThread(() =>
			 {
				 base.MoveItem(oldIndex, newIndex);
			 });
		}
		public async Task InsertItemAsync(int index, T newItem)
		{
			await ThreadingHelper.RunOnMainUIThread(() =>
			 {
				 base.InsertItem(index, newItem);
			 });
		}
		public new void Clear()
		{
			base.ClearItems();
			OnPropertyChanged(new PropertyChangedEventArgs("Count"));
			OnClear?.Invoke(this, null);
		}
		public async Task ClearAsync()
		{
			await ThreadingHelper.RunOnMainUIThread(() =>
			 {
				 Clear();
			 });
		}
		#region ext

		public async Task AddRangeAsync(IEnumerable<T> list, bool ClearOld = false)
		{
			if (list == null)
			{
				return;
			}
			if (ClearOld)
			{
				await ClearAsync();
			}

			foreach (T item in list)
			{
				await this.AddAsync(item);
			}
		}
		public async Task AddRangeAsync(IList<T> list, bool ClearOld = false)
		{
			if (list == null)
			{
				return;
			}
			if (ClearOld)
			{
				await ClearAsync();
			}

			foreach (T item in list)
			{
				await this.AddAsync(item);
			}
		}
		public async Task AddRangeAsync(T[] list, bool ClearOld = false)
		{
			if (list == null)
			{
				return;
			}
			if (ClearOld)
			{
				await ClearAsync();
			}

			foreach (T item in list)
			{
				await AddAsync(item);
			}
		}

		#endregion
	}
}