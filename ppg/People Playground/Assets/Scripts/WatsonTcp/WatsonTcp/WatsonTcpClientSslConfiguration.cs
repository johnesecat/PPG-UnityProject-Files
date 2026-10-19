using System;
using System.Collections;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace WatsonTcp
{
	public class WatsonTcpClientSslConfiguration
	{
		private LocalCertificateSelectionCallback _ClientCertSelectionCallback;

		private RemoteCertificateValidationCallback _ServerCertValidationCallback;

		public LocalCertificateSelectionCallback ClientCertificateSelectionCallback
		{
			get
			{
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Expected O, but got Unknown
				if (_ClientCertSelectionCallback == null)
				{
					_ClientCertSelectionCallback = new LocalCertificateSelectionCallback(DefaultSelectClientCertificate);
				}
				return _ClientCertSelectionCallback;
			}
			set
			{
				_ClientCertSelectionCallback = value;
			}
		}

		public RemoteCertificateValidationCallback ServerCertificateValidationCallback
		{
			get
			{
				//IL_0010: Unknown result type (might be due to invalid IL or missing references)
				//IL_001a: Expected O, but got Unknown
				if (_ServerCertValidationCallback == null)
				{
					_ServerCertValidationCallback = new RemoteCertificateValidationCallback(DefaultValidateServerCertificate);
				}
				return _ServerCertValidationCallback;
			}
			set
			{
				_ServerCertValidationCallback = value;
			}
		}

		public WatsonTcpClientSslConfiguration()
		{
		}

		public WatsonTcpClientSslConfiguration(WatsonTcpClientSslConfiguration configuration)
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			if (configuration == null)
			{
				throw new ArgumentNullException("Can not copy from null client SSL configuration");
			}
			_ClientCertSelectionCallback = configuration._ClientCertSelectionCallback;
			_ServerCertValidationCallback = configuration._ServerCertValidationCallback;
		}

		private static X509Certificate DefaultSelectClientCertificate(object sender, string targetHost, X509CertificateCollection clientCertificates, X509Certificate serverCertificate, string[] acceptableIssuers)
		{
			if (clientCertificates == null || ((CollectionBase)clientCertificates).get_Count() == 0)
			{
				return null;
			}
			return clientCertificates.get_Item(0);
		}

		private static bool DefaultValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return true;
		}
	}
}
