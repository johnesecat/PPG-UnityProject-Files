using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Utilities
{
	[NullableContext(2)]
	[Nullable(0)]
	internal class FSharpFunction
	{
		private readonly object _instance;

		[Nullable(new byte[] { 1, 2, 1 })]
		private readonly MethodCall<object, object> _invoker;

		public FSharpFunction(object instance, [Nullable(new byte[] { 1, 2, 1 })] MethodCall<object, object> invoker)
		{
			_instance = instance;
			_invoker = invoker;
		}

		[NullableContext(1)]
		public object Invoke(params object[] args)
		{
			return _invoker(_instance, args);
		}
	}
}
