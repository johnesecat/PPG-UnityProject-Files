using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct CreateBeaconCallback_t : ICallbackData
	{
		internal Result Result;

		internal ulong BeaconID;

		public static int _datasize = Marshal.SizeOf(typeof(CreateBeaconCallback_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.CreateBeaconCallback;
	}
}
