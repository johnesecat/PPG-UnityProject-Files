using System;
using System.IO;

namespace NAudio.SoundFont
{
	internal class ZoneBuilder : StructureBuilder<Zone>
	{
		private Zone lastZone;

		public Zone[] Zones => data.ToArray();

		public override int Length => 4;

		public override Zone Read(BinaryReader br)
		{
			Zone zone = new Zone();
			zone.generatorIndex = br.ReadUInt16();
			zone.modulatorIndex = br.ReadUInt16();
			if (lastZone != null)
			{
				lastZone.generatorCount = (ushort)(zone.generatorIndex - lastZone.generatorIndex);
				lastZone.modulatorCount = (ushort)(zone.modulatorIndex - lastZone.modulatorIndex);
			}
			data.Add(zone);
			lastZone = zone;
			return zone;
		}

		public override void Write(BinaryWriter bw, Zone zone)
		{
		}

		public void Load(Modulator[] modulators, Generator[] generators)
		{
			for (int i = 0; i < data.get_Count() - 1; i++)
			{
				Zone zone = data.get_Item(i);
				zone.Generators = new Generator[zone.generatorCount];
				global::System.Array.Copy((global::System.Array)generators, (int)zone.generatorIndex, (global::System.Array)zone.Generators, 0, (int)zone.generatorCount);
				zone.Modulators = new Modulator[zone.modulatorCount];
				global::System.Array.Copy((global::System.Array)modulators, (int)zone.modulatorIndex, (global::System.Array)zone.Modulators, 0, (int)zone.modulatorCount);
			}
			data.RemoveAt(data.get_Count() - 1);
		}
	}
}
