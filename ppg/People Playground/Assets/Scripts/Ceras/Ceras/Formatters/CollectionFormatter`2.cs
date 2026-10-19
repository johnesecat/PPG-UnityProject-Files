using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Ceras.Helpers;

namespace Ceras.Formatters
{
	public class CollectionFormatter<TCollection, TItem> : IFormatter<TCollection>, IFormatter where TCollection : ICollection<TItem>
	{
		private readonly IFormatter<TItem> _itemFormatter;

		private readonly uint _maxSize;

		private readonly Func<int, TCollection> _capacityConstructor;

		public CollectionFormatter(CerasSerializer serializer)
		{
			Type typeFromHandle = typeof(TItem);
			_itemFormatter = (IFormatter<TItem>)serializer.GetReferenceFormatter(typeFromHandle);
			_maxSize = serializer.Config.Advanced.SizeLimits.MaxCollectionSize;
			Type typeFromHandle2 = typeof(TCollection);
			if (typeFromHandle2.IsGenericType)
			{
				ConstructorInfo constructorInfo = null;
				if (typeFromHandle2.GetGenericTypeDefinition() == typeof(List<>))
				{
					constructorInfo = typeFromHandle2.GetConstructor(new Type[1] { typeof(int) });
				}
				else if (typeFromHandle2.GetGenericTypeDefinition() == typeof(Dictionary<, >))
				{
					constructorInfo = typeFromHandle2.GetConstructor(new Type[1] { typeof(int) });
				}
				if (constructorInfo != null && serializer.Config.Advanced.AotMode == AotMode.None)
				{
					ParameterExpression parameterExpression = Expression.Parameter(typeof(int));
					_capacityConstructor = Expression.Lambda<Func<int, TCollection>>(Expression.New(constructorInfo, parameterExpression), new ParameterExpression[1] { parameterExpression }).Compile();
					CerasSerializer.AddFormatterConstructedType(typeFromHandle2);
				}
			}
		}

		public void Serialize(ref byte[] buffer, ref int offset, TCollection value)
		{
			if (value.IsReadOnly)
			{
				ThrowReadonly(value);
			}
			SerializerBinary.WriteUInt32(ref buffer, ref offset, (uint)value.Count);
			IFormatter<TItem> itemFormatter = _itemFormatter;
			IEnumerator<TItem> enumerator = value.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					itemFormatter.Serialize(ref buffer, ref offset, enumerator.Current);
				}
			}
			finally
			{
				enumerator.Dispose();
			}
		}

		public void Deserialize(byte[] buffer, ref int offset, ref TCollection value)
		{
			uint num = SerializerBinary.ReadUInt32(buffer, ref offset);
			if (num > _maxSize)
			{
				throw new InvalidOperationException($"The data contains a '{typeof(TCollection)}' with '{num}' entries, which exceeds the allowed limit of '{_maxSize}'");
			}
			if (value == null)
			{
				value = _capacityConstructor((int)num);
			}
			else if (value.Count > 0)
			{
				value.Clear();
			}
			if (value.IsReadOnly)
			{
				ThrowReadonly(value);
			}
			IFormatter<TItem> itemFormatter = _itemFormatter;
			for (int i = 0; i < num; i++)
			{
				TItem value2 = default(TItem);
				itemFormatter.Deserialize(buffer, ref offset, ref value2);
				value.Add(value2);
			}
		}

		private static void ThrowReadonly(object collection)
		{
			Type type = collection.GetType();
			string text = type.FriendlyName();
			if (type.FullName.Contains("System.Collections.Immutable"))
			{
				throw new InvalidOperationException("To serialize types from the 'System.Collections.Immutable' library, please install 'Ceras.ImmutableCollections' from NuGet. The affect type is '" + text + "'");
			}
			throw new InvalidOperationException("To serialize readonly collections you must configure a construction mode for the type '" + text + "'. (It's pretty easy, take a look at the tutorial or open an issue on GitHub)");
		}
	}
}
