using System.Linq.Expressions;
using System.Reflection;

namespace Ceras.Formatters
{
	internal struct MemberParameterPair
	{
		public MemberInfo Member;

		public ParameterExpression LocalVar;
	}
}
