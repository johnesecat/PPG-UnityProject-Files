using System;

namespace Ceras.Helpers
{
	internal class StackSlim<T>
	{
		private struct Entry
		{
			public T Item;

			public static implicit operator T(Entry e)
			{
				return e.Item;
			}

			public static implicit operator Entry(T t)
			{
				Entry result = default(Entry);
				result.Item = t;
				return result;
			}
		}

		private Entry[] _array;

		public int Count { get; private set; }

		public StackSlim(int capacity = 4)
		{
			if (capacity < 4)
			{
				capacity = 4;
			}
			capacity = HashHelpers.PowerOf2(capacity);
			_array = new Entry[capacity];
		}

		public void Push(T item)
		{
			if (Count == _array.Length)
			{
				Entry[] array = new Entry[_array.Length * 2];
				Array.Copy(_array, array, _array.Length);
				_array = array;
			}
			_array[Count] = item;
			Count++;
		}

		public T Pop()
		{
			Entry[] array = _array;
			int num = --Count;
			T item = array[num].Item;
			array[num] = default(Entry);
			return item;
		}

		public void Clear()
		{
			Array.Clear(_array, 0, Count);
			Count = 0;
		}
	}
}
