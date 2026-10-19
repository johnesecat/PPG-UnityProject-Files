using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WatsonTcp
{
	internal static class WatsonCommon
	{
		internal static byte[] ReadStreamFully(Stream input)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			byte[] array = new byte[65536];
			MemoryStream val = new MemoryStream();
			try
			{
				int num = 0;
				while (true)
				{
					num = input.Read(array, 0, array.Length);
					if (num <= 0)
					{
						break;
					}
					((Stream)val).Write(array, 0, num);
				}
				return val.ToArray();
			}
			finally
			{
				((global::System.IDisposable)val)?.Dispose();
			}
		}

		internal static byte[] ReadFromStream(Stream stream, long count, int bufferLen)
		{
			//IL_0015: Unknown result type (might be due to invalid IL or missing references)
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Expected O, but got Unknown
			//IL_0060: Unknown result type (might be due to invalid IL or missing references)
			if (count <= 0)
			{
				return new byte[0];
			}
			if (bufferLen <= 0)
			{
				throw new ArgumentException("Buffer must be greater than zero bytes.");
			}
			byte[] array = new byte[bufferLen];
			int num = 0;
			long num2 = count;
			MemoryStream val = new MemoryStream();
			while (num2 > 0)
			{
				if (bufferLen > num2)
				{
					array = new byte[num2];
				}
				num = stream.Read(array, 0, array.Length);
				if (num > 0)
				{
					((Stream)val).Write(array, 0, num);
					num2 -= num;
					continue;
				}
				throw new IOException("Could not read from supplied stream.");
			}
			return val.ToArray();
		}

		internal static MemoryStream DataStreamToMemoryStream(long contentLength, Stream stream, int bufferLen)
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Expected O, but got Unknown
			//IL_001a: Unknown result type (might be due to invalid IL or missing references)
			//IL_002b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Expected O, but got Unknown
			//IL_0065: Unknown result type (might be due to invalid IL or missing references)
			if (contentLength <= 0)
			{
				return new MemoryStream(new byte[0]);
			}
			if (bufferLen <= 0)
			{
				throw new ArgumentException("Buffer must be greater than zero bytes.");
			}
			byte[] array = new byte[bufferLen];
			int num = 0;
			long num2 = contentLength;
			MemoryStream val = new MemoryStream();
			while (num2 > 0)
			{
				if (bufferLen > num2)
				{
					array = new byte[num2];
				}
				num = stream.Read(array, 0, array.Length);
				if (num > 0)
				{
					((Stream)val).Write(array, 0, num);
					num2 -= num;
					continue;
				}
				throw new IOException("Could not read from supplied stream.");
			}
			((Stream)val).Seek(0L, (SeekOrigin)0);
			return val;
		}

		[AsyncStateMachine(typeof(<ReadFromStreamAsync>d__3))]
		internal static global::System.Threading.Tasks.Task<byte[]> ReadFromStreamAsync(Stream stream, long count, int bufferLen)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			<ReadFromStreamAsync>d__3 <ReadFromStreamAsync>d__ = default(<ReadFromStreamAsync>d__3);
			<ReadFromStreamAsync>d__.<>t__builder = AsyncTaskMethodBuilder<byte[]>.Create();
			<ReadFromStreamAsync>d__.stream = stream;
			<ReadFromStreamAsync>d__.count = count;
			<ReadFromStreamAsync>d__.bufferLen = bufferLen;
			<ReadFromStreamAsync>d__.<>1__state = -1;
			<ReadFromStreamAsync>d__.<>t__builder.Start<<ReadFromStreamAsync>d__3>(ref <ReadFromStreamAsync>d__);
			return <ReadFromStreamAsync>d__.<>t__builder.get_Task();
		}

		[AsyncStateMachine(typeof(<ReadMessageDataAsync>d__4))]
		internal static global::System.Threading.Tasks.Task<byte[]> ReadMessageDataAsync(WatsonMessage msg, int bufferLen)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			<ReadMessageDataAsync>d__4 <ReadMessageDataAsync>d__ = default(<ReadMessageDataAsync>d__4);
			<ReadMessageDataAsync>d__.<>t__builder = AsyncTaskMethodBuilder<byte[]>.Create();
			<ReadMessageDataAsync>d__.msg = msg;
			<ReadMessageDataAsync>d__.bufferLen = bufferLen;
			<ReadMessageDataAsync>d__.<>1__state = -1;
			<ReadMessageDataAsync>d__.<>t__builder.Start<<ReadMessageDataAsync>d__4>(ref <ReadMessageDataAsync>d__);
			return <ReadMessageDataAsync>d__.<>t__builder.get_Task();
		}

		internal static byte[] AppendBytes(byte[] head, byte[] tail)
		{
			byte[] array = new byte[head.Length + tail.Length];
			global::System.Array.Copy((global::System.Array)head, 0, (global::System.Array)array, 0, head.Length);
			global::System.Array.Copy((global::System.Array)tail, 0, (global::System.Array)array, head.Length, tail.Length);
			return array;
		}

		internal static string ByteArrayToHex(byte[] data)
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000b: Expected O, but got Unknown
			StringBuilder val = new StringBuilder(data.Length * 2);
			foreach (byte b in data)
			{
				val.AppendFormat("{0:x2}", (object)b);
			}
			return ((object)val).ToString();
		}

		internal static void BytesToStream(byte[] data, int start, out int contentLength, out Stream stream)
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0010: Expected O, but got Unknown
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0025: Expected O, but got Unknown
			contentLength = 0;
			stream = (Stream)new MemoryStream(new byte[0]);
			if (data != null && data.Length != 0)
			{
				contentLength = data.Length - start;
				stream = (Stream)new MemoryStream();
				stream.Write(data, start, contentLength);
				stream.Seek(0L, (SeekOrigin)0);
			}
		}

		internal static global::System.DateTime GetExpirationTimestamp(WatsonMessage msg)
		{
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			global::System.DateTime result = msg.Expiration.get_Value();
			if (msg.SenderTimestamp.get_HasValue())
			{
				TimeSpan val = global::System.DateTime.get_Now() - msg.SenderTimestamp.get_Value();
				result = result.AddMilliseconds(((TimeSpan)(ref val)).get_TotalMilliseconds());
			}
			return result;
		}
	}
}
