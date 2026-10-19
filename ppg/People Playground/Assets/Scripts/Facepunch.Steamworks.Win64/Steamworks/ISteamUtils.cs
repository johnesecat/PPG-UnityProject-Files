using System;
using System.Runtime.InteropServices;
using Steamworks.Data;

namespace Steamworks
{
	internal class ISteamUtils : SteamInterface
	{
		internal ISteamUtils(bool IsGameServer)
		{
			SetupInterface(IsGameServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr SteamAPI_SteamUtils_v009();

		public override IntPtr GetUserInterfacePointer()
		{
			return SteamAPI_SteamUtils_v009();
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr SteamAPI_SteamGameServerUtils_v009();

		public override IntPtr GetServerInterfacePointer()
		{
			return SteamAPI_SteamGameServerUtils_v009();
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUtils_GetSecondsSinceAppActive")]
		private static extern uint _GetSecondsSinceAppActive(IntPtr self);

		internal uint GetSecondsSinceAppActive()
		{
			return _GetSecondsSinceAppActive(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUtils_GetSecondsSinceComputerActive")]
		private static extern uint _GetSecondsSinceComputerActive(IntPtr self);

		internal uint GetSecondsSinceComputerActive()
		{
			return _GetSecondsSinceComputerActive(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUtils_GetConnectedUniverse")]
		private static extern Universe _GetConnectedUniverse(IntPtr self);

		internal Universe GetConnectedUniverse()
		{
			return _GetConnectedUniverse(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUtils_GetServerRealTime")]
		private static extern uint _GetServerRealTime(IntPtr self);

		internal uint GetServerRealTime()
		{
			return _GetServerRealTime(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUtils_GetIPCountry")]
		private static extern Utf8StringPointer _GetIPCountry(IntPtr self);

		internal string GetIPCountry()
		{
			Utf8StringPointer utf8StringPointer = _GetIPCountry(Self);
			return utf8StringPointer;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUtils_GetImageSize")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetImageSize(IntPtr self, int iImage, ref uint pnWidth, ref uint pnHeight);

		internal bool GetImageSize(int iImage, ref uint pnWidth, ref uint pnHeight)
		{
			return _GetImageSize(Self, iImage, ref pnWidth, ref pnHeight);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUtils_GetImageRGBA")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetImageRGBA(IntPtr self, int iImage, [In][Out] byte[] pubDest, int nDestBufferSize);

		internal bool GetImageRGBA(int iImage, [In][Out] byte[] pubDest, int nDestBufferSize)
		{
			return _GetImageRGBA(Self, iImage, pubDest, nDestBufferSize);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUtils_GetCSERIPPort")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetCSERIPPort(IntPtr self, ref uint unIP, ref ushort usPort);

		internal bool GetCSERIPPort(ref uint unIP, ref ushort usPort)
		{
			return _GetCSERIPPort(Self, ref unIP, ref usPort);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUtils_GetCurrentBatteryPower")]
		private static extern byte _GetCurrentBatteryPower(IntPtr self);

		internal byte GetCurrentBatteryPower()
		{
			return _GetCurrentBatteryPower(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUtils_GetAppID")]
		private static extern uint _GetAppID(IntPtr self);

		internal uint GetAppID()
		{
			return _GetAppID(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUtils_SetOverlayNotificationPosition")]
		private static extern void _SetOverlayNotificationPosition(IntPtr self, NotificationPosition eNotificationPosition);

		internal void SetOverlayNotificationPosition(NotificationPosition eNotificationPosition)
		{
			_SetOverlayNotificationPosition(Self, eNotificationPosition);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUtils_IsAPICallCompleted")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _IsAPICallCompleted(IntPtr self, SteamAPICall_t hSteamAPICall, [MarshalAs(UnmanagedType.U1)] ref bool pbFailed);

		internal bool IsAPICallCompleted(SteamAPICall_t hSteamAPICall, [MarshalAs(UnmanagedType.U1)] ref bool pbFailed)
		{
			return _IsAPICallCompleted(Self, hSteamAPICall, ref pbFailed);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUtils_GetAPICallFailureReason")]
		private static extern SteamAPICallFailure _GetAPICallFailureReason(IntPtr self, SteamAPICall_t hSteamAPICall);

		internal SteamAPICallFailure GetAPICallFailureReason(SteamAPICall_t hSteamAPICall)
		{
			return _GetAPICallFailureReason(Self, hSteamAPICall);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUtils_GetAPICallResult")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetAPICallResult(IntPtr self, SteamAPICall_t hSteamAPICall, IntPtr pCallback, int cubCallback, int iCallbackExpected, [MarshalAs(UnmanagedType.U1)] ref bool pbFailed);

		internal bool GetAPICallResult(SteamAPICall_t hSteamAPICall, IntPtr pCallback, int cubCallback, int iCallbackExpected, [MarshalAs(UnmanagedType.U1)] ref bool pbFailed)
		{
			return _GetAPICallResult(Self, hSteamAPICall, pCallback, cubCallback, iCallbackExpected, ref pbFailed);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUtils_GetIPCCallCount")]
		private static extern uint _GetIPCCallCount(IntPtr self);

		internal uint GetIPCCallCount()
		{
			return _GetIPCCallCount(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUtils_SetWarningMessageHook")]
		private static extern void _SetWarningMessageHook(IntPtr self, IntPtr pFunction);

		internal void SetWarningMessageHook(IntPtr pFunction)
		{
			_SetWarningMessageHook(Self, pFunction);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUtils_IsOverlayEnabled")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _IsOverlayEnabled(IntPtr self);

		internal bool IsOverlayEnabled()
		{
			return _IsOverlayEnabled(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUtils_BOverlayNeedsPresent")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _BOverlayNeedsPresent(IntPtr self);

		internal bool BOverlayNeedsPresent()
		{
			return _BOverlayNeedsPresent(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUtils_CheckFileSignature")]
		private static extern SteamAPICall_t _CheckFileSignature(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string szFileName);

		internal CallResult<CheckFileSignature_t> CheckFileSignature([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string szFileName)
		{
			SteamAPICall_t call = _CheckFileSignature(Self, szFileName);
			return new CallResult<CheckFileSignature_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUtils_ShowGamepadTextInput")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _ShowGamepadTextInput(IntPtr self, GamepadTextInputMode eInputMode, GamepadTextInputLineMode eLineInputMode, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchDescription, uint unCharMax, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchExistingText);

		internal bool ShowGamepadTextInput(GamepadTextInputMode eInputMode, GamepadTextInputLineMode eLineInputMode, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchDescription, uint unCharMax, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchExistingText)
		{
			return _ShowGamepadTextInput(Self, eInputMode, eLineInputMode, pchDescription, unCharMax, pchExistingText);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUtils_GetEnteredGamepadTextLength")]
		private static extern uint _GetEnteredGamepadTextLength(IntPtr self);

		internal uint GetEnteredGamepadTextLength()
		{
			return _GetEnteredGamepadTextLength(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUtils_GetEnteredGamepadTextInput")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetEnteredGamepadTextInput(IntPtr self, IntPtr pchText, uint cchText);

		internal bool GetEnteredGamepadTextInput(out string pchText)
		{
			IntPtr intPtr = Helpers.TakeMemory();
			bool result = _GetEnteredGamepadTextInput(Self, intPtr, 32768u);
			pchText = Helpers.MemoryToString(intPtr);
			return result;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUtils_GetSteamUILanguage")]
		private static extern Utf8StringPointer _GetSteamUILanguage(IntPtr self);

		internal string GetSteamUILanguage()
		{
			Utf8StringPointer utf8StringPointer = _GetSteamUILanguage(Self);
			return utf8StringPointer;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUtils_IsSteamRunningInVR")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _IsSteamRunningInVR(IntPtr self);

		internal bool IsSteamRunningInVR()
		{
			return _IsSteamRunningInVR(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUtils_SetOverlayNotificationInset")]
		private static extern void _SetOverlayNotificationInset(IntPtr self, int nHorizontalInset, int nVerticalInset);

		internal void SetOverlayNotificationInset(int nHorizontalInset, int nVerticalInset)
		{
			_SetOverlayNotificationInset(Self, nHorizontalInset, nVerticalInset);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUtils_IsSteamInBigPictureMode")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _IsSteamInBigPictureMode(IntPtr self);

		internal bool IsSteamInBigPictureMode()
		{
			return _IsSteamInBigPictureMode(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUtils_StartVRDashboard")]
		private static extern void _StartVRDashboard(IntPtr self);

		internal void StartVRDashboard()
		{
			_StartVRDashboard(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUtils_IsVRHeadsetStreamingEnabled")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _IsVRHeadsetStreamingEnabled(IntPtr self);

		internal bool IsVRHeadsetStreamingEnabled()
		{
			return _IsVRHeadsetStreamingEnabled(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUtils_SetVRHeadsetStreamingEnabled")]
		private static extern void _SetVRHeadsetStreamingEnabled(IntPtr self, [MarshalAs(UnmanagedType.U1)] bool bEnabled);

		internal void SetVRHeadsetStreamingEnabled([MarshalAs(UnmanagedType.U1)] bool bEnabled)
		{
			_SetVRHeadsetStreamingEnabled(Self, bEnabled);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUtils_IsSteamChinaLauncher")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _IsSteamChinaLauncher(IntPtr self);

		internal bool IsSteamChinaLauncher()
		{
			return _IsSteamChinaLauncher(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUtils_InitFilterText")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _InitFilterText(IntPtr self);

		internal bool InitFilterText()
		{
			return _InitFilterText(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUtils_FilterText")]
		private static extern int _FilterText(IntPtr self, IntPtr pchOutFilteredText, uint nByteSizeOutFilteredText, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchInputMessage, [MarshalAs(UnmanagedType.U1)] bool bLegalOnly);

		internal int FilterText(out string pchOutFilteredText, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchInputMessage, [MarshalAs(UnmanagedType.U1)] bool bLegalOnly)
		{
			IntPtr intPtr = Helpers.TakeMemory();
			int result = _FilterText(Self, intPtr, 32768u, pchInputMessage, bLegalOnly);
			pchOutFilteredText = Helpers.MemoryToString(intPtr);
			return result;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUtils_GetIPv6ConnectivityState")]
		private static extern SteamIPv6ConnectivityState _GetIPv6ConnectivityState(IntPtr self, SteamIPv6ConnectivityProtocol eProtocol);

		internal SteamIPv6ConnectivityState GetIPv6ConnectivityState(SteamIPv6ConnectivityProtocol eProtocol)
		{
			return _GetIPv6ConnectivityState(Self, eProtocol);
		}
	}
}
