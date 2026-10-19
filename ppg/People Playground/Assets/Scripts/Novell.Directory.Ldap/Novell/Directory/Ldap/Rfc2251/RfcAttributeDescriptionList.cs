using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Rfc2251
{
	public class RfcAttributeDescriptionList : Asn1SequenceOf
	{
		public RfcAttributeDescriptionList(int size)
			: base(size)
		{
		}

		public RfcAttributeDescriptionList(string[] attrs)
			: base((attrs != null) ? attrs.Length : 0)
		{
			if (attrs != null)
			{
				for (int i = 0; i < attrs.Length; i++)
				{
					add(new RfcAttributeDescription(attrs[i]));
				}
			}
		}
	}
}
