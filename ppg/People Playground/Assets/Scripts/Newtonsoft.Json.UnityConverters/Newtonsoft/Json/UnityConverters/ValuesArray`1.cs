using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Unity.Collections;

namespace Newtonsoft.Json.UnityConverters
{
	public struct ValuesArray<TInner> : IReadOnlyList<TInner>, IEnumerable<TInner>, IEnumerable, IReadOnlyCollection<TInner>, IDisposable where TInner : struct
	{
		private NativeArray<TInner> _array;

		public int Length => _array.Length;

		int IReadOnlyCollection<TInner>.Count => _array.Length;

		public TInner this[int index]
		{
			[return: System.Diagnostics.CodeAnalysis.MaybeNull]
			get
			{
				return _array[index];
			}
			set
			{
				_array[index] = value;
			}
		}

		public ValuesArray(TInner[] array, Allocator allocator)
		{
			_array = new NativeArray<TInner>(array, allocator);
		}

		public ValuesArray(int capacity, Allocator allocator, NativeArrayOptions options = NativeArrayOptions.ClearMemory)
		{
			_array = new NativeArray<TInner>(capacity, allocator, options);
		}

		[return: System.Diagnostics.CodeAnalysis.MaybeNull]
		public T GetAsTypeOrDefault<T>(int index) where T : struct
		{
			return (_array[index] as T?).GetValueOrDefault();
		}

		[return: NotNullIfNotNull("fallback")]
		public T GetAsTypeOrDefault<T>(int index, [System.Diagnostics.CodeAnalysis.AllowNull] T fallback) where T : struct
		{
			return (_array[index] as T?) ?? fallback;
		}

		public IEnumerator<TInner> GetEnumerator()
		{
			return _array.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Dispose()
		{
			_array.Dispose();
		}
	}
}
