using System;
using System.Runtime.InteropServices;

namespace NAudio.Wave.Compression
{
	[StructLayout(0, Size = 128)]
	internal class AcmStreamHeaderStruct
	{
		public int cbStruct;

		public AcmStreamHeaderStatusFlags fdwStatus;

		public global::System.IntPtr userData;

		public global::System.IntPtr sourceBufferPointer;

		public int sourceBufferLength;

		public int sourceBufferLengthUsed;

		public global::System.IntPtr sourceUserData;

		public global::System.IntPtr destBufferPointer;

		public int destBufferLength;

		public int destBufferLengthUsed;

		public global::System.IntPtr destUserData;
	}
}
