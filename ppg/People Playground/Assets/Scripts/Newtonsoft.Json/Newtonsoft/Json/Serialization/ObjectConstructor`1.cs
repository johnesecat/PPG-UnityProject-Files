using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Serialization
{
	public delegate object ObjectConstructor<[Nullable(2)] T>([Nullable(new byte[] { 1, 2 })] params object[] args);
}
