using System;
using System.Collections.Generic;

namespace Linefy
{
	internal class HashedCollection<T>
	{
		private List<T> items = new List<T>();

		private Dictionary<T, int> dict = new Dictionary<T, int>();

		private Action<T> collisionCallback;

		public int Count => items.Count;

		public T this[int idx]
		{
			get
			{
				return items[idx];
			}
			set
			{
				items[idx] = value;
			}
		}

		public HashedCollection(Action<T> collisionCallback)
		{
			this.collisionCallback = collisionCallback;
		}

		public T FindOrAdd(T item)
		{
			int value = -1;
			if (dict.TryGetValue(item, out value))
			{
				T obj = items[value];
				if (collisionCallback != null)
				{
					collisionCallback(obj);
				}
				return items[value];
			}
			dict.Add(item, items.Count);
			items.Add(item);
			return item;
		}

		public int FindOrAddIdx(T item)
		{
			int value = -1;
			if (dict.TryGetValue(item, out value))
			{
				T obj = items[value];
				if (collisionCallback != null)
				{
					collisionCallback(obj);
				}
				return value;
			}
			dict.Add(item, items.Count);
			items.Add(item);
			return items.Count - 1;
		}

		public T[] ToArray()
		{
			return items.ToArray();
		}
	}
}
