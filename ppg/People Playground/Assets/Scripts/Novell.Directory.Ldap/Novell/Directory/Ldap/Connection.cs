using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Novell.Directory.Ldap.Asn1;
using Novell.Directory.Ldap.Rfc2251;
using Novell.Directory.Ldap.Utilclass;

namespace Novell.Directory.Ldap
{
	internal sealed class Connection
	{
		public enum CertificateProblem : long
		{
			CertEXPIRED = 2148204801L,
			CertVALIDITYPERIODNESTING = 2148204802L,
			CertROLE = 2148204803L,
			CertPATHLENCONST = 2148204804L,
			CertCRITICAL = 2148204805L,
			CertPURPOSE = 2148204806L,
			CertISSUERCHAINING = 2148204807L,
			CertMALFORMED = 2148204808L,
			CertUNTRUSTEDROOT = 2148204809L,
			CertCHAINING = 2148204810L,
			CertREVOKED = 2148204812L,
			CertUNTRUSTEDTESTROOT = 2148204813L,
			CertREVOCATION_FAILURE = 2148204814L,
			CertCN_NO_MATCH = 2148204815L,
			CertWRONG_USAGE = 2148204816L,
			CertUNTRUSTEDCA = 2148204818L
		}

		public class ReaderThread
		{
			private Connection enclosingInstance;

			public Connection Enclosing_Instance => enclosingInstance;

			private void InitBlock(Connection enclosingInstance)
			{
				this.enclosingInstance = enclosingInstance;
			}

			public ReaderThread(Connection enclosingInstance)
			{
				InitBlock(enclosingInstance);
			}

			public virtual void Run()
			{
				string reason = "reader: thread stopping";
				InterThreadException ex = null;
				Message message = null;
				IOException deadReaderException = null;
				enclosingInstance.reader = Thread.CurrentThread;
				try
				{
					while (true)
					{
						Stream in_Renamed = enclosingInstance.in_Renamed;
						if (in_Renamed == null)
						{
							break;
						}
						Asn1Identifier asn1Identifier = new Asn1Identifier(in_Renamed);
						_ = asn1Identifier.Tag;
						if (asn1Identifier.Tag != 16)
						{
							continue;
						}
						Asn1Length asn1Length = new Asn1Length(in_Renamed);
						RfcLdapMessage rfcLdapMessage = new RfcLdapMessage(enclosingInstance.decoder, in_Renamed, asn1Length.Length);
						int messageID = rfcLdapMessage.MessageID;
						try
						{
							message = enclosingInstance.messages.findMessageById(messageID);
							message.putReply(rfcLdapMessage);
						}
						catch (FieldAccessException)
						{
							if (messageID == 0)
							{
								enclosingInstance.notifyAllUnsolicitedListeners(rfcLdapMessage);
								if (enclosingInstance.unsolSvrShutDnNotification)
								{
									ex = new InterThreadException("SERVER_SHUTDOWN_REQ", new object[2] { enclosingInstance.host, enclosingInstance.port }, 91, null, null);
									return;
								}
							}
						}
						if (enclosingInstance.stopReaderMessageID == messageID || enclosingInstance.stopReaderMessageID == -98)
						{
							return;
						}
					}
				}
				catch (ThreadAbortException)
				{
					return;
				}
				catch (IOException ex4)
				{
					deadReaderException = ex4;
					if (enclosingInstance.stopReaderMessageID != -98 && enclosingInstance.clientActive)
					{
						ex = new InterThreadException("CONNECTION_WAIT", new object[2] { enclosingInstance.host, enclosingInstance.port }, 91, ex4, message);
					}
					enclosingInstance.in_Renamed = null;
					enclosingInstance.out_Renamed = null;
				}
				finally
				{
					if (!enclosingInstance.clientActive || ex != null)
					{
						enclosingInstance.shutdown(reason, 0, ex);
					}
					else
					{
						enclosingInstance.stopReaderMessageID = -99;
					}
				}
				enclosingInstance.deadReaderException = deadReaderException;
				enclosingInstance.deadReader = enclosingInstance.reader;
				enclosingInstance.reader = null;
			}
		}

		private class UnsolicitedListenerThread : SupportClass.ThreadClass
		{
			private Connection enclosingInstance;

			private LdapUnsolicitedNotificationListener listenerObj;

			private LdapExtendedResponse unsolicitedMsg;

			public Connection Enclosing_Instance => enclosingInstance;

			private void InitBlock(Connection enclosingInstance)
			{
				this.enclosingInstance = enclosingInstance;
			}

