using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WatsonTcp
{
	internal class WatsonMessage
	{
		[JsonProperty("s")]
		public MessageStatus Status;

		[JsonProperty("sreq")]
		public bool? SyncRequest;

		[JsonProperty("sresp")]
		public bool? SyncResponse;

		[JsonProperty("sts")]
		public global::System.DateTime? SenderTimestamp;

		[JsonProperty("exp")]
		public global::System.DateTime? Expiration;

		[JsonProperty("guid")]
		public string ConversationGuid;

		private Action<Severity, string> _Logger;

		private string _Header = "[WatsonMessage] ";

		private string _DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fffzzz";

		private int _ReadStreamBuffer = 65536;

		private byte[] _PresharedKey;

		private Dictionary<object, object> _Metadata;

		private Stream _DataStream;

		[JsonProperty("len")]
		public long ContentLength
		{
			[CompilerGenerated]
			get
			{
				return <ContentLength>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<ContentLength>k__BackingField = value;
			}
		}

		[JsonProperty("psk")]
		public byte[] PresharedKey
		{
			get
			{
				return _PresharedKey;
			}
			set
			{
				//IL_0017: Unknown result type (might be due to invalid IL or missing references)
				if (value == null)
				{
					_PresharedKey = null;
					return;
				}
				if (value.Length != 16)
				{
					throw new ArgumentException("PresharedKey must be 16 bytes.");
				}
				_PresharedKey = new byte[16];
				Buffer.BlockCopy((global::System.Array)value, 0, (global::System.Array)_PresharedKey, 0, 16);
			}
		}

		[JsonProperty("md")]
		public Dictionary<object, object> Metadata
		{
			get
			{
				return _Metadata;
			}
			set
			{
				_Metadata = value;
			}
		}

		[JsonIgnore]
		public Stream DataStream => _DataStream;

		[JsonIgnore]
		public byte[] HeaderBytes
		{
			get
			{
				string text = SerializationHelper.SerializeJson(this, pretty: false);
				byte[] bytes = Encoding.get_UTF8().GetBytes(text);
				byte[] bytes2 = Encoding.get_UTF8().GetBytes("\r\n\r\n");
				return WatsonCommon.AppendBytes(bytes, bytes2);
			}
		}

		internal int BufferSize
		{
			get
			{
				return _ReadStreamBuffer;
			}
			set
			{
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				if (value < 1)
				{
					throw new ArgumentException("ReadStreamBuffer must be greater than zero bytes.");
				}
				_ReadStreamBuffer = value;
			}
		}

		internal WatsonMessage()
		{
			Status = MessageStatus.Normal;
		}

		internal WatsonMessage(Dictionary<object, object> metadata, long contentLength, Stream stream, bool syncRequest, bool syncResponse, global::System.DateTime? expiration, string convGuid, Action<Severity, string> logger)
		{
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Unknown result type (might be due to invalid IL or missing references)
			if (contentLength < 0)
			{
				throw new ArgumentException("Content length must be zero or greater.");
			}
			if (contentLength > 0 && (stream == null || !stream.get_CanRead()))
			{
				throw new ArgumentException("Cannot read from supplied stream.");
			}
			Status = MessageStatus.Normal;
			ContentLength = contentLength;
			Metadata = metadata;
			if (syncRequest)
			{
				SyncRequest = true;
			}
			if (syncResponse)
			{
				SyncResponse = true;
			}
			Expiration = expiration;
			ConversationGuid = convGuid;
			if (SyncRequest.get_HasValue() && SyncRequest.get_Value())
			{
				SenderTimestamp = global::System.DateTime.get_Now();
			}
			_DataStream = stream;
			_Logger = logger;
		}

		internal WatsonMessage(Stream stream, Action<Severity, string> logger)
		{
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			if (stream == null)
			{
				throw new ArgumentNullException("stream");
			}
			if (!stream.get_CanRead())
			{
				throw new ArgumentException("Cannot read from stream.");
			}
			Status = MessageStatus.Normal;
			_DataStream = stream;
			_Logger = logger;
		}

		[AsyncStateMachine(typeof(<BuildFromStream>d__33))]
		internal global::System.Threading.Tasks.Task<bool> BuildFromStream(CancellationToken token)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Unknown result type (might be due to invalid IL or missing references)
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_0017: Unknown result type (might be due to invalid IL or missing references)
			<BuildFromStream>d__33 <BuildFromStream>d__ = default(<BuildFromStream>d__33);
			<BuildFromStream>d__.<>t__builder = AsyncTaskMethodBuilder<bool>.Create();
			<BuildFromStream>d__.<>4__this = this;
			<BuildFromStream>d__.token = token;
			<BuildFromStream>d__.<>1__state = -1;
			<BuildFromStream>d__.<>t__builder.Start<<BuildFromStream>d__33>(ref <BuildFromStream>d__);
			return <BuildFromStream>d__.<>t__builder.get_Task();
		}

		public override string ToString()
		{
			string text = "---" + Environment.get_NewLine();
			text = text + "  Preshared key     : " + ((PresharedKey != null) ? WatsonCommon.ByteArrayToHex(PresharedKey) : "null") + Environment.get_NewLine();
			text = text + "  Status            : " + ((object)Status).ToString() + Environment.get_NewLine();
			text = text + "  SyncRequest       : " + ((object)SyncRequest).ToString() + Environment.get_NewLine();
			text = text + "  SyncResponse      : " + ((object)SyncResponse).ToString() + Environment.get_NewLine();
			text = text + "  ExpirationUtc     : " + (Expiration.get_HasValue() ? Expiration.get_Value().ToString(_DateTimeFormat) : "null") + Environment.get_NewLine();
			text = text + "  Conversation      : " + ConversationGuid + Environment.get_NewLine();
			if (Metadata != null)
			{
				text = string.Concat(new string[5]
				{
					text,
					"  Metadata          : ",
					Metadata.get_Count().ToString(),
					" entries",
					Environment.get_NewLine()
				});
			}
			if (DataStream != null)
			{
				text = string.Concat(new string[5]
				{
					text,
					"  DataStream        : present, ",
					ContentLength.ToString(),
					" bytes",
					Environment.get_NewLine()
				});
			}
			return text;
		}
	}
}
