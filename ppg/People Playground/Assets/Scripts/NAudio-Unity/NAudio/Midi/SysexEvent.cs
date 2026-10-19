using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NAudio.Midi
{
	public class SysexEvent : MidiEvent
	{
		private byte[] data;

		public static SysexEvent ReadSysexEvent(BinaryReader br)
		{
			SysexEvent sysexEvent = new SysexEvent();
			List<byte> val = new List<byte>();
			bool flag = true;
			while (flag)
			{
				byte b = br.ReadByte();
				if (b == 247)
				{
					flag = false;
				}
				else
				{
					val.Add(b);
				}
			}
			sysexEvent.data = val.ToArray();
			return sysexEvent;
		}

		public override MidiEvent Clone()
		{
			return new SysexEvent
			{
				data = (byte[])((global::System.Array)data)?.Clone()
			};
		}

		public override string ToString()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_0006: Expected O, but got Unknown
			StringBuilder val = new StringBuilder();
			byte[] array = data;
			foreach (byte b in array)
			{
				val.AppendFormat("{0:X2} ", (object)b);
			}
			return $"{base.AbsoluteTime} Sysex: {data.Length} bytes\r\n{((object)val).ToString()}";
		}

		public override void Export(ref long absoluteTime, BinaryWriter writer)
		{
			base.Export(ref absoluteTime, writer);
			writer.Write(data, 0, data.Length);
			writer.Write((byte)247);
		}
	}
}
