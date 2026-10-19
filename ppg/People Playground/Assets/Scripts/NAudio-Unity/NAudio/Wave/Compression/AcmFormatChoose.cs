using System;
using System.Runtime.InteropServices;

namespace NAudio.Wave.Compression
{
	[StructLayout(0, CharSet = 4, Pack = 4)]
	internal struct AcmFormatChoose
	{
		public int structureSize;

		public AcmFormatChooseStyleFlags styleFlags;

		public global::System.IntPtr ownerWindowHandle;

		public global::System.IntPtr selectedWaveFormatPointer;

		public int selectedWaveFormatByteSize;

		[MarshalAs(22)]
		public string title;

		[MarshalAs(23, SizeConst = 48)]
		public string formatTagDescription;

		[MarshalAs(23, SizeConst = 128)]
		public string formatDescription;

		[MarshalAs(22)]
		public string name;

		public int nameByteSize;

		public AcmFormatEnumFlags formatEnumFlags;

		public global::System.IntPtr waveFormatEnumPointer;

		public global::System.IntPtr instanceHandle;

		[MarshalAs(22)]
		public string templateName;

		public global::System.IntPtr customData;

		public AcmInterop.AcmFormatChooseHookProc windowCallbackFunction;
	}
}
