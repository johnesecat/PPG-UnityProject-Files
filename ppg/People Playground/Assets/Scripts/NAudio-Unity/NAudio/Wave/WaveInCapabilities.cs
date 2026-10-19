using System;
using System.Runtime.InteropServices;

namespace NAudio.Wave
{
	[StructLayout(0, CharSet = 4)]
	public struct WaveInCapabilities
	{
		private short manufacturerId;

		private short productId;

		private int driverVersion;

		[MarshalAs(23, SizeConst = 32)]
		private string productName;

		private SupportedWaveFormat supportedFormats;

		private short channels;

		private short reserved;

		private Guid manufacturerGuid;

		private Guid productGuid;

		private Guid nameGuid;

		private const int MaxProductNameLength = 32;

		public int Channels => channels;

		public string ProductName => productName;

		public Guid NameGuid => nameGuid;

		public Guid ProductGuid => productGuid;

		public Guid ManufacturerGuid => manufacturerGuid;

		public bool SupportsWaveFormat(SupportedWaveFormat waveFormat)
		{
			return (supportedFormats & waveFormat) == waveFormat;
		}
	}
}
