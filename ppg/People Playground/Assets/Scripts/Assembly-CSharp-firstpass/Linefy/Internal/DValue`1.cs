using System;

namespace Linefy.Internal
{
	public class DValue<T> where T : struct, IEquatable<T>
	{
		public string name;

		protected T _value;

		protected DFlag[] dFlags;

		public void ForceSetDirty()
		{
			DFlag[] array = dFlags;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].Set();
			}
		}

		public DValue(T initialValue, params DFlag[] dirtyFlags)
		{
			_value = initialValue;
			dFlags = dirtyFlags;
		}

		public void AssignDirtyFlags(DFlag[] dirtyFlags)
		{
			dFlags = dirtyFlags;
		}

		public void SetValue(T value)
		{
			if (!value.Equals(_value))
			{
				DFlag[] array = dFlags;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].Set();
				}
				_value = value;
			}
		}

		public static implicit operator T(DValue<T> v)
		{
			return v._value;
		}
	}
}
