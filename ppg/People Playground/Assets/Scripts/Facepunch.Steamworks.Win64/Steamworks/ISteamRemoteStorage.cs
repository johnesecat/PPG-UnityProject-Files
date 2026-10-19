using System;
using System.Runtime.InteropServices;
using Steamworks.Data;

namespace Steamworks
{
	internal class ISteamRemoteStorage : SteamInterface
	{
		internal ISteamRemoteStorage(bool IsGameServer)
		{
			SetupInterface(IsGameServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr SteamAPI_SteamRemoteStorage_v014();

		public override IntPtr GetUserInterfacePointer()
		{
			return SteamAPI_SteamRemoteStorage_v014();
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamRemoteStorage_FileWrite")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _FileWrite(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchFile, IntPtr pvData, int cubData);

		internal bool FileWrite([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchFile, IntPtr pvData, int cubData)
		{
			return _FileWrite(Self, pchFile, pvData, cubData);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamRemoteStorage_FileRead")]
		private static extern int _FileRead(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchFile, IntPtr pvData, int cubDataToRead);

		internal int FileRead([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchFile, IntPtr pvData, int cubDataToRead)
		{
			return _FileRead(Self, pchFile, pvData, cubDataToRead);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamRemoteStorage_FileWriteAsync")]
		private static extern SteamAPICall_t _FileWriteAsync(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchFile, IntPtr pvData, uint cubData);

		internal CallResult<RemoteStorageFileWriteAsyncComplete_t> FileWriteAsync([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchFile, IntPtr pvData, uint cubData)
		{
			SteamAPICall_t call = _FileWriteAsync(Self, pchFile, pvData, cubData);
			return new CallResult<RemoteStorageFileWriteAsyncComplete_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamRemoteStorage_FileReadAsync")]
		private static extern SteamAPICall_t _FileReadAsync(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchFile, uint nOffset, uint cubToRead);

		internal CallResult<RemoteStorageFileReadAsyncComplete_t> FileReadAsync([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchFile, uint nOffset, uint cubToRead)
		{
			SteamAPICall_t call = _FileReadAsync(Self, pchFile, nOffset, cubToRead);
			return new CallResult<RemoteStorageFileReadAsyncComplete_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamRemoteStorage_FileReadAsyncComplete")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _FileReadAsyncComplete(IntPtr self, SteamAPICall_t hReadCall, IntPtr pvBuffer, uint cubToRead);

		internal bool FileReadAsyncComplete(SteamAPICall_t hReadCall, IntPtr pvBuffer, uint cubToRead)
		{
			return _FileReadAsyncComplete(Self, hReadCall, pvBuffer, cubToRead);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamRemoteStorage_FileForget")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _FileForget(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchFile);

		internal bool FileForget([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchFile)
		{
			return _FileForget(Self, pchFile);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamRemoteStorage_FileDelete")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _FileDelete(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchFile);

		internal bool FileDelete([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchFile)
		{
			return _FileDelete(Self, pchFile);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamRemoteStorage_FileShare")]
		private static extern SteamAPICall_t _FileShare(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchFile);

		internal CallResult<RemoteStorageFileShareResult_t> FileShare([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchFile)
		{
			SteamAPICall_t call = _FileShare(Self, pchFile);
			return new CallResult<RemoteStorageFileShareResult_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamRemoteStorage_SetSyncPlatforms")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetSyncPlatforms(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchFile, RemoteStoragePlatform eRemoteStoragePlatform);

		internal bool SetSyncPlatforms([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchFile, RemoteStoragePlatform eRemoteStoragePlatform)
		{
			return _SetSyncPlatforms(Self, pchFile, eRemoteStoragePlatform);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamRemoteStorage_FileWriteStreamOpen")]
		private static extern UGCFileWriteStreamHandle_t _FileWriteStreamOpen(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchFile);

		internal UGCFileWriteStreamHandle_t FileWriteStreamOpen([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchFile)
		{
			return _FileWriteStreamOpen(Self, pchFile);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamRemoteStorage_FileWriteStreamWriteChunk")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _FileWriteStreamWriteChunk(IntPtr self, UGCFileWriteStreamHandle_t writeHandle, IntPtr pvData, int cubData);

		internal bool FileWriteStreamWriteChunk(UGCFileWriteStreamHandle_t writeHandle, IntPtr pvData, int cubData)
		{
			return _FileWriteStreamWriteChunk(Self, writeHandle, pvData, cubData);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamRemoteStorage_FileWriteStreamClose")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _FileWriteStreamClose(IntPtr self, UGCFileWriteStreamHandle_t writeHandle);

		internal bool FileWriteStreamClose(UGCFileWriteStreamHandle_t writeHandle)
		{
			return _FileWriteStreamClose(Self, writeHandle);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamRemoteStorage_FileWriteStreamCancel")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _FileWriteStreamCancel(IntPtr self, UGCFileWriteStreamHandle_t writeHandle);

		internal bool FileWriteStreamCancel(UGCFileWriteStreamHandle_t writeHandle)
		{
			return _FileWriteStreamCancel(Self, writeHandle);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamRemoteStorage_FileExists")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _FileExists(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchFile);

		internal bool FileExists([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchFile)
		{
			return _FileExists(Self, pchFile);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamRemoteStorage_FilePersisted")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _FilePersisted(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchFile);

		internal bool FilePersisted([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchFile)
		{
			return _FilePersisted(Self, pchFile);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamRemoteStorage_GetFileSize")]
		private static extern int _GetFileSize(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchFile);

		internal int GetFileSize([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchFile)
		{
			return _GetFileSize(Self, pchFile);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamRemoteStorage_GetFileTimestamp")]
		private static extern long _GetFileTimestamp(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchFile);

		internal long GetFileTimestamp([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchFile)
		{
			return _GetFileTimestamp(Self, pchFile);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamRemoteStorage_GetSyncPlatforms")]
		private static extern RemoteStoragePlatform _GetSyncPlatforms(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchFile);

		internal RemoteStoragePlatform GetSyncPlatforms([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchFile)
		{
			return _GetSyncPlatforms(Self, pchFile);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamRemoteStorage_GetFileCount")]
		private static extern int _GetFileCount(IntPtr self);

		internal int GetFileCount()
		{
			return _GetFileCount(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamRemoteStorage_GetFileNameAndSize")]
		private static extern Utf8StringPointer _GetFileNameAndSize(IntPtr self, int iFile, ref int pnFileSizeInBytes);

		internal string GetFileNameAndSize(int iFile, ref int pnFileSizeInBytes)
		{
			Utf8StringPointer utf8StringPointer = _GetFileNameAndSize(Self, iFile, ref pnFileSizeInBytes);
			return utf8StringPointer;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamRemoteStorage_GetQuota")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetQuota(IntPtr self, ref ulong pnTotalBytes, ref ulong puAvailableBytes);

		internal bool GetQuota(ref ulong pnTotalBytes, ref ulong puAvailableBytes)
		{
			return _GetQuota(Self, ref pnTotalBytes, ref puAvailableBytes);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamRemoteStorage_IsCloudEnabledForAccount")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _IsCloudEnabledForAccount(IntPtr self);

		internal bool IsCloudEnabledForAccount()
		{
			return _IsCloudEnabledForAccount(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamRemoteStorage_IsCloudEnabledForApp")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _IsCloudEnabledForApp(IntPtr self);

		internal bool IsCloudEnabledForApp()
		{
			return _IsCloudEnabledForApp(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamRemoteStorage_SetCloudEnabledForApp")]
		private static extern void _SetCloudEnabledForApp(IntPtr self, [MarshalAs(UnmanagedType.U1)] bool bEnabled);

		internal void SetCloudEnabledForApp([MarshalAs(UnmanagedType.U1)] bool bEnabled)
		{
			_SetCloudEnabledForApp(Self, bEnabled);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamRemoteStorage_UGCDownload")]
		private static extern SteamAPICall_t _UGCDownload(IntPtr self, UGCHandle_t hContent, uint unPriority);

		internal CallResult<RemoteStorageDownloadUGCResult_t> UGCDownload(UGCHandle_t hContent, uint unPriority)
		{
			SteamAPICall_t call = _UGCDownload(Self, hContent, unPriority);
			return new CallResult<RemoteStorageDownloadUGCResult_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamRemoteStorage_GetUGCDownloadProgress")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetUGCDownloadProgress(IntPtr self, UGCHandle_t hContent, ref int pnBytesDownloaded, ref int pnBytesExpected);

		internal bool GetUGCDownloadProgress(UGCHandle_t hContent, ref int pnBytesDownloaded, ref int pnBytesExpected)
		{
			return _GetUGCDownloadProgress(Self, hContent, ref pnBytesDownloaded, ref pnBytesExpected);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamRemoteStorage_GetUGCDetails")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetUGCDetails(IntPtr self, UGCHandle_t hContent, ref AppId pnAppID, [In][Out] ref char[] ppchName, ref int pnFileSizeInBytes, ref SteamId pSteamIDOwner);

		internal bool GetUGCDetails(UGCHandle_t hContent, ref AppId pnAppID, [In][Out] ref char[] ppchName, ref int pnFileSizeInBytes, ref SteamId pSteamIDOwner)
		{
			return _GetUGCDetails(Self, hContent, ref pnAppID, ref ppchName, ref pnFileSizeInBytes, ref pSteamIDOwner);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamRemoteStorage_UGCRead")]
		private static extern int _UGCRead(IntPtr self, UGCHandle_t hContent, IntPtr pvData, int cubDataToRead, uint cOffset, UGCReadAction eAction);

		internal int UGCRead(UGCHandle_t hContent, IntPtr pvData, int cubDataToRead, uint cOffset, UGCReadAction eAction)
		{
			return _UGCRead(Self, hContent, pvData, cubDataToRead, cOffset, eAction);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamRemoteStorage_GetCachedUGCCount")]
		private static extern int _GetCachedUGCCount(IntPtr self);

		internal int GetCachedUGCCount()
		{
			return _GetCachedUGCCount(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamRemoteStorage_GetCachedUGCHandle")]
		private static extern UGCHandle_t _GetCachedUGCHandle(IntPtr self, int iCachedContent);

		internal UGCHandle_t GetCachedUGCHandle(int iCachedContent)
		{
			return _GetCachedUGCHandle(Self, iCachedContent);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamRemoteStorage_UGCDownloadToLocation")]
		private static extern SteamAPICall_t _UGCDownloadToLocation(IntPtr self, UGCHandle_t hContent, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchLocation, uint unPriority);

		internal CallResult<RemoteStorageDownloadUGCResult_t> UGCDownloadToLocation(UGCHandle_t hContent, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchLocation, uint unPriority)
		{
			SteamAPICall_t call = _UGCDownloadToLocation(Self, hContent, pchLocation, unPriority);
			return new CallResult<RemoteStorageDownloadUGCResult_t>(call, base.IsServer);
		}
	}
}
