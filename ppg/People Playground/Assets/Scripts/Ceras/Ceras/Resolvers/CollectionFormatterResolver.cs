using System;
using System.Collections.Generic;
using Ceras.Formatters;
using Ceras.Helpers;

namespace Ceras.Resolvers
{
	public class CollectionFormatterResolver : IFormatterResolver
	{
		private readonly CerasSerializer _ceras;

		private Dictionary<Type, IFormatter> _formatterInstances = new Dictionary<Type, IFormatter>();

		public CollectionFormatterResolver(CerasSerializer ceras)
		{
			_ceras = ceras;
		}

		public IFormatter GetFormatter(Type type)
		{
			if (_formatterInstances.TryGetValue(type, out var value))
			{
				return value;
			}
			if (type.IsArray)
			{
				Type elementType = type.GetElementType();
				int arrayRank = type.GetArrayRank();
				uint num = ((elementType == typeof(byte)) ? _ceras.Config.Advanced.SizeLimits.MaxByteArraySize : _ceras.Config.Advanced.SizeLimits.MaxArraySize);
				if (arrayRank == 1 && _ceras.Config.Advanced.UseReinterpretFormatter && ReflectionHelper.IsBlittableType(elementType))
				{
					value = (IFormatter)Activator.CreateInstance(typeof(ReinterpretArrayFormatter<>).MakeGenericType(elementType), num);
				}
				else if (arrayRank == 1)
				{
					value = (IFormatter)Activator.CreateInstance(typeof(ArrayFormatter<>).MakeGenericType(elementType), _ceras, num);
				}
				else if (arrayRank <= 6)
				{
					value = (IFormatter)Activator.CreateInstance(typeof(MultiDimensionalArrayFormatter<>).MakeGenericType(elementType), _ceras, num);
				}
				else
				{
					ArrayRankTooHigh(arrayRank);
				}
				_formatterInstances[type] = value;
				return value;
			}
			Type type2 = ReflectionHelper.FindClosedType(type, typeof(Stack<>));
			if (type2 != null)
			{
				value = (IFormatter)Activator.CreateInstance(typeof(StackFormatter<>).MakeGenericType(type2.GetGenericArguments()));
				_formatterInstances[type] = value;
				return value;
			}
			Type type3 = ReflectionHelper.FindClosedType(type, typeof(Queue<>));
			if (type3 != null)
			{
				value = (IFormatter)Activator.CreateInstance(typeof(QueueFormatter<>).MakeGenericType(type3.GetGenericArguments()));
				_formatterInstances[type] = value;
				return value;
			}
			Type type4 = ReflectionHelper.FindClosedType(type, typeof(ICollection<>));
			if (type4 != null)
			{
				Type type5 = type4.GetGenericArguments()[0];
				value = (IFormatter)Activator.CreateInstance(typeof(CollectionFormatter<, >).MakeGenericType(type, type5), _ceras);
				_formatterInstances[type] = value;
				return value;
			}
			return null;
		}

		private static void ArrayRankTooHigh(int rank)
		{
			throw new InvalidOperationException("Multi-dimensional array of rank " + rank + " is not yet supported, please open an issue on github");
		}
	}
}
