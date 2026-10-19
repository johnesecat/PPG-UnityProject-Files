using System.Runtime.InteropServices;

namespace NAudio.Wave.Asio
{
	[StructLayout(0, Pack = 4)]
	public struct AsioChannelInfo
	{
		public int channel;

		public bool isInput;

		public bool isActive;

		public int channelGroup;

		[MarshalAs(8)]
		public AsioSampleType type;

		[MarshalAs(23, SizeConst = 32)]
		public string name;
	}
}
