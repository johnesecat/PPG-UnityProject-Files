using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Serialization
{
	[NullableContext(1)]
	public interface IValueProvider
	{
		void SetValue(object target, [Nullable(2)] object value);

		[return: Nullable(2)]
		object GetValue(object target);
	}
}
