using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct EndGameResultCallback_t : ICallbackData
	{
		internal Result Result;

		internal ulong UllUniqueGameID;

		public static int _datasize = Marshal.SizeOf(typeof(EndGameResultCallback_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.EndGameResultCallback;
	}
}
