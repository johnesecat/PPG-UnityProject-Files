using System;

namespace UnityEngine.Rendering
{
	public abstract class VolumeParameter
	{
		public const string k_DebuggerDisplay = "{m_Value} ({m_OverrideState})";

		[SerializeField]
		protected bool m_OverrideState;

		public virtual bool overrideState
		{
			get
			{
				return m_OverrideState;
			}
			set
			{
				m_OverrideState = value;
			}
		}

		internal abstract void Interp(VolumeParameter from, VolumeParameter to, float t);

		public T GetValue<T>()
		{
			return ((VolumeParameter<T>)this).value;
		}

		public abstract void SetValue(VolumeParameter parameter);

		protected internal virtual void OnEnable()
		{
		}

		protected internal virtual void OnDisable()
		{
		}

		public static bool IsObjectParameter(Type type)
		{
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ObjectParameter<>))
			{
				return true;
			}
			if (type.BaseType != null)
			{
				return IsObjectParameter(type.BaseType);
			}
			return false;
		}

		public virtual void Release()
		{
		}
	}
}
