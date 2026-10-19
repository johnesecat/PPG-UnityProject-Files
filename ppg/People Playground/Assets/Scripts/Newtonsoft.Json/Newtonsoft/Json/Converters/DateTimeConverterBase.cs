using System;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Converters
{
	public abstract class DateTimeConverterBase : JsonConverter
	{
		[NullableContext(1)]
		public override bool CanConvert(Type objectType)
		{
			if (objectType == typeof(DateTime) || objectType == typeof(DateTime?))
			{
				return true;
			}
			if (objectType == typeof(DateTimeOffset) || objectType == typeof(DateTimeOffset?))
			{
				return true;
			}
			return false;
		}
	}
}
