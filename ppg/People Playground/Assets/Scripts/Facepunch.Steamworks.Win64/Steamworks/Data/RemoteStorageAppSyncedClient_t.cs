using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct RemoteStorageAppSyncedClient_t : ICallbackData
	{
		internal AppId AppID;

		internal Result Result;

		internal int NumDownloads;

		public static int _datasize = Marshal.SizeOf(typeof(RemoteStorageAppSyncedClient_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.RemoteStorageAppSyncedClient;
	}
}
