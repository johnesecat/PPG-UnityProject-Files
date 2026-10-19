using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct CheckFileSignature_t : ICallbackData
	{
		internal CheckFileSignature CheckFileSignature;

		public static int _datasize = Marshal.SizeOf(typeof(CheckFileSignature_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.CheckFileSignature;
	}
}
