using System;
using System.Runtime.InteropServices;

namespace NAudio.Wave.Asio
{
	[StructLayout(0, Pack = 4)]
	internal struct AsioBufferInfo
	{
		public bool isInput;

		public int channelNum;

		public global::System.IntPtr pBuffer0;

		public global::System.IntPtr pBuffer1;

		public global::System.IntPtr Buffer(int bufferIndex)
		{
			if (bufferIndex != 0)
			{
				return pBuffer1;
			}
			return pBuffer0;
		}
	}
}
