using System;

namespace NaughtyAttributes
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public class EnableIfAttribute : EnableIfAttributeBase
	{
		public EnableIfAttribute(string condition)
			: base(condition)
		{
			base.Inverted = false;
		}

		public EnableIfAttribute(EConditionOperator conditionOperator, params string[] conditions)
			: base(conditionOperator, conditions)
		{
			base.Inverted = false;
		}

		public EnableIfAttribute(string enumName, object enumValue)
			: base(enumName, enumValue as Enum)
		{
			base.Inverted = false;
		}
	}
}
