using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;

namespace Ceras.Formatters
{
	internal class MemberListBindingFormatter : IFormatter<MemberListBinding>, IFormatter
	{
		private IFormatter<MemberInfo> _memberInfoFormatter;

		private IFormatter<ElementInit[]> _initArFormatter;

		public MemberListBindingFormatter()
		{
			CerasSerializer.AddFormatterConstructedType(typeof(MemberListBinding));
		}

		public void Serialize(ref byte[] buffer, ref int offset, MemberListBinding binding)
		{
			_memberInfoFormatter.Serialize(ref buffer, ref offset, binding.Member);
			ReadOnlyCollection<ElementInit> initializers = binding.Initializers;
			ElementInit[] array = new ElementInit[initializers.Count];
			initializers.CopyTo(array, 0);
			_initArFormatter.Serialize(ref buffer, ref offset, array);
		}

		public void Deserialize(byte[] buffer, ref int offset, ref MemberListBinding binding)
		{
			MemberInfo value = null;
			_memberInfoFormatter.Deserialize(buffer, ref offset, ref value);
			ElementInit[] value2 = null;
			_initArFormatter.Deserialize(buffer, ref offset, ref value2);
			binding = Expression.ListBind(value, value2);
		}
	}
}
