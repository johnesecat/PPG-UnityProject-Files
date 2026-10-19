using System;
using System.Runtime.CompilerServices;

namespace NAudio.Codecs
{
	public class G722CodecState
	{
		public bool ItuTestMode
		{
			[CompilerGenerated]
			get
			{
				return <ItuTestMode>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<ItuTestMode>k__BackingField = value;
			}
		}

		public bool Packed
		{
			[CompilerGenerated]
			get
			{
				return <Packed>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				<Packed>k__BackingField = value;
			}
		}

		public bool EncodeFrom8000Hz
		{
			[CompilerGenerated]
			get
			{
				return <EncodeFrom8000Hz>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				<EncodeFrom8000Hz>k__BackingField = value;
			}
		}

		public int BitsPerSample
		{
			[CompilerGenerated]
			get
			{
				return <BitsPerSample>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				<BitsPerSample>k__BackingField = value;
			}
		}

		public int[] QmfSignalHistory
		{
			[CompilerGenerated]
			get
			{
				return <QmfSignalHistory>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				<QmfSignalHistory>k__BackingField = value;
			}
		}

		public Band[] Band
		{
			[CompilerGenerated]
			get
			{
				return <Band>k__BackingField;
			}
			[CompilerGenerated]
			private set
			{
				<Band>k__BackingField = value;
			}
		}

		public uint InBuffer
		{
			[CompilerGenerated]
			get
			{
				return <InBuffer>k__BackingField;
			}
			[CompilerGenerated]
			internal set
			{
				<InBuffer>k__BackingField = value;
			}
		}

		public int InBits
		{
			[CompilerGenerated]
			get
			{
				return <InBits>k__BackingField;
			}
			[CompilerGenerated]
			internal set
			{
				<InBits>k__BackingField = value;
			}
		}

		public uint OutBuffer
		{
			[CompilerGenerated]
			get
			{
				return <OutBuffer>k__BackingField;
			}
			[CompilerGenerated]
			internal set
			{
				<OutBuffer>k__BackingField = value;
			}
		}

		public int OutBits
		{
			[CompilerGenerated]
			get
			{
				return <OutBits>k__BackingField;
			}
			[CompilerGenerated]
			internal set
			{
				<OutBits>k__BackingField = value;
			}
		}

		public G722CodecState(int rate, G722Flags options)
		{
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			Band = new Band[2]
			{
				new Band(),
				new Band()
			};
			QmfSignalHistory = new int[24];
			ItuTestMode = false;
			switch (rate)
			{
			case 48000:
				BitsPerSample = 6;
				break;
			case 56000:
				BitsPerSample = 7;
				break;
			case 64000:
				BitsPerSample = 8;
				break;
			default:
				throw new ArgumentException("Invalid rate, should be 48000, 56000 or 64000");
			}
			if ((options & G722Flags.SampleRate8000) == G722Flags.SampleRate8000)
			{
				EncodeFrom8000Hz = true;
			}
			if ((options & G722Flags.Packed) == G722Flags.Packed && BitsPerSample != 8)
			{
				Packed = true;
			}
			else
			{
				Packed = false;
			}
			Band[0].det = 32;
			Band[1].det = 8;
		}
	}
}
