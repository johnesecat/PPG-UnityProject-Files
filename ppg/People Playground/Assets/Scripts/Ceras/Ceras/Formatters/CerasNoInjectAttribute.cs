using System;

namespace Ceras.Formatters
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field)]
	public class CerasNoInjectAttribute : Attribute
	{
	}
}
