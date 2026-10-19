using System;
using System.Runtime.CompilerServices;

namespace NAudio.Codecs
{
	public static class MuLawDecoder
	{
		private static readonly short[] MuLawDecompressTable;

		public static short MuLawToLinearSample(byte muLaw)
		{
			return MuLawDecompressTable[muLaw];
		}

		static MuLawDecoder()
		{
			short[] array = new short[256];
			RuntimeHelpers.InitializeArray((global::System.Array)array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
			MuLawDecompressTable = array;
		}
	}
}
