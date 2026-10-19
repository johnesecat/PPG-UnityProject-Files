using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NAudio.Utils;

namespace NAudio.Wave
{
	public class Id3v2Tag
	{
		private long tagStartPosition;

		private long tagEndPosition;

		private byte[] rawData;

		public byte[] RawData => rawData;

		public static Id3v2Tag ReadTag(Stream input)
		{
			try
			{
				return new Id3v2Tag(input);
			}
			catch (FormatException)
			{
				return null;
			}
		}

		public static Id3v2Tag Create(global::System.Collections.Generic.IEnumerable<KeyValuePair<string, string>> tags)
		{
			return ReadTag(CreateId3v2TagStream(tags));
		}

		private static byte[] FrameSizeToBytes(int n)
		{
			byte[] bytes = BitConverter.GetBytes(n);
			global::System.Array.Reverse((global::System.Array)bytes);
			return bytes;
		}

		private static byte[] CreateId3v2Frame(string key, string value)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException("key");
			}
			if (string.IsNullOrEmpty(value))
			{
				throw new ArgumentNullException("value");
			}
			if (key.get_Length() != 4)
			{
				throw new ArgumentOutOfRangeException("key", "key " + key + " must be 4 characters long");
			}
			byte[] array = new byte[2] { 255, 254 };
			byte[] array2 = new byte[3];
			byte[] array3 = new byte[2];
			byte[] array4 = ((!(key == "COMM")) ? ByteArrayExtensions.Concat(new byte[1] { 1 }, array, Encoding.get_Unicode().GetBytes(value)) : ByteArrayExtensions.Concat(new byte[1] { 1 }, array2, array3, array, Encoding.get_Unicode().GetBytes(value)));
			return ByteArrayExtensions.Concat(Encoding.get_UTF8().GetBytes(key), FrameSizeToBytes(array4.Length), new byte[2], array4);
		}

		private static byte[] GetId3TagHeaderSize(int size)
		{
			byte[] array = new byte[4];
			for (int num = array.Length - 1; num >= 0; num--)
			{
				array[num] = (byte)(size % 128);
				size /= 128;
			}
			return array;
		}

		private static byte[] CreateId3v2TagHeader(global::System.Collections.Generic.IEnumerable<byte[]> frames)
		{
			int num = 0;
			global::System.Collections.Generic.IEnumerator<byte[]> enumerator = frames.GetEnumerator();
			try
			{
				while (((global::System.Collections.IEnumerator)enumerator).MoveNext())
				{
					byte[] current = enumerator.get_Current();
					num += current.Length;
				}
			}
			finally
			{
				((global::System.IDisposable)enumerator)?.Dispose();
			}
			return ByteArrayExtensions.Concat(Encoding.get_UTF8().GetBytes("ID3"), new byte[2] { 3, 0 }, new byte[1], GetId3TagHeaderSize(num));
		}

		private static Stream CreateId3v2TagStream(global::System.Collections.Generic.IEnumerable<KeyValuePair<string, string>> tags)
		{
			//IL_0010: Unknown result type (might be due to invalid IL or missing references)
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0051: Expected O, but got Unknown
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			List<byte[]> val = new List<byte[]>();
			global::System.Collections.Generic.IEnumerator<KeyValuePair<string, string>> enumerator = tags.GetEnumerator();
			try
			{
				while (((global::System.Collections.IEnumerator)enumerator).MoveNext())
				{
					KeyValuePair<string, string> current = enumerator.get_Current();
					val.Add(CreateId3v2Frame(current.get_Key(), current.get_Value()));
				}
			}
			finally
			{
				((global::System.IDisposable)enumerator)?.Dispose();
			}
			byte[] array = CreateId3v2TagHeader((global::System.Collections.Generic.IEnumerable<byte[]>)val);
			MemoryStream val2 = new MemoryStream();
			((Stream)val2).Write(array, 0, array.Length);
			Enumerator<byte[]> enumerator2 = val.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					byte[] current2 = enumerator2.get_Current();
					((Stream)val2).Write(current2, 0, current2.Length);
				}
			}
			finally
			{
				((global::System.IDisposable)enumerator2).Dispose();
			}
			((Stream)val2).set_Position(0L);
			return (Stream)(object)val2;
		}

		private Id3v2Tag(Stream input)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Expected O, but got Unknown
			//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
			tagStartPosition = input.get_Position();
			BinaryReader val = new BinaryReader(input);
			byte[] array = val.ReadBytes(10);
			if (array.Length >= 3 && array[0] == 73 && array[1] == 68 && array[2] == 51)
			{
				if ((array[5] & 0x40) == 64)
				{
					byte[] array2 = val.ReadBytes(4);
					_ = array2[0] * 2097152 + array2[1] * 16384 + array2[2] * 128;
					_ = array2[3];
				}
				int num = array[6] * 2097152;
				num += array[7] * 16384;
				num += array[8] * 128;
				num += array[9];
				val.ReadBytes(num);
				if ((array[5] & 0x10) == 16)
				{
					val.ReadBytes(10);
				}
				tagEndPosition = input.get_Position();
				input.set_Position(tagStartPosition);
				rawData = val.ReadBytes((int)(tagEndPosition - tagStartPosition));
				return;
			}
			input.set_Position(tagStartPosition);
			throw new FormatException("Not an ID3v2 tag");
		}
	}
}
