using System;

namespace NaughtyAttributes
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class DisableIfAttribute : EnableIfAttributeBase
	{
		public DisableIfAttribute(string condition)
			: base(condition)
		{
			base.Inverted = true;
		}

		public DisableIfAttribute(EConditionOperator conditionOperator, params string[] conditions)
			: base(conditionOperator, conditions)
		{
			base.Inverted = true;
		}

		public DisableIfAttribute(string enumName, object enumValue)
			: base(enumName, enumValue as Enum)
		{
			base.Inverted = true;
		}
	}
}
