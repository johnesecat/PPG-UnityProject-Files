using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct ModResourceCache
{
	private struct Key
	{
		public string Filename;

		public Type Type;

		public override bool Equals(object obj)
		{
			if (obj is Key)
			{
				Key key = (Key)obj;
				if (Filename == key.Filename)
				{
					return EqualityComparer<Type>.Default.Equals(Type, key.Type);
				}
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (705263445 * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Filename)) * -1521134295 + EqualityComparer<Type>.Default.GetHashCode(Type);
		}

		public static bool operator ==(Key left, Key right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(Key left, Key right)
		{
			return !(left == right);
		}
	}

	private static Dictionary<Key, UnityEngine.Object> cache = new Dictionary<Key, UnityEngine.Object>();

	public static bool TryGet<T>(string key, out T obj) where T : UnityEngine.Object
	{
		obj = null;
		if (cache.TryGetValue(new Key
		{
			Filename = key,
			Type = typeof(T)
		}, out var value))
		{
			obj = (T)value;
			return true;
		}
		return false;
	}

	public static void Cache(string key, UnityEngine.Object obj)
	{
		cache.Add(new Key
		{
			Filename = key,
			Type = obj.GetType()
		}, obj);
	}

	public static void Remove(string key, Type type)
	{
		cache.Remove(new Key
		{
			Filename = key,
			Type = type
		});
	}

	public static void Clean()
	{
		cache.Clear();
	}
}
