using System;
using System.IO;
using System.Text;

namespace NAudio.SoundFont
{
	internal class InstrumentBuilder : StructureBuilder<Instrument>
	{
		private Instrument lastInstrument;

		public override int Length => 22;

		public Instrument[] Instruments => data.ToArray();

		public override Instrument Read(BinaryReader br)
		{
			Instrument instrument = new Instrument();
			string text = Encoding.get_UTF8().GetString(br.ReadBytes(20), 0, 20);
			if (text.IndexOf('\0') >= 0)
			{
				text = text.Substring(0, text.IndexOf('\0'));
			}
			instrument.Name = text;
			instrument.startInstrumentZoneIndex = br.ReadUInt16();
			if (lastInstrument != null)
			{
				lastInstrument.endInstrumentZoneIndex = (ushort)(instrument.startInstrumentZoneIndex - 1);
			}
			data.Add(instrument);
			lastInstrument = instrument;
			return instrument;
		}

		public override void Write(BinaryWriter bw, Instrument instrument)
		{
		}

		public void LoadZones(Zone[] zones)
		{
			for (int i = 0; i < data.get_Count() - 1; i++)
			{
				Instrument instrument = data.get_Item(i);
				instrument.Zones = new Zone[instrument.endInstrumentZoneIndex - instrument.startInstrumentZoneIndex + 1];
				global::System.Array.Copy((global::System.Array)zones, (int)instrument.startInstrumentZoneIndex, (global::System.Array)instrument.Zones, 0, instrument.Zones.Length);
			}
			data.RemoveAt(data.get_Count() - 1);
		}
	}
}
