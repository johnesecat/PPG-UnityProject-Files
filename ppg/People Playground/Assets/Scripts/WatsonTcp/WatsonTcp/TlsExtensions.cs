using System;
using System.Security.Authentication;

namespace WatsonTcp
{
	public static class TlsExtensions
	{
		public static SslProtocols ToSslProtocols(this TlsVersion tlsVersion)
		{
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			if (tlsVersion == TlsVersion.Tls12)
			{
				return (SslProtocols)3072;
			}
			throw new ArgumentOutOfRangeException($"Unsupported TLS version {tlsVersion}.");
		}
	}
}
