using System;
using System.Runtime.CompilerServices;

namespace NAudio.Codecs
{
	public static class ALawEncoder
	{
		private const int cBias = 132;

		private const int cClip = 32635;

		private static readonly byte[] ALawCompressTable;

		public static byte LinearToALawSample(short sample)
		{
			int num = (~sample >> 8) & 0x80;
			if (num == 0)
			{
				sample = (short)(-sample);
			}
			if (sample > 32635)
			{
				sample = 32635;
			}
			byte b;
			if (sample >= 256)
			{
				int num2 = ALawCompressTable[(sample >> 8) & 0x7F];
				int num3 = (sample >> num2 + 3) & 0xF;
				b = (byte)((num2 << 4) | num3);
			}
			else
			{
				b = (byte)(sample >> 4);
			}
			return (byte)(b ^ (byte)((uint)num ^ 0x55u));
		}

		static ALawEncoder()
		{
			byte[] array = new byte[128];
			RuntimeHelpers.InitializeArray((global::System.Array)array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
			ALawCompressTable = array;
		}
	}
}
