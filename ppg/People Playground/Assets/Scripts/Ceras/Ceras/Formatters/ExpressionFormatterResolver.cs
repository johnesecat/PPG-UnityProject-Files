using System;
using System.Linq.Expressions;
using Ceras.Resolvers;

namespace Ceras.Formatters
{
	public class ExpressionFormatterResolver : IFormatterResolver
	{
		private readonly LabelTargetFormatter _labelTargetFormatter;

		private readonly LabelFormatter _labelFormatter;

		private readonly MemberAssignmentFormatter _memberAssignmentFormatter;

		private readonly MemberListBindingFormatter _memberListBindingFormatter;

		private readonly MemberMemberBindingFormatter _memberMemberBindingFormatter;

		public ExpressionFormatterResolver()
		{
			_labelTargetFormatter = new LabelTargetFormatter();
			_labelFormatter = new LabelFormatter();
			_memberAssignmentFormatter = new MemberAssignmentFormatter();
			_memberListBindingFormatter = new MemberListBindingFormatter();
			_memberMemberBindingFormatter = new MemberMemberBindingFormatter();
		}

		public IFormatter GetFormatter(Type type)
		{
			if (type == typeof(LabelTarget))
			{
				return _labelTargetFormatter;
			}
			if (type == typeof(LabelFormatter))
			{
				return _labelFormatter;
			}
			if (type == typeof(MemberAssignment))
			{
				return _memberAssignmentFormatter;
			}
			if (type == typeof(MemberListBinding))
			{
				return _memberListBindingFormatter;
			}
			if (type == typeof(MemberMemberBinding))
			{
				return _memberMemberBindingFormatter;
			}
			return null;
		}
	}
}
