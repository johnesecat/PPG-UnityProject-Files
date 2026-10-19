using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Newtonsoft.Json.Linq.JsonPath
{
	[NullableContext(1)]
	[Nullable(0)]
	internal class CompositeExpression : QueryExpression
	{
		public List<QueryExpression> Expressions { get; set; }

		public CompositeExpression(QueryOperator @operator)
			: base(@operator)
		{
			Expressions = new List<QueryExpression>();
		}

		public override bool IsMatch(JToken root, JToken t, [Nullable(2)] JsonSelectSettings settings)
		{
			switch (Operator)
			{
			case QueryOperator.And:
				foreach (QueryExpression expression in Expressions)
				{
					if (!expression.IsMatch(root, t, settings))
					{
						return false;
					}
				}
				return true;
			case QueryOperator.Or:
				foreach (QueryExpression expression2 in Expressions)
				{
					if (expression2.IsMatch(root, t, settings))
					{
						return true;
					}
				}
				return false;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
	}
}
