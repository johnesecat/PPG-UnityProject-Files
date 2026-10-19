using System;
using System.Collections;
using System.Collections.Generic;

namespace ImmersiveVRTools.Runtime.Common.DataStructure
{
	public class RingBufferList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
	{
		private readonly List<T> _list;

		private int _currentStartOfList;

		private readonly int _maxSize;

		public T this[int index]
		{
			get
			{
				if (index < 0 || index >= _list.Count)
				{
					throw new IndexOutOfRangeException();
				}
				return _list[(_currentStartOfList + index) % _maxSize];
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public int Count => _list.Count;

		public bool IsReadOnly => false;

		public RingBufferList(int maxSize)
		{
			_currentStartOfList = 0;
			_maxSize = maxSize;
			_list = new List<T>();
		}

		public void Add(T item)
		{
			if (_list.Count < _maxSize)
			{
				_list.Add(item);
				return;
			}
			_list[_currentStartOfList] = item;
			_currentStartOfList = (_currentStartOfList + 1) % _maxSize;
		}

		public void Clear()
		{
			_currentStartOfList = 0;
			_list.Clear();
		}

		public bool Contains(T item)
		{
			return _list.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			for (int i = 0; i < _list.Count; i++)
			{
				int index = (_currentStartOfList + i) % _maxSize;
				array[arrayIndex + i] = _list[index];
			}
		}

		public int IndexOf(T item)
		{
			int num = _list.IndexOf(item);
			if (num == -1)
			{
				return -1;
			}
			if (num >= _currentStartOfList)
			{
				return num - _currentStartOfList;
			}
			return _list.Count - _currentStartOfList + num;
		}

		public IEnumerator<T> GetEnumerator()
		{
			for (int i = 0; i < _list.Count; i++)
			{
				int index = (_currentStartOfList + i) % _maxSize;
				yield return _list[index];
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Insert(int index, T item)
		{
			throw new NotSupportedException();
		}

		public bool Remove(T item)
		{
			throw new NotSupportedException();
		}

		public void RemoveAt(int index)
		{
			throw new NotSupportedException();
		}
	}
}
