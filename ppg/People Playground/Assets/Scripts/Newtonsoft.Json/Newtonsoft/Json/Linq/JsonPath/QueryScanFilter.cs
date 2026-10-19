using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Linq.JsonPath
{
	[NullableContext(1)]
	[Nullable(0)]
	internal class QueryScanFilter : PathFilter
	{
		internal QueryExpression Expression;

		public QueryScanFilter(QueryExpression expression)
		{
			Expression = expression;
		}

		public override IEnumerable<JToken> ExecuteFilter(JToken root, IEnumerable<JToken> current, [Nullable(2)] JsonSelectSettings settings)
		{
			foreach (JToken item in current)
			{
				JContainer jContainer = item as JContainer;
				if (jContainer != null)
				{
					foreach (JToken item2 in jContainer.DescendantsAndSelf())
					{
						if (Expression.IsMatch(root, item2, settings))
						{
							yield return item2;
						}
					}
				}
				else if (Expression.IsMatch(root, item, settings))
				{
					yield return item;
				}
			}
		}
	}
}
