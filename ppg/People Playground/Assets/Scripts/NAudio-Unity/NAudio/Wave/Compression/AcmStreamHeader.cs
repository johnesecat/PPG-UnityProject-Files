using System;
using System.Runtime.InteropServices;

namespace NAudio.Wave.Compression
{
	internal class AcmStreamHeader : global::System.IDisposable
	{
		private AcmStreamHeaderStruct streamHeader;

		private byte[] sourceBuffer;

		private GCHandle hSourceBuffer;

		private byte[] destBuffer;

		private GCHandle hDestBuffer;

		private global::System.IntPtr streamHandle;

		private bool firstTime;

		private bool disposed;

		public byte[] SourceBuffer => sourceBuffer;

		public byte[] DestBuffer => destBuffer;

		public AcmStreamHeader(global::System.IntPtr streamHandle, int sourceBufferLength, int destBufferLength)
		{
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			streamHeader = new AcmStreamHeaderStruct();
			sourceBuffer = new byte[sourceBufferLength];
			hSourceBuffer = GCHandle.Alloc((object)sourceBuffer, (GCHandleType)3);
			destBuffer = new byte[destBufferLength];
			hDestBuffer = GCHandle.Alloc((object)destBuffer, (GCHandleType)3);
			this.streamHandle = streamHandle;
			firstTime = true;
		}

		private void Prepare()
		{
			streamHeader.cbStruct = Marshal.SizeOf((object)streamHeader);
			streamHeader.sourceBufferLength = sourceBuffer.Length;
			streamHeader.sourceBufferPointer = ((GCHandle)(ref hSourceBuffer)).AddrOfPinnedObject();
			streamHeader.destBufferLength = destBuffer.Length;
			streamHeader.destBufferPointer = ((GCHandle)(ref hDestBuffer)).AddrOfPinnedObject();
			MmException.Try(AcmInterop.acmStreamPrepareHeader(streamHandle, streamHeader, 0), "acmStreamPrepareHeader");
		}

		private void Unprepare()
		{
			streamHeader.sourceBufferLength = sourceBuffer.Length;
			streamHeader.sourceBufferPointer = ((GCHandle)(ref hSourceBuffer)).AddrOfPinnedObject();
			streamHeader.destBufferLength = destBuffer.Length;
			streamHeader.destBufferPointer = ((GCHandle)(ref hDestBuffer)).AddrOfPinnedObject();
			MmResult mmResult = AcmInterop.acmStreamUnprepareHeader(streamHandle, streamHeader, 0);
			if (mmResult != 0)
			{
				throw new MmException(mmResult, "acmStreamUnprepareHeader");
			}
		}

		public void Reposition()
		{
			firstTime = true;
		}

		public int Convert(int bytesToConvert, out int sourceBytesConverted)
		{
			Prepare();
			try
			{
				streamHeader.sourceBufferLength = bytesToConvert;
				streamHeader.sourceBufferLengthUsed = bytesToConvert;
				AcmStreamConvertFlags streamConvertFlags = (firstTime ? ((AcmStreamConvertFlags)20) : AcmStreamConvertFlags.BlockAlign);
				MmException.Try(AcmInterop.acmStreamConvert(streamHandle, streamHeader, streamConvertFlags), "acmStreamConvert");
				firstTime = false;
				sourceBytesConverted = streamHeader.sourceBufferLengthUsed;
			}
			finally
			{
				Unprepare();
			}
			return streamHeader.destBufferLengthUsed;
		}

		public void Dispose()
		{
			GC.SuppressFinalize((object)this);
			Dispose(disposing: true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				sourceBuffer = null;
				destBuffer = null;
				((GCHandle)(ref hSourceBuffer)).Free();
				((GCHandle)(ref hDestBuffer)).Free();
			}
			disposed = true;
		}

		~AcmStreamHeader()
		{
			Dispose(disposing: false);
		}
	}
}
