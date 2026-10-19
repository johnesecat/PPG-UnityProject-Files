using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace WatsonTcp
{
	public class WatsonTcpClientEvents
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

		public event EventHandler AuthenticationSucceeded
		{
			[CompilerGenerated]
			add
			{
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Expected O, but got Unknown
				EventHandler val = this.AuthenticationSucceeded;
				EventHandler val2;
				do
				{
					val2 = val;
					EventHandler val3 = (EventHandler)global::System.Delegate.Combine((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
					val = Interlocked.CompareExchange<EventHandler>(ref global::System.Runtime.CompilerServices.Unsafe.As<EventHandler, EventHandler>(ref this.AuthenticationSucceeded), val3, val2);
				}
				while (val != val2);
			}
			[CompilerGenerated]
			remove
			{
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Expected O, but got Unknown
				EventHandler val = this.AuthenticationSucceeded;
				EventHandler val2;
				do
				{
					val2 = val;
					EventHandler val3 = (EventHandler)global::System.Delegate.Remove((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
					val = Interlocked.CompareExchange<EventHandler>(ref global::System.Runtime.CompilerServices.Unsafe.As<EventHandler, EventHandler>(ref this.AuthenticationSucceeded), val3, val2);
				}
				while (val != val2);
			}
		}

		public event EventHandler AuthenticationFailure
		{
			[CompilerGenerated]
			add
			{
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Expected O, but got Unknown
				EventHandler val = this.AuthenticationFailure;
				EventHandler val2;
				do
				{
					val2 = val;
					EventHandler val3 = (EventHandler)global::System.Delegate.Combine((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
					val = Interlocked.CompareExchange<EventHandler>(ref global::System.Runtime.CompilerServices.Unsafe.As<EventHandler, EventHandler>(ref this.AuthenticationFailure), val3, val2);
				}
				while (val != val2);
			}
			[CompilerGenerated]
			remove
			{
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_0016: Expected O, but got Unknown
				EventHandler val = this.AuthenticationFailure;
				EventHandler val2;
				do
				{
					val2 = val;
					EventHandler val3 = (EventHandler)global::System.Delegate.Remove((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
					val = Interlocked.CompareExchange<EventHandler>(ref global::System.Runtime.CompilerServices.Unsafe.As<EventHandler, EventHandler>(ref this.AuthenticationFailure), val3, val2);
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

		public event EventHandler<ConnectionEventArgs> ServerConnected
		{
			[CompilerGenerated]
			add
			{
				EventHandler<ConnectionEventArgs> val = this.ServerConnected;
				EventHandler<ConnectionEventArgs> val2;
				do
				{
					val2 = val;
					EventHandler<ConnectionEventArgs> val3 = (EventHandler<ConnectionEventArgs>)(object)global::System.Delegate.Combine((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
					val = Interlocked.CompareExchange<EventHandler<ConnectionEventArgs>>(ref this.ServerConnected, val3, val2);
				}
				while (val != val2);
			}
			[CompilerGenerated]
			remove
			{
				EventHandler<ConnectionEventArgs> val = this.ServerConnected;
				EventHandler<ConnectionEventArgs> val2;
				do
				{
					val2 = val;
					EventHandler<ConnectionEventArgs> val3 = (EventHandler<ConnectionEventArgs>)(object)global::System.Delegate.Remove((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
					val = Interlocked.CompareExchange<EventHandler<ConnectionEventArgs>>(ref this.ServerConnected, val3, val2);
				}
				while (val != val2);
			}
		}

		public event EventHandler<DisconnectionEventArgs> ServerDisconnected
		{
			[CompilerGenerated]
			add
			{
				EventHandler<DisconnectionEventArgs> val = this.ServerDisconnected;
				EventHandler<DisconnectionEventArgs> val2;
				do
				{
					val2 = val;
					EventHandler<DisconnectionEventArgs> val3 = (EventHandler<DisconnectionEventArgs>)(object)global::System.Delegate.Combine((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
					val = Interlocked.CompareExchange<EventHandler<DisconnectionEventArgs>>(ref this.ServerDisconnected, val3, val2);
				}
				while (val != val2);
			}
			[CompilerGenerated]
			remove
			{
				EventHandler<DisconnectionEventArgs> val = this.ServerDisconnected;
				EventHandler<DisconnectionEventArgs> val2;
				do
				{
					val2 = val;
					EventHandler<DisconnectionEventArgs> val3 = (EventHandler<DisconnectionEventArgs>)(object)global::System.Delegate.Remove((global::System.Delegate)(object)val2, (global::System.Delegate)(object)value);
					val = Interlocked.CompareExchange<EventHandler<DisconnectionEventArgs>>(ref this.ServerDisconnected, val3, val2);
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

		internal void HandleAuthenticationSucceeded(object sender, EventArgs args)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Expected O, but got Unknown
			<>c__DisplayClass26_0 CS$<>8__locals0 = new <>c__DisplayClass26_0();
			CS$<>8__locals0.<>4__this = this;
			CS$<>8__locals0.sender = sender;
			CS$<>8__locals0.args = args;
			WrappedEventHandler((Action)delegate
			{
				object authenticationSucceeded = (object)CS$<>8__locals0.<>4__this.AuthenticationSucceeded;
				if (authenticationSucceeded != null)
				{
					((EventHandler)authenticationSucceeded).Invoke(CS$<>8__locals0.sender, CS$<>8__locals0.args);
				}
			}, "ServerConnected", CS$<>8__locals0.sender);
		}

		internal void HandleAuthenticationFailure(object sender, EventArgs args)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Expected O, but got Unknown
			<>c__DisplayClass27_0 CS$<>8__locals0 = new <>c__DisplayClass27_0();
			CS$<>8__locals0.<>4__this = this;
			CS$<>8__locals0.sender = sender;
			CS$<>8__locals0.args = args;
			WrappedEventHandler((Action)delegate
			{
				object authenticationFailure = (object)CS$<>8__locals0.<>4__this.AuthenticationFailure;
				if (authenticationFailure != null)
				{
					((EventHandler)authenticationFailure).Invoke(CS$<>8__locals0.sender, CS$<>8__locals0.args);
				}
			}, "AuthenticationFailure", CS$<>8__locals0.sender);
		}

		internal void HandleMessageReceived(object sender, MessageReceivedEventArgs args)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Expected O, but got Unknown
			<>c__DisplayClass28_0 CS$<>8__locals0 = new <>c__DisplayClass28_0();
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
			<>c__DisplayClass29_0 CS$<>8__locals0 = new <>c__DisplayClass29_0();
			CS$<>8__locals0.<>4__this = this;
			CS$<>8__locals0.sender = sender;
			CS$<>8__locals0.args = args;
			WrappedEventHandler((Action)delegate
			{
				CS$<>8__locals0.<>4__this.StreamReceived?.Invoke(CS$<>8__locals0.sender, CS$<>8__locals0.args);
			}, "StreamReceived", CS$<>8__locals0.sender);
		}

		internal void HandleServerConnected(object sender, ConnectionEventArgs args)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Expected O, but got Unknown
			<>c__DisplayClass30_0 CS$<>8__locals0 = new <>c__DisplayClass30_0();
			CS$<>8__locals0.<>4__this = this;
			CS$<>8__locals0.sender = sender;
			CS$<>8__locals0.args = args;
			WrappedEventHandler((Action)delegate
			{
				CS$<>8__locals0.<>4__this.ServerConnected?.Invoke(CS$<>8__locals0.sender, CS$<>8__locals0.args);
			}, "ServerConnected", CS$<>8__locals0.sender);
		}

		internal void HandleServerDisconnected(object sender, DisconnectionEventArgs args)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Expected O, but got Unknown
			<>c__DisplayClass31_0 CS$<>8__locals0 = new <>c__DisplayClass31_0();
			CS$<>8__locals0.<>4__this = this;
			CS$<>8__locals0.sender = sender;
			CS$<>8__locals0.args = args;
			WrappedEventHandler((Action)delegate
			{
				CS$<>8__locals0.<>4__this.ServerDisconnected?.Invoke(CS$<>8__locals0.sender, CS$<>8__locals0.args);
			}, "ServerDisconnected", CS$<>8__locals0.sender);
		}

		internal void HandleExceptionEncountered(object sender, ExceptionEventArgs args)
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Expected O, but got Unknown
			<>c__DisplayClass32_0 CS$<>8__locals0 = new <>c__DisplayClass32_0();
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
				try
				{
					action.Invoke();
				}
				catch (global::System.Exception obj)
				{
					(((WatsonTcpClient)sender).Settings?.Logger)?.Invoke(Severity.Error, string.Concat(new string[5]
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
