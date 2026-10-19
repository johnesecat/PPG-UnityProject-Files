using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WatsonTcp
{
	internal static class SerializationHelper
	{
		private static readonly JsonSerializerSettings HardenedSerializerSettings;

		private static readonly JsonSerializerSettings SerializerDefaults;

		internal static void InstantiateConverter()
		{
			try
			{
				Activator.CreateInstance<StringEnumConverter>();
			}
			catch (global::System.Exception)
			{
			}
		}

		internal static T DeserializeJson<T>(string json)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			if (string.IsNullOrEmpty(json))
			{
				throw new ArgumentNullException("json");
			}
			return JsonConvert.DeserializeObject<T>(json, HardenedSerializerSettings);
		}

		internal static string SerializeJson(object obj, bool pretty)
		{
			if (obj == null)
			{
				return null;
			}
			if (pretty)
			{
				return JsonConvert.SerializeObject(obj, (Formatting)1, SerializerDefaults);
			}
			return JsonConvert.SerializeObject(obj, SerializerDefaults);
		}

		static SerializationHelper()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Expected O, but got Unknown
			JsonSerializerSettings val = new JsonSerializerSettings();
			val.set_TypeNameHandling((TypeNameHandling)0);
			HardenedSerializerSettings = val;
			JsonSerializerSettings val2 = new JsonSerializerSettings();
			val2.set_NullValueHandling((NullValueHandling)1);
			val2.set_DateTimeZoneHandling((DateTimeZoneHandling)0);
			SerializerDefaults = val2;
		}
	}
}
