using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using NAudio.CoreAudioApi.Interfaces;
using NAudio.MediaFoundation;
using NAudio.Utils;

namespace NAudio.Wave
{
	public class MediaFoundationReader : WaveStream
	{
		public class MediaFoundationReaderSettings
		{
			public bool RequestFloatOutput
			{
				[CompilerGenerated]
				get
				{
					return <RequestFloatOutput>k__BackingField;
				}
				[CompilerGenerated]
				set
				{
					<RequestFloatOutput>k__BackingField = value;
				}
			}

			public bool SingleReaderObject
			{
				[CompilerGenerated]
				get
				{
					return <SingleReaderObject>k__BackingField;
				}
				[CompilerGenerated]
				set
				{
					<SingleReaderObject>k__BackingField = value;
				}
			}

			public bool RepositionInRead
			{
				[CompilerGenerated]
				get
				{
					return <RepositionInRead>k__BackingField;
				}
				[CompilerGenerated]
				set
				{
					<RepositionInRead>k__BackingField = value;
				}
			}

			public MediaFoundationReaderSettings()
			{
				RepositionInRead = true;
			}
		}

		private WaveFormat waveFormat;

		private long length;

		private MediaFoundationReaderSettings settings;

		private readonly string file;

		private IMFSourceReader pReader;

		private long position;

		private byte[] decoderOutputBuffer;

		private int decoderOutputOffset;

		private int decoderOutputCount;

		private long repositionTo = -1L;

		public override WaveFormat WaveFormat => waveFormat;

		public override long Length => length;

		public override long Position
		{
			get
			{
				return position;
			}
			set
			{
				//IL_000f: Unknown result type (might be due to invalid IL or missing references)
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value", "Position cannot be less than 0");
				}
				if (settings.RepositionInRead)
				{
					repositionTo = value;
					position = value;
				}
				else
				{
					Reposition(value);
				}
			}
		}

