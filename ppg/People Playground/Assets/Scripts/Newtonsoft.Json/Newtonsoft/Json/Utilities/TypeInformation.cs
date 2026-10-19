using System;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Utilities
{
	[NullableContext(1)]
	[Nullable(0)]
	internal class TypeInformation
	{
		public Type Type { get; }

		public PrimitiveTypeCode TypeCode { get; }

		public TypeInformation(Type type, PrimitiveTypeCode typeCode)
		{
			Type = type;
			TypeCode = typeCode;
		}
	}
}
