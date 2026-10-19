using System;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Serialization
{
	[NullableContext(1)]
	public interface IContractResolver
	{
		JsonContract ResolveContract(Type type);
	}
}
