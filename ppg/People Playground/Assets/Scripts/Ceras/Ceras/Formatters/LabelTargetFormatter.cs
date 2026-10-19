using System;
using System.Linq.Expressions;

namespace Ceras.Formatters
{
	internal class LabelTargetFormatter : IFormatter<LabelTarget>, IFormatter
	{
		private IFormatter<string> _stringFormatter;

		private IFormatter<Type> _typeFormatter;

		public LabelTargetFormatter()
		{
			CerasSerializer.AddFormatterConstructedType(typeof(LabelTarget));
		}

		public void Serialize(ref byte[] buffer, ref int offset, LabelTarget exp)
		{
			_stringFormatter.Serialize(ref buffer, ref offset, exp.Name);
			_typeFormatter.Serialize(ref buffer, ref offset, exp.Type);
		}

		public void Deserialize(byte[] buffer, ref int offset, ref LabelTarget exp)
		{
			string value = null;
			_stringFormatter.Deserialize(buffer, ref offset, ref value);
			Type value2 = null;
			_typeFormatter.Deserialize(buffer, ref offset, ref value2);
			if (exp == null || !(exp.Name == value) || !(exp.Type == value2))
			{
				exp = Expression.Label(value2, value);
			}
		}
	}
}
