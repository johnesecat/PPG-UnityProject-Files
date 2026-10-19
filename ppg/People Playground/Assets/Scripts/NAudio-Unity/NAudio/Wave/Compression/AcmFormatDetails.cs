using System;
using System.Runtime.InteropServices;

namespace NAudio.Wave.Compression
{
	[StructLayout(0, Pack = 4)]
	internal struct AcmFormatDetails
	{
		public int structSize;

		public int formatIndex;

		public int formatTag;

		public AcmDriverDetailsSupportFlags supportFlags;

		public global::System.IntPtr waveFormatPointer;

		public int waveFormatByteSize;

		[MarshalAs(23, SizeConst = 128)]
		public string formatDescription;

		public const int FormatDescriptionChars = 128;
	}
}
