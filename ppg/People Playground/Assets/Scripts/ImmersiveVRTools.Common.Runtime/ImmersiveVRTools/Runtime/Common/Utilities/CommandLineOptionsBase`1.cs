namespace ImmersiveVRTools.Runtime.Common.Utilities
{
	public abstract class CommandLineOptionsBase<T> : ICommandLineOptions where T : ICommandLineOptions, new()
	{
		private static T _instance;

		public static T Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = (T)new T().GenerateStaticInstance();
				}
				return _instance;
			}
			private set
			{
				_instance = value;
			}
		}

		protected abstract T GenerateStaticInstanceTyped();

		public object GenerateStaticInstance()
		{
			return GenerateStaticInstanceTyped();
		}
	}
}
