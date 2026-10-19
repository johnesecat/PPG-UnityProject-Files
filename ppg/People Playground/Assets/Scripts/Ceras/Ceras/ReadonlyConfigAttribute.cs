using System;

namespace Ceras
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public sealed class ReadonlyConfigAttribute : Attribute
	{
		public ReadonlyFieldHandling ReadonlyFieldHandling { get; set; }

		public ReadonlyConfigAttribute(ReadonlyFieldHandling readonlyFieldHandling = ReadonlyFieldHandling.ExcludeFromSerialization)
		{
			ReadonlyFieldHandling = readonlyFieldHandling;
		}
	}
}
