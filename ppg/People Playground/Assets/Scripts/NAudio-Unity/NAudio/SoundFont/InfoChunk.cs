using System.Runtime.CompilerServices;
using NAudio.Utils;

namespace NAudio.SoundFont
{
	public class InfoChunk
	{
		public SFVersion SoundFontVersion
		{
			[CompilerGenerated]
			get
			{
				return <SoundFontVersion>k__BackingField;
			}
		}

		public string WaveTableSoundEngine
		{
			[CompilerGenerated]
			get
			{
				return <WaveTableSoundEngine>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<WaveTableSoundEngine>k__BackingField = value;
			}
		}

		public string BankName
		{
			[CompilerGenerated]
			get
			{
				return <BankName>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<BankName>k__BackingField = value;
			}
		}

		public string DataROM
		{
			[CompilerGenerated]
			get
			{
				return <DataROM>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<DataROM>k__BackingField = value;
			}
		}

		public string CreationDate
		{
			[CompilerGenerated]
			get
			{
				return <CreationDate>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<CreationDate>k__BackingField = value;
			}
		}

		public string Author
		{
			[CompilerGenerated]
			get
			{
				return <Author>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<Author>k__BackingField = value;
			}
		}

		public string TargetProduct
		{
			[CompilerGenerated]
			get
			{
				return <TargetProduct>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<TargetProduct>k__BackingField = value;
			}
		}

		public string Copyright
		{
			[CompilerGenerated]
			get
			{
				return <Copyright>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<Copyright>k__BackingField = value;
			}
		}

		public string Comments
		{
			[CompilerGenerated]
			get
			{
				return <Comments>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<Comments>k__BackingField = value;
			}
		}

		public string Tools
		{
			[CompilerGenerated]
			get
			{
				return <Tools>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<Tools>k__BackingField = value;
			}
		}

		public SFVersion ROMVersion
		{
			[CompilerGenerated]
			get
			{
				return <ROMVersion>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<ROMVersion>k__BackingField = value;
			}
		}

		internal InfoChunk(RiffChunk chunk)
		{
			bool flag = false;
			bool flag2 = false;
			if (chunk.ReadChunkID() != "INFO")
			{
				throw new InvalidDataException("Not an INFO chunk");
			}
			RiffChunk nextSubChunk;
			while ((nextSubChunk = chunk.GetNextSubChunk()) != null)
			{
				string chunkID = nextSubChunk.ChunkID;
				switch (<PrivateImplementationDetails>.ComputeStringHash(chunkID))
				{
				case 3349434627u:
					if (!(chunkID == "ifil"))
					{
						break;
					}
					flag = true;
					<SoundFontVersion>k__BackingField = nextSubChunk.GetDataAsStructure(new SFVersionBuilder());
					continue;
				case 2408165810u:
					if (!(chunkID == "isng"))
					{
						break;
					}
					WaveTableSoundEngine = nextSubChunk.GetDataAsString();
					continue;
				case 186167360u:
					if (!(chunkID == "INAM"))
					{
						break;
					}
					flag2 = true;
					BankName = nextSubChunk.GetDataAsString();
					continue;
				case 1286587406u:
					if (!(chunkID == "irom"))
					{
						break;
					}
					DataROM = nextSubChunk.GetDataAsString();
					continue;
				case 2337496513u:
					if (!(chunkID == "iver"))
					{
						break;
					}
					ROMVersion = nextSubChunk.GetDataAsStructure(new SFVersionBuilder());
					continue;
				case 3696333059u:
					if (!(chunkID == "ICRD"))
					{
						break;
					}
					CreationDate = nextSubChunk.GetDataAsString();
					continue;
				case 802522432u:
					if (!(chunkID == "IENG"))
					{
						break;
					}
					Author = nextSubChunk.GetDataAsString();
					continue;
				case 1438214986u:
					if (!(chunkID == "IPRD"))
					{
						break;
					}
					TargetProduct = nextSubChunk.GetDataAsString();
					continue;
				case 3794880202u:
					if (!(chunkID == "ICOP"))
					{
						break;
					}
					Copyright = nextSubChunk.GetDataAsString();
					continue;
				case 439062212u:
					if (!(chunkID == "ICMT"))
					{
						break;
					}
					Comments = nextSubChunk.GetDataAsString();
					continue;
				case 3955789767u:
					if (!(chunkID == "ISFT"))
					{
						break;
					}
					Tools = nextSubChunk.GetDataAsString();
					continue;
				}
				throw new InvalidDataException($"Unknown chunk type {nextSubChunk.ChunkID}");
			}
			if (!flag)
			{
				throw new InvalidDataException("Missing SoundFont version information");
			}
			if (!flag2)
			{
				throw new InvalidDataException("Missing SoundFont name information");
			}
		}

		public override string ToString()
		{
			return string.Format("Bank Name: {0}\r\nAuthor: {1}\r\nCopyright: {2}\r\nCreation Date: {3}\r\nTools: {4}\r\nComments: {5}\r\nSound Engine: {6}\r\nSoundFont Version: {7}\r\nTarget Product: {8}\r\nData ROM: {9}\r\nROM Version: {10}", new object[11]
			{
				BankName, Author, Copyright, CreationDate, Tools, "TODO-fix comments", WaveTableSoundEngine, SoundFontVersion, TargetProduct, DataROM,
				ROMVersion
			});
		}
	}
}
