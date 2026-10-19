using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Newtonsoft.Json.Serialization
{
	[NullableContext(1)]
	[Nullable(0)]
	internal class SerializationBinderAdapter : ISerializationBinder
	{
		public readonly SerializationBinder SerializationBinder;

		public SerializationBinderAdapter(SerializationBinder serializationBinder)
		{
			SerializationBinder = serializationBinder;
		}

		public Type BindToType([Nullable(2)] string assemblyName, string typeName)
		{
			return SerializationBinder.BindToType(assemblyName, typeName);
		}

		[NullableContext(2)]
		public void BindToName([Nullable(1)] Type serializedType, out string assemblyName, out string typeName)
		{
			SerializationBinder.BindToName(serializedType, out assemblyName, out typeName);
		}
	}
}
