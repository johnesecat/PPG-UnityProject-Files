using System;
using System.Runtime.CompilerServices;

namespace NAudio.Codecs
{
	public class ALawDecoder
	{
		private static readonly short[] ALawDecompressTable;

		public static short ALawToLinearSample(byte aLaw)
		{
			return ALawDecompressTable[aLaw];
		}

		static ALawDecoder()
		{
			short[] array = new short[256];
			RuntimeHelpers.InitializeArray((global::System.Array)array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
			ALawDecompressTable = array;
		}
	}
}
