using System;
using System.Runtime.InteropServices;

namespace NAudio.MediaFoundation
{
	[ComImport]
	[InterfaceType(/*Could not decode attribute arguments.*/)]
	[Guid("70ae66f2-c809-4e4f-8915-bdcb406b7993")]
	public interface IMFSourceReader
	{
		void GetStreamSelection([In] int dwStreamIndex, [MarshalAs(2)] out bool pSelected);

		void SetStreamSelection([In] int dwStreamIndex, [In][MarshalAs(2)] bool pSelected);

		void GetNativeMediaType([In] int dwStreamIndex, [In] int dwMediaTypeIndex, out IMFMediaType ppMediaType);

		void GetCurrentMediaType([In] int dwStreamIndex, out IMFMediaType ppMediaType);

		void SetCurrentMediaType([In] int dwStreamIndex, global::System.IntPtr pdwReserved, [In] IMFMediaType pMediaType);

		void SetCurrentPosition([In][MarshalAs(43)] Guid guidTimeFormat, [In] global::System.IntPtr varPosition);

		void ReadSample([In] int dwStreamIndex, [In] int dwControlFlags, out int pdwActualStreamIndex, out MF_SOURCE_READER_FLAG pdwStreamFlags, out ulong pllTimestamp, out IMFSample ppSample);

		void Flush([In] int dwStreamIndex);

		void GetServiceForStream([In] int dwStreamIndex, [In][MarshalAs(43)] Guid guidService, [In][MarshalAs(43)] Guid riid, out global::System.IntPtr ppvObject);

		[PreserveSig]
		int GetPresentationAttribute([In] int dwStreamIndex, [In][MarshalAs(43)] Guid guidAttribute, [Out] global::System.IntPtr pvarAttribute);
	}
}
