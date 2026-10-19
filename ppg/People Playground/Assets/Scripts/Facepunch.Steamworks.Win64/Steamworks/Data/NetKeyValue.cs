using System;
using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Explicit, Pack = 8)]
	internal struct NetKeyValue
	{
		[FieldOffset(0)]
		internal NetConfig Value;

		[FieldOffset(4)]
		internal NetConfigType DataType;

		[FieldOffset(8)]
		internal long Int64Value;

		[FieldOffset(8)]
		internal int Int32Value;

		[FieldOffset(8)]
		internal float FloatValue;

		[FieldOffset(8)]
		internal IntPtr PointerValue;
	}
}
