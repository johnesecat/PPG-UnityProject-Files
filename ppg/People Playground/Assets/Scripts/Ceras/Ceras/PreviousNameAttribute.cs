using System;

namespace Ceras
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class PreviousNameAttribute : Attribute
	{
		public readonly string[] AlternativeNames = new string[0];

		public readonly string Name;

		public PreviousNameAttribute()
		{
		}

		public PreviousNameAttribute(string name)
		{
			Name = name;
		}

		public PreviousNameAttribute(string name, params string[] alternativeNames)
		{
			Name = name;
			AlternativeNames = alternativeNames;
		}
	}
}
