using System;
using System.Runtime.CompilerServices;

namespace NAudio.Wave
{
	public class BextChunkInfo
	{
		public string Description
		{
			[CompilerGenerated]
			get
			{
				return <Description>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<Description>k__BackingField = value;
			}
		}

		public string Originator
		{
			[CompilerGenerated]
			get
			{
				return <Originator>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<Originator>k__BackingField = value;
			}
		}

		public string OriginatorReference
		{
			[CompilerGenerated]
			get
			{
				return <OriginatorReference>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<OriginatorReference>k__BackingField = value;
			}
		}

		public global::System.DateTime OriginationDateTime
		{
			[CompilerGenerated]
			get
			{
				return <OriginationDateTime>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<OriginationDateTime>k__BackingField = value;
			}
		}

		public string OriginationDate => OriginationDateTime.ToString("yyyy-MM-dd");

		public string OriginationTime => OriginationDateTime.ToString("HH:mm:ss");

		public long TimeReference
		{
			[CompilerGenerated]
			get
			{
				return <TimeReference>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<TimeReference>k__BackingField = value;
			}
		}

		public ushort Version => 1;

		public string UniqueMaterialIdentifier
		{
			[CompilerGenerated]
			get
			{
				return <UniqueMaterialIdentifier>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<UniqueMaterialIdentifier>k__BackingField = value;
			}
		}

		public byte[] Reserved
		{
			[CompilerGenerated]
			get
			{
				return <Reserved>k__BackingField;
			}
		}

		public string CodingHistory
		{
			[CompilerGenerated]
			get
			{
				return <CodingHistory>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<CodingHistory>k__BackingField = value;
			}
		}

		public BextChunkInfo()
		{
			<Reserved>k__BackingField = new byte[190];
		}
	}
}
