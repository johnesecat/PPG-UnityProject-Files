using System;
using System.Runtime.InteropServices;
using Steamworks.Data;

namespace Steamworks
{
	internal class ISteamTV : SteamInterface
	{
		internal ISteamTV(bool IsGameServer)
		{
			SetupInterface(IsGameServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr SteamAPI_SteamTV_v001();

		public override IntPtr GetUserInterfacePointer()
		{
			return SteamAPI_SteamTV_v001();
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamTV_IsBroadcasting")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _IsBroadcasting(IntPtr self, ref int pnNumViewers);

		internal bool IsBroadcasting(ref int pnNumViewers)
		{
			return _IsBroadcasting(Self, ref pnNumViewers);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamTV_AddBroadcastGameData")]
		private static extern void _AddBroadcastGameData(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchKey, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchValue);

		internal void AddBroadcastGameData([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchKey, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchValue)
		{
			_AddBroadcastGameData(Self, pchKey, pchValue);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamTV_RemoveBroadcastGameData")]
		private static extern void _RemoveBroadcastGameData(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchKey);

		internal void RemoveBroadcastGameData([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchKey)
		{
			_RemoveBroadcastGameData(Self, pchKey);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamTV_AddTimelineMarker")]
		private static extern void _AddTimelineMarker(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchTemplateName, [MarshalAs(UnmanagedType.U1)] bool bPersistent, byte nColorR, byte nColorG, byte nColorB);

		internal void AddTimelineMarker([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchTemplateName, [MarshalAs(UnmanagedType.U1)] bool bPersistent, byte nColorR, byte nColorG, byte nColorB)
		{
			_AddTimelineMarker(Self, pchTemplateName, bPersistent, nColorR, nColorG, nColorB);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamTV_RemoveTimelineMarker")]
		private static extern void _RemoveTimelineMarker(IntPtr self);

		internal void RemoveTimelineMarker()
		{
			_RemoveTimelineMarker(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamTV_AddRegion")]
		private static extern uint _AddRegion(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchElementName, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchTimelineDataSection, ref SteamTVRegion_t pSteamTVRegion, SteamTVRegionBehavior eSteamTVRegionBehavior);

		internal uint AddRegion([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchElementName, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchTimelineDataSection, ref SteamTVRegion_t pSteamTVRegion, SteamTVRegionBehavior eSteamTVRegionBehavior)
		{
			return _AddRegion(Self, pchElementName, pchTimelineDataSection, ref pSteamTVRegion, eSteamTVRegionBehavior);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamTV_RemoveRegion")]
		private static extern void _RemoveRegion(IntPtr self, uint unRegionHandle);

		internal void RemoveRegion(uint unRegionHandle)
		{
			_RemoveRegion(Self, unRegionHandle);
		}
	}
}
