using System.Runtime.InteropServices;

namespace NAudio.Midi
{
	[StructLayout(0, CharSet = 4)]
	public struct MidiInCapabilities
	{
		private ushort manufacturerId;

		private ushort productId;

		private uint driverVersion;

		[MarshalAs(23, SizeConst = 32)]
		private string productName;

		private int support;

		private const int MaxProductNameLength = 32;

		public Manufacturers Manufacturer => (Manufacturers)manufacturerId;

		public int ProductId => productId;

		public string ProductName => productName;
	}
}
