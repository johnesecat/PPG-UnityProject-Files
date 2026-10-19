using System;
using System.IO;
using System.Threading.Tasks;

namespace Ceras.Helpers
{
	public static class CerasNetworkingExampleExtensions
	{
		[ThreadStatic]
		private static byte[] _lengthPrefixBuffer;

		[ThreadStatic]
		private static byte[] _streamBuffer;

		public static void WriteToStream(this CerasSerializer ceras, Stream stream, object obj)
		{
			if (_lengthPrefixBuffer == null)
			{
				_lengthPrefixBuffer = new byte[5];
			}
			int num = ceras.Serialize(obj, ref _streamBuffer);
			int offset = 0;
			SerializerBinary.WriteUInt32(ref _lengthPrefixBuffer, ref offset, (uint)num);
			stream.Write(_lengthPrefixBuffer, 0, offset);
			stream.Write(_streamBuffer, 0, num);
		}

		public static async Task<object> ReadFromStream(this CerasSerializer ceras, Stream stream)
		{
			int length = (int)(await ReadVarIntFromStream(stream));
			byte[] recvBuffer = new byte[length];
			int num;
			for (int totalRead = 0; totalRead < length; totalRead += num)
			{
				int count = length - totalRead;
				num = await stream.ReadAsync(recvBuffer, totalRead, count);
				if (num <= 0)
				{
					throw new Exception("Stream closed");
				}
			}
			return ceras.Deserialize<object>(recvBuffer);
		}

		private static async Task<uint> ReadVarIntFromStream(Stream stream)
		{
			byte[] recvPrefixBuffer = new byte[1];
			int shift = 0;
			ulong result = 0uL;
			while (true)
			{
				if (await stream.ReadAsync(recvPrefixBuffer, 0, 1) <= 0)
				{
					throw new Exception("Stream terminated");
				}
				long num = recvPrefixBuffer[0];
				ulong num2 = (ulong)(num & 0x7F);
				result |= num2 << shift;
				if (shift > 32)
				{
					throw new Exception("Malformed VarInt");
				}
				if ((num & 0x80) != 128)
				{
					break;
				}
				shift += 7;
			}
			return (uint)result;
		}
	}
}
