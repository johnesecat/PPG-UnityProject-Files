using System;
using System.Runtime.InteropServices;

namespace NAudio.MediaFoundation
{
	[ComImport]
	[InterfaceType(/*Could not decode attribute arguments.*/)]
	[Guid("045FA593-8799-42b8-BC8D-8968C6453507")]
	public interface IMFMediaBuffer
	{
		void Lock(out global::System.IntPtr ppbBuffer, out int pcbMaxLength, out int pcbCurrentLength);

		void Unlock();

		void GetCurrentLength(out int pcbCurrentLength);

		void SetCurrentLength(int cbCurrentLength);

		void GetMaxLength(out int pcbMaxLength);
	}
}
