using System.Runtime.InteropServices;

namespace NAudio.Wave.Compression
{
	[StructLayout(0)]
	public class WaveFilter
	{
		public int StructureSize = Marshal.SizeOf(typeof(WaveFilter));

		public int FilterTag;

		public int Filter;

		[MarshalAs(30, SizeConst = 5)]
		public int[] Reserved;
	}
}
