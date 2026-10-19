using System;
using System.Collections.Generic;

namespace Ceras.Helpers
{
	internal class TypeCache
	{
		internal class TypeRefProxy
		{
			public Type Type;

			public override string ToString()
			{
				return $"TypeRefProxy: {Type}";
			}
		}

		private readonly Type[] _knownTypes;

		private readonly TypeDictionary<int> _serializationCache = new TypeDictionary<int>();

		private readonly List<TypeRefProxy> _deserializationCache = new List<TypeRefProxy>();

		private readonly StackSlim<TypeRefProxy> _typeRefProxyPool = new StackSlim<TypeRefProxy>(16);

		public TypeCache(Type[] knownTypes)
		{
			_knownTypes = knownTypes;
			foreach (Type type in knownTypes)
			{
				RegisterObject(type);
				CreateDeserializationProxy().Type = type;
			}
		}

		internal bool TryGetExistingObjectId(Type value, out int id)
		{
			return _serializationCache.TryGetValue(value, out id);
		}

		internal int RegisterObject(Type value)
		{
			int count = _serializationCache.Count;
			_serializationCache.GetOrAddValueRef(value) = count;
			return count;
		}

		internal TypeRefProxy CreateDeserializationProxy()
		{
			TypeRefProxy typeRefProxy = ((_typeRefProxyPool.Count != 0) ? _typeRefProxyPool.Pop() : new TypeRefProxy());
			_deserializationCache.Add(typeRefProxy);
			return typeRefProxy;
		}

		internal Type GetExistingObject(int id)
		{
			return _deserializationCache[id].Type;
		}

		internal void ResetSerializationCache()
		{
			if (_serializationCache.Count != _knownTypes.Length)
			{
				_serializationCache.Clear();
				for (int i = 0; i < _knownTypes.Length; i++)
				{
					Type value = _knownTypes[i];
					RegisterObject(value);
				}
			}
		}

		internal void ResetDeserializationCache()
		{
			for (int i = _knownTypes.Length; i < _deserializationCache.Count; i++)
			{
				TypeRefProxy item = _deserializationCache[i];
				_typeRefProxyPool.Push(item);
			}
			int count = _deserializationCache.Count - _knownTypes.Length;
			_deserializationCache.RemoveRange(_knownTypes.Length, count);
		}
	}
}
