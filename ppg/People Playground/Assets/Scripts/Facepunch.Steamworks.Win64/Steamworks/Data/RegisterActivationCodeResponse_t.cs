using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct RegisterActivationCodeResponse_t : ICallbackData
	{
		internal RegisterActivationCodeResult Result;

		internal uint PackageRegistered;

		public static int _datasize = Marshal.SizeOf(typeof(RegisterActivationCodeResponse_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.RegisterActivationCodeResponse;
	}
}
