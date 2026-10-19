using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace WatsonTcp
{
	public class WatsonTcpServerEvents
	{
		internal bool IsUsingMessages
		{
			get
			{
				if (this.MessageReceived != null && ((global::System.Delegate)(object)this.MessageReceived).GetInvocationList().Length != 0)
				{
					return true;
				}
				return false;
			}
		}

		internal bool IsUsingStreams
		{
			get
			{
				if (this.StreamReceived != null && ((global::System.Delegate)(object)this.StreamReceived).GetInvocationList().Length != 0)
				{
					return true;
				}
				return false;
			}
		}

		public event EventHandler<AuthenticationRequestedEventArgs> AuthenticationRequested
		{
			[CompilerGenerated]
			add
			{
				EventHandler<AuthenticationRequestedEventArgs> val = this.AuthenticationRequested;
				EventHandler<AuthenticationRequestedEventArgs> val2;
				do
				{
					val2 = val;
					EventHandler<AuthenticationRequestedEventArgs> val3 = (EventHandler<AuthenticationRequestedEventArgs>)(object)global::System.Delegate.Combine((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
					val = Interlocked.CompareExchange<EventHandler<AuthenticationRequestedEventArgs>>(ref this.AuthenticationRequested, val3, val2);
				}
				while (val != val2);
			}
			[CompilerGenerated]
			remove
			{
				EventHandler<AuthenticationRequestedEventArgs> val = this.AuthenticationRequested;
				EventHandler<AuthenticationRequestedEventArgs> val2;
				do
				{
					val2 = val;
					EventHandler<AuthenticationRequestedEventArgs> val3 = (EventHandler<AuthenticationRequestedEventArgs>)(object)global::System.Delegate.Remove((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
					val = Interlocked.CompareExchange<EventHandler<AuthenticationRequestedEventArgs>>(ref this.AuthenticationRequested, val3, val2);
				}
				while (val != val2);
			}
		}

		public event EventHandler<AuthenticationSucceededEventArgs> AuthenticationSucceeded
		{
			[CompilerGenerated]
			add
			{
				EventHandler<AuthenticationSucceededEventArgs> val = this.AuthenticationSucceeded;
				EventHandler<AuthenticationSucceededEventArgs> val2;
				do
				{
					val2 = val;
					EventHandler<AuthenticationSucceededEventArgs> val3 = (EventHandler<AuthenticationSucceededEventArgs>)(object)global::System.Delegate.Combine((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
					val = Interlocked.CompareExchange<EventHandler<AuthenticationSucceededEventArgs>>(ref this.AuthenticationSucceeded, val3, val2);
				}
				while (val != val2);
			}
			[CompilerGenerated]
			remove
			{
				EventHandler<AuthenticationSucceededEventArgs> val = this.AuthenticationSucceeded;
				EventHandler<AuthenticationSucceededEventArgs> val2;
				do
				{
					val2 = val;
					EventHandler<AuthenticationSucceededEventArgs> val3 = (EventHandler<AuthenticationSucceededEventArgs>)(object)global::System.Delegate.Remove((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
					val = Interlocked.CompareExchange<EventHandler<AuthenticationSucceededEventArgs>>(ref this.AuthenticationSucceeded, val3, val2);
				}
				while (val != val2);
			}
		}

		public event EventHandler<AuthenticationFailedEventArgs> AuthenticationFailed
		{
			[CompilerGenerated]
			add
			{
				EventHandler<AuthenticationFailedEventArgs> val = this.AuthenticationFailed;
				EventHandler<AuthenticationFailedEventArgs> val2;
				do
				{
					val2 = val;
					EventHandler<AuthenticationFailedEventArgs> val3 = (EventHandler<AuthenticationFailedEventArgs>)(object)global::System.Delegate.Combine((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
					val = Interlocked.CompareExchange<EventHandler<AuthenticationFailedEventArgs>>(ref this.AuthenticationFailed, val3, val2);
				}
				while (val != val2);
			}
			[CompilerGenerated]
			remove
			{
				EventHandler<AuthenticationFailedEventArgs> val = this.AuthenticationFailed;
				EventHandler<AuthenticationFailedEventArgs> val2;
				do
				{
					val2 = val;
					EventHandler<AuthenticationFailedEventArgs> val3 = (EventHandler<AuthenticationFailedEventArgs>)(object)global::System.Delegate.Remove((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
					val = Interlocked.CompareExchange<EventHandler<AuthenticationFailedEventArgs>>(ref this.AuthenticationFailed, val3, val2);
				}
				while (val != val2);
			}
		}

		public event EventHandler<ConnectionEventArgs> ClientConnected
		{
			[CompilerGenerated]
			add
			{
				EventHandler<ConnectionEventArgs> val = this.ClientConnected;
				EventHandler<ConnectionEventArgs> val2;
				do
				{
					val2 = val;
					EventHandler<ConnectionEventArgs> val3 = (EventHandler<ConnectionEventArgs>)(object)global::System.Delegate.Combine((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
					val = Interlocked.CompareExchange<EventHandler<ConnectionEventArgs>>(ref this.ClientConnected, val3, val2);
				}
				while (val != val2);
			}
			[CompilerGenerated]
			remove
			{
				EventHandler<ConnectionEventArgs> val = this.ClientConnected;
				EventHandler<ConnectionEventArgs> val2;
				do
				{
					val2 = val;
					EventHandler<ConnectionEventArgs> val3 = (EventHandler<ConnectionEventArgs>)(object)global::System.Delegate.Remove((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
					val = Interlocked.CompareExchange<EventHandler<ConnectionEventArgs>>(ref this.ClientConnected, val3, val2);
				}
				while (val != val2);
			}
		}

		public event EventHandler<DisconnectionEventArgs> ClientDisconnected
		{
			[CompilerGenerated]
			add
			{
				EventHandler<DisconnectionEventArgs> val = this.ClientDisconnected;
				EventHandler<DisconnectionEventArgs> val2;
				do
				{
					val2 = val;
					EventHandler<DisconnectionEventArgs> val3 = (EventHandler<DisconnectionEventArgs>)(object)global::System.Delegate.Combine((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
					val = Interlocked.CompareExchange<EventHandler<DisconnectionEventArgs>>(ref this.ClientDisconnected, val3, val2);
				}
				while (val != val2);
			}
			[CompilerGenerated]
			remove
			{
				EventHandler<DisconnectionEventArgs> val = this.ClientDisconnected;
				EventHandler<DisconnectionEventArgs> val2;
				do
				{
					val2 = val;
					EventHandler<DisconnectionEventArgs> val3 = (EventHandler<DisconnectionEventArgs>)(object)global::System.Delegate.Remove((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
					val = Interlocked.CompareExchange<EventHandler<DisconnectionEventArgs>>(ref this.ClientDisconnected, val3, val2);
				}
				while (val != val2);
			}
		}

		public event EventHandler<MessageReceivedEventArgs> MessageReceived
		{
			[CompilerGenerated]
			add
			{
				EventHandler<MessageReceivedEventArgs> val = this.MessageReceived;
				EventHandler<MessageReceivedEventArgs> val2;
				do
				{
					val2 = val;
					EventHandler<MessageReceivedEventArgs> val3 = (EventHandler<MessageReceivedEventArgs>)(object)global::System.Delegate.Combine((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
					val = Interlocked.CompareExchange<EventHandler<MessageReceivedEventArgs>>(ref this.MessageReceived, val3, val2);
				}
				while (val != val2);
			}
			[CompilerGenerated]
			remove
			{
				EventHandler<MessageReceivedEventArgs> val = this.MessageReceived;
				EventHandler<MessageReceivedEventArgs> val2;
				do
				{
					val2 = val;
					EventHandler<MessageReceivedEventArgs> val3 = (EventHandler<MessageReceivedEventArgs>)(object)global::System.Delegate.Remove((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
					val = Interlocked.CompareExchange<EventHandler<MessageReceivedEventArgs>>(ref this.MessageReceived, val3, val2);
				}
				while (val != val2);
			}
		}

		public event EventHandler<StreamReceivedEventArgs> StreamReceived
		{
			[CompilerGenerated]
			add
			{
				EventHandler<StreamReceivedEventArgs> val = this.StreamReceived;
				EventHandler<StreamReceivedEventArgs> val2;
				do
				{
					val2 = val;
					EventHandler<StreamReceivedEventArgs> val3 = (EventHandler<StreamReceivedEventArgs>)(object)global::System.Delegate.Combine((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
					val = Interlocked.CompareExchange<EventHandler<StreamReceivedEventArgs>>(ref this.StreamReceived, val3, val2);
				}
				while (val != val2);
			}
			[CompilerGenerated]
			remove
			{
				EventHandler<StreamReceivedEventArgs> val = this.StreamReceived;
				EventHandler<StreamReceivedEventArgs> val2;
				do
				{
					val2 = val;
					EventHandler<StreamReceivedEventArgs> val3 = (EventHandler<StreamReceivedEventArgs>)(object)global::System.Delegate.Remove((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
					val = Interlocked.CompareExchange<EventHandler<StreamReceivedEventArgs>>(ref this.StreamReceived, val3, val2);
				}
				while (val != val2);
			}
		}

		public event EventHandler ServerStarted
		{
			[CompilerGenerated]
			add
			{
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Expected O, but got Unknown
				EventHandler val = this.ServerStarted;
				EventHandler val2;
				do
				{
					val2 = val;
					EventHandler val3 = (EventHandler)global::System.Delegate.Combine((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
					val = Interlocked.CompareExchange<EventHandler>(ref global::System.Runtime.CompilerServices.Unsafe.As<EventHandler, EventHandler>(ref this.ServerStarted), val3, val2);
				}
				while (val != val2);
			}
			[CompilerGenerated]
			remove
			{
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Expected O, but got Unknown
				EventHandler val = this.ServerStarted;
				EventHandler val2;
				do
				{
					val2 = val;
					EventHandler val3 = (EventHandler)global::System.Delegate.Remove((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
					val = Interlocked.CompareExchange<EventHandler>(ref global::System.Runtime.CompilerServices.Unsafe.As<EventHandler, EventHandler>(ref this.ServerStarted), val3, val2);
				}
				while (val != val2);
			}
		}

		public event EventHandler ServerStopped
		{
			[CompilerGenerated]
			add
			{
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Expected O, but got Unknown
				EventHandler val = this.ServerStopped;
				EventHandler val2;
				do
				{
					val2 = val;
					EventHandler val3 = (EventHandler)global::System.Delegate.Combine((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
					val = Interlocked.CompareExchange<EventHandler>(ref global::System.Runtime.CompilerServices.Unsafe.As<EventHandler, EventHandler>(ref this.ServerStopped), val3, val2);
				}
				while (val != val2);
			}
			[CompilerGenerated]
			remove
			{
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Expected O, but got Unknown
				EventHandler val = this.ServerStopped;
				EventHandler val2;
				do
				{
					val2 = val;
					EventHandler val3 = (EventHandler)global::System.Delegate.Remove((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
					val = Interlocked.CompareExchange<EventHandler>(ref global::System.Runtime.CompilerServices.Unsafe.As<EventHandler, EventHandler>(ref this.ServerStopped), val3, val2);
				}
				while (val != val2);
			}
		}

		public event EventHandler<ExceptionEventArgs> ExceptionEncountered
		{
			[CompilerGenerated]
			add
			{
				EventHandler<ExceptionEventArgs> val = this.ExceptionEncountered;
				EventHandler<ExceptionEventArgs> val2;
				do
				{
					val2 = val;
					EventHandler<ExceptionEventArgs> val3 = (EventHandler<ExceptionEventArgs>)(object)global::System.Delegate.Combine((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
					val = Interlocked.CompareExchange<EventHandler<ExceptionEventArgs>>(ref this.ExceptionEncountered, val3, val2);
				}
				while (val != val2);
			}
			[CompilerGenerated]
			remove
			{
				EventHandler<ExceptionEventArgs> val = this.ExceptionEncountered;
				EventHandler<ExceptionEventArgs> val2;
				do
				{
					val2 = val;
					EventHandler<ExceptionEventArgs> val3 = (EventHandler<ExceptionEventArgs>)(object)global::System.Delegate.Remove((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
					val = Interlocked.CompareExchange<EventHandler<ExceptionEventArgs>>(ref this.ExceptionEncountered, val3, val2);
				}
				while (val != val2);
			}
		}

		internal void HandleAuthenticationRequested(object sender, AuthenticationRequestedEventArgs args)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Expected O, but got Unknown
			<>c__DisplayClass35_0 CS$<>8__locals0 = new <>c__DisplayClass35_0();
			CS$<>8__locals0.<>4__this = this;
			CS$<>8__locals0.sender = sender;
			CS$<>8__locals0.args = args;
			WrappedEventHandler((Action)delegate
			{
				CS$<>8__locals0.<>4__this.AuthenticationRequested?.Invoke(CS$<>8__locals0.sender, CS$<>8__locals0.args);
			}, "AuthenticationRequested", CS$<>8__locals0.sender);
		}

		internal void HandleAuthenticationSucceeded(object sender, AuthenticationSucceededEventArgs args)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Expected O, but got Unknown
			<>c__DisplayClass36_0 CS$<>8__locals0 = new <>c__DisplayClass36_0();
			CS$<>8__locals0.<>4__this = this;
			CS$<>8__locals0.sender = sender;
			CS$<>8__locals0.args = args;
			WrappedEventHandler((Action)delegate
			{
				CS$<>8__locals0.<>4__this.AuthenticationSucceeded?.Invoke(CS$<>8__locals0.sender, CS$<>8__locals0.args);
			}, "AuthenticationSucceeded", CS$<>8__locals0.sender);
		}

		internal void HandleAuthenticationFailed(object sender, AuthenticationFailedEventArgs args)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Expected O, but got Unknown
			<>c__DisplayClass37_0 CS$<>8__locals0 = new <>c__DisplayClass37_0();
			CS$<>8__locals0.<>4__this = this;
			CS$<>8__locals0.sender = sender;
			CS$<>8__locals0.args = args;
			WrappedEventHandler((Action)delegate
			{
				CS$<>8__locals0.<>4__this.AuthenticationFailed?.Invoke(CS$<>8__locals0.sender, CS$<>8__locals0.args);
			}, "AuthenticationFailed", CS$<>8__locals0.sender);
		}

		internal void HandleClientConnected(object sender, ConnectionEventArgs args)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Expected O, but got Unknown
			<>c__DisplayClass38_0 CS$<>8__locals0 = new <>c__DisplayClass38_0();
			CS$<>8__locals0.<>4__this = this;
			CS$<>8__locals0.sender = sender;
			CS$<>8__locals0.args = args;
			WrappedEventHandler((Action)delegate
			{
				CS$<>8__locals0.<>4__this.ClientConnected?.Invoke(CS$<>8__locals0.sender, CS$<>8__locals0.args);
			}, "ClientConnected", CS$<>8__locals0.sender);
		}

		internal void HandleClientDisconnected(object sender, DisconnectionEventArgs args)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Expected O, but got Unknown
			<>c__DisplayClass39_0 CS$<>8__locals0 = new <>c__DisplayClass39_0();
			CS$<>8__locals0.<>4__this = this;
			CS$<>8__locals0.sender = sender;
			CS$<>8__locals0.args = args;
			WrappedEventHandler((Action)delegate
			{
				CS$<>8__locals0.<>4__this.ClientDisconnected?.Invoke(CS$<>8__locals0.sender, CS$<>8__locals0.args);
			}, "ClientDisconnected", CS$<>8__locals0.sender);
		}

		internal void HandleMessageReceived(object sender, MessageReceivedEventArgs args)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Expected O, but got Unknown
			<>c__DisplayClass40_0 CS$<>8__locals0 = new <>c__DisplayClass40_0();
			CS$<>8__locals0.<>4__this = this;
			CS$<>8__locals0.sender = sender;
			CS$<>8__locals0.args = args;
			WrappedEventHandler((Action)delegate
			{
				CS$<>8__locals0.<>4__this.MessageReceived?.Invoke(CS$<>8__locals0.sender, CS$<>8__locals0.args);
			}, "MessageReceived", CS$<>8__locals0.sender);
		}

		internal void HandleStreamReceived(object sender, StreamReceivedEventArgs args)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Expected O, but got Unknown
			<>c__DisplayClass41_0 CS$<>8__locals0 = new <>c__DisplayClass41_0();
			CS$<>8__locals0.<>4__this = this;
			CS$<>8__locals0.sender = sender;
			CS$<>8__locals0.args = args;
			WrappedEventHandler((Action)delegate
			{
				CS$<>8__locals0.<>4__this.StreamReceived?.Invoke(CS$<>8__locals0.sender, CS$<>8__locals0.args);
			}, "StreamReceived", CS$<>8__locals0.sender);
		}

		internal void HandleServerStarted(object sender, EventArgs args)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Expected O, but got Unknown
			<>c__DisplayClass42_0 CS$<>8__locals0 = new <>c__DisplayClass42_0();
			CS$<>8__locals0.<>4__this = this;
			CS$<>8__locals0.sender = sender;
			CS$<>8__locals0.args = args;
			WrappedEventHandler((Action)delegate
			{
				object serverStarted = (object)CS$<>8__locals0.<>4__this.ServerStarted;
				if (serverStarted != null)
				{
					((EventHandler)serverStarted).Invoke(CS$<>8__locals0.sender, CS$<>8__locals0.args);
				}
			}, "ServerStarted", CS$<>8__locals0.sender);
		}

		internal void HandleServerStopped(object sender, EventArgs args)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Expected O, but got Unknown
			<>c__DisplayClass43_0 CS$<>8__locals0 = new <>c__DisplayClass43_0();
			CS$<>8__locals0.<>4__this = this;
			CS$<>8__locals0.sender = sender;
			CS$<>8__locals0.args = args;
			WrappedEventHandler((Action)delegate
			{
				object serverStopped = (object)CS$<>8__locals0.<>4__this.ServerStopped;
				if (serverStopped != null)
				{
					((EventHandler)serverStopped).Invoke(CS$<>8__locals0.sender, CS$<>8__locals0.args);
				}
			}, "ServerStopped", CS$<>8__locals0.sender);
		}

		internal void HandleExceptionEncountered(object sender, ExceptionEventArgs args)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Expected O, but got Unknown
			<>c__DisplayClass44_0 CS$<>8__locals0 = new <>c__DisplayClass44_0();
			CS$<>8__locals0.<>4__this = this;
			CS$<>8__locals0.sender = sender;
			CS$<>8__locals0.args = args;
			WrappedEventHandler((Action)delegate
			{
				CS$<>8__locals0.<>4__this.ExceptionEncountered?.Invoke(CS$<>8__locals0.sender, CS$<>8__locals0.args);
			}, "ExceptionEncountered", CS$<>8__locals0.sender);
		}

		internal void WrappedEventHandler(Action action, string handler, object sender)
		{
			if (action != null)
			{
				Action<Severity, string> logger = ((WatsonTcpServer)sender).Settings.Logger;
				try
				{
					action.Invoke();
				}
				catch (global::System.Exception obj)
				{
					logger?.Invoke(Severity.Error, string.Concat(new string[5]
					{
						"Event handler exception in ",
						handler,
						": ",
						Environment.get_NewLine(),
						SerializationHelper.SerializeJson(obj, pretty: true)
					}));
				}
			}
		}
	}
}
