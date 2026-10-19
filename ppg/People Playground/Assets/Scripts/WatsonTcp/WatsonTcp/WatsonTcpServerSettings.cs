using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace WatsonTcp
{
	public class WatsonTcpServerSettings
	{
		public bool DebugMessages;

		public Action<Severity, string> Logger;

		public bool AcceptInvalidCertificates = true;

		public bool MutuallyAuthenticate;

		public string PresharedKey;

		private int _StreamBufferSize = 65536;

		private int _MaxProxiedStreamSize = 67108864;

		private int _MaxConnections = 4096;

		private int _IdleClientTimeoutSeconds;

		private List<string> _PermittedIPs = new List<string>();

		public int StreamBufferSize
		{
			get
			{
				return _StreamBufferSize;
			}
			set
			{
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				if (value < 1)
				{
					throw new ArgumentException("Stream buffer size must be greater than zero.");
				}
				_StreamBufferSize = value;
			}
		}

		public int MaxProxiedStreamSize
		{
			get
			{
				return _MaxProxiedStreamSize;
			}
			set
			{
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				if (value < 1)
				{
					throw new ArgumentException("MaxProxiedStreamSize must be greater than zero.");
				}
				_MaxProxiedStreamSize = value;
			}
		}

		public int IdleClientTimeoutSeconds
		{
			get
			{
				return _IdleClientTimeoutSeconds;
			}
			set
			{
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				if (value < 0)
				{
					throw new ArgumentException("IdleClientTimeoutSeconds must be zero or greater.");
				}
				_IdleClientTimeoutSeconds = value;
			}
		}

		public int MaxConnections
		{
			get
			{
				return _MaxConnections;
			}
			set
			{
				//IL_0009: Unknown result type (might be due to invalid IL or missing references)
				if (value < 1)
				{
					throw new ArgumentException("Max connections must be greater than zero.");
				}
				_MaxConnections = value;
			}
		}

		public List<string> PermittedIPs
		{
			get
			{
				return _PermittedIPs;
			}
			set
			{
				if (value == null)
				{
					_PermittedIPs = new List<string>();
				}
				else
				{
					_PermittedIPs = value;
				}
			}
		}

		public bool NoDelay
		{
			[CompilerGenerated]
			get
			{
				return <NoDelay>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<NoDelay>k__BackingField = value;
			}
		}
	}
}
