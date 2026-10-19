using System;

namespace NaughtyAttributes
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class DropdownAttribute : DrawerAttribute
	{
		public string ValuesName { get; private set; }

		public DropdownAttribute(string valuesName)
		{
			ValuesName = valuesName;
		}
	}
}
