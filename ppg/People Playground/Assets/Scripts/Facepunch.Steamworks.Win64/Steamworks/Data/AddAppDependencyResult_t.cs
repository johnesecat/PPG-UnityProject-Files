using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct AddAppDependencyResult_t : ICallbackData
	{
		internal Result Result;

		internal PublishedFileId PublishedFileId;

		internal AppId AppID;

		public static int _datasize = Marshal.SizeOf(typeof(AddAppDependencyResult_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.AddAppDependencyResult;
	}
}
