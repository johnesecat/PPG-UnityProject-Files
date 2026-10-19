using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct PersonaStateChange_t : ICallbackData
	{
		internal ulong SteamID;

		internal int ChangeFlags;

		public static int _datasize = Marshal.SizeOf(typeof(PersonaStateChange_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.PersonaStateChange;
	}
}
