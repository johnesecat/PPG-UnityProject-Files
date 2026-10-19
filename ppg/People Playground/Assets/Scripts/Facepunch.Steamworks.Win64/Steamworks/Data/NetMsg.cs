using System;
using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	internal struct NetMsg
	{
		internal IntPtr DataPtr;

		internal int DataSize;

		internal Connection Connection;

		internal NetIdentity Identity;

		internal long ConnectionUserData;

		internal long RecvTime;

		internal long MessageNumber;

		internal IntPtr FreeDataPtr;

		internal IntPtr ReleasePtr;

		internal int Channel;

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_SteamNetworkingMessage_t_Release")]
		internal unsafe static extern void InternalRelease(NetMsg* self);
	}
}
