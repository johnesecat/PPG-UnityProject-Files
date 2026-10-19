using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WatsonTcp
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum DisconnectReason
	{
		[EnumMember(Value = "Normal")]
		Normal,
		[EnumMember(Value = "Removed")]
		Removed,
		[EnumMember(Value = "Timeout")]
		Timeout,
		[EnumMember(Value = "Shutdown")]
		Shutdown,
		[EnumMember(Value = "AuthFailure")]
		AuthFailure
	}
}
