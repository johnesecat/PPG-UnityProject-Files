using System;
using System.Runtime.InteropServices;

namespace NAudio.MediaFoundation
{
	[ComImport]
	[Guid("E7FE2E12-661C-40DA-92F9-4F002AB67627")]
	[InterfaceType(/*Could not decode attribute arguments.*/)]
	public interface IMFReadWriteClassFactory
	{
		void CreateInstanceFromURL([In][MarshalAs(43)] Guid clsid, [In][MarshalAs(21)] string pwszURL, [In][MarshalAs(28)] IMFAttributes pAttributes, [In][MarshalAs(43)] Guid riid, [MarshalAs(28)] out object ppvObject);

		void CreateInstanceFromObject([In][MarshalAs(43)] Guid clsid, [In][MarshalAs(25)] object punkObject, [In][MarshalAs(28)] IMFAttributes pAttributes, [In][MarshalAs(43)] Guid riid, [MarshalAs(28)] out object ppvObject);
	}
}