			internal UnsolicitedListenerThread(Connection enclosingInstance, LdapUnsolicitedNotificationListener l, LdapExtendedResponse m)
			{
				InitBlock(enclosingInstance);
				listenerObj = l;
				unsolicitedMsg = m;
			}

			public override void Run()
			{
				listenerObj.messageReceived(unsolicitedMsg);
			}
		}

		private ArrayList handshakeProblemsEncountered = new ArrayList();

		private object writeSemaphore;

		private int writeSemaphoreOwner;

		private int writeSemaphoreCount;

		private int ephemeralId = -1;

		private BindProperties bindProperties;

		private int bindSemaphoreId;

		private Thread reader;

		private Thread deadReader;

		private IOException deadReaderException;

		private LBEREncoder encoder;

		private LBERDecoder decoder;

		private Socket sock;

		private TcpClient socket;

		private TcpClient nonTLSBackup;

		private Stream in_Renamed;

		private Stream out_Renamed;

		private bool clientActive = true;

		private bool ssl;

		private bool unsolSvrShutDnNotification;

		private const int CONTINUE_READING = -99;

		private const int STOP_READING = -98;

		private int stopReaderMessageID;

		private MessageVector messages;

		private ReferralInfo activeReferral;

		private ArrayList unsolicitedListeners;

		private string host;

		private int port;

		private int cloneCount;

		private string name = "";

		private static object nameLock;

		private static int connNum;

		internal static string sdk;

		internal static int protocol;

		internal static string security;

		internal bool Cloned => cloneCount > 0;

		internal bool Ssl
		{
			get
			{
				return ssl;
			}
			set
			{
				ssl = value;
			}
		}

		internal string Host => host;

		internal int Port => port;

		internal int BindSemId
		{
			get
			{
				return bindSemaphoreId;
			}
			set
			{
				bindSemaphoreId = value;
			}
		}

		internal bool BindSemIdClear
		{
			get
			{
				if (bindSemaphoreId == 0)
				{
					return true;
				}
				return false;
			}
		}

		internal bool Bound
		{
			get
			{
				if (bindProperties != null)
				{
					return !bindProperties.Anonymous;
				}
				return false;
			}
		}

		internal bool Connected => in_Renamed != null;

		internal BindProperties BindProperties
		{
			get
			{
				return bindProperties;
			}
			set
			{
				bindProperties = value;
			}
		}

		internal ReferralInfo ActiveReferral
		{
			get
			{
				return activeReferral;
			}
			set
			{
				activeReferral = value;
			}
		}

		internal string ConnectionName => name;

		internal bool TLS => nonTLSBackup != null;

		internal Stream InputStream => in_Renamed;

		internal Stream OutputStream => out_Renamed;

		public event CertificateValidationCallback OnCertificateValidation;

		private static string GetProblemMessage(CertificateProblem Problem)
		{
			string text = "";
			string text2 = Enum.GetName(typeof(CertificateProblem), Problem);
			if (text2 != null)
			{
				return text + text2;
			}
			return "Unknown Certificate Problem";
		}

		private void InitBlock()
		{
			writeSemaphore = new object();
			encoder = new LBEREncoder();
			decoder = new LBERDecoder();
			stopReaderMessageID = -99;
			messages = new MessageVector(5, 5);
			unsolicitedListeners = new ArrayList(3);
		}

		internal Connection()
		{
			InitBlock();
		}

		internal object copy()
		{
			Connection result = new Connection
			{
				host = host,
				port = port
			};
			protocol = protocol;
			return result;
		}

		internal int acquireWriteSemaphore()
		{
			return acquireWriteSemaphore(0);
		}

		internal int acquireWriteSemaphore(int msgId)
		{
			int num = msgId;
			lock (writeSemaphore)
			{
				if (num == 0)
				{
					ephemeralId = ((ephemeralId == int.MinValue) ? (ephemeralId = -1) : (--ephemeralId));
					num = ephemeralId;
				}
				while (true)
				{
					if (writeSemaphoreOwner == 0)
					{
						writeSemaphoreOwner = num;
						break;
					}
					if (writeSemaphoreOwner == num)
					{
						break;
					}
					try
					{
						Monitor.Wait(writeSemaphore);
					}
					catch (ThreadInterruptedException)
					{
					}
				}
				writeSemaphoreCount++;
				return num;
			}
		}

		internal void freeWriteSemaphore(int msgId)
		{
			lock (writeSemaphore)
			{
				if (writeSemaphoreOwner == 0)
				{
					throw new SystemException("Connection.freeWriteSemaphore(" + msgId + "): semaphore not owned by any thread");
				}
				if (writeSemaphoreOwner != msgId)
				{
					throw new SystemException("Connection.freeWriteSemaphore(" + msgId + "): thread does not own the semaphore, owned by " + writeSemaphoreOwner);
				}
				if (--writeSemaphoreCount == 0)
				{
					writeSemaphoreOwner = 0;
					Monitor.Pulse(writeSemaphore);
				}
			}
		}

