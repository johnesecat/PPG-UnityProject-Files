using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct SteamNetworkingQuickConnectionStatus
	{
		internal ConnectionState State;

		internal int Ping;

		internal float ConnectionQualityLocal;

		internal float ConnectionQualityRemote;

		internal float OutPacketsPerSec;

		internal float OutBytesPerSec;

		internal float InPacketsPerSec;

		internal float InBytesPerSec;

		internal int SendRateBytesPerSecond;

		internal int CbPendingUnreliable;

		internal int CbPendingReliable;

		internal int CbSentUnackedReliable;

		internal long EcQueueTime;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.U4)]
		internal uint[] Reserved;
	}
}
