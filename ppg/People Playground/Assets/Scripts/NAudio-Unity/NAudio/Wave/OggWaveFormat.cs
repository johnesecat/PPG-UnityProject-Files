using System.Runtime.InteropServices;

namespace NAudio.Wave
{
	[StructLayout(0, Pack = 2)]
	internal class OggWaveFormat : WaveFormat
	{
		public uint dwVorbisACMVersion;

		public uint dwLibVorbisVersion;
	}
}
