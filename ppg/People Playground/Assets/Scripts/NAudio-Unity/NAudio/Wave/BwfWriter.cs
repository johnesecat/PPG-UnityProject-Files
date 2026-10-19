using System;
using System.IO;
using System.Text;

namespace NAudio.Wave
{
	public class BwfWriter : global::System.IDisposable
	{
		private readonly WaveFormat format;

		private readonly BinaryWriter writer;

		private readonly long dataChunkSizePosition;

		private long dataLength;

		private bool isDisposed;

		public BwfWriter(string filename, WaveFormat format, BextChunkInfo bextChunkInfo)
		{
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Expected O, but got Unknown
			this.format = format;
			writer = new BinaryWriter((Stream)(object)File.OpenWrite(filename));
			writer.Write(Encoding.get_UTF8().GetBytes("RIFF"));
			writer.Write(0);
			writer.Write(Encoding.get_UTF8().GetBytes("WAVE"));
			writer.Write(Encoding.get_UTF8().GetBytes("JUNK"));
			writer.Write(28);
			writer.Write(0L);
			writer.Write(0L);
			writer.Write(0L);
			writer.Write(0);
			writer.Write(Encoding.get_UTF8().GetBytes("bext"));
			byte[] bytes = Encoding.get_ASCII().GetBytes(bextChunkInfo.CodingHistory ?? "");
			int num = 602 + bytes.Length;
			if (num % 2 != 0)
			{
				num++;
			}
			writer.Write(num);
			writer.get_BaseStream().get_Position();
			writer.Write(GetAsBytes(bextChunkInfo.Description, 256));
			writer.Write(GetAsBytes(bextChunkInfo.Originator, 32));
			writer.Write(GetAsBytes(bextChunkInfo.OriginatorReference, 32));
			writer.Write(GetAsBytes(bextChunkInfo.OriginationDate, 10));
			writer.Write(GetAsBytes(bextChunkInfo.OriginationTime, 8));
			writer.Write(bextChunkInfo.TimeReference);
			writer.Write(bextChunkInfo.Version);
			writer.Write(GetAsBytes(bextChunkInfo.UniqueMaterialIdentifier, 64));
			writer.Write(bextChunkInfo.Reserved);
			writer.Write(bytes);
			if (bytes.Length % 2 != 0)
			{
				writer.Write((byte)0);
			}
			writer.Write(Encoding.get_UTF8().GetBytes("fmt "));
			format.Serialize(writer);
			writer.Write(Encoding.get_UTF8().GetBytes("data"));
			dataChunkSizePosition = writer.get_BaseStream().get_Position();
			writer.Write(-1);
		}

		public void Write(byte[] buffer, int offset, int count)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			if (isDisposed)
			{
				throw new ObjectDisposedException("This BWF Writer already disposed");
			}
			writer.Write(buffer, offset, count);
			dataLength += count;
		}

		public void Flush()
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			if (isDisposed)
			{
				throw new ObjectDisposedException("This BWF Writer already disposed");
			}
			writer.Flush();
			FixUpChunkSizes(restorePosition: true);
		}

		private void FixUpChunkSizes(bool restorePosition)
		{
			long position = writer.get_BaseStream().get_Position();
			bool num = dataLength > 2147483647;
			long num2 = writer.get_BaseStream().get_Length() - 8;
			if (num)
			{
				int num3 = format.BitsPerSample / 8 * format.Channels;
				writer.get_BaseStream().set_Position(0L);
				writer.Write(Encoding.get_UTF8().GetBytes("RF64"));
				writer.Write(-1);
				Stream baseStream = writer.get_BaseStream();
				baseStream.set_Position(baseStream.get_Position() + 4);
				writer.Write(Encoding.get_UTF8().GetBytes("ds64"));
				Stream baseStream2 = writer.get_BaseStream();
				baseStream2.set_Position(baseStream2.get_Position() + 4);
				writer.Write(num2);
				writer.Write(dataLength);
				writer.Write(dataLength / num3);
			}
			else
			{
				writer.get_BaseStream().set_Position(4L);
				writer.Write((uint)num2);
				writer.get_BaseStream().set_Position(dataChunkSizePosition);
				writer.Write((uint)dataLength);
			}
			if (restorePosition)
			{
				writer.get_BaseStream().set_Position(position);
			}
		}

		public void Dispose()
		{
			if (!isDisposed)
			{
				FixUpChunkSizes(restorePosition: false);
				writer.Close();
				isDisposed = true;
			}
		}

		private static byte[] GetAsBytes(string message, int byteSize)
		{
			byte[] array = new byte[byteSize];
			byte[] bytes = Encoding.get_ASCII().GetBytes(message ?? "");
			global::System.Array.Copy((global::System.Array)bytes, (global::System.Array)array, Math.Min(bytes.Length, byteSize));
			return array;
		}
	}
}
