using System;

namespace WatsonTcp
{
	public class WatsonTcpClientCallbacks
	{
		public Func<string> AuthenticationRequested;

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

		internal string HandleAuthenticationRequested()
		{
			string result = null;
			if (AuthenticationRequested != null)
			{
				try
				{
					result = AuthenticationRequested.Invoke();
					return result;
				}
				catch (global::System.Exception)
				{
					return result;
				}
			}
			return result;
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
