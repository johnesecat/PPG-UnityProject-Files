using System;

namespace WatsonTcp
{
	public class WatsonTcpServerCallbacks
	{
		private Func<SyncRequest, SyncResponse> _SyncRequestReceived;

		public Func<SyncRequest, SyncResponse> SyncRequestReceived
		{
			get
			{
				return _SyncRequestReceived;
			}
			set
			{
				_SyncRequestReceived = value;
			}
		}

		internal SyncResponse HandleSyncRequestReceived(SyncRequest req)
		{
			SyncResponse result = null;
			if (SyncRequestReceived != null)
			{
				try
				{
					result = SyncRequestReceived.Invoke(req);
					return result;
				}
				catch (global::System.Exception)
				{
					return result;
				}
			}
			return result;
		}
	}
}
