using System.Reflection;

namespace Ceras
{
	public class MemberConfig<TDeclaring> : MemberConfig
	{
		public new TypeConfig<TDeclaring> TypeConfig => (TypeConfig<TDeclaring>)base.TypeConfig;

		public MemberConfig(TypeConfig typeConfig, MemberInfo member)
			: base(typeConfig, member)
		{
		}

		public TypeConfig<TDeclaring> SetReadonlyHandling(ReadonlyFieldHandling r)
		{
			_readonlyOverride = r;
			return TypeConfig;
		}

		public TypeConfig<TDeclaring> Include()
		{
			SetIncludeWithReason(SerializationOverride.ForceInclude, "User called Include()");
			return TypeConfig;
		}

		public TypeConfig<TDeclaring> Include(ReadonlyFieldHandling readonlyHandling)
		{
			base.ReadonlyFieldHandling = readonlyHandling;
			SetIncludeWithReason(SerializationOverride.ForceInclude, "User called Include()");
			return TypeConfig;
		}

		public TypeConfig<TDeclaring> Exclude()
		{
			return Exclude("User called Exclude()");
		}

		public TypeConfig<TDeclaring> Exclude(string customReason)
		{
			SetIncludeWithReason(SerializationOverride.ForceSkip, customReason);
			return TypeConfig;
		}
	}
}
