using System.Text;
using NAudio.Utils;

namespace NAudio.SoundFont
{
	public class PresetsChunk
	{
		private PresetBuilder presetHeaders = new PresetBuilder();

		private ZoneBuilder presetZones = new ZoneBuilder();

		private ModulatorBuilder presetZoneModulators = new ModulatorBuilder();

		private GeneratorBuilder presetZoneGenerators = new GeneratorBuilder();

		private InstrumentBuilder instruments = new InstrumentBuilder();

		private ZoneBuilder instrumentZones = new ZoneBuilder();

		private ModulatorBuilder instrumentZoneModulators = new ModulatorBuilder();

		private GeneratorBuilder instrumentZoneGenerators = new GeneratorBuilder();

		private SampleHeaderBuilder sampleHeaders = new SampleHeaderBuilder();

		public Preset[] Presets => presetHeaders.Presets;

		public Instrument[] Instruments => instruments.Instruments;

		public SampleHeader[] SampleHeaders => sampleHeaders.SampleHeaders;

		internal PresetsChunk(RiffChunk chunk)
		{
			string text = chunk.ReadChunkID();
			if (text != "pdta")
			{
				throw new InvalidDataException($"Not a presets data chunk ({text})");
			}
			RiffChunk nextSubChunk;
			while ((nextSubChunk = chunk.GetNextSubChunk()) != null)
			{
				string chunkID = nextSubChunk.ChunkID;
				uint num = <PrivateImplementationDetails>.ComputeStringHash(chunkID);
				if (num <= 1912422739)
				{
					if (num <= 835588718)
					{
						if (num <= 446289113)
						{
							if (num != 11082205)
							{
								if (num == 446289113 && chunkID == "inst")
								{
									goto IL_037e;
								}
							}
							else if (chunkID == "PMOD")
							{
								goto IL_0360;
							}
						}
						else if (num != 454631123)
						{
							if (num == 835588718 && chunkID == "IGEN")
							{
								goto IL_03ab;
							}
						}
						else if (chunkID == "PHDR")
						{
							goto IL_033f;
						}
					}
					else if (num <= 1458428061)
					{
						if (num != 880558638)
						{
							if (num == 1458428061 && chunkID == "pmod")
							{
								goto IL_0360;
							}
						}
						else if (chunkID == "igen")
						{
							goto IL_03ab;
						}
					}
					else if (num != 1475086809)
					{
						if (num != 1659751530)
						{
							if (num == 1912422739 && chunkID == "phdr")
							{
								goto IL_033f;
							}
						}
						else if (chunkID == "ibag")
						{
							goto IL_038d;
						}
					}
					else if (chunkID == "INST")
					{
						goto IL_037e;
					}
				}
				else if (num <= 2815087768u)
				{
					if (num <= 2688549226u)
					{
						if (num != 2630176223u)
						{
							if (num == 2688549226u && chunkID == "IBAG")
							{
								goto IL_038d;
							}
						}
						else if (chunkID == "PBAG")
						{
							goto IL_0351;
						}
					}
					else if (num != 2770221016u)
					{
						if (num == 2815087768u && chunkID == "imod")
						{
							goto IL_039c;
						}
					}
					else if (chunkID == "IMOD")
					{
						goto IL_039c;
					}
				}
				else if (num <= 3289792331u)
				{
					if (num != 3160262260u)
					{
						if (num == 3289792331u && chunkID == "PGEN")
						{
							goto IL_036f;
						}
					}
					else if (chunkID == "shdr")
					{
						goto IL_03ba;
					}
				}
				else if (num != 3673816331u)
				{
					if (num != 4036048543u)
					{
						if (num == 4189059956u && chunkID == "SHDR")
						{
							goto IL_03ba;
						}
					}
					else if (chunkID == "pbag")
					{
						goto IL_0351;
					}
				}
				else if (chunkID == "pgen")
				{
					goto IL_036f;
				}
				throw new InvalidDataException($"Unknown chunk type {nextSubChunk.ChunkID}");
				IL_033f:
				nextSubChunk.GetDataAsStructureArray(presetHeaders);
				continue;
				IL_03ba:
				nextSubChunk.GetDataAsStructureArray(sampleHeaders);
				continue;
				IL_037e:
				nextSubChunk.GetDataAsStructureArray(instruments);
				continue;
				IL_0360:
				nextSubChunk.GetDataAsStructureArray(presetZoneModulators);
				continue;
				IL_03ab:
				nextSubChunk.GetDataAsStructureArray(instrumentZoneGenerators);
				continue;
				IL_036f:
				nextSubChunk.GetDataAsStructureArray(presetZoneGenerators);
				continue;
				IL_039c:
				nextSubChunk.GetDataAsStructureArray(instrumentZoneModulators);
				continue;
				IL_038d:
				nextSubChunk.GetDataAsStructureArray(instrumentZones);
				continue;
				IL_0351:
				nextSubChunk.GetDataAsStructureArray(presetZones);
			}
			instrumentZoneGenerators.Load(sampleHeaders.SampleHeaders);
			instrumentZones.Load(instrumentZoneModulators.Modulators, instrumentZoneGenerators.Generators);
			instruments.LoadZones(instrumentZones.Zones);
			presetZoneGenerators.Load(instruments.Instruments);
			presetZones.Load(presetZoneModulators.Modulators, presetZoneGenerators.Generators);
			presetHeaders.LoadZones(presetZones.Zones);
			sampleHeaders.RemoveEOS();
		}

		public override string ToString()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			StringBuilder val = new StringBuilder();
			val.Append("Preset Headers:\r\n");
			Preset[] presets = presetHeaders.Presets;
			foreach (Preset preset in presets)
			{
				val.AppendFormat("{0}\r\n", (object)preset);
			}
			val.Append("Instruments:\r\n");
			Instrument[] array = instruments.Instruments;
			foreach (Instrument instrument in array)
			{
				val.AppendFormat("{0}\r\n", (object)instrument);
			}
			return ((object)val).ToString();
		}
	}
}
