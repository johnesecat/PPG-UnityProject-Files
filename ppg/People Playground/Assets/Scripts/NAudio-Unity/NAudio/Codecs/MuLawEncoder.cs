using System;
using System.Runtime.CompilerServices;

namespace NAudio.Codecs
{
	public static class MuLawEncoder
	{
		private const int cBias = 132;

		private const int cClip = 32635;

		private static readonly byte[] MuLawCompressTable;

		public static byte LinearToMuLawSample(short sample)
		{
			int num = (sample >> 8) & 0x80;
			if (num != 0)
			{
				sample = (short)(-sample);
			}
			if (sample > 32635)
			{
				sample = 32635;
			}
			sample = (short)(sample + 132);
			int num2 = MuLawCompressTable[(sample >> 7) & 0xFF];
			int num3 = (sample >> num2 + 3) & 0xF;
			return (byte)(~(num | (num2 << 4) | num3));
		}

		static MuLawEncoder()
		{
			byte[] array = new byte[256];
			RuntimeHelpers.InitializeArray((global::System.Array)array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
			MuLawCompressTable = array;
		}
	}
}
