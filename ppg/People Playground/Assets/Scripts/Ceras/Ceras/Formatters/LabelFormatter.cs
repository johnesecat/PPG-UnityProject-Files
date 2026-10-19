using System.Linq.Expressions;

namespace Ceras.Formatters
{
	internal class LabelFormatter : IFormatter<LabelExpression>, IFormatter
	{
		private IFormatter<LabelTarget> _labelTargetFormatter;

		private IFormatter<Expression> _expressionFormatter;

		public LabelFormatter()
		{
			CerasSerializer.AddFormatterConstructedType(typeof(LabelExpression));
		}

		public void Serialize(ref byte[] buffer, ref int offset, LabelExpression label)
		{
			_labelTargetFormatter.Serialize(ref buffer, ref offset, label.Target);
			_expressionFormatter.Serialize(ref buffer, ref offset, label.DefaultValue);
		}

		public void Deserialize(byte[] buffer, ref int offset, ref LabelExpression label)
		{
			LabelTarget value = null;
			_labelTargetFormatter.Deserialize(buffer, ref offset, ref value);
			Expression value2 = null;
			_expressionFormatter.Deserialize(buffer, ref offset, ref value2);
			label = Expression.Label(value, value2);
		}
	}
}
