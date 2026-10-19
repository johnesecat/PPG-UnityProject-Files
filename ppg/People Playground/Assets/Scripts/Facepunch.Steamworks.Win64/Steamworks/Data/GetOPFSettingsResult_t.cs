using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct GetOPFSettingsResult_t : ICallbackData
	{
		internal Result Result;

		internal AppId VideoAppID;

		public static int _datasize = Marshal.SizeOf(typeof(GetOPFSettingsResult_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.GetOPFSettingsResult;
	}
}
