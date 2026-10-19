using System;
using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct SteamParamStringArray_t
	{
		internal IntPtr Strings;

		internal int NumStrings;
	}
}
