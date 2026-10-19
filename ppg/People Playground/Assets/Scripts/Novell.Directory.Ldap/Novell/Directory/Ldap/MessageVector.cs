using System;
using System.Collections;

namespace Novell.Directory.Ldap
{
	internal class MessageVector : IList, ICollection, IEnumerable
	{
		private readonly ArrayList _innerList;

		internal object[] ObjectArray
		{
			get
			{
				lock (SyncRoot)
				{
					object[] result = ToArray();
					Clear();
					return result;
				}
			}
		}

		public bool IsFixedSize => _innerList.IsFixedSize;

		public bool IsReadOnly => _innerList.IsReadOnly;

		public object this[int index]
		{
			get
			{
				return _innerList[index];
			}
			set
			{
				_innerList[index] = value;
			}
		}

		public int Count => _innerList.Count;

		public bool IsSynchronized => _innerList.IsSynchronized;

		public object SyncRoot => _innerList.SyncRoot;

		internal MessageVector(int cap, int incr)
		{
			_innerList = ArrayList.Synchronized(new ArrayList(cap));
		}

		internal Message findMessageById(int msgId)
		{
			lock (SyncRoot)
			{
				Message message = null;
				for (int i = 0; i < Count; i++)
				{
					if ((message = (Message)this[i]) == null)
					{
						throw new FieldAccessException();
					}
					if (message.MessageID == msgId)
					{
						return message;
					}
				}
				throw new FieldAccessException();
			}
		}

		public object[] ToArray()
		{
			return _innerList.ToArray();
		}

		public int Add(object value)
		{
			return _innerList.Add(value);
		}

		public void Clear()
		{
			_innerList.Clear();
		}

		public bool Contains(object value)
		{
			return _innerList.Contains(value);
		}

		public int IndexOf(object value)
		{
			return _innerList.IndexOf(value);
		}

		public void Insert(int index, object value)
		{
			_innerList.Insert(index, value);
		}

		public void Remove(object value)
		{
			_innerList.Remove(value);
		}

		public void RemoveAt(int index)
		{
			_innerList.RemoveAt(index);
		}

		public void CopyTo(Array array, int index)
		{
			_innerList.CopyTo(array, index);
		}

		public IEnumerator GetEnumerator()
		{
			return _innerList.GetEnumerator();
		}
	}
}
