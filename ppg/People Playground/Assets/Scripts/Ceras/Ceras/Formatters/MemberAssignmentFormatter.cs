using System.Linq.Expressions;
using System.Reflection;

namespace Ceras.Formatters
{
	internal class MemberAssignmentFormatter : IFormatter<MemberAssignment>, IFormatter
	{
		private IFormatter<MemberInfo> _memberInfoFormatter;

		private IFormatter<Expression> _expressionFormatter;

		public MemberAssignmentFormatter()
		{
			CerasSerializer.AddFormatterConstructedType(typeof(MemberAssignment));
		}

		public void Serialize(ref byte[] buffer, ref int offset, MemberAssignment binding)
		{
			_memberInfoFormatter.Serialize(ref buffer, ref offset, binding.Member);
			_expressionFormatter.Serialize(ref buffer, ref offset, binding.Expression);
		}

		public void Deserialize(byte[] buffer, ref int offset, ref MemberAssignment binding)
		{
			MemberInfo value = null;
			_memberInfoFormatter.Deserialize(buffer, ref offset, ref value);
			Expression value2 = null;
			_expressionFormatter.Deserialize(buffer, ref offset, ref value2);
			binding = Expression.Bind(value, value2);
		}
	}
}
