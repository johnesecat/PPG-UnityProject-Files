using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct SteamTVRegion_t
	{
		internal uint UnMinX;

		internal uint UnMinY;

		internal uint UnMaxX;

		internal uint UnMaxY;
	}
}
