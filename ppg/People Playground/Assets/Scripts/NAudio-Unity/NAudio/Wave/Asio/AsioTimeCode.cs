using System.Runtime.InteropServices;

namespace NAudio.Wave.Asio
{
	[StructLayout(0, Pack = 4)]
	internal struct AsioTimeCode
	{
		public double speed;

		public Asio64Bit timeCodeSamples;

		public AsioTimeCodeFlags flags;

		[MarshalAs(23, SizeConst = 64)]
		public string future;
	}
}
