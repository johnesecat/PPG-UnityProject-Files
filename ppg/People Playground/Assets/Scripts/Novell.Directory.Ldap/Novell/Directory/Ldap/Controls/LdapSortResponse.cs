using System;
using System.IO;
using Novell.Directory.Ldap.Asn1;

namespace Novell.Directory.Ldap.Controls
{
	public class LdapSortResponse : LdapControl
	{
		private string failedAttribute;

		private int resultCode;

		public virtual string FailedAttribute => failedAttribute;

		public virtual int ResultCode => resultCode;

		[CLSCompliant(false)]
		public LdapSortResponse(string oid, bool critical, sbyte[] values)
			: base(oid, critical, values)
		{
			Asn1Object asn1Object = (new LBERDecoder() ?? throw new IOException("Decoding error")).decode(values);
			if (asn1Object == null || !(asn1Object is Asn1Sequence))
			{
				throw new IOException("Decoding error");
			}
			Asn1Object asn1Object2 = ((Asn1Sequence)asn1Object).get_Renamed(0);
			if (asn1Object2 != null && asn1Object2 is Asn1Enumerated)
			{
				resultCode = ((Asn1Enumerated)asn1Object2).intValue();
			}
			if (((Asn1Sequence)asn1Object).size() > 1)
			{
				Asn1Object asn1Object3 = ((Asn1Sequence)asn1Object).get_Renamed(1);
				if (asn1Object3 != null && asn1Object3 is Asn1OctetString)
				{
					failedAttribute = ((Asn1OctetString)asn1Object3).stringValue();
				}
			}
		}
	}
}
