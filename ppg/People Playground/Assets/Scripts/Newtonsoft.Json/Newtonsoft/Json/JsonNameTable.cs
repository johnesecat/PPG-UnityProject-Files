using System.Runtime.CompilerServices;

namespace Newtonsoft.Json
{
	public abstract class JsonNameTable
	{
		[NullableContext(1)]
		[return: Nullable(2)]
		public abstract string Get(char[] key, int start, int length);
	}
}
