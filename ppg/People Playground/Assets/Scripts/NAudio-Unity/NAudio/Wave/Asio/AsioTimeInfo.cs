using System.Runtime.InteropServices;

namespace NAudio.Wave.Asio
{
	[StructLayout(0, Pack = 4)]
	internal struct AsioTimeInfo
	{
		public double speed;

		public Asio64Bit systemTime;

		public Asio64Bit samplePosition;

		public double sampleRate;

		public AsioTimeInfoFlags flags;

		[MarshalAs(23, SizeConst = 12)]
		public string reserved;
	}
}
