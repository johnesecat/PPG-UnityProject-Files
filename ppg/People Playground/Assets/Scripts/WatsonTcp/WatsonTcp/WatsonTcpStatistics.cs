using System;
using System.Threading;

namespace WatsonTcp
{
	public class WatsonTcpStatistics
	{
		private global::System.DateTime _StartTime = global::System.DateTime.get_Now().ToUniversalTime();

		private long _ReceivedBytes;

		private long _ReceivedMessages;

		private long _SentBytes;

		private long _SentMessages;

		public global::System.DateTime StartTime => _StartTime;

		public TimeSpan UpTime => global::System.DateTime.get_Now().ToUniversalTime() - _StartTime;

		public long ReceivedBytes
		{
			get
			{
				return _ReceivedBytes;
			}
			internal set
			{
				_ReceivedBytes = value;
			}
		}

		public long ReceivedMessages
		{
			get
			{
				return _ReceivedMessages;
			}
			internal set
			{
				_ReceivedMessages = value;
			}
		}

		public int ReceivedMessageSizeAverage
		{
			get
			{
				if (_ReceivedBytes > 0 && _ReceivedMessages > 0)
				{
					return (int)(_ReceivedBytes / _ReceivedMessages);
				}
				return 0;
			}
		}

		public long SentBytes
		{
			get
			{
				return _SentBytes;
			}
			internal set
			{
				_SentBytes = value;
			}
		}

		public long SentMessages
		{
			get
			{
				return _SentMessages;
			}
			internal set
			{
				_SentMessages = value;
			}
		}

		public decimal SentMessageSizeAverage
		{
			get
			{
				if (_SentBytes > 0 && _SentMessages > 0)
				{
					return decimal.op_Implicit((int)(_SentBytes / _SentMessages));
				}
				return 0m;
			}
		}

		public override string ToString()
		{
			//IL_0040: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			string[] obj = new string[32]
			{
				"--- Statistics ---",
				Environment.get_NewLine(),
				"    Started     : ",
				_StartTime.ToString(),
				Environment.get_NewLine(),
				"    Uptime      : ",
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			TimeSpan upTime = UpTime;
			obj[6] = ((object)(TimeSpan)(ref upTime)).ToString();
			obj[7] = Environment.get_NewLine();
			obj[8] = "    Received    : ";
			obj[9] = Environment.get_NewLine();
			obj[10] = "       Bytes    : ";
			obj[11] = ReceivedBytes.ToString();
			obj[12] = Environment.get_NewLine();
			obj[13] = "       Messages : ";
			obj[14] = ReceivedMessages.ToString();
			obj[15] = Environment.get_NewLine();
			obj[16] = "       Average  : ";
			obj[17] = ReceivedMessageSizeAverage.ToString();
			obj[18] = " bytes";
			obj[19] = Environment.get_NewLine();
			obj[20] = "    Sent        : ";
			obj[21] = Environment.get_NewLine();
			obj[22] = "       Bytes    : ";
			obj[23] = SentBytes.ToString();
			obj[24] = Environment.get_NewLine();
			obj[25] = "       Messages : ";
			obj[26] = SentMessages.ToString();
			obj[27] = Environment.get_NewLine();
			obj[28] = "       Average  : ";
			obj[29] = SentMessageSizeAverage.ToString();
			obj[30] = " bytes";
			obj[31] = Environment.get_NewLine();
			return string.Concat(obj);
		}

		public void Reset()
		{
			_ReceivedBytes = 0L;
			_ReceivedMessages = 0L;
			_SentBytes = 0L;
			_SentMessages = 0L;
		}

		internal void IncrementReceivedMessages()
		{
			_ReceivedMessages = Interlocked.Increment(ref _ReceivedMessages);
		}

		internal void IncrementSentMessages()
		{
			_SentMessages = Interlocked.Increment(ref _SentMessages);
		}

		internal void AddReceivedBytes(long bytes)
		{
			_ReceivedBytes = Interlocked.Add(ref _ReceivedBytes, bytes);
		}

		internal void AddSentBytes(long bytes)
		{
			_SentBytes = Interlocked.Add(ref _SentBytes, bytes);
		}
	}
}
