using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct RemoteStorageAppSyncedServer_t : ICallbackData
	{
		internal AppId AppID;

		internal Result Result;

		internal int NumUploads;

		public static int _datasize = Marshal.SizeOf(typeof(RemoteStorageAppSyncedServer_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.RemoteStorageAppSyncedServer;
	}
}
