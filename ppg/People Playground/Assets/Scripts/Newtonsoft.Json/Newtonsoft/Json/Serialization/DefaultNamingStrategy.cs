using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Serialization
{
	public class DefaultNamingStrategy : NamingStrategy
	{
		[NullableContext(1)]
		protected override string ResolvePropertyName(string name)
		{
			return name;
		}
	}
}
