using System;

namespace Ceras
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
	public sealed class MemberConfigAttribute : Attribute
	{
		public TargetMember TargetMembers { get; set; }

		public ReadonlyFieldHandling ReadonlyFieldHandling { get; set; }

		public MemberConfigAttribute(TargetMember targetMembers = TargetMember.PublicFields, ReadonlyFieldHandling readonlyFieldHandling = ReadonlyFieldHandling.ExcludeFromSerialization)
		{
			TargetMembers = targetMembers;
			ReadonlyFieldHandling = readonlyFieldHandling;
		}
	}
}
