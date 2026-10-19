using System;
using System.Runtime.InteropServices;

namespace NAudio.MediaFoundation
{
	[ComImport]
	[InterfaceType(/*Could not decode attribute arguments.*/)]
	[Guid("ad4c1b00-4bf7-422f-9175-756693d9130d")]
	public interface IMFByteStream
	{
		void GetCapabilities(ref int pdwCapabiities);

		void GetLength(ref long pqwLength);

		void SetLength(long qwLength);

		void GetCurrentPosition(ref long pqwPosition);

		void SetCurrentPosition(long qwPosition);

		void IsEndOfStream([MarshalAs(2)] ref bool pfEndOfStream);

		void Read(global::System.IntPtr pb, int cb, ref int pcbRead);

		void BeginRead(global::System.IntPtr pb, int cb, global::System.IntPtr pCallback, global::System.IntPtr punkState);

		void EndRead(global::System.IntPtr pResult, ref int pcbRead);

		void Write(global::System.IntPtr pb, int cb, ref int pcbWritten);

		void BeginWrite(global::System.IntPtr pb, int cb, global::System.IntPtr pCallback, global::System.IntPtr punkState);

		void EndWrite(global::System.IntPtr pResult, ref int pcbWritten);

		void Seek(int SeekOrigin, long llSeekOffset, int dwSeekFlags, ref long pqwCurrentPosition);

		void Flush();

		void Close();
	}
}
