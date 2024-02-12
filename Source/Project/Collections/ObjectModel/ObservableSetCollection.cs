using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

namespace RegionOrebroLan.Localization.Collections.ObjectModel
{
	public class ObservableSetCollection<T> : ObservableCollection<T>
	{
		#region Methods

		[SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters")]
		public virtual void Add(IEnumerable<T> items)
		{
			items = items?.ToArray();

			if(items == null)
				throw new ArgumentNullException(nameof(items));

			if(items.Any(item => item == null))
				throw new ArgumentException("The items can not contain null-values.", nameof(items));

			if(items.Count() > items.Distinct().Count())
				throw new ArgumentException("The items can not contain duplicate values.", nameof(items));

			if(items.Intersect(this).Any())
				throw new ArgumentException("The collection already contains one ore more of the items.", nameof(items));

			foreach(var item in items)
			{
				this.Add(item);
			}
		}

		protected override void InsertItem(int index, T item)
		{
			this.ValidateSet(item);

			base.InsertItem(index, item);
		}

		protected override void SetItem(int index, T item)
		{
			this.ValidateSet(item);

			base.SetItem(index, item);
		}

		[SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters")]
		protected internal virtual void ValidateSet(T item)
		{
			if(item == null)
				throw new ArgumentNullException(nameof(item));

			if(this.Contains(item))
				throw new ArgumentException("The collection already contains the item.", nameof(item));
		}

		#endregion
	}
}