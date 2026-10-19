using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Linq.JsonPath
{
	[NullableContext(1)]
	[Nullable(0)]
	internal abstract class QueryExpression
	{
		internal QueryOperator Operator;

		public QueryExpression(QueryOperator @operator)
		{
			Operator = @operator;
		}

		public bool IsMatch(JToken root, JToken t)
		{
			return IsMatch(root, t, null);
		}

		public abstract bool IsMatch(JToken root, JToken t, [Nullable(2)] JsonSelectSettings settings);
	}
}
