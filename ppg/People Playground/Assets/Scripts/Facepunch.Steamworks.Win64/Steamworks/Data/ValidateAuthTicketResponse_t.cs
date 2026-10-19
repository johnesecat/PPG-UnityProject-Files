using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct ValidateAuthTicketResponse_t : ICallbackData
	{
		internal ulong SteamID;

		internal AuthResponse AuthSessionResponse;

		internal ulong OwnerSteamID;

		public static int _datasize = Marshal.SizeOf(typeof(ValidateAuthTicketResponse_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.ValidateAuthTicketResponse;
	}
}
