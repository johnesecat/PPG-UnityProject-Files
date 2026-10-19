using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct NumberOfCurrentPlayers_t : ICallbackData
	{
		internal byte Success;

		internal int CPlayers;

		public static int _datasize = Marshal.SizeOf(typeof(NumberOfCurrentPlayers_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.NumberOfCurrentPlayers;
	}
}
