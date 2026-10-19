using System;
using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Rfc2251
{
	public class RfcBindRequest : Asn1Sequence, RfcRequest
	{
		private static readonly Asn1Identifier ID = new Asn1Identifier(1, constructed: true, 0);

		public virtual Asn1Integer Version
		{
			get
			{
				return (Asn1Integer)get_Renamed(0);
			}
			set
			{
				set_Renamed(0, value);
			}
		}

		public virtual RfcLdapDN Name
		{
			get
			{
				return (RfcLdapDN)get_Renamed(1);
			}
			set
			{
				set_Renamed(1, value);
			}
		}

		public virtual RfcAuthenticationChoice AuthenticationChoice
		{
			get
			{
				return (RfcAuthenticationChoice)get_Renamed(2);
			}
			set
			{
				set_Renamed(2, value);
			}
		}

		public RfcBindRequest(Asn1Integer version, RfcLdapDN name, RfcAuthenticationChoice auth)
			: base(3)
		{
			add(version);
			add(name);
			add(auth);
		}

		[CLSCompliant(false)]
		public RfcBindRequest(int version, string dn, string mechanism, sbyte[] credentials)
			: this(new Asn1Integer(version), new RfcLdapDN(dn), new RfcAuthenticationChoice(mechanism, credentials))
		{
		}

		internal RfcBindRequest(Asn1Object[] origRequest, string base_Renamed)
			: base(origRequest, origRequest.Length)
		{
			if (base_Renamed != null)
			{
				set_Renamed(1, new RfcLdapDN(base_Renamed));
			}
		}

		public override Asn1Identifier getIdentifier()
		{
			return ID;
		}

		public RfcRequest dupRequest(string base_Renamed, string filter, bool request)
		{
			return new RfcBindRequest(toArray(), base_Renamed);
		}

		public string getRequestDN()
		{
			return ((RfcLdapDN)get_Renamed(1)).stringValue();
		}
	}
}
