using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WatsonTcp
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum MessageStatus
	{
		[EnumMember(Value = "Normal")]
		Normal,
		[EnumMember(Value = "Success")]
		Success,
		[EnumMember(Value = "Failure")]
		Failure,
		[EnumMember(Value = "AuthRequired")]
		AuthRequired,
		[EnumMember(Value = "AuthRequested")]
		AuthRequested,
		[EnumMember(Value = "AuthSuccess")]
		AuthSuccess,
		[EnumMember(Value = "AuthFailure")]
		AuthFailure,
		[EnumMember(Value = "Removed")]
		Removed,
		[EnumMember(Value = "Shutdown")]
		Shutdown,
		[EnumMember(Value = "Heartbeat")]
		Heartbeat,
		[EnumMember(Value = "Timeout")]
		Timeout
	}
}
