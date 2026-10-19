using System;
using System.IO;
using System.Runtime.Serialization;

namespace Novell.Directory.Ldap.Asn1
{
	[CLSCompliant(false)]
	public interface Asn1Decoder : ISerializable
	{
		Asn1Object decode(sbyte[] value_Renamed);

		Asn1Object decode(Stream in_Renamed);

		Asn1Object decode(Stream in_Renamed, int[] length);

		object decodeBoolean(Stream in_Renamed, int len);

		object decodeNumeric(Stream in_Renamed, int len);

		object decodeOctetString(Stream in_Renamed, int len);

		object decodeCharacterString(Stream in_Renamed, int len);
	}
}