		private void waitForReader(Thread thread)
		{
			Thread objA = ((reader == null) ? null : reader);
			Thread objB = ((thread == null) ? null : thread);
			while (!object.Equals(objA, objB))
			{
				try
				{
					if (thread == deadReader)
					{
						if (thread == null)
						{
							return;
						}
						IOException rootException = deadReaderException;
						deadReaderException = null;
						deadReader = null;
						throw new LdapException("CONNECTION_READER", 91, null, rootException);
					}
					lock (this)
					{
						Monitor.Wait(this, TimeSpan.FromMilliseconds(5.0));
					}
				}
				catch (ThreadInterruptedException)
				{
				}
				objA = ((reader == null) ? null : reader);
				objB = ((thread == null) ? null : thread);
			}
			deadReaderException = null;
			deadReader = null;
		}

		internal void connect(string host, int port)
		{
			connect(host, port, 0);
		}

		public bool ServerCertificateValidation(X509Certificate certificate, int[] certificateErrors)
		{
			if (this.OnCertificateValidation != null)
			{
				return this.OnCertificateValidation(certificate, certificateErrors);
			}
			return DefaultCertificateValidationHandler(certificate, certificateErrors);
		}

		public bool DefaultCertificateValidationHandler(X509Certificate certificate, int[] certificateErrors)
		{
			bool flag = false;
			if (certificateErrors != null && certificateErrors.Length != 0)
			{
				if (certificateErrors.Length == 1 && certificateErrors[0] == -2146762481)
				{
					return true;
				}
				Console.WriteLine("Detected errors in the Server Certificate:");
				for (int i = 0; i < certificateErrors.Length; i++)
				{
					handshakeProblemsEncountered.Add((CertificateProblem)(uint)certificateErrors[i]);
					Console.WriteLine(certificateErrors[i]);
				}
				return false;
			}
			return true;
		}

		private void connect(string host, int port, int semaphoreId)
		{
			waitForReader(null);
			unsolSvrShutDnNotification = false;
			int msgId = acquireWriteSemaphore(semaphoreId);
			try
			{
				if (port == 0)
				{
					port = 389;
				}
				try
				{
					if (in_Renamed == null || out_Renamed == null)
					{
						if (Ssl)
						{
							this.host = host;
							this.port = port;
							sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
							IPEndPoint remoteEP = new IPEndPoint(Dns.Resolve(host).AddressList[0], port);
							sock.Connect(remoteEP);
							NetworkStream networkStream = new NetworkStream(sock, ownsSocket: true);
							Assembly assembly;
							try
							{
								assembly = Assembly.LoadWithPartialName("Mono.Security");
							}
							catch (FileNotFoundException)
							{
								throw new LdapException("SSL_PROVIDER_MISSING", 114, null);
							}
							Type type = assembly.GetType("Mono.Security.Protocol.Tls.SslClientStream");
							object[] array = new object[4] { networkStream, host, false, null };
							Type type2 = assembly.GetType("Mono.Security.Protocol.Tls.SecurityProtocolType");
							_ = (Enum)Activator.CreateInstance(type2);
							int num = (int)Enum.Parse(type2, "Ssl3");
							int num2 = (int)Enum.Parse(type2, "Tls");
							array[3] = Enum.ToObject(type2, num | num2);
							object obj = Activator.CreateInstance(type, array);
							PropertyInfo property = type.GetProperty("ServerCertValidationDelegate");
							property.SetValue(obj, Delegate.CreateDelegate(property.PropertyType, this, "ServerCertificateValidation"), null);
							in_Renamed = (Stream)obj;
							out_Renamed = (Stream)obj;
						}
						else
						{
							socket = new TcpClient(host, port);
							in_Renamed = socket.GetStream();
							out_Renamed = socket.GetStream();
						}
					}
					else
					{
						Console.WriteLine("connect input/out Stream specified");
					}
				}
				catch (SocketException rootException)
				{
					sock = null;
					socket = null;
					throw new LdapException("CONNECTION_ERROR", new object[2] { host, port }, 91, null, rootException);
				}
				catch (IOException rootException2)
				{
					sock = null;
					socket = null;
					throw new LdapException("CONNECTION_ERROR", new object[2] { host, port }, 91, null, rootException2);
				}
				this.host = host;
				this.port = port;
				startReader();
				clientActive = true;
			}
			finally
			{
				freeWriteSemaphore(msgId);
			}
		}

