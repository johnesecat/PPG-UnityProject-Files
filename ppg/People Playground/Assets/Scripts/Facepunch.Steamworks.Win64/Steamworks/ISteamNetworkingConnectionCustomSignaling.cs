using System;
using System.Runtime.InteropServices;
using Steamworks.Data;

namespace Steamworks
{
	internal class ISteamNetworkingConnectionCustomSignaling : SteamInterface
	{
		internal ISteamNetworkingConnectionCustomSignaling(bool IsGameServer)
		{
			SetupInterface(IsGameServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingConnectionCustomSignaling_SendSignal")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SendSignal(IntPtr self, Connection hConn, ref ConnectionInfo info, IntPtr pMsg, int cbMsg);

		internal bool SendSignal(Connection hConn, ref ConnectionInfo info, IntPtr pMsg, int cbMsg)
		{
			return _SendSignal(Self, hConn, ref info, pMsg, cbMsg);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingConnectionCustomSignaling_Release")]
		private static extern void _Release(IntPtr self);

		internal void Release()
		{
			_Release(Self);
		}
	}
}
