using System;
using System.Runtime.InteropServices;

namespace NAudio.CoreAudioApi.Interfaces
{
	[Guid("D666063F-1587-4E43-81F1-B948E807363F")]
	[InterfaceType(/*Could not decode attribute arguments.*/)]
	internal interface IMMDevice
	{
		int Activate(ref Guid id, ClsCtx clsCtx, global::System.IntPtr activationParams, [MarshalAs(25)] out object interfacePointer);

		int OpenPropertyStore(StorageAccessMode stgmAccess, out IPropertyStore properties);

		int GetId([MarshalAs(21)] out string id);

		int GetState(out DeviceState state);
	}
}
