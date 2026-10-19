using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;

namespace WatsonTcp
{
	public class StreamReceivedEventArgs : EventArgs
	{
		private Dictionary<object, object> _Metadata = new Dictionary<object, object>();

		private byte[] _Data;

		private int _BufferSize = 65536;

		public string IpPort
		{
			[CompilerGenerated]
			get
			{
				return <IpPort>k__BackingField;
			}
		}

		public Dictionary<object, object> Metadata
		{
			get
			{
				return _Metadata;
			}
			set
			{
				if (value == null)
				{
					_Metadata = new Dictionary<object, object>();
				}
				else
				{
					_Metadata = value;
				}
			}
		}

		public long ContentLength
		{
			[CompilerGenerated]
			get
			{
				return <ContentLength>k__BackingField;
			}
		}

		public Stream DataStream
		{
			[CompilerGenerated]
			get
			{
				return <DataStream>k__BackingField;
			}
		}

		public byte[] Data
		{
			get
			{
				if (_Data != null)
				{
					return _Data;
				}
				if (ContentLength <= 0)
				{
					return null;
				}
				_Data = ReadFromStream(DataStream, ContentLength);
				return _Data;
			}
		}

		internal StreamReceivedEventArgs(string ipPort, Dictionary<object, object> metadata, long contentLength, Stream stream)
			: this()
		{
			<IpPort>k__BackingField = ipPort;
			Metadata = metadata;
			<ContentLength>k__BackingField = contentLength;
			<DataStream>k__BackingField = stream;
		}

		private byte[] ReadFromStream(Stream stream, long count)
		{
			//IL_001c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0022: Expected O, but got Unknown
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			if (count <= 0)
			{
				return new byte[0];
			}
			byte[] array = new byte[_BufferSize];
			int num = 0;
			long num2 = count;
			MemoryStream val = new MemoryStream();
			while (num2 > 0)
			{
				if (_BufferSize > num2)
				{
					array = new byte[num2];
				}
				num = stream.Read(array, 0, array.Length);
				if (num > 0)
				{
					((Stream)val).Write(array, 0, num);
					num2 -= num;
					continue;
				}
				throw new IOException("Could not read from supplied stream.");
			}
			return val.ToArray();
		}
	}
}
