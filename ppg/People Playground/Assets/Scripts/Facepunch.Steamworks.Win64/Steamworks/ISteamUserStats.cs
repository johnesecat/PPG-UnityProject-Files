using System;
using System.Runtime.InteropServices;
using Steamworks.Data;

namespace Steamworks
{
	internal class ISteamUserStats : SteamInterface
	{
		internal ISteamUserStats(bool IsGameServer)
		{
			SetupInterface(IsGameServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr SteamAPI_SteamUserStats_v011();

		public override IntPtr GetUserInterfacePointer()
		{
			return SteamAPI_SteamUserStats_v011();
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_RequestCurrentStats")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _RequestCurrentStats(IntPtr self);

		internal bool RequestCurrentStats()
		{
			return _RequestCurrentStats(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetStatInt32")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetStat(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchName, ref int pData);

		internal bool GetStat([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchName, ref int pData)
		{
			return _GetStat(Self, pchName, ref pData);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetStatFloat")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetStat(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchName, ref float pData);

		internal bool GetStat([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchName, ref float pData)
		{
			return _GetStat(Self, pchName, ref pData);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_SetStatInt32")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetStat(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchName, int nData);

		internal bool SetStat([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchName, int nData)
		{
			return _SetStat(Self, pchName, nData);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_SetStatFloat")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetStat(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchName, float fData);

		internal bool SetStat([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchName, float fData)
		{
			return _SetStat(Self, pchName, fData);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_UpdateAvgRateStat")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _UpdateAvgRateStat(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchName, float flCountThisSession, double dSessionLength);

		internal bool UpdateAvgRateStat([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchName, float flCountThisSession, double dSessionLength)
		{
			return _UpdateAvgRateStat(Self, pchName, flCountThisSession, dSessionLength);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetAchievement")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetAchievement(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchName, [MarshalAs(UnmanagedType.U1)] ref bool pbAchieved);

		internal bool GetAchievement([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchName, [MarshalAs(UnmanagedType.U1)] ref bool pbAchieved)
		{
			return _GetAchievement(Self, pchName, ref pbAchieved);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_SetAchievement")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetAchievement(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchName);

		internal bool SetAchievement([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchName)
		{
			return _SetAchievement(Self, pchName);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_ClearAchievement")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _ClearAchievement(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchName);

		internal bool ClearAchievement([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchName)
		{
			return _ClearAchievement(Self, pchName);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetAchievementAndUnlockTime")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetAchievementAndUnlockTime(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchName, [MarshalAs(UnmanagedType.U1)] ref bool pbAchieved, ref uint punUnlockTime);

		internal bool GetAchievementAndUnlockTime([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchName, [MarshalAs(UnmanagedType.U1)] ref bool pbAchieved, ref uint punUnlockTime)
		{
			return _GetAchievementAndUnlockTime(Self, pchName, ref pbAchieved, ref punUnlockTime);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_StoreStats")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _StoreStats(IntPtr self);

		internal bool StoreStats()
		{
			return _StoreStats(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetAchievementIcon")]
		private static extern int _GetAchievementIcon(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchName);

		internal int GetAchievementIcon([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchName)
		{
			return _GetAchievementIcon(Self, pchName);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetAchievementDisplayAttribute")]
		private static extern Utf8StringPointer _GetAchievementDisplayAttribute(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchName, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchKey);

		internal string GetAchievementDisplayAttribute([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchName, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchKey)
		{
			Utf8StringPointer utf8StringPointer = _GetAchievementDisplayAttribute(Self, pchName, pchKey);
			return utf8StringPointer;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_IndicateAchievementProgress")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _IndicateAchievementProgress(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchName, uint nCurProgress, uint nMaxProgress);

		internal bool IndicateAchievementProgress([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchName, uint nCurProgress, uint nMaxProgress)
		{
			return _IndicateAchievementProgress(Self, pchName, nCurProgress, nMaxProgress);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetNumAchievements")]
		private static extern uint _GetNumAchievements(IntPtr self);

		internal uint GetNumAchievements()
		{
			return _GetNumAchievements(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetAchievementName")]
		private static extern Utf8StringPointer _GetAchievementName(IntPtr self, uint iAchievement);

		internal string GetAchievementName(uint iAchievement)
		{
			Utf8StringPointer utf8StringPointer = _GetAchievementName(Self, iAchievement);
			return utf8StringPointer;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_RequestUserStats")]
		private static extern SteamAPICall_t _RequestUserStats(IntPtr self, SteamId steamIDUser);

		internal CallResult<UserStatsReceived_t> RequestUserStats(SteamId steamIDUser)
		{
			SteamAPICall_t call = _RequestUserStats(Self, steamIDUser);
			return new CallResult<UserStatsReceived_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetUserStatInt32")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetUserStat(IntPtr self, SteamId steamIDUser, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchName, ref int pData);

		internal bool GetUserStat(SteamId steamIDUser, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchName, ref int pData)
		{
			return _GetUserStat(Self, steamIDUser, pchName, ref pData);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetUserStatFloat")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetUserStat(IntPtr self, SteamId steamIDUser, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchName, ref float pData);

		internal bool GetUserStat(SteamId steamIDUser, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchName, ref float pData)
		{
			return _GetUserStat(Self, steamIDUser, pchName, ref pData);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetUserAchievement")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetUserAchievement(IntPtr self, SteamId steamIDUser, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchName, [MarshalAs(UnmanagedType.U1)] ref bool pbAchieved);

		internal bool GetUserAchievement(SteamId steamIDUser, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchName, [MarshalAs(UnmanagedType.U1)] ref bool pbAchieved)
		{
			return _GetUserAchievement(Self, steamIDUser, pchName, ref pbAchieved);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetUserAchievementAndUnlockTime")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetUserAchievementAndUnlockTime(IntPtr self, SteamId steamIDUser, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchName, [MarshalAs(UnmanagedType.U1)] ref bool pbAchieved, ref uint punUnlockTime);

		internal bool GetUserAchievementAndUnlockTime(SteamId steamIDUser, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchName, [MarshalAs(UnmanagedType.U1)] ref bool pbAchieved, ref uint punUnlockTime)
		{
			return _GetUserAchievementAndUnlockTime(Self, steamIDUser, pchName, ref pbAchieved, ref punUnlockTime);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_ResetAllStats")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _ResetAllStats(IntPtr self, [MarshalAs(UnmanagedType.U1)] bool bAchievementsToo);

		internal bool ResetAllStats([MarshalAs(UnmanagedType.U1)] bool bAchievementsToo)
		{
			return _ResetAllStats(Self, bAchievementsToo);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_FindOrCreateLeaderboard")]
		private static extern SteamAPICall_t _FindOrCreateLeaderboard(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchLeaderboardName, LeaderboardSort eLeaderboardSortMethod, LeaderboardDisplay eLeaderboardDisplayType);

		internal CallResult<LeaderboardFindResult_t> FindOrCreateLeaderboard([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchLeaderboardName, LeaderboardSort eLeaderboardSortMethod, LeaderboardDisplay eLeaderboardDisplayType)
		{
			SteamAPICall_t call = _FindOrCreateLeaderboard(Self, pchLeaderboardName, eLeaderboardSortMethod, eLeaderboardDisplayType);
			return new CallResult<LeaderboardFindResult_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_FindLeaderboard")]
		private static extern SteamAPICall_t _FindLeaderboard(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchLeaderboardName);

		internal CallResult<LeaderboardFindResult_t> FindLeaderboard([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchLeaderboardName)
		{
			SteamAPICall_t call = _FindLeaderboard(Self, pchLeaderboardName);
			return new CallResult<LeaderboardFindResult_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetLeaderboardName")]
		private static extern Utf8StringPointer _GetLeaderboardName(IntPtr self, SteamLeaderboard_t hSteamLeaderboard);

		internal string GetLeaderboardName(SteamLeaderboard_t hSteamLeaderboard)
		{
			Utf8StringPointer utf8StringPointer = _GetLeaderboardName(Self, hSteamLeaderboard);
			return utf8StringPointer;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetLeaderboardEntryCount")]
		private static extern int _GetLeaderboardEntryCount(IntPtr self, SteamLeaderboard_t hSteamLeaderboard);

		internal int GetLeaderboardEntryCount(SteamLeaderboard_t hSteamLeaderboard)
		{
			return _GetLeaderboardEntryCount(Self, hSteamLeaderboard);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetLeaderboardSortMethod")]
		private static extern LeaderboardSort _GetLeaderboardSortMethod(IntPtr self, SteamLeaderboard_t hSteamLeaderboard);

		internal LeaderboardSort GetLeaderboardSortMethod(SteamLeaderboard_t hSteamLeaderboard)
		{
			return _GetLeaderboardSortMethod(Self, hSteamLeaderboard);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetLeaderboardDisplayType")]
		private static extern LeaderboardDisplay _GetLeaderboardDisplayType(IntPtr self, SteamLeaderboard_t hSteamLeaderboard);

		internal LeaderboardDisplay GetLeaderboardDisplayType(SteamLeaderboard_t hSteamLeaderboard)
		{
			return _GetLeaderboardDisplayType(Self, hSteamLeaderboard);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_DownloadLeaderboardEntries")]
		private static extern SteamAPICall_t _DownloadLeaderboardEntries(IntPtr self, SteamLeaderboard_t hSteamLeaderboard, LeaderboardDataRequest eLeaderboardDataRequest, int nRangeStart, int nRangeEnd);

		internal CallResult<LeaderboardScoresDownloaded_t> DownloadLeaderboardEntries(SteamLeaderboard_t hSteamLeaderboard, LeaderboardDataRequest eLeaderboardDataRequest, int nRangeStart, int nRangeEnd)
		{
			SteamAPICall_t call = _DownloadLeaderboardEntries(Self, hSteamLeaderboard, eLeaderboardDataRequest, nRangeStart, nRangeEnd);
			return new CallResult<LeaderboardScoresDownloaded_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_DownloadLeaderboardEntriesForUsers")]
		private static extern SteamAPICall_t _DownloadLeaderboardEntriesForUsers(IntPtr self, SteamLeaderboard_t hSteamLeaderboard, [In][Out] SteamId[] prgUsers, int cUsers);

		internal CallResult<LeaderboardScoresDownloaded_t> DownloadLeaderboardEntriesForUsers(SteamLeaderboard_t hSteamLeaderboard, [In][Out] SteamId[] prgUsers, int cUsers)
		{
			SteamAPICall_t call = _DownloadLeaderboardEntriesForUsers(Self, hSteamLeaderboard, prgUsers, cUsers);
			return new CallResult<LeaderboardScoresDownloaded_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetDownloadedLeaderboardEntry")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetDownloadedLeaderboardEntry(IntPtr self, SteamLeaderboardEntries_t hSteamLeaderboardEntries, int index, ref LeaderboardEntry_t pLeaderboardEntry, [In][Out] int[] pDetails, int cDetailsMax);

		internal bool GetDownloadedLeaderboardEntry(SteamLeaderboardEntries_t hSteamLeaderboardEntries, int index, ref LeaderboardEntry_t pLeaderboardEntry, [In][Out] int[] pDetails, int cDetailsMax)
		{
			return _GetDownloadedLeaderboardEntry(Self, hSteamLeaderboardEntries, index, ref pLeaderboardEntry, pDetails, cDetailsMax);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_UploadLeaderboardScore")]
		private static extern SteamAPICall_t _UploadLeaderboardScore(IntPtr self, SteamLeaderboard_t hSteamLeaderboard, LeaderboardUploadScoreMethod eLeaderboardUploadScoreMethod, int nScore, [In][Out] int[] pScoreDetails, int cScoreDetailsCount);

		internal CallResult<LeaderboardScoreUploaded_t> UploadLeaderboardScore(SteamLeaderboard_t hSteamLeaderboard, LeaderboardUploadScoreMethod eLeaderboardUploadScoreMethod, int nScore, [In][Out] int[] pScoreDetails, int cScoreDetailsCount)
		{
			SteamAPICall_t call = _UploadLeaderboardScore(Self, hSteamLeaderboard, eLeaderboardUploadScoreMethod, nScore, pScoreDetails, cScoreDetailsCount);
			return new CallResult<LeaderboardScoreUploaded_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_AttachLeaderboardUGC")]
		private static extern SteamAPICall_t _AttachLeaderboardUGC(IntPtr self, SteamLeaderboard_t hSteamLeaderboard, UGCHandle_t hUGC);

		internal CallResult<LeaderboardUGCSet_t> AttachLeaderboardUGC(SteamLeaderboard_t hSteamLeaderboard, UGCHandle_t hUGC)
		{
			SteamAPICall_t call = _AttachLeaderboardUGC(Self, hSteamLeaderboard, hUGC);
			return new CallResult<LeaderboardUGCSet_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetNumberOfCurrentPlayers")]
		private static extern SteamAPICall_t _GetNumberOfCurrentPlayers(IntPtr self);

		internal CallResult<NumberOfCurrentPlayers_t> GetNumberOfCurrentPlayers()
		{
			SteamAPICall_t call = _GetNumberOfCurrentPlayers(Self);
			return new CallResult<NumberOfCurrentPlayers_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_RequestGlobalAchievementPercentages")]
		private static extern SteamAPICall_t _RequestGlobalAchievementPercentages(IntPtr self);

		internal CallResult<GlobalAchievementPercentagesReady_t> RequestGlobalAchievementPercentages()
		{
			SteamAPICall_t call = _RequestGlobalAchievementPercentages(Self);
			return new CallResult<GlobalAchievementPercentagesReady_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetMostAchievedAchievementInfo")]
		private static extern int _GetMostAchievedAchievementInfo(IntPtr self, IntPtr pchName, uint unNameBufLen, ref float pflPercent, [MarshalAs(UnmanagedType.U1)] ref bool pbAchieved);

		internal int GetMostAchievedAchievementInfo(out string pchName, ref float pflPercent, [MarshalAs(UnmanagedType.U1)] ref bool pbAchieved)
		{
			IntPtr intPtr = Helpers.TakeMemory();
			int result = _GetMostAchievedAchievementInfo(Self, intPtr, 32768u, ref pflPercent, ref pbAchieved);
			pchName = Helpers.MemoryToString(intPtr);
			return result;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetNextMostAchievedAchievementInfo")]
		private static extern int _GetNextMostAchievedAchievementInfo(IntPtr self, int iIteratorPrevious, IntPtr pchName, uint unNameBufLen, ref float pflPercent, [MarshalAs(UnmanagedType.U1)] ref bool pbAchieved);

		internal int GetNextMostAchievedAchievementInfo(int iIteratorPrevious, out string pchName, ref float pflPercent, [MarshalAs(UnmanagedType.U1)] ref bool pbAchieved)
		{
			IntPtr intPtr = Helpers.TakeMemory();
			int result = _GetNextMostAchievedAchievementInfo(Self, iIteratorPrevious, intPtr, 32768u, ref pflPercent, ref pbAchieved);
			pchName = Helpers.MemoryToString(intPtr);
			return result;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetAchievementAchievedPercent")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetAchievementAchievedPercent(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchName, ref float pflPercent);

		internal bool GetAchievementAchievedPercent([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchName, ref float pflPercent)
		{
			return _GetAchievementAchievedPercent(Self, pchName, ref pflPercent);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_RequestGlobalStats")]
		private static extern SteamAPICall_t _RequestGlobalStats(IntPtr self, int nHistoryDays);

		internal CallResult<GlobalStatsReceived_t> RequestGlobalStats(int nHistoryDays)
		{
			SteamAPICall_t call = _RequestGlobalStats(Self, nHistoryDays);
			return new CallResult<GlobalStatsReceived_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetGlobalStatInt64")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetGlobalStat(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchStatName, ref long pData);

		internal bool GetGlobalStat([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchStatName, ref long pData)
		{
			return _GetGlobalStat(Self, pchStatName, ref pData);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetGlobalStatDouble")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetGlobalStat(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchStatName, ref double pData);

		internal bool GetGlobalStat([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchStatName, ref double pData)
		{
			return _GetGlobalStat(Self, pchStatName, ref pData);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetGlobalStatHistoryInt64")]
		private static extern int _GetGlobalStatHistory(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchStatName, [In][Out] long[] pData, uint cubData);

		internal int GetGlobalStatHistory([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchStatName, [In][Out] long[] pData, uint cubData)
		{
			return _GetGlobalStatHistory(Self, pchStatName, pData, cubData);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUserStats_GetGlobalStatHistoryDouble")]
		private static extern int _GetGlobalStatHistory(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchStatName, [In][Out] double[] pData, uint cubData);

		internal int GetGlobalStatHistory([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchStatName, [In][Out] double[] pData, uint cubData)
		{
			return _GetGlobalStatHistory(Self, pchStatName, pData, cubData);
		}
	}
}
