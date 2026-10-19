using System;
using System.IO;
using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Rfc2251
{
	public class RfcLdapMessage : Asn1Sequence
	{
		private Asn1Object op;

		private RfcControls controls;

		private LdapMessage requestMessage;

		public virtual int MessageID => ((Asn1Integer)get_Renamed(0)).intValue();

		public virtual int Type => get_Renamed(1).getIdentifier().Tag;

		public virtual Asn1Object Response => get_Renamed(1);

		public virtual RfcControls Controls
		{
			get
			{
				if (size() > 2)
				{
					return (RfcControls)get_Renamed(2);
				}
				return null;
			}
		}

		public virtual string RequestDN => ((RfcRequest)op).getRequestDN();

		public virtual LdapMessage RequestingMessage
		{
			get
			{
				return requestMessage;
			}
			set
			{
				requestMessage = value;
			}
		}

		internal RfcLdapMessage(Asn1Object[] origContent, RfcRequest origRequest, string dn, string filter, bool reference)
			: base(origContent, origContent.Length)
		{
			set_Renamed(0, new RfcMessageID());
			RfcRequest rfcRequest = ((RfcRequest)origContent[1]).dupRequest(dn, filter, reference);
			op = (Asn1Object)rfcRequest;
			set_Renamed(1, (Asn1Object)rfcRequest);
		}

		public RfcLdapMessage(RfcRequest op)
			: this(op, null)
		{
		}

		public RfcLdapMessage(RfcRequest op, RfcControls controls)
			: base(3)
		{
			this.op = (Asn1Object)op;
			this.controls = controls;
			add(new RfcMessageID());
			add((Asn1Object)op);
			if (controls != null)
			{
				add(controls);
			}
		}

		public RfcLdapMessage(Asn1Sequence op)
			: this(op, null)
		{
		}

		public RfcLdapMessage(Asn1Sequence op, RfcControls controls)
			: base(3)
		{
			this.op = op;
			this.controls = controls;
			add(new RfcMessageID());
			add(op);
			if (controls != null)
			{
				add(controls);
			}
		}

		[CLSCompliant(false)]
		public RfcLdapMessage(Asn1Decoder dec, Stream in_Renamed, int len)
			: base(dec, in_Renamed, len)
		{
			Asn1Tagged obj = (Asn1Tagged)get_Renamed(1);
			Asn1Identifier identifier = obj.getIdentifier();
			sbyte[] array = ((Asn1OctetString)obj.taggedValue()).byteValue();
			MemoryStream in_Renamed2 = new MemoryStream(SupportClass.ToByteArray(array));
			switch (identifier.Tag)
			{
			case 4:
				set_Renamed(1, new RfcSearchResultEntry(dec, in_Renamed2, array.Length));
				break;
			case 5:
				set_Renamed(1, new RfcSearchResultDone(dec, in_Renamed2, array.Length));
				break;
			case 19:
				set_Renamed(1, new RfcSearchResultReference(dec, in_Renamed2, array.Length));
				break;
			case 9:
				set_Renamed(1, new RfcAddResponse(dec, in_Renamed2, array.Length));
				break;
			case 1:
				set_Renamed(1, new RfcBindResponse(dec, in_Renamed2, array.Length));
				break;
			case 15:
				set_Renamed(1, new RfcCompareResponse(dec, in_Renamed2, array.Length));
				break;
			case 11:
				set_Renamed(1, new RfcDelResponse(dec, in_Renamed2, array.Length));
				break;
			case 24:
				set_Renamed(1, new RfcExtendedResponse(dec, in_Renamed2, array.Length));
				break;
			case 25:
				set_Renamed(1, new RfcIntermediateResponse(dec, in_Renamed2, array.Length));
				break;
			case 7:
				set_Renamed(1, new RfcModifyResponse(dec, in_Renamed2, array.Length));
				break;
			case 13:
				set_Renamed(1, new RfcModifyDNResponse(dec, in_Renamed2, array.Length));
				break;
			default:
				throw new SystemException("RfcLdapMessage: Invalid tag: " + identifier.Tag);
			}
			if (size() > 2)
			{
				array = ((Asn1OctetString)((Asn1Tagged)get_Renamed(2)).taggedValue()).byteValue();
				in_Renamed2 = new MemoryStream(SupportClass.ToByteArray(array));
				set_Renamed(2, new RfcControls(dec, in_Renamed2, array.Length));
			}
		}

		public RfcRequest getRequest()
		{
			return (RfcRequest)get_Renamed(1);
		}

		public virtual bool isRequest()
		{
			return get_Renamed(1) is RfcRequest;
		}

		public object dupMessage(string dn, string filter, bool reference)
		{
			if (op == null)
			{
				throw new LdapException("DUP_ERROR", 82, null);
			}
			return new RfcLdapMessage(toArray(), (RfcRequest)get_Renamed(1), dn, filter, reference);
		}
	}
}
