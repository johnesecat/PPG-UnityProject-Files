using System;
using System.IO;
using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Rfc2251
{
	public class RfcControl : Asn1Sequence
	{
		public virtual Asn1OctetString ControlType => (Asn1OctetString)get_Renamed(0);

		public virtual Asn1Boolean Criticality
		{
			get
			{
				if (size() > 1)
				{
					Asn1Object asn1Object = get_Renamed(1);
					if (asn1Object is Asn1Boolean)
					{
						return (Asn1Boolean)asn1Object;
					}
				}
				return new Asn1Boolean(content: false);
			}
		}

		public virtual Asn1OctetString ControlValue
		{
			get
			{
				if (size() > 2)
				{
					return (Asn1OctetString)get_Renamed(2);
				}
				if (size() > 1)
				{
					Asn1Object asn1Object = get_Renamed(1);
					if (asn1Object is Asn1OctetString)
					{
						return (Asn1OctetString)asn1Object;
					}
				}
				return null;
			}
			set
			{
				if (value == null)
				{
					return;
				}
				if (size() == 3)
				{
					set_Renamed(2, value);
				}
				else if (size() == 2)
				{
					if (get_Renamed(1) is Asn1OctetString)
					{
						set_Renamed(1, value);
					}
					else
					{
						add(value);
					}
				}
			}
		}

		public RfcControl(RfcLdapOID controlType)
			: this(controlType, new Asn1Boolean(content: false), null)
		{
		}

		public RfcControl(RfcLdapOID controlType, Asn1Boolean criticality)
			: this(controlType, criticality, null)
		{
		}

		public RfcControl(RfcLdapOID controlType, Asn1Boolean criticality, Asn1OctetString controlValue)
			: base(3)
		{
			add(controlType);
			if (criticality.booleanValue())
			{
				add(criticality);
			}
			if (controlValue != null)
			{
				add(controlValue);
			}
		}

		[CLSCompliant(false)]
		public RfcControl(Asn1Decoder dec, Stream in_Renamed, int len)
			: base(dec, in_Renamed, len)
		{
		}

		public RfcControl(Asn1Sequence seqObj)
			: base(3)
		{
			int num = seqObj.size();
			for (int i = 0; i < num; i++)
			{
				add(seqObj.get_Renamed(i));
			}
		}
	}
}