		internal void incrCloneCount()
		{
			lock (this)
			{
				cloneCount++;
			}
		}

		internal Connection destroyClone(bool apiCall)
		{
			lock (this)
			{
				Connection result = this;
				if (cloneCount > 0)
				{
					cloneCount--;
					result = ((!apiCall) ? null : ((Connection)copy()));
				}
				else if (in_Renamed != null)
				{
					InterThreadException notifyUser = new InterThreadException(apiCall ? "CONNECTION_CLOSED" : "CONNECTION_FINALIZED", null, 91, null, null);
					shutdown("destroy clone", 0, notifyUser);
				}
				return result;
			}
		}

		internal void clearBindSemId()
		{
			bindSemaphoreId = 0;
		}

		internal void writeMessage(Message info)
		{
			object[][] contents = new ExceptionMessages().getContents();
			messages.Add(info);
			if (info.BindRequest && !Connected && host != null)
			{
				connect(host, port, info.MessageID);
			}
			if (Connected)
			{
				LdapMessage request = info.Request;
				writeMessage(request);
				return;
			}
			int num = 0;
			for (num = 0; num < contents.Length && contents[num][0] != "CONNECTION_CLOSED"; num++)
			{
			}
			throw new LdapException("CONNECTION_CLOSED", new object[2] { host, port }, 91, (string)contents[num][1]);
		}

		internal void writeMessage(LdapMessage msg)
		{
			int msgId = ((bindSemaphoreId != 0) ? bindSemaphoreId : msg.MessageID);
			Stream stream = out_Renamed;
			acquireWriteSemaphore(msgId);
			try
			{
				if (stream == null)
				{
					throw new IOException("Output stream not initialized");
				}
				if (stream.CanWrite)
				{
					sbyte[] encoding = msg.Asn1Object.getEncoding(encoder);
					stream.Write(SupportClass.ToByteArray(encoding), 0, encoding.Length);
					stream.Flush();
				}
			}
			catch (IOException rootException)
			{
				if (msg.Type == 0 && ssl)
				{
					string text = "Following problem(s) occurred while establishing SSL based Connection : ";
					if (handshakeProblemsEncountered.Count > 0)
					{
						text += GetProblemMessage((CertificateProblem)handshakeProblemsEncountered[0]);
						for (int i = 1; i < handshakeProblemsEncountered.Count; i++)
						{
							text = text + ", " + GetProblemMessage((CertificateProblem)handshakeProblemsEncountered[i]);
						}
					}
					else
					{
						text += "Unknown Certificate Problem";
					}
					throw new LdapException(text, new object[2] { host, port }, 113, null, rootException);
				}
				if (clientActive)
				{
					if (unsolSvrShutDnNotification)
					{
						throw new LdapException("SERVER_SHUTDOWN_REQ", new object[2] { host, port }, 91, null, rootException);
					}
					throw new LdapException("IO_EXCEPTION", new object[2] { host, port }, 91, null, rootException);
				}
			}
			finally
			{
				freeWriteSemaphore(msgId);
				handshakeProblemsEncountered.Clear();
			}
		}

		internal MessageAgent getMessageAgent(int msgId)
		{
			return messages.findMessageById(msgId).MessageAgent;
		}

		internal void removeMessage(Message info)
		{
			SupportClass.VectorRemoveElement(messages, info);
		}

		~Connection()
		{
			shutdown("Finalize", 0, null);
		}

		private void shutdown(string reason, int semaphoreId, InterThreadException notifyUser)
		{
			Message message = null;
			if (!clientActive)
			{
				return;
			}
			clientActive = false;
			while (true)
			{
				try
				{
					object obj = messages[0];
					messages.RemoveAt(0);
					message = (Message)obj;
				}
				catch (ArgumentOutOfRangeException)
				{
					break;
				}
				message.Abandon(null, notifyUser);
			}
			int msgId = acquireWriteSemaphore(semaphoreId);
			if (bindProperties != null && out_Renamed != null && out_Renamed.CanWrite && !bindProperties.Anonymous)
			{
				try
				{
					sbyte[] encoding = new LdapUnbindRequest(null).Asn1Object.getEncoding(encoder);
					out_Renamed.Write(SupportClass.ToByteArray(encoding), 0, encoding.Length);
					out_Renamed.Flush();
					out_Renamed.Close();
				}
				catch (Exception)
				{
				}
			}
			bindProperties = null;
			if (socket != null || sock != null)
			{
				if (reader != null && reason != "reader: thread stopping")
				{
					reader.Abort();
				}
				try
				{
					if (Ssl)
					{
						try
						{
							sock.Shutdown(SocketShutdown.Both);
						}
						catch
						{
						}
						sock.Close();
					}
					else
					{
						if (in_Renamed != null)
						{
							in_Renamed.Close();
						}
						socket.Close();
					}
				}
				catch (Exception)
				{
				}
				socket = null;
				sock = null;
				in_Renamed = null;
				out_Renamed = null;
			}
			freeWriteSemaphore(msgId);
		}

