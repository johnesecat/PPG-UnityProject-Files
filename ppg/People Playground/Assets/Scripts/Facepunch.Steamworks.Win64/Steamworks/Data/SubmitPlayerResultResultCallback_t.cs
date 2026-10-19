using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct SubmitPlayerResultResultCallback_t : ICallbackData
	{
		internal Result Result;

		internal ulong UllUniqueGameID;

		internal ulong SteamIDPlayer;

		public static int _datasize = Marshal.SizeOf(typeof(SubmitPlayerResultResultCallback_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.SubmitPlayerResultResultCallback;
	}
}
