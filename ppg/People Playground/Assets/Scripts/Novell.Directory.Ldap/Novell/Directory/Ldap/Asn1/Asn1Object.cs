using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace Novell.Directory.Ldap.Asn1
{
	[Serializable]
	public abstract class Asn1Object : ISerializable
	{
		private Asn1Identifier id;

		public Asn1Object(Asn1Identifier id)
		{
			this.id = id;
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
		}

		public abstract void encode(Asn1Encoder enc, Stream out_Renamed);

		public virtual Asn1Identifier getIdentifier()
		{
			return id;
		}

		public virtual void setIdentifier(Asn1Identifier id)
		{
			this.id = id;
		}

		[CLSCompliant(false)]
		public sbyte[] getEncoding(Asn1Encoder enc)
		{
			MemoryStream memoryStream = new MemoryStream();
			try
			{
				encode(enc, memoryStream);
			}
			catch (IOException ex)
			{
				throw new SystemException("IOException while encoding to byte array: " + ex.ToString());
			}
			return SupportClass.ToSByteArray(memoryStream.ToArray());
		}

		[CLSCompliant(false)]
		public override string ToString()
		{
			string[] array = new string[4] { "[UNIVERSAL ", "[APPLICATION ", "[CONTEXT ", "[PRIVATE " };
			StringBuilder stringBuilder = new StringBuilder();
			Asn1Identifier identifier = getIdentifier();
			stringBuilder.Append(array[identifier.Asn1Class]).Append(identifier.Tag).Append("] ");
			return stringBuilder.ToString();
		}
	}
}
