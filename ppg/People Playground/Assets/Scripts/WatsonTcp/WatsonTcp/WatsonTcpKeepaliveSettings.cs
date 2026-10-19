using System;

namespace WatsonTcp
{
	public class WatsonTcpKeepaliveSettings
	{
		public bool EnableTcpKeepAlives;

		private int _TcpKeepAliveInterval = 5;

		private int _TcpKeepAliveTime = 5;

		private int _TcpKeepAliveRetryCount = 5;

		public int TcpKeepAliveInterval
		{
			get
			{
				return _TcpKeepAliveInterval;
			}
			set
			{
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				if (value < 1)
				{
					throw new ArgumentException("TcpKeepAliveInterval must be greater than zero.");
				}
				_TcpKeepAliveInterval = value;
			}
		}

		public int TcpKeepAliveTime
		{
			get
			{
				return _TcpKeepAliveTime;
			}
			set
			{
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				if (value < 1)
				{
					throw new ArgumentException("TcpKeepAliveTime must be greater than zero.");
				}
				_TcpKeepAliveTime = value;
			}
		}

		public int TcpKeepAliveRetryCount
		{
			get
			{
				return _TcpKeepAliveRetryCount;
			}
			set
			{
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				if (value < 1)
				{
					throw new ArgumentException("TcpKeepAliveRetryCount must be greater than zero.");
				}
				_TcpKeepAliveRetryCount = value;
			}
		}
	}
}
