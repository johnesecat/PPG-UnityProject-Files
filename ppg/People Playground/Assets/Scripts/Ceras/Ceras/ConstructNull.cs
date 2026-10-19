using System;

namespace Ceras
{
	internal class ConstructNull : TypeConstruction
	{
		public static ConstructNull Instance { get; } = new ConstructNull();


		internal override bool HasDataArguments => false;

		private ConstructNull()
		{
		}

		internal override Func<object> GetRefFormatterConstructor(bool allowDynamicCodeGen)
		{
			return () => null;
		}
	}
}
