using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WatsonTcp
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum TlsVersion
	{
		Tls12,
		Tls13
	}
}
