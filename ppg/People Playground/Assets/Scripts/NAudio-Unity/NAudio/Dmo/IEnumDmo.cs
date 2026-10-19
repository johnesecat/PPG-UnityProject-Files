using System;
using System.Runtime.InteropServices;

namespace NAudio.Dmo
{
	[Guid("2c3cd98a-2bfa-4a53-9c27-5249ba64ba0f")]
	[InterfaceType(/*Could not decode attribute arguments.*/)]
	internal interface IEnumDmo
	{
		int Next(int itemsToFetch, out Guid clsid, out global::System.IntPtr name, out int itemsFetched);

		int Skip(int itemsToSkip);

		int Reset();

		int Clone(out IEnumDmo enumPointer);
	}
}
