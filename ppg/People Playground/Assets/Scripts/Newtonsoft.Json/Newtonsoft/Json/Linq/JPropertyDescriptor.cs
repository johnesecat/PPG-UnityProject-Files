using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Linq
{
	[NullableContext(1)]
	[Nullable(0)]
	public class JPropertyDescriptor : PropertyDescriptor
	{
		public override Type ComponentType => typeof(JObject);

		public override bool IsReadOnly => false;

		public override Type PropertyType => typeof(object);

		protected override int NameHashCode => base.NameHashCode;

		public JPropertyDescriptor(string name)
			: base(name, null)
		{
		}

		private static JObject CastInstance(object instance)
		{
			return (JObject)instance;
		}

		public override bool CanResetValue(object component)
		{
			return false;
		}

		[return: Nullable(2)]
		public override object GetValue(object component)
		{
			return (component as JObject)?[Name];
		}

		public override void ResetValue(object component)
		{
		}

		public override void SetValue(object component, object value)
		{
			JObject jObject = component as JObject;
			if (jObject != null)
			{
				JToken value2 = (value as JToken) ?? new JValue(value);
				jObject[Name] = value2;
			}
		}

		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}
	}
}
