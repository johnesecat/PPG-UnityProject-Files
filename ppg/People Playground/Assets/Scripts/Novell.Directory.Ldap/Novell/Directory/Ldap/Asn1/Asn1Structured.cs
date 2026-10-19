using System;
using System.IO;
using System.Text;

namespace Novell.Directory.Ldap.Asn1
{
	public abstract class Asn1Structured : Asn1Object
	{
		private Asn1Object[] content;

		private int contentIndex;

		protected internal Asn1Structured(Asn1Identifier id)
			: this(id, 10)
		{
		}

		protected internal Asn1Structured(Asn1Identifier id, int size)
			: base(id)
		{
			content = new Asn1Object[size];
		}

		protected internal Asn1Structured(Asn1Identifier id, Asn1Object[] newContent, int size)
			: base(id)
		{
			content = newContent;
			contentIndex = size;
		}

		public override void encode(Asn1Encoder enc, Stream out_Renamed)
		{
			enc.encode(this, out_Renamed);
		}

		[CLSCompliant(false)]
		protected internal void decodeStructured(Asn1Decoder dec, Stream in_Renamed, int len)
		{
			int[] array = new int[1];
			while (len > 0)
			{
				add(dec.decode(in_Renamed, array));
				len -= array[0];
			}
		}

		public Asn1Object[] toArray()
		{
			Asn1Object[] array = new Asn1Object[contentIndex];
			Array.Copy(content, 0, array, 0, contentIndex);
			return array;
		}

		public void add(Asn1Object value_Renamed)
		{
			if (contentIndex == content.Length)
			{
				Asn1Object[] destinationArray = new Asn1Object[contentIndex + contentIndex];
				Array.Copy(content, 0, destinationArray, 0, contentIndex);
				content = destinationArray;
			}
			content[contentIndex++] = value_Renamed;
		}

		public void set_Renamed(int index, Asn1Object value_Renamed)
		{
			if (index >= contentIndex || index < 0)
			{
				throw new IndexOutOfRangeException("Asn1Structured: get: index " + index + ", size " + contentIndex);
			}
			content[index] = value_Renamed;
		}

		public Asn1Object get_Renamed(int index)
		{
			if (index >= contentIndex || index < 0)
			{
				throw new IndexOutOfRangeException("Asn1Structured: set: index " + index + ", size " + contentIndex);
			}
			return content[index];
		}

		public int size()
		{
			return contentIndex;
		}

		[CLSCompliant(false)]
		public virtual string toString(string type)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(type);
			for (int i = 0; i < contentIndex; i++)
			{
				stringBuilder.Append(content[i]);
				if (i != contentIndex - 1)
				{
					stringBuilder.Append(", ");
				}
			}
			stringBuilder.Append(" }");
			return ToString() + stringBuilder.ToString();
		}
	}
}
