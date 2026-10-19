using System;

namespace Ceras.Formatters
{
	[AttributeUsage(AttributeTargets.Class)]
	public class CerasInjectAttribute : Attribute
	{
		internal static readonly CerasInjectAttribute Default = new CerasInjectAttribute();

		public bool IncludePrivate { get; set; } = true;

	}
}
