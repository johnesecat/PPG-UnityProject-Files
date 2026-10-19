using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Ceras.Helpers
{
	[DebuggerTypeProxy(typeof(TypeDictionaryDebugView<>))]
	[DebuggerDisplay("Count = {Count}")]
	internal class TypeDictionary<TValue> : IReadOnlyCollection<KeyValuePair<Type, TValue>>, IEnumerable<KeyValuePair<Type, TValue>>, IEnumerable
	{
		[DebuggerDisplay("({key}, {value})->{next}")]
		private struct Entry
		{
			public Type key;

			public TValue value;

			public int next;
		}

		public struct Enumerator : IEnumerator<KeyValuePair<Type, TValue>>, IDisposable, IEnumerator
		{
			private readonly TypeDictionary<TValue> _dictionary;

			private int _index;

			private int _count;

			private KeyValuePair<Type, TValue> _current;

			public KeyValuePair<Type, TValue> Current => _current;

			object IEnumerator.Current => _current;

			internal Enumerator(TypeDictionary<TValue> dictionary)
			{
				_dictionary = dictionary;
				_index = 0;
				_count = _dictionary._count;
				_current = default(KeyValuePair<Type, TValue>);
			}

			public bool MoveNext()
			{
				if (_count == 0)
				{
					_current = default(KeyValuePair<Type, TValue>);
					return false;
				}
				_count--;
				while (_dictionary._entries[_index].next < -1)
				{
					_index++;
				}
				_current = new KeyValuePair<Type, TValue>(_dictionary._entries[_index].key, _dictionary._entries[_index++].value);
				return true;
			}

			void IEnumerator.Reset()
			{
				_index = 0;
				_count = _dictionary._count;
				_current = default(KeyValuePair<Type, TValue>);
			}

			public void Dispose()
			{
			}
		}

		private static readonly Entry[] InitialEntries = new Entry[1];

		private int _count;

		private int _freeList = -1;

		private int[] _buckets;

		private Entry[] _entries;

		public int Count => _count;

		public TypeDictionary()
		{
			_buckets = HashHelpers.SizeOneIntArray;
			_entries = InitialEntries;
		}

		public TypeDictionary(int capacity)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException("capacity");
			}
			if (capacity < 2)
			{
				capacity = 2;
			}
			capacity = HashHelpers.PowerOf2(capacity);
			_buckets = new int[capacity];
			_entries = new Entry[capacity];
		}

		public void Clear()
		{
			_count = 0;
			_freeList = -1;
			_buckets = HashHelpers.SizeOneIntArray;
			_entries = InitialEntries;
		}

		public bool ContainsKey(Type key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			Entry[] entries = _entries;
			int num = 0;
			int num2 = _buckets[RuntimeHelpers.GetHashCode(key) & (_buckets.Length - 1)] - 1;
			while ((uint)num2 < (uint)entries.Length)
			{
				if ((object)key == entries[num2].key)
				{
					return true;
				}
				if (num == entries.Length)
				{
					throw new InvalidOperationException("concurrent operations not supported");
				}
				num++;
				num2 = entries[num2].next;
			}
			return false;
		}

		public bool TryGetValue(Type key, out TValue value)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			Entry[] entries = _entries;
			int num = 0;
			int num2 = _buckets[RuntimeHelpers.GetHashCode(key) & (_buckets.Length - 1)] - 1;
			while ((uint)num2 < (uint)entries.Length)
			{
				if ((object)key == entries[num2].key)
				{
					value = entries[num2].value;
					return true;
				}
				if (num == entries.Length)
				{
					throw new InvalidOperationException("concurrent operations not supported");
				}
				num++;
				num2 = entries[num2].next;
			}
			value = default(TValue);
			return false;
		}

		public bool Remove(Type key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			Entry[] entries = _entries;
			int num = RuntimeHelpers.GetHashCode(key) & (_buckets.Length - 1);
			int num2 = _buckets[num] - 1;
			int num3 = -1;
			int num4 = 0;
			while (num2 != -1)
			{
				Entry entry = entries[num2];
				if ((object)entry.key == key)
				{
					if (num3 != -1)
					{
						entries[num3].next = entry.next;
					}
					else
					{
						_buckets[num] = entry.next + 1;
					}
					entries[num2] = default(Entry);
					entries[num2].next = -3 - _freeList;
					_freeList = num2;
					_count--;
					return true;
				}
				num3 = num2;
				num2 = entry.next;
				if (num4 == entries.Length)
				{
					throw new InvalidOperationException("concurrent operations not supported");
				}
				num4++;
			}
			return false;
		}

		public ref TValue GetOrAddValueRef(Type key)
		{
			if (key == null)
			{
				throw new ArgumentNullException("key");
			}
			Entry[] entries = _entries;
			int num = 0;
			int num2 = RuntimeHelpers.GetHashCode(key) & (_buckets.Length - 1);
			int num3 = _buckets[num2] - 1;
			while ((uint)num3 < (uint)entries.Length)
			{
				if ((object)key == entries[num3].key)
				{
					return ref entries[num3].value;
				}
				if (num == entries.Length)
				{
					throw new InvalidOperationException("concurrent operations not supported");
				}
				num++;
				num3 = entries[num3].next;
			}
			return ref AddKey(key, num2);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private ref TValue AddKey(Type key, int bucketIndex)
		{
			Entry[] array = _entries;
			int num;
			if (_freeList != -1)
			{
				num = _freeList;
				_freeList = -3 - array[_freeList].next;
			}
			else
			{
				if (_count == array.Length || array.Length == 1)
				{
					array = Resize();
					bucketIndex = RuntimeHelpers.GetHashCode(key) & (_buckets.Length - 1);
				}
				num = _count;
			}
			array[num].key = key;
			array[num].next = _buckets[bucketIndex] - 1;
			_buckets[bucketIndex] = num + 1;
			_count++;
			return ref array[num].value;
		}

		private Entry[] Resize()
		{
			int count = _count;
			int num = _entries.Length * 2;
			if ((uint)num > 2147483647u)
			{
				throw new InvalidOperationException("capacity overflow");
			}
			Entry[] array = new Entry[num];
			Array.Copy(_entries, 0, array, 0, count);
			int[] array2 = new int[array.Length];
			while (count-- > 0)
			{
				int num2 = RuntimeHelpers.GetHashCode(array[count].key) & (array2.Length - 1);
				array[count].next = array2[num2] - 1;
				array2[num2] = count + 1;
			}
			_buckets = array2;
			_entries = array;
			return array;
		}

		public Enumerator GetEnumerator()
		{
			return new Enumerator(this);
		}

		IEnumerator<KeyValuePair<Type, TValue>> IEnumerable<KeyValuePair<Type, TValue>>.GetEnumerator()
		{
			return new Enumerator(this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new Enumerator(this);
		}
	}
}