		internal bool areMessagesComplete()
		{
			object[] objectArray = messages.ObjectArray;
			int num = objectArray.Length;
			if (bindSemaphoreId != 0)
			{
				return false;
			}
			if (num == 0)
			{
				return true;
			}
			for (int i = 0; i < num; i++)
			{
				if (!((Message)objectArray[i]).Complete)
				{
					return false;
				}
			}
			return true;
		}

		internal void stopReaderOnReply(int messageID)
		{
			stopReaderMessageID = messageID;
		}

		internal void startReader()
		{
			Thread thread = new Thread(new ReaderThread(this).Run);
			thread.IsBackground = true;
			thread.Start();
			waitForReader(thread);
		}

		internal void startTLS()
		{
			try
			{
				waitForReader(null);
				nonTLSBackup = socket;
				Assembly assembly = null;
				try
				{
					assembly = Assembly.LoadFrom("Mono.Security.dll");
				}
				catch (FileNotFoundException)
				{
					throw new LdapException("SSL_PROVIDER_MISSING", 114, null);
				}
				Type type = assembly.GetType("Mono.Security.Protocol.Tls.SslClientStream");
				object[] array = new object[4]
				{
					socket.GetStream(),
					host,
					false,
					null
				};
				Type type2 = assembly.GetType("Mono.Security.Protocol.Tls.SecurityProtocolType");
				_ = (Enum)Activator.CreateInstance(type2);
				int num = (int)Enum.Parse(type2, "Ssl3");
				int num2 = (int)Enum.Parse(type2, "Tls");
				array[3] = Enum.ToObject(type2, num | num2);
				object obj = Activator.CreateInstance(type, array);
				EventInfo @event = type.GetEvent("ServerCertValidationDelegate");
				@event.AddEventHandler(obj, Delegate.CreateDelegate(@event.EventHandlerType, this, "ServerCertificateValidation"));
				in_Renamed = (Stream)obj;
				out_Renamed = (Stream)obj;
			}
			catch (IOException rootException)
			{
				nonTLSBackup = null;
				throw new LdapException("Could not negotiate a secure connection", 91, null, rootException);
			}
			catch (Exception rootException2)
			{
				nonTLSBackup = null;
				throw new LdapException("The host is unknown", 91, null, rootException2);
			}
		}

		internal void stopTLS()
		{
			try
			{
				stopReaderMessageID = -98;
				out_Renamed.Close();
				in_Renamed.Close();
				waitForReader(null);
				socket = nonTLSBackup;
				in_Renamed = socket.GetStream();
				out_Renamed = socket.GetStream();
				stopReaderMessageID = -99;
			}
			catch (IOException rootException)
			{
				throw new LdapException("STOPTLS_ERROR", 91, null, rootException);
			}
			finally
			{
				nonTLSBackup = null;
				startReader();
			}
		}

		internal void ReplaceStreams(Stream newIn, Stream newOut)
		{
			waitForReader(null);
			in_Renamed = newIn;
			out_Renamed = newOut;
			startReader();
		}

		internal void AddUnsolicitedNotificationListener(LdapUnsolicitedNotificationListener listener)
		{
			unsolicitedListeners.Add(listener);
		}

		internal void RemoveUnsolicitedNotificationListener(LdapUnsolicitedNotificationListener listener)
		{
			SupportClass.VectorRemoveElement(unsolicitedListeners, listener);
		}

		private void notifyAllUnsolicitedListeners(RfcLdapMessage message)
		{
			if (new LdapExtendedResponse(message).ID.Equals("1.3.6.1.4.1.1466.20036"))
			{
				unsolSvrShutDnNotification = true;
			}
			int count = unsolicitedListeners.Count;
			for (int i = 0; i < count; i++)
			{
				LdapUnsolicitedNotificationListener l = (LdapUnsolicitedNotificationListener)unsolicitedListeners[i];
				LdapExtendedResponse m = new LdapExtendedResponse(message);
				new UnsolicitedListenerThread(this, l, m).Start();
			}
		}

		static Connection()
		{
			connNum = 0;
			security = "simple";
			nameLock = new object();
			sdk = new StringBuilder("2.1.8").ToString();
			protocol = 3;
		}
	}
}
