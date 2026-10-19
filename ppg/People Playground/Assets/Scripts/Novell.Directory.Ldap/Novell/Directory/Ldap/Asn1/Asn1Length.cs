using System.IO;

namespace Novell.Directory.Ldap.Asn1
{
	public class Asn1Length
	{
		private int length;

		private int encodedLength;

		public virtual int Length => length;

		public virtual int EncodedLength => encodedLength;

		public Asn1Length()
		{
		}

		public Asn1Length(int length)
		{
			this.length = length;
		}

		public Asn1Length(Stream in_Renamed)
		{
			int num = in_Renamed.ReadByte();
			encodedLength++;
			if (num == 128)
			{
				length = -1;
				return;
			}
			if (num < 128)
			{
				length = num;
				return;
			}
			length = 0;
			for (num &= 0x7F; num > 0; num--)
			{
				int num2 = in_Renamed.ReadByte();
				encodedLength++;
				if (num2 < 0)
				{
					throw new EndOfStreamException("BERDecoder: decode: EOF in Asn1Length");
				}
				length = (length << 8) + num2;
			}
		}

		public void reset(Stream in_Renamed)
		{
			encodedLength = 0;
			int num = in_Renamed.ReadByte();
			encodedLength++;
			if (num == 128)
			{
				length = -1;
				return;
			}
			if (num < 128)
			{
				length = num;
				return;
			}
			length = 0;
			for (num &= 0x7F; num > 0; num--)
			{
				int num2 = in_Renamed.ReadByte();
				encodedLength++;
				if (num2 < 0)
				{
					throw new EndOfStreamException("BERDecoder: decode: EOF in Asn1Length");
				}
				length = (length << 8) + num2;
			}
		}
	}
}
