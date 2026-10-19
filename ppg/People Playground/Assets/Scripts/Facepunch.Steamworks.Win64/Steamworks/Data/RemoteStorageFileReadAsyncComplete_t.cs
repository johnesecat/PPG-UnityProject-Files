using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct RemoteStorageFileReadAsyncComplete_t : ICallbackData
	{
		internal ulong FileReadAsync;

		internal Result Result;

		internal uint Offset;

		internal uint Read;

		public static int _datasize = Marshal.SizeOf(typeof(RemoteStorageFileReadAsyncComplete_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.RemoteStorageFileReadAsyncComplete;
	}
}
