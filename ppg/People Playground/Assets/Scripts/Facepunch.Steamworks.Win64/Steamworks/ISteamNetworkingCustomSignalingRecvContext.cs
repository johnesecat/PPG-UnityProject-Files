using System;
using System.Runtime.InteropServices;
using Steamworks.Data;

namespace Steamworks
{
	internal class ISteamNetworkingCustomSignalingRecvContext : SteamInterface
	{
		internal ISteamNetworkingCustomSignalingRecvContext(bool IsGameServer)
		{
			SetupInterface(IsGameServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingCustomSignalingRecvContext_OnConnectRequest")]
		private static extern IntPtr _OnConnectRequest(IntPtr self, Connection hConn, ref NetIdentity identityPeer);

		internal IntPtr OnConnectRequest(Connection hConn, ref NetIdentity identityPeer)
		{
			return _OnConnectRequest(Self, hConn, ref identityPeer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingCustomSignalingRecvContext_SendRejectionSignal")]
		private static extern void _SendRejectionSignal(IntPtr self, ref NetIdentity identityPeer, IntPtr pMsg, int cbMsg);

		internal void SendRejectionSignal(ref NetIdentity identityPeer, IntPtr pMsg, int cbMsg)
		{
			_SendRejectionSignal(Self, ref identityPeer, pMsg, cbMsg);
		}
	}
}
