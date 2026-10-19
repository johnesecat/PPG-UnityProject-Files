using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Reflection;

namespace Ceras.Formatters
{
	internal class MemberMemberBindingFormatter : IFormatter<MemberMemberBinding>, IFormatter
	{
		private IFormatter<MemberInfo> _memberInfoFormatter;

		private IFormatter<MemberBinding[]> _bindingsArFormatter;

		public MemberMemberBindingFormatter()
		{
			CerasSerializer.AddFormatterConstructedType(typeof(MemberMemberBinding));
		}

		public void Serialize(ref byte[] buffer, ref int offset, MemberMemberBinding binding)
		{
			_memberInfoFormatter.Serialize(ref buffer, ref offset, binding.Member);
			ReadOnlyCollection<MemberBinding> bindings = binding.Bindings;
			MemberBinding[] array = new MemberBinding[bindings.Count];
			bindings.CopyTo(array, 0);
			_bindingsArFormatter.Serialize(ref buffer, ref offset, array);
		}

		public void Deserialize(byte[] buffer, ref int offset, ref MemberMemberBinding binding)
		{
			MemberInfo value = null;
			_memberInfoFormatter.Deserialize(buffer, ref offset, ref value);
			MemberBinding[] value2 = null;
			_bindingsArFormatter.Deserialize(buffer, ref offset, ref value2);
			binding = Expression.MemberBind(value, value2);
		}
	}
}