		public event EventHandler WaveFormatChanged
		{
			[CompilerGenerated]
			add
			{
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Expected O, but got Unknown
				EventHandler val = this.WaveFormatChanged;
				EventHandler val2;
				do
				{
					val2 = val;
					EventHandler val3 = (EventHandler)global::System.Delegate.Combine((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
					val = Interlocked.CompareExchange<EventHandler>(ref global::System.Runtime.CompilerServices.Unsafe.As<EventHandler, EventHandler>(ref this.WaveFormatChanged), val3, val2);
				}
				while (val != val2);
			}
			[CompilerGenerated]
			remove
			{
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Expected O, but got Unknown
				EventHandler val = this.WaveFormatChanged;
				EventHandler val2;
				do
				{
					val2 = val;
					EventHandler val3 = (EventHandler)global::System.Delegate.Remove((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
					val = Interlocked.CompareExchange<EventHandler>(ref global::System.Runtime.CompilerServices.Unsafe.As<EventHandler, EventHandler>(ref this.WaveFormatChanged), val3, val2);
				}
				while (val != val2);
			}
		}

		protected MediaFoundationReader()
		{
		}

		public MediaFoundationReader(string file)
			: this(file, null)
		{
		}

		public MediaFoundationReader(string file, MediaFoundationReaderSettings settings)
		{
			this.file = file;
			Init(settings);
		}

		protected void Init(MediaFoundationReaderSettings initialSettings)
		{
			MediaFoundationApi.Startup();
			settings = initialSettings ?? new MediaFoundationReaderSettings();
			IMFSourceReader iMFSourceReader = CreateReader(settings);
			waveFormat = GetCurrentWaveFormat(iMFSourceReader);
			iMFSourceReader.SetStreamSelection(-3, pSelected: true);
			length = GetLength(iMFSourceReader);
			if (settings.SingleReaderObject)
			{
				pReader = iMFSourceReader;
			}
			else
			{
				Marshal.ReleaseComObject((object)iMFSourceReader);
			}
		}

		private WaveFormat GetCurrentWaveFormat(IMFSourceReader reader)
		{
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0034: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_006a: Unknown result type (might be due to invalid IL or missing references)
			reader.GetCurrentMediaType(-3, out var ppMediaType);
			MediaType mediaType = new MediaType(ppMediaType);
			_ = mediaType.MajorType;
			Guid subType = mediaType.SubType;
			int channelCount = mediaType.ChannelCount;
			int bitsPerSample = mediaType.BitsPerSample;
			int sampleRate = mediaType.SampleRate;
			if (subType == AudioSubtypes.MFAudioFormat_PCM)
			{
				return new WaveFormat(sampleRate, bitsPerSample, channelCount);
			}
			if (subType == AudioSubtypes.MFAudioFormat_Float)
			{
				return WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channelCount);
			}
			string text = FieldDescriptionHelper.Describe(typeof(AudioSubtypes), subType);
			throw new InvalidDataException($"Unsupported audio sub Type {text}");
		}

		private static MediaType GetCurrentMediaType(IMFSourceReader reader)
		{
			reader.GetCurrentMediaType(-3, out var ppMediaType);
			return new MediaType(ppMediaType);
		}

		protected virtual IMFSourceReader CreateReader(MediaFoundationReaderSettings settings)
		{
			//IL_0027: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
			MediaFoundationInterop.MFCreateSourceReaderFromURL(file, null, out var ppSourceReader);
			ppSourceReader.SetStreamSelection(-2, pSelected: false);
			ppSourceReader.SetStreamSelection(-3, pSelected: true);
			MediaType mediaType = new MediaType();
			mediaType.MajorType = MediaTypes.MFMediaType_Audio;
			mediaType.SubType = (settings.RequestFloatOutput ? AudioSubtypes.MFAudioFormat_Float : AudioSubtypes.MFAudioFormat_PCM);
			MediaType currentMediaType = GetCurrentMediaType(ppSourceReader);
			mediaType.ChannelCount = currentMediaType.ChannelCount;
			mediaType.SampleRate = currentMediaType.SampleRate;
			try
			{
				ppSourceReader.SetCurrentMediaType(-3, global::System.IntPtr.Zero, mediaType.MediaFoundationObject);
			}
			catch (object obj) when (((Func<bool>)delegate
			{
				// Could not convert BlockContainer to single expression
				object obj2 = ((obj is COMException) ? obj : null);
				return obj2 != null && ((COMException)obj2).GetHResult() == -1072875852;
			}).Invoke())
			{
				if (!(currentMediaType.SubType == AudioSubtypes.MFAudioFormat_AAC) || currentMediaType.ChannelCount != 1)
				{
					throw;
				}
				mediaType.SampleRate = (currentMediaType.SampleRate *= 2);
				mediaType.ChannelCount = (currentMediaType.ChannelCount *= 2);
				ppSourceReader.SetCurrentMediaType(-3, global::System.IntPtr.Zero, mediaType.MediaFoundationObject);
			}
			Marshal.ReleaseComObject((object)currentMediaType.MediaFoundationObject);
			return ppSourceReader;
		}

		private long GetLength(IMFSourceReader reader)
		{
			//IL_000d: Unknown result type (might be due to invalid IL or missing references)
			global::System.IntPtr intPtr = Marshal.AllocHGlobal(MarshalHelpers.SizeOf<PropVariant>());
			try
			{
				int presentationAttribute = reader.GetPresentationAttribute(-1, MediaFoundationAttributes.MF_PD_DURATION, intPtr);
				switch (presentationAttribute)
				{
				case -1072875802:
					return 0L;
				default:
					Marshal.ThrowExceptionForHR(presentationAttribute);
					break;
				case 0:
					break;
				}
				return (long)MarshalHelpers.PtrToStructure<PropVariant>(intPtr).Value * waveFormat.AverageBytesPerSecond / 10000000;
			}
			finally
			{
				PropVariant.Clear(intPtr);
				Marshal.FreeHGlobal(intPtr);
			}
		}

		private void EnsureBuffer(int bytesRequired)
		{
			if (decoderOutputBuffer == null || decoderOutputBuffer.Length < bytesRequired)
			{
				decoderOutputBuffer = new byte[bytesRequired];
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
			if (pReader == null)
			{
				pReader = CreateReader(settings);
			}
			if (repositionTo != -1)
			{
				Reposition(repositionTo);
			}
			int num = 0;
			if (decoderOutputCount > 0)
			{
				num += ReadFromDecoderBuffer(buffer, offset, count - num);
			}
			while (num < count)
			{
				pReader.ReadSample(-3, 0, out var _, out var pdwStreamFlags, out var _, out var ppSample);
				if ((pdwStreamFlags & MF_SOURCE_READER_FLAG.MF_SOURCE_READERF_ENDOFSTREAM) != 0)
				{
					break;
				}
				if ((pdwStreamFlags & MF_SOURCE_READER_FLAG.MF_SOURCE_READERF_CURRENTMEDIATYPECHANGED) != 0)
				{
					waveFormat = GetCurrentWaveFormat(pReader);
					OnWaveFormatChanged();
				}
				else if (pdwStreamFlags != 0)
				{
					throw new InvalidOperationException($"MediaFoundationReadError {pdwStreamFlags}");
				}
				ppSample.ConvertToContiguousBuffer(out var ppBuffer);
				ppBuffer.Lock(out var ppbBuffer, out var _, out var pcbCurrentLength);
				EnsureBuffer(pcbCurrentLength);
				Marshal.Copy(ppbBuffer, decoderOutputBuffer, 0, pcbCurrentLength);
				decoderOutputOffset = 0;
				decoderOutputCount = pcbCurrentLength;
				num += ReadFromDecoderBuffer(buffer, offset + num, count - num);
				ppBuffer.Unlock();
				Marshal.ReleaseComObject((object)ppBuffer);
				Marshal.ReleaseComObject((object)ppSample);
			}
			position += num;
			return num;
		}

		private int ReadFromDecoderBuffer(byte[] buffer, int offset, int needed)
		{
			int num = Math.Min(needed, decoderOutputCount);
			global::System.Array.Copy((global::System.Array)decoderOutputBuffer, decoderOutputOffset, (global::System.Array)buffer, offset, num);
			decoderOutputOffset += num;
			decoderOutputCount -= num;
			if (decoderOutputCount == 0)
			{
				decoderOutputOffset = 0;
			}
			return num;
		}

		private void Reposition(long desiredPosition)
		{
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			PropVariant propVariant = PropVariant.FromLong(10000000 * repositionTo / waveFormat.AverageBytesPerSecond);
			global::System.IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf((object)propVariant));
			Marshal.StructureToPtr((object)propVariant, intPtr, false);
			pReader.SetCurrentPosition(Guid.Empty, intPtr);
			Marshal.FreeHGlobal(intPtr);
			decoderOutputCount = 0;
			decoderOutputOffset = 0;
			position = desiredPosition;
			repositionTo = -1L;
		}

		protected override void Dispose(bool disposing)
		{
			if (pReader != null)
			{
				Marshal.ReleaseComObject((object)pReader);
				pReader = null;
			}
			((Stream)this).Dispose(disposing);
		}

		private void OnWaveFormatChanged()
		{
			EventHandler waveFormatChanged = this.WaveFormatChanged;
			if (waveFormatChanged != null)
			{
				waveFormatChanged.Invoke((object)this, EventArgs.Empty);
			}
		}
	}
}
