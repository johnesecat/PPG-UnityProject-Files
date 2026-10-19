using System;

namespace Ceras
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public sealed class IncludeAttribute : Attribute
	{
	}
}
