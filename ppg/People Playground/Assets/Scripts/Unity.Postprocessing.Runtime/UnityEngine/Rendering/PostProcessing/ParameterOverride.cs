namespace UnityEngine.Rendering.PostProcessing
{
	public abstract class ParameterOverride
	{
		public bool overrideState;

		internal abstract void Interp(ParameterOverride from, ParameterOverride to, float t);

		public abstract int GetHash();

		public T GetValue<T>()
		{
			return ((ParameterOverride<T>)this).value;
		}

		protected internal virtual void OnEnable()
		{
		}

		protected internal virtual void OnDisable()
		{
		}

		internal abstract void SetValue(ParameterOverride parameter);
	}
}
