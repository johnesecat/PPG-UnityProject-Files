using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WatsonTcp
{
	[JsonConverter(typeof(StringEnumConverter))]
	internal enum Mode
	{
		[EnumMember(Value = "Tcp")]
		Tcp,
		[EnumMember(Value = "Ssl")]
		Ssl
	}
}
