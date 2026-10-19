using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace NAudio.Wave
{
	public class Mp3Frame
	{
		private static readonly int[,,] bitRates;

		private static readonly int[,] samplesPerFrame;

		private static readonly int[] sampleRatesVersion1;

		private static readonly int[] sampleRatesVersion2;

		private static readonly int[] sampleRatesVersion25;

		private const int MaxFrameLength = 16384;

		public int SampleRate
		{
			[CompilerGenerated]
			get
			{
				return <SampleRate>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				<SampleRate>k__BackingField = value;
			}
		}

		public int FrameLength
		{
			[CompilerGenerated]
			get
			{
				return <FrameLength>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				<FrameLength>k__BackingField = value;
			}
		}

		public int BitRate
		{
			[CompilerGenerated]
			get
			{
				return <BitRate>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				<BitRate>k__BackingField = value;
			}
		}

		public byte[] RawData
		{
			[CompilerGenerated]
			get
			{
				return <RawData>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				<RawData>k__BackingField = value;
			}
		}

		public MpegVersion MpegVersion
		{
			[CompilerGenerated]
			get
			{
				return <MpegVersion>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				<MpegVersion>k__BackingField = value;
			}
		}

		public MpegLayer MpegLayer
		{
			[CompilerGenerated]
			get
			{
				return <MpegLayer>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				<MpegLayer>k__BackingField = value;
			}
		}

		public ChannelMode ChannelMode
		{
			[CompilerGenerated]
			get
			{
				return <ChannelMode>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				<ChannelMode>k__BackingField = value;
			}
		}

		public int SampleCount
		{
			[CompilerGenerated]
			get
			{
				return <SampleCount>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				<SampleCount>k__BackingField = value;
			}
		}

		public int ChannelExtension
		{
			[CompilerGenerated]
			get
			{
				return <ChannelExtension>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				<ChannelExtension>k__BackingField = value;
			}
		}

		public int BitRateIndex
		{
			[CompilerGenerated]
			get
			{
				return <BitRateIndex>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				<BitRateIndex>k__BackingField = value;
			}
		}

		public bool Copyright
		{
			[CompilerGenerated]
			get
			{
				return <Copyright>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				<Copyright>k__BackingField = value;
			}
		}

		public bool CrcPresent
		{
			[CompilerGenerated]
			get
			{
				return <CrcPresent>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				<CrcPresent>k__BackingField = value;
			}
		}

		public long FileOffset
		{
			[CompilerGenerated]
			get
			{
				return <FileOffset>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				<FileOffset>k__BackingField = value;
			}
		}

		public static Mp3Frame LoadFromStream(Stream input)
		{
			return LoadFromStream(input, readData: true);
		}

		public static Mp3Frame LoadFromStream(Stream input, bool readData)
		{
			//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
			Mp3Frame mp3Frame = new Mp3Frame();
			mp3Frame.FileOffset = input.get_Position();
			byte[] array = new byte[4];
			if (input.Read(array, 0, array.Length) < array.Length)
			{
				return null;
			}
			while (!IsValidHeader(array, mp3Frame))
			{
				array[0] = array[1];
				array[1] = array[2];
				array[2] = array[3];
				if (input.Read(array, 3, 1) < 1)
				{
					return null;
				}
				mp3Frame.FileOffset++;
			}
			int num = mp3Frame.FrameLength - 4;
			if (readData)
			{
				mp3Frame.RawData = new byte[mp3Frame.FrameLength];
				global::System.Array.Copy((global::System.Array)array, (global::System.Array)mp3Frame.RawData, 4);
				if (input.Read(mp3Frame.RawData, 4, num) < num)
				{
					throw new EndOfStreamException("Unexpected end of stream before frame complete");
				}
			}
			else
			{
				input.set_Position(input.get_Position() + num);
			}
			return mp3Frame;
		}

		private Mp3Frame()
		{
		}

		private static bool IsValidHeader(byte[] headerBytes, Mp3Frame frame)
		{
			if (headerBytes[0] == 255 && (headerBytes[1] & 0xE0) == 224)
			{
				frame.MpegVersion = (MpegVersion)((headerBytes[1] & 0x18) >> 3);
				if (frame.MpegVersion == MpegVersion.Reserved)
				{
					return false;
				}
				frame.MpegLayer = (MpegLayer)((headerBytes[1] & 6) >> 1);
				if (frame.MpegLayer == MpegLayer.Reserved)
				{
					return false;
				}
				int num = ((frame.MpegLayer != MpegLayer.Layer1) ? ((frame.MpegLayer == MpegLayer.Layer2) ? 1 : 2) : 0);
				frame.CrcPresent = (headerBytes[1] & 1) == 0;
				frame.BitRateIndex = (headerBytes[2] & 0xF0) >> 4;
				if (frame.BitRateIndex == 15)
				{
					return false;
				}
				int num2 = ((frame.MpegVersion != MpegVersion.Version1) ? 1 : 0);
				frame.BitRate = bitRates[num2, num, frame.BitRateIndex] * 1000;
				if (frame.BitRate == 0)
				{
					return false;
				}
				int num3 = (headerBytes[2] & 0xC) >> 2;
				if (num3 == 3)
				{
					return false;
				}
				if (frame.MpegVersion == MpegVersion.Version1)
				{
					frame.SampleRate = sampleRatesVersion1[num3];
				}
				else if (frame.MpegVersion == MpegVersion.Version2)
				{
					frame.SampleRate = sampleRatesVersion2[num3];
				}
				else
				{
					frame.SampleRate = sampleRatesVersion25[num3];
				}
				bool flag = (headerBytes[2] & 2) == 2;
				_ = headerBytes[2];
				frame.ChannelMode = (ChannelMode)((headerBytes[3] & 0xC0) >> 6);
				frame.ChannelExtension = (headerBytes[3] & 0x30) >> 4;
				if (frame.ChannelExtension != 0 && frame.ChannelMode != ChannelMode.JointStereo)
				{
					return false;
				}
				frame.Copyright = (headerBytes[3] & 8) == 8;
				_ = headerBytes[3];
				_ = headerBytes[3];
				int num4 = (flag ? 1 : 0);
				frame.SampleCount = samplesPerFrame[num2, num];
				int num5 = frame.SampleCount / 8;
				if (frame.MpegLayer == MpegLayer.Layer1)
				{
					frame.FrameLength = (num5 * frame.BitRate / frame.SampleRate + num4) * 4;
				}
				else
				{
					frame.FrameLength = num5 * frame.BitRate / frame.SampleRate + num4;
				}
				if (frame.FrameLength > 16384)
				{
					return false;
				}
				return true;
			}
			return false;
		}

		static Mp3Frame()
		{
			int[,,] array = new int[2, 3, 15];
			RuntimeHelpers.InitializeArray((global::System.Array)array, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
			bitRates = array;
			int[,] array2 = new int[2, 3];
			RuntimeHelpers.InitializeArray((global::System.Array)array2, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
			samplesPerFrame = array2;
			int[] array3 = new int[3];
			RuntimeHelpers.InitializeArray((global::System.Array)array3, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
			sampleRatesVersion1 = array3;
			int[] array4 = new int[3];
			RuntimeHelpers.InitializeArray((global::System.Array)array4, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
			sampleRatesVersion2 = array4;
			int[] array5 = new int[3];
			RuntimeHelpers.InitializeArray((global::System.Array)array5, (RuntimeFieldHandle)/*OpCode not supported: LdMemberToken*/);
			sampleRatesVersion25 = array5;
		}
	}
}
