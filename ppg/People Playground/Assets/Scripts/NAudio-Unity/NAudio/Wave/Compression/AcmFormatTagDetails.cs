using System.Runtime.InteropServices;

namespace NAudio.Wave.Compression
{
	internal struct AcmFormatTagDetails
	{
		public int structureSize;

		public int formatTagIndex;

		public int formatTag;

		public int formatSize;

		public AcmDriverDetailsSupportFlags supportFlags;

		public int standardFormatsCount;

		[MarshalAs(23, SizeConst = 48)]
		public string formatDescription;

		public const int FormatTagDescriptionChars = 48;
	}
}
