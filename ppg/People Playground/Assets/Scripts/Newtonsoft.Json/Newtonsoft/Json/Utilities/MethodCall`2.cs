using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Utilities
{
	internal delegate TResult MethodCall<[Nullable(2)] T, [Nullable(2)] TResult>(T target, [Nullable(new byte[] { 1, 2 })] params object[] args);
}
