using System.IO;
using System.Runtime.Serialization;

namespace Novell.Directory.Ldap.Asn1
{
	public class LBEREncoder : Asn1Encoder, ISerializable
	{
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
		}

		public virtual void encode(Asn1Boolean b, Stream out_Renamed)
		{
			encode(b.getIdentifier(), out_Renamed);
			out_Renamed.WriteByte(1);
			out_Renamed.WriteByte((byte)(b.booleanValue() ? ((sbyte)SupportClass.Identity(255L)) : 0));
		}

		public void encode(Asn1Numeric n, Stream out_Renamed)
		{
			sbyte[] array = new sbyte[8];
			long num = n.longValue();
			long num2 = ((num < 0) ? (-1) : 0);
			long num3 = num2 & 0x80;
			sbyte b = 0;
			while (b == 0 || num != num2 || (array[b - 1] & 0x80) != num3)
			{
				array[b] = (sbyte)(num & 0xFF);
				num >>= 8;
				b = (sbyte)(b + 1);
			}
			encode(n.getIdentifier(), out_Renamed);
			out_Renamed.WriteByte((byte)b);
			for (int num4 = b - 1; num4 >= 0; num4--)
			{
				out_Renamed.WriteByte((byte)array[num4]);
			}
		}

		public void encode(Asn1Null n, Stream out_Renamed)
		{
			encode(n.getIdentifier(), out_Renamed);
			out_Renamed.WriteByte(0);
		}

		public void encode(Asn1OctetString os, Stream out_Renamed)
		{
			encode(os.getIdentifier(), out_Renamed);
			encodeLength(os.byteValue().Length, out_Renamed);
			sbyte[] array = os.byteValue();
			out_Renamed.Write(SupportClass.ToByteArray(array), 0, array.Length);
		}

		public void encode(Asn1Structured c, Stream out_Renamed)
		{
			encode(c.getIdentifier(), out_Renamed);
			Asn1Object[] array = c.toArray();
			MemoryStream memoryStream = new MemoryStream();
			for (int i = 0; i < array.Length; i++)
			{
				array[i].encode(this, memoryStream);
			}
			encodeLength((int)memoryStream.Length, out_Renamed);
			sbyte[] array2 = SupportClass.ToSByteArray(memoryStream.ToArray());
			out_Renamed.Write(SupportClass.ToByteArray(array2), 0, array2.Length);
		}

		public void encode(Asn1Tagged t, Stream out_Renamed)
		{
			if (t.Explicit)
			{
				encode(t.getIdentifier(), out_Renamed);
				MemoryStream memoryStream = new MemoryStream();
				t.taggedValue().encode(this, memoryStream);
				encodeLength((int)memoryStream.Length, out_Renamed);
				sbyte[] array = SupportClass.ToSByteArray(memoryStream.ToArray());
				out_Renamed.Write(SupportClass.ToByteArray(array), 0, array.Length);
			}
			else
			{
				t.taggedValue().encode(this, out_Renamed);
			}
		}

		public void encode(Asn1Identifier id, Stream out_Renamed)
		{
			int asn1Class = id.Asn1Class;
			int tag = id.Tag;
			sbyte b = (sbyte)((asn1Class << 6) | (id.Constructed ? 32 : 0));
			if (tag < 30)
			{
				out_Renamed.WriteByte((byte)(b | tag));
				return;
			}
			out_Renamed.WriteByte((byte)((uint)b | 0x1Fu));
			encodeTagInteger(tag, out_Renamed);
		}

		private void encodeLength(int length, Stream out_Renamed)
		{
			if (length < 128)
			{
				out_Renamed.WriteByte((byte)length);
				return;
			}
			sbyte[] array = new sbyte[4];
			sbyte b = 0;
			while (length != 0)
			{
				array[b] = (sbyte)(length & 0xFF);
				length >>= 8;
				b = (sbyte)(b + 1);
			}
			out_Renamed.WriteByte((byte)(0x80u | (uint)b));
			for (int num = b - 1; num >= 0; num--)
			{
				out_Renamed.WriteByte((byte)array[num]);
			}
		}

		private void encodeTagInteger(int value_Renamed, Stream out_Renamed)
		{
			sbyte[] array = new sbyte[5];
			int num = 0;
			while (value_Renamed != 0)
			{
				array[num] = (sbyte)(value_Renamed & 0x7F);
				value_Renamed >>= 7;
				num++;
			}
			for (int num2 = num - 1; num2 > 0; num2--)
			{
				out_Renamed.WriteByte((byte)((uint)array[num2] | 0x80u));
			}
			out_Renamed.WriteByte((byte)array[0]);
		}
	}
}
