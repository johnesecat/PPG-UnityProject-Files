using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Ceras.Helpers
{
	internal class ObjectCache
	{
		internal abstract class RefProxy
		{
			public abstract object ObjectValue { get; }

			public abstract void ResetAndReturn();
		}

		internal sealed class RefProxy<T> : RefProxy where T : class
		{
			public T Value;

			public override object ObjectValue => Value;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public override void ResetAndReturn()
			{
				Value = null;
				RefProxyPool<T>.Return(this);
			}

			public override string ToString()
			{
				return $"RefProxy({typeof(T).FriendlyName()}): {Value}";
			}
		}

		internal static class RefProxyPool<T> where T : class
		{
			private static readonly FactoryPool<RefProxy<T>> _proxyPool;

			static RefProxyPool()
			{
				_proxyPool = new FactoryPool<RefProxy<T>>(CreateProxy, 8);
				RefProxyPoolRegister.RegisterPool(_proxyPool);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal static RefProxy<T> Rent()
			{
				return _proxyPool.RentObject();
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			internal static void Return(RefProxy<T> refProxy)
			{
				_proxyPool.ReturnObject(refProxy);
			}

			internal static int GetPoolCapacity()
			{
				return _proxyPool.Capacity;
			}

			private static RefProxy<T> CreateProxy()
			{
				return new RefProxy<T>();
			}
		}

		internal static class RefProxyPoolRegister
		{
			private static List<IFactoryPool> _genericPools = new List<IFactoryPool>();

			public static void RegisterPool(IFactoryPool pool)
			{
				lock (_genericPools)
				{
					_genericPools.Add(pool);
				}
			}

			public static void TrimAll()
			{
				lock (_genericPools)
				{
					foreach (IFactoryPool genericPool in _genericPools)
					{
						genericPool.TrimPool();
					}
				}
			}
		}

		private readonly Dictionary<object, int> _serializationCache = new Dictionary<object, int>(64);

		private readonly List<RefProxy> _deserializationCache = new List<RefProxy>(64);

		internal bool TryGetExistingObjectId<T>(T value, out int id) where T : class
		{
			return _serializationCache.TryGetValue(value, out id);
		}

		internal int RegisterObject<T>(T value) where T : class
		{
			int count = _serializationCache.Count;
			_serializationCache.Add(value, count);
			return count;
		}

		internal RefProxy<T> CreateDeserializationProxy<T>() where T : class
		{
			RefProxy<T> refProxy = RefProxyPool<T>.Rent();
			_deserializationCache.Add(refProxy);
			return refProxy;
		}

		internal T GetExistingObject<T>(int id) where T : class
		{
			return (T)_deserializationCache[id].ObjectValue;
		}

		internal void ClearSerializationCache()
		{
			_serializationCache.Clear();
		}

		internal void ClearDeserializationCache()
		{
			for (int i = 0; i < _deserializationCache.Count; i++)
			{
				_deserializationCache[i].ResetAndReturn();
			}
			_deserializationCache.Clear();
		}
	}
}
