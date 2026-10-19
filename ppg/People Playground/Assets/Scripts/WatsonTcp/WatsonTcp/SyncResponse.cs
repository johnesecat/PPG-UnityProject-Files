using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace WatsonTcp
{
	public class SyncResponse
	{
		public Dictionary<object, object> Metadata
		{
			[CompilerGenerated]
			get
			{
				return <Metadata>k__BackingField;
			}
		}

		public byte[] Data
		{
			[CompilerGenerated]
			get
			{
				return <Data>k__BackingField;
			}
		}

		internal global::System.DateTime ExpirationUtc
		{
			[CompilerGenerated]
			get
			{
				return <ExpirationUtc>k__BackingField;
			}
			[CompilerGenerated]
			set
			{
				<ExpirationUtc>k__BackingField = value;
			}
		}

		public SyncResponse(SyncRequest req, string data)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			if (req == null)
			{
				throw new ArgumentNullException("req");
			}
			ExpirationUtc = req.ExpirationUtc;
			<Metadata>k__BackingField = new Dictionary<object, object>();
			if (string.IsNullOrEmpty(data))
			{
				<Data>k__BackingField = new byte[0];
			}
			else
			{
				<Data>k__BackingField = Encoding.get_UTF8().GetBytes(data);
			}
		}

		public SyncResponse(SyncRequest req, byte[] data)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			if (req == null)
			{
				throw new ArgumentNullException("req");
			}
			ExpirationUtc = req.ExpirationUtc;
			<Metadata>k__BackingField = new Dictionary<object, object>();
			<Data>k__BackingField = data;
		}

		public SyncResponse(SyncRequest req, Dictionary<object, object> metadata, string data)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			if (req == null)
			{
				throw new ArgumentNullException("req");
			}
			ExpirationUtc = req.ExpirationUtc;
			<Metadata>k__BackingField = metadata;
			if (string.IsNullOrEmpty(data))
			{
				<Data>k__BackingField = new byte[0];
			}
			else
			{
				<Data>k__BackingField = Encoding.get_UTF8().GetBytes(data);
			}
		}

		public SyncResponse(SyncRequest req, Dictionary<object, object> metadata, byte[] data)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			if (req == null)
			{
				throw new ArgumentNullException("req");
			}
			ExpirationUtc = req.ExpirationUtc;
			<Metadata>k__BackingField = metadata;
			<Data>k__BackingField = data;
		}

		internal SyncResponse(global::System.DateTime expirationUtc, Dictionary<object, object> metadata, byte[] data)
		{
			ExpirationUtc = expirationUtc;
			<Metadata>k__BackingField = metadata;
			<Data>k__BackingField = data;
		}
	}
}
