using System;
using System.Runtime.InteropServices;

namespace NAudio.Wave
{
	[StructLayout(0)]
	internal class WaveHeader
	{
		public global::System.IntPtr dataBuffer;

		public int bufferLength;

		public int bytesRecorded;

		public global::System.IntPtr userData;

		public WaveHeaderFlags flags;

		public int loops;

		public global::System.IntPtr next;

		public global::System.IntPtr reserved;
	}
}
