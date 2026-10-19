using System.Runtime.CompilerServices;
using System.Text;
using Ceras;
using Ceras.Formatters;

public class CustomStringFormatter : IFormatter<string>, IFormatter
{
	private static Encoding[] EncodingStack = new Encoding[1]
	{
		new UTF8Encoding(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true)
	};

	public void Serialize(ref byte[] buffer, ref int offset, string value)
	{
		WriteString(ref buffer, ref offset, value);
	}

	public void Deserialize(byte[] buffer, ref int offset, ref string value)
	{
		value = ReadString(buffer, ref offset);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string ReadString(byte[] buffer, ref int offset)
	{
		int num = SerializerBinary.ReadUInt32Bias(buffer, ref offset, 1);
		if (num == -1)
		{
			return null;
		}
		string @string = EncodingStack[0].GetString(buffer, offset, num);
		offset += num;
		return @string;
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static void WriteString(ref byte[] buffer, ref int offset, string value)
	{
		if (value == null)
		{
			SerializerBinary.WriteUInt32Bias(ref buffer, ref offset, -1, 1);
			return;
		}
		int byteCount = EncodingStack[0].GetByteCount(value);
		SerializerBinary.EnsureCapacity(ref buffer, offset, byteCount + 5);
		SerializerBinary.WriteUInt32BiasNoCheck(buffer, ref offset, byteCount, 1);
		int bytes = EncodingStack[0].GetBytes(value, 0, value.Length, buffer, offset);
		offset += bytes;
	}
}
