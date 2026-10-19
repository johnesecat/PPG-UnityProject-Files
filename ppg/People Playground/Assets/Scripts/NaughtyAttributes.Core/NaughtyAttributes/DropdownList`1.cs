using System.Collections;
using System.Collections.Generic;

namespace NaughtyAttributes
{
	public class DropdownList<T> : IDropdownList, IEnumerable<KeyValuePair<string, object>>, IEnumerable
	{
		private List<KeyValuePair<string, object>> _values;

		public DropdownList()
		{
			_values = new List<KeyValuePair<string, object>>();
		}

		public void Add(string displayName, T value)
		{
			_values.Add(new KeyValuePair<string, object>(displayName, value));
		}

		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			return _values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public static explicit operator DropdownList<object>(DropdownList<T> target)
		{
			DropdownList<object> dropdownList = new DropdownList<object>();
			foreach (KeyValuePair<string, object> item in target)
			{
				dropdownList.Add(item.Key, item.Value);
			}
			return dropdownList;
		}
	}
}
