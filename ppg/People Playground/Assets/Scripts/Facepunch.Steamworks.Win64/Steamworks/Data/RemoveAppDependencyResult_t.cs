using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct RemoveAppDependencyResult_t : ICallbackData
	{
		internal Result Result;

		internal PublishedFileId PublishedFileId;

		internal AppId AppID;

		public static int _datasize = Marshal.SizeOf(typeof(RemoveAppDependencyResult_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.RemoveAppDependencyResult;
	}
}
