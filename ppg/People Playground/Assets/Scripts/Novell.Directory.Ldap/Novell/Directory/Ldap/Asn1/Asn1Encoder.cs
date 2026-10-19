using System.IO;
using System.Runtime.Serialization;

namespace Novell.Directory.Ldap.Asn1
{
	public interface Asn1Encoder : ISerializable
	{
		void encode(Asn1Boolean b, Stream out_Renamed);

		void encode(Asn1Numeric n, Stream out_Renamed);

		void encode(Asn1Null n, Stream out_Renamed);

		void encode(Asn1OctetString os, Stream out_Renamed);

		void encode(Asn1Structured c, Stream out_Renamed);

		void encode(Asn1Tagged t, Stream out_Renamed);

		void encode(Asn1Identifier id, Stream out_Renamed);
	}
}
