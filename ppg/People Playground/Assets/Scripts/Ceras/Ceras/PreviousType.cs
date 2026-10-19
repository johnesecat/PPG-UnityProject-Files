using System;

namespace Ceras
{
	internal class PreviousType : PreviousNameAttribute
	{
		public Type MemberType { get; }

		public PreviousType(Type memberType)
			: base(null)
		{
			MemberType = memberType;
		}

		public PreviousType(string previousName, Type memberType)
			: base(previousName)
		{
			MemberType = memberType;
		}
	}
}
