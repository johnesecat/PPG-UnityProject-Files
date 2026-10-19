using System.Collections;
using Novell.Directory.Ldap.Asn1;
using Novell.Directory.Ldap.Rfc2251;

namespace Novell.Directory.Ldap
{
	public class LdapSearchRequest : LdapMessage
	{
		public const int AND = 0;

		public const int OR = 1;

		public const int NOT = 2;

		public const int EQUALITY_MATCH = 3;

		public const int SUBSTRINGS = 4;

		public const int GREATER_OR_EQUAL = 5;

		public const int LESS_OR_EQUAL = 6;

		public const int PRESENT = 7;

		public const int APPROX_MATCH = 8;

		public const int EXTENSIBLE_MATCH = 9;

		public const int INITIAL = 0;

		public const int ANY = 1;

		public const int FINAL = 2;

		public virtual string DN => Asn1Object.RequestDN;

		public virtual int Scope => ((Asn1Enumerated)((RfcSearchRequest)Asn1Object.get_Renamed(1)).get_Renamed(1)).intValue();

		public virtual int Dereference => ((Asn1Enumerated)((RfcSearchRequest)Asn1Object.get_Renamed(1)).get_Renamed(2)).intValue();

		public virtual int MaxResults => ((Asn1Integer)((RfcSearchRequest)Asn1Object.get_Renamed(1)).get_Renamed(3)).intValue();

		public virtual int ServerTimeLimit => ((Asn1Integer)((RfcSearchRequest)Asn1Object.get_Renamed(1)).get_Renamed(4)).intValue();

		public virtual bool TypesOnly => ((Asn1Boolean)((RfcSearchRequest)Asn1Object.get_Renamed(1)).get_Renamed(5)).booleanValue();

		public virtual string[] Attributes
		{
			get
			{
				RfcAttributeDescriptionList rfcAttributeDescriptionList = (RfcAttributeDescriptionList)((RfcSearchRequest)Asn1Object.get_Renamed(1)).get_Renamed(7);
				string[] array = new string[rfcAttributeDescriptionList.size()];
				for (int i = 0; i < array.Length; i++)
				{
					array[i] = ((RfcAttributeDescription)rfcAttributeDescriptionList.get_Renamed(i)).stringValue();
				}
				return array;
			}
		}

		public virtual string StringFilter => RfcFilter.filterToString();

		private RfcFilter RfcFilter => (RfcFilter)((RfcSearchRequest)Asn1Object.get_Renamed(1)).get_Renamed(6);

		public virtual IEnumerator SearchFilter => RfcFilter.getFilterIterator();

		public LdapSearchRequest(string base_Renamed, int scope, string filter, string[] attrs, int dereference, int maxResults, int serverTimeLimit, bool typesOnly, LdapControl[] cont)
			: base(3, new RfcSearchRequest(new RfcLdapDN(base_Renamed), new Asn1Enumerated(scope), new Asn1Enumerated(dereference), new Asn1Integer(maxResults), new Asn1Integer(serverTimeLimit), new Asn1Boolean(typesOnly), new RfcFilter(filter), new RfcAttributeDescriptionList(attrs)), cont)
		{
		}

		public LdapSearchRequest(string base_Renamed, int scope, RfcFilter filter, string[] attrs, int dereference, int maxResults, int serverTimeLimit, bool typesOnly, LdapControl[] cont)
			: base(3, new RfcSearchRequest(new RfcLdapDN(base_Renamed), new Asn1Enumerated(scope), new Asn1Enumerated(dereference), new Asn1Integer(maxResults), new Asn1Integer(serverTimeLimit), new Asn1Boolean(typesOnly), filter, new RfcAttributeDescriptionList(attrs)), cont)
		{
		}
	}
}
